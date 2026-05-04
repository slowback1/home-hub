# PRD: Epub-to-Audiobook GPU Service

## Status

`Draft`

## Overview

A containerized FastAPI microservice that runs on a dedicated GPU workstation on the home LAN and wraps the [ai-epub-to-audiobook](https://github.com/slowback1/ai-epub-to-audiobook) script. It accepts epub file uploads, manages a serial conversion queue using XTTS_v2, and makes completed M4B audiobooks available for the main HomeHub server to pull. The service is independently deployable and owns its own job state, enabling it to survive restarts mid-conversion and resume from the last completed chapter.

## Problem Statement

The main HomeHub server lacks the GPU resources required for PyTorch/XTTS_v2 TTS inference. Converting an epub to an audiobook can take 24+ hours for long books and must be offloaded to a separate GPU machine. Without this service, conversion requires manually SSHing into the GPU workstation and running scripts by hand — no history, no cancellation, no status visibility from the dashboard.

## Goals

- A containerized Python service (FastAPI) runs on the GPU workstation and is independently startable/stoppable
- Accepts epub uploads and voice sample selection via HTTP
- Manages a serial conversion queue (one job at a time)
- Returns a job ID; the main HomeHub server polls for status and pulls the finished M4B on completion
- Supports chapter-level crash recovery — resumes from the last unfinished chapter after a restart
- Supports job cancellation (SIGTERM the subprocess) via the API
- Manages voice sample assets independently of conversion jobs
- Cleans up completed M4B files from the GPU disk after the main server downloads them

## Non-Goals

- No dashboard UI — this is a headless API service; the UI is covered in the `epub-to-audiobook-ui` PRD
- No push/callback to the main server — communication is strictly unidirectional (main server → GPU service)
- No multi-job parallelism
- No cloud deployment — LAN-only
- No in-browser playback — streaming download only
- No per-job TTS tuning (silence duration, sample rate, chunk sizes) — these are env-var defaults on the GPU service

## User Stories / Use Cases

- **As the** HomeHub backend, **I want to** submit an epub file with a voice sample name and receive a job ID, **so that** I can track conversion progress.
- **As the** HomeHub backend, **I want to** poll a job's status, **so that** I can display progress to the user.
- **As the** HomeHub backend, **I want to** stream-download a completed M4B file and then signal cleanup, **so that** the file is stored on the main server and the GPU disk stays tidy.
- **As the** HomeHub backend, **I want to** cancel an in-progress or queued job, **so that** the user can abort a long conversion without SSHing into the GPU box.
- **As the** HomeHub backend, **I want to** list available voice samples, **so that** the UI can present a selection dropdown.
- **As an** admin, **I want to** upload and delete voice samples on the GPU service, **so that** new voices can be added without modifying config files.
- **As the** HomeHub backend, **I want to** call a health endpoint, **so that** I can detect when the GPU box is offline and surface a clear error.

## Proposed Solution

A FastAPI application running in a Docker container on the GPU workstation (Ubuntu 22.04, same LAN as the main server). Jobs are tracked in a SQLite database on a mounted Docker volume. The service wraps the `ai-epub-to-audiobook` script, passing the epub file, the selected voice sample path, and env-var-configured defaults (silence, sample rate, chunk sizes) as CLI arguments.

Chapter WAV files are written to a mounted volume. On startup, any job left in `in_progress` state is automatically re-queued and resumed from the first chapter WAV that is missing from disk.

When conversion completes, the M4B is staged for download at `GET /jobs/{id}/file`. The main server polls until status is `completed`, pulls the file, then issues `DELETE /jobs/{id}/file` to clean up. All requests from the main server carry a shared API key (`Authorization: Bearer <key>`).

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Framework | FastAPI (Python) | Matches the underlying Python script; async-friendly; natural fit for file upload/streaming |
| Job state | SQLite on mounted Docker volume | Self-contained, survives container restarts, no external DB dependency |
| Communication | Unidirectional — main server polls GPU service | Keeps GPU service as a dumb compute endpoint; avoids callback/push complexity |
| Auth | Shared API key (Bearer token) | Simple; stored in `.env` on GPU box and SystemConfig table on main server |
| Output format | M4B | Built-in chapter support; handled natively by the existing script |
| Voice samples | Managed via dedicated endpoints, referenced by name in job submission | Decouples asset management from job lifecycle |
| Concurrency | Serial queue (one job at a time) | XTTS_v2 uses ~70% VRAM; parallel jobs would OOM |
| Crash recovery | Chapter-level resume | Script produces per-chapter WAVs; resume by scanning for missing chapters on startup |
| Job cancellation | `DELETE /jobs/{id}` — SIGTERM the subprocess | Allows aborting 24-hour jobs from the dashboard |
| File cleanup | Main server issues `DELETE /jobs/{id}/file` after successful download | GPU disk (~256 GB) stays bounded without manual intervention |
| GPU offline behavior | Fail fast with a clear error | Main server surfaces "GPU service offline" immediately; no silent retry queue |
| GPU container access | NVIDIA Container Toolkit | Required for PyTorch CUDA inside Docker; must be installed before deployment |

### API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/health` | Health check — used by the main server to detect if the GPU box is online |
| POST | `/jobs` | Submit a new conversion job (multipart: `epub_file`, `voice_sample_name`) |
| GET | `/jobs` | List all jobs (id, status, filename, timestamps) |
| GET | `/jobs/{id}` | Get job status and metadata |
| DELETE | `/jobs/{id}` | Cancel a queued or in-progress job (SIGTERM subprocess) |
| GET | `/jobs/{id}/file` | Stream the completed M4B file |
| DELETE | `/jobs/{id}/file` | Delete the completed M4B from GPU disk (called by main server after download) |
| GET | `/voice-samples` | List available voice sample names |
| POST | `/voice-samples` | Upload a new WAV voice sample |
| DELETE | `/voice-samples/{name}` | Delete a voice sample |

### Job Status State Machine

```
queued → in_progress → completed
                     ↘ failed
queued     → cancelled
in_progress → cancelled
```

On service startup: any job in `in_progress` state is moved back to `queued` and re-entered into the serial queue; the worker resumes from the first missing chapter WAV.

### Docker Volume Mounts

| Mount path | Purpose |
|------------|---------|
| `/data/jobs` | SQLite DB, per-job chapter WAVs, completed M4B files |
| `/data/voice-samples` | WAV voice sample files |

### Environment Variables (GPU Service)

| Variable | Purpose |
|----------|---------|
| `API_KEY` | Shared secret — all inbound requests must present this as a Bearer token |
| `TTS_SILENCE` | Silence between chunks in seconds (default: `1.5`) |
| `TTS_SAMPLE_RATE` | Audio sample rate (default: `22050`) |
| `TTS_MIN_CHARS` | Minimum characters per TTS chunk (default: `500`) |
| `TTS_MAX_CHARS` | Maximum characters per TTS chunk (default: `500`) |
| `TTS_MODEL` | TTS model identifier (default: `tts_models/multilingual/multi-dataset/xtts_v2`) |

### NVIDIA Container Toolkit Setup (pre-deployment step)

Must be run once on the GPU workstation before deploying the container:

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

### Dependencies

- [ai-epub-to-audiobook](https://github.com/slowback1/ai-epub-to-audiobook) — underlying conversion script
- **XTTS_v2** (`tts_models/multilingual/multi-dataset/xtts_v2`) — TTS model (downloaded on first run)
- **ffmpeg** — M4B assembly (must be available on PATH inside the container)
- **PyTorch with CUDA** — GPU inference
- **NVIDIA Container Toolkit** — GPU passthrough into Docker
- **FastAPI + uvicorn** — HTTP service
- **SQLite** — job state persistence

## Open Questions

- [ ] How much VRAM does the GPU workstation have? This determines whether model weights can stay loaded between jobs or must be reloaded each time (affects per-job cold-start latency).
- [ ] Should XTTS_v2 model weights be baked into the Docker image at build time (larger image, faster cold start) or downloaded on first run (smaller image, slow first job)?
- [ ] What port should the service listen on? Suggest `8765` to avoid conflicts with common dev ports — confirm there's no conflict on the GPU workstation.
- [ ] Should the `GPU_SERVICE_URL` SystemConfig entry on the main server be seeded via an EF migration, or configured manually in the admin UI after deployment?

## Out of Scope

- Dashboard UI for submitting jobs and viewing history (covered in `epub-to-audiobook-ui` PRD)
- Multiple concurrent conversion jobs
- Cloud or remote GPU support
- In-browser audiobook playback
- Per-job exposure of TTS tuning parameters (silence, sample rate, chunk sizes)
- Automatic retry of failed jobs

## Success Metrics

- A submitted epub job progresses from `queued` → `in_progress` → `completed` without manual intervention
- A job interrupted mid-conversion resumes from the correct chapter after a service restart
- The main HomeHub server can submit, poll, download, and cancel jobs entirely via the HTTP API
- GPU disk usage remains bounded — completed M4B files are deleted after the main server downloads them
- The service returns a clear error on health check when the GPU box is unreachable

## Timeline / Milestones

_TBD_
