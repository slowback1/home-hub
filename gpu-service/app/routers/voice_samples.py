import os
from fastapi import APIRouter, Depends, HTTPException, UploadFile, status
from app.auth import require_api_key

router = APIRouter(prefix="/voice-samples", tags=["voice-samples"])


def get_voice_samples_dir() -> str:
    return os.getenv("VOICE_SAMPLES_DIR", "/data/voice-samples")


@router.get("", dependencies=[Depends(require_api_key)])
def list_voice_samples() -> list[str]:
    samples_dir = get_voice_samples_dir()
    os.makedirs(samples_dir, exist_ok=True)
    return [os.path.splitext(e.name)[0] for e in sorted(os.scandir(samples_dir), key=lambda e: e.name) if e.name.endswith(".wav")]


@router.post("", status_code=status.HTTP_201_CREATED, dependencies=[Depends(require_api_key)])
async def upload_voice_sample(file: UploadFile) -> dict:
    if not file.filename or not file.filename.lower().endswith(".wav"):
        raise HTTPException(status_code=400, detail="Only .wav files are accepted")
    name = os.path.splitext(file.filename)[0]
    samples_dir = get_voice_samples_dir()
    os.makedirs(samples_dir, exist_ok=True)
    dest = os.path.join(samples_dir, f"{name}.wav")
    content = await file.read()
    with open(dest, "wb") as f:
        f.write(content)
    return {"name": name}


@router.delete("/{name}", status_code=status.HTTP_204_NO_CONTENT, dependencies=[Depends(require_api_key)])
def delete_voice_sample(name: str) -> None:
    path = os.path.join(get_voice_samples_dir(), f"{name}.wav")
    if not os.path.exists(path):
        raise HTTPException(status_code=404, detail=f"Voice sample '{name}' not found")
    os.remove(path)
