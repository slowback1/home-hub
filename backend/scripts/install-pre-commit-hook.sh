cd "$(git rev-parse --show-toplevel)" || exit 1

mkdir -p .git/hooks

cat > .git/hooks/pre-commit <<'HOOK'
#!/usr/bin/env bash
set -e
task precommit

HOOK

chmod +x .git/hooks/pre-commit