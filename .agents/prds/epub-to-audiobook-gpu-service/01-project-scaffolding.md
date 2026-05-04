# Project Scaffolding

## Status

`done`

## Description

Stand up the Python FastAPI project for the GPU service. This includes the full directory structure, a CUDA-capable Dockerfile, docker-compose.yml with GPU passthrough and volume mounts, dependency manifest, environment variable template, and a README covering the NVIDIA Container Toolkit prerequisite setup.

## Acceptance Criteria

- [ ] FastAPI application boots (`uvicorn app.main:app`) with no errors
- [ ] Dockerfile uses a CUDA-capable base image (e.g. `nvidia/cuda`) and installs all Python dependencies
- [ ] `docker-compose.yml` configures `--gpus all`, mounts `/data/jobs` and `/data/voice-samples` as named volumes
- [ ] `.env.example` documents all required env vars (`API_KEY`, `TTS_SILENCE`, `TTS_SAMPLE_RATE`, `TTS_MIN_CHARS`, `TTS_MAX_CHARS`, `TTS_MODEL`)
- [ ] README includes step-by-step NVIDIA Container Toolkit installation instructions for Ubuntu 22.04

## Notes

NVIDIA Container Toolkit setup steps (must be documented in README):
```bash
curl -fsSL https://nvidia.github.io/libnvidia-container/gpgkey \
  | sudo gpg --dearmor -o /usr/share/keyrings/nvidia-container-toolkit-keyring.gpg
curl -s -L https://nvidia.github.io/libnvidia-container/stable/deb/nvidia-container-toolkit.list \
  | sed 's#deb https://#deb [signed-by=/usr/share/keyrings/nvidia-container-toolkit-keyring.gpg] https://#g' \
  | sudo tee /etc/apt/sources.list.d/nvidia-container-toolkit.list
sudo apt-get update && sudo apt-get install -y nvidia-container-toolkit
sudo nvidia-ctk runtime configure --runtime=docker
sudo systemctl restart docker
```

Key dependencies: `fastapi`, `uvicorn`, `python-multipart`, `torch` (CUDA build), `TTS` (Coqui), `ffmpeg` (system package in Dockerfile).
