#!/usr/bin/env bash
set -euo pipefail

REMOTE_NAME="${1:-origin}"
REMOTE_URL="${2:-https://github.com/adi8hub/SD_Project.git}"
BRANCH="${3:-$(git branch --show-current)}"

if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "Error: current directory is not a git repository."
  exit 1
fi

if [ -z "$BRANCH" ]; then
  echo "Error: could not detect current branch."
  exit 1
fi

if git remote get-url "$REMOTE_NAME" >/dev/null 2>&1; then
  git remote set-url "$REMOTE_NAME" "$REMOTE_URL"
  echo "Updated remote '$REMOTE_NAME' -> $REMOTE_URL"
else
  git remote add "$REMOTE_NAME" "$REMOTE_URL"
  echo "Added remote '$REMOTE_NAME' -> $REMOTE_URL"
fi

echo "Pushing branch '$BRANCH'..."
git push -u "$REMOTE_NAME" "$BRANCH"
