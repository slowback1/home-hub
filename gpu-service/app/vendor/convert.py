# Vendored from https://github.com/slowback1/ai-epub-to-audiobook
# Modifications:
#   - Output placed directly in output_dir (no book_name subdirectory)
#   - Added --start-chapter N to resume from a specific chapter index (0-based)
import argparse
import os
import re
import subprocess
from pathlib import Path

import ebooklib
from ebooklib import epub
from bs4 import BeautifulSoup
from TTS.api import TTS
import soundfile as sf
import numpy as np
import torch

DEFAULT_SAMPLE_RATE = 22050


def extract_chapters(epub_path, min_chars=500):
    book = epub.read_epub(epub_path)
    chapters = []
    for item in book.get_items():
        if item.get_type() == ebooklib.ITEM_DOCUMENT:
            soup = BeautifulSoup(item.get_content(), "html.parser")
            title = None
            for tag in ["h1", "h2", "h3"]:
                heading = soup.find(tag)
                if heading:
                    title = heading.get_text(strip=True)
                    break
            text = clean_text(soup.get_text(separator=" ", strip=True))
            if len(text) >= min_chars:
                chapters.append({"title": title or f"Chapter {len(chapters) + 1}", "text": text})
    return chapters


def clean_text(text):
    text = re.sub(r"\s+", " ", text)
    text = re.sub(r"\s\d{1,4}\s", " ", text)
    text = text.replace("\xa0", " ").replace("​", "")
    return text.strip()


def chunk_text(text, max_chars=500):
    sentences = re.split(r"(?<=[.!?])\s+", text)
    chunks, current = [], ""
    for sentence in sentences:
        if len(current) + len(sentence) < max_chars:
            current += " " + sentence
        else:
            if current:
                chunks.append(current.strip())
            current = sentence
    if current:
        chunks.append(current.strip())
    return chunks


def text_to_audio(tts, text, output_dir, voice_sample, sample_rate=DEFAULT_SAMPLE_RATE, max_chunk_chars=500):
    chunks = chunk_text(text, max_chars=max_chunk_chars)
    all_audio = []
    temp_path = os.path.join(output_dir, "_temp_chunk.wav")
    for chunk in chunks:
        tts.tts_to_file(text=chunk, speaker_wav=voice_sample, language="en", file_path=temp_path)
        audio, sr = sf.read(temp_path)
        if sr != sample_rate:
            ratio = sample_rate / float(sr)
            new_len = int(len(audio) * ratio)
            audio = np.interp(
                np.linspace(0, len(audio), new_len, endpoint=False), np.arange(len(audio)), audio
            )
        all_audio.append(audio)
    if all_audio:
        return np.concatenate(all_audio)
    return np.array([], dtype=np.float32)


def add_silence(duration_sec=1.0, sample_rate=DEFAULT_SAMPLE_RATE):
    return np.zeros(int(sample_rate * duration_sec))


def build_m4b(chapter_files, output_path, chapter_titles, sample_rate=DEFAULT_SAMPLE_RATE):
    output_dir = os.path.dirname(output_path)
    os.makedirs(output_dir, exist_ok=True)
    filelist_path = os.path.join(output_dir, "filelist.txt")
    metadata_path = os.path.join(output_dir, "metadata.txt")

    with open(filelist_path, "w", encoding="utf-8") as f:
        for wav in chapter_files:
            f.write(f"file '{os.path.abspath(wav)}'\n")

    timestamps = [0]
    for wav in chapter_files[:-1]:
        info = sf.info(wav)
        duration_ms = int(info.frames / info.samplerate * 1000)
        timestamps.append(timestamps[-1] + duration_ms)

    with open(metadata_path, "w", encoding="utf-8") as f:
        f.write(";FFMETADATA1\n")
        for i, (title, start) in enumerate(zip(chapter_titles, timestamps)):
            end = timestamps[i + 1] if i + 1 < len(timestamps) else start + 999999
            f.write(f"\n[CHAPTER]\nTIMEBASE=1/1000\nSTART={start}\nEND={end}\ntitle={title}\n")

    subprocess.run(
        [
            "ffmpeg", "-y",
            "-f", "concat", "-safe", "0", "-i", filelist_path,
            "-i", metadata_path,
            "-map_metadata", "1",
            "-c:a", "aac", "-b:a", "64k",
            output_path,
        ],
        check=True,
    )


