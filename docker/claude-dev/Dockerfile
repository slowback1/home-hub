FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN apt-get update && apt-get install -y \
    curl \
    wget \
    git \
    ca-certificates \
    gnupg \
    docker.io \
    && rm -rf /var/lib/apt/lists/*

RUN curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*

# Install Playwright system dependencies for Chromium (must run as root)
RUN npx -y playwright@1.49.0 install-deps chromium

RUN sh -c "$(curl --location https://taskfile.dev/install.sh)" -- -d -b /usr/local/bin

RUN npm install -g @anthropic-ai/claude-code

RUN existing=$(getent passwd 1000 | cut -d: -f1) \
    && [ -n "$existing" ] && userdel "$existing" || true \
    && useradd -m -s /bin/bash -u 1000 claude \
    && usermod -aG docker claude

USER claude
WORKDIR /workspace
