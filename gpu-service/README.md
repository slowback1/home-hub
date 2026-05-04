# Epub-to-Audiobook GPU Service

A FastAPI microservice that converts epub files to M4B audiobooks using XTTS_v2 TTS on a dedicated GPU workstation.

## Prerequisites

### 1. NVIDIA Container Toolkit (Ubuntu 22.04)

Run once on the GPU workstation before deploying:

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

Verify GPU passthrough works:
```bash
docker run --rm --gpus all nvidia/cuda:12.4.1-runtime-ubuntu22.04 nvidia-smi
```

### 2. Docker (already installed)

## Configuration

```bash
cp .env.example .env
# Edit .env and set a strong API_KEY
```

## Running

```bash
docker compose up -d
```

The service listens on port `8765`. Check health:
```bash
curl http://localhost:8765/health
```

## Development

```bash
pip install -r requirements-dev.txt
pytest
uvicorn app.main:app --reload
```

## API

| Method | Path | Description |
|--------|------|-------------|
| GET | `/health` | Health check (no auth required) |
| POST | `/jobs` | Submit a conversion job |
| GET | `/jobs` | List all jobs |
| GET | `/jobs/{id}` | Get job status |
| DELETE | `/jobs/{id}` | Cancel a job |
| GET | `/jobs/{id}/file` | Download completed M4B |
| DELETE | `/jobs/{id}/file` | Clean up completed file |
| GET | `/voice-samples` | List voice samples |
| POST | `/voice-samples` | Upload a voice sample |
| DELETE | `/voice-samples/{name}` | Delete a voice sample |
