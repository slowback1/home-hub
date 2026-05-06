FROM nvidia/cuda:12.4.1-runtime-ubuntu22.04

ENV DEBIAN_FRONTEND=noninteractive
ENV PYTHONUNBUFFERED=1

RUN apt-get update && apt-get install -y \
    python3 \
    python3-dev \
    python3-pip \
    ffmpeg \
    curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY requirements.txt .
RUN pip3 install --extra-index-url https://download.pytorch.org/whl/cu124 -r requirements.txt

COPY . .

EXPOSE 8765

CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8765"]