def process_epub(
    tts, epub_path, output_dir, voice_sample,
    sample_rate=DEFAULT_SAMPLE_RATE, silence_between=1.5,
    min_chars=500, max_chunk_chars=500, start_chapter=0,
):
    epub_path = Path(epub_path)
    book_name = epub_path.stem
    out_dir = Path(output_dir)
    out_dir.mkdir(parents=True, exist_ok=True)

    print(f"Processing '{epub_path.name}' -> '{out_dir}'")
    chapters = extract_chapters(str(epub_path), min_chars=min_chars)
    if not chapters:
        print("  No chapters found (skipping)")
        return

    # Collect already-synthesised chapters so we can pass them to build_m4b
    chapter_files = []
    chapter_titles = []

    for i, chapter in enumerate(chapters):
        wav_path = out_dir / f"chapter_{i + 1:03d}.wav"
        if i < start_chapter and wav_path.exists():
            # Resume: reuse existing WAV
            print(f"  Chapter {i+1}/{len(chapters)}: resuming from existing WAV")
            chapter_files.append(str(wav_path))
            chapter_titles.append(chapter["title"])
            continue

        print(f"  Chapter {i+1}/{len(chapters)}: {chapter['title']}")
        audio = text_to_audio(
            tts, chapter["text"], str(out_dir), voice_sample,
            sample_rate=sample_rate, max_chunk_chars=max_chunk_chars,
        )
        audio = np.concatenate([audio, add_silence(silence_between, sample_rate)])
        sf.write(str(wav_path), audio, sample_rate)
        chapter_files.append(str(wav_path))
        chapter_titles.append(chapter["title"])
        print(f"    Saved {wav_path.name}")

    output_m4b = out_dir / f"{book_name}.m4b"
    print(f"  Stitching into {output_m4b}")
    build_m4b(chapter_files, str(output_m4b), chapter_titles, sample_rate=sample_rate)
    print(f"  Done: {output_m4b}\n")


def find_epubs(input_dir):
    return sorted(str(p) for p in Path(input_dir).glob("*.epub"))


def main():
    parser = argparse.ArgumentParser(description="Convert EPUBs to M4B audiobooks")
    parser.add_argument("input_dir", help="Directory containing a single .epub file")
    parser.add_argument("--output-dir", default="output", help="Directory for output files")
    parser.add_argument("--voice-sample", required=True, help="Path to speaker WAV for voice cloning")
    parser.add_argument("--sample-rate", type=int, default=DEFAULT_SAMPLE_RATE)
    parser.add_argument("--silence", type=float, default=1.5)
    parser.add_argument("--min-chars", type=int, default=500)
    parser.add_argument("--max-chunk-chars", type=int, default=500)
    parser.add_argument("--model", default="tts_models/multilingual/multi-dataset/xtts_v2")
    parser.add_argument("--start-chapter", type=int, default=0,
                        help="Resume from this 0-based chapter index (skip earlier chapters)")
    args = parser.parse_args()

    device = "cuda" if torch.cuda.is_available() else "cpu"
    print(f"Using device: {device}")

    epub_files = find_epubs(args.input_dir)
    if not epub_files:
        print("No .epub files found in", args.input_dir)
        return

    print(f"Loading TTS model: {args.model}")
    tts = TTS(args.model).to(device)

    for epub_path in epub_files:
        try:
            process_epub(
                tts, epub_path, args.output_dir, args.voice_sample,
                sample_rate=args.sample_rate, silence_between=args.silence,
                min_chars=args.min_chars, max_chunk_chars=args.max_chunk_chars,
                start_chapter=args.start_chapter,
            )
        except Exception as e:
            print(f"Error processing {epub_path}: {e}")
            raise


if __name__ == "__main__":
    main()
