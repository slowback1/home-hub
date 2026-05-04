# Chapter-Level Crash Recovery

## Status

`done`

## Description

Enable the service to resume a conversion that was interrupted mid-run. This requires two changes: (1) fork/vendor the `ai-epub-to-audiobook` script to add a `--start-chapter` parameter that skips already-generated WAVs, and (2) add startup logic to the GPU service that detects any `in_progress` job, scans its chapter WAVs to determine the resume offset, and re-queues it with that offset.

## Acceptance Criteria

- [ ] The vendored script accepts a `--start-chapter <N>` argument and skips synthesis for chapters with indices below N
- [ ] On service startup, any job in `in_progress` state is reset to `queued` and the resume chapter index is derived by counting existing chapter WAV files in `/data/jobs/<job_id>/`
- [ ] The worker passes `--start-chapter <N>` to the subprocess when a resume offset is present; N defaults to 0 for new jobs
- [ ] A job with 5 of 10 chapters already converted resumes from chapter 6 rather than starting over
- [ ] Existing chapter WAVs are not deleted or re-synthesized on resume
- [ ] Unit tests cover: fresh job starts at chapter 0, interrupted job with N existing WAVs resumes at chapter N, startup scan correctly counts existing WAVs

## Notes

The upstream script lives at https://github.com/slowback1/ai-epub-to-audiobook. Vendor it into the service repository (e.g. `app/vendor/epub_to_audiobook/`) rather than installing as a package, so the `--start-chapter` modification can be maintained locally. The chapter WAV naming convention the script uses (e.g. `chapter_001.wav`) determines the scan logic — verify the exact naming before implementing the counter.
