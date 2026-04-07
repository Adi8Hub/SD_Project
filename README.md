# Scalable URL Shortener

A GitHub-ready backend project for a production-style, scalable URL shortener.

## What this project includes
- FastAPI service with endpoints for shorten, redirect, and stats.
- PostgreSQL persistence for durable URL mappings.
- Redis cache for low-latency redirects.
- Containerized local development via Docker Compose.
- CI workflow for lint + tests.
- Architecture notes for future extension.

## Repository Structure

```text
.
├── .github/workflows/ci.yml
├── api
│   ├── app
│   │   ├── config.py
│   │   ├── crud.py
│   │   ├── database.py
│   │   ├── main.py
│   │   ├── models.py
│   │   ├── routes.py
│   │   ├── schemas.py
│   │   └── utils.py
│   ├── Dockerfile
│   └── requirements.txt
├── docs/architecture.md
├── docker-compose.yml
├── Makefile
└── README.md
```

For a dedicated, maintained tree snapshot, see `docs/file_structure.md`.

## Quick Start

1. Clone the repository.
2. Copy env file:
   ```bash
   cp .env.example .env
   ```
3. Start services:
   ```bash
   make up
   ```
4. Open API docs:
   - http://localhost:8000/docs

## API Endpoints

- `POST /api/v1/shorten`
  - Request body:
    ```json
    { "original_url": "https://example.com/a/very/long/url" }
    ```
  - Response:
    ```json
    {
      "short_code": "Ab1xYz9",
      "short_url": "http://localhost:8000/Ab1xYz9",
      "original_url": "https://example.com/a/very/long/url"
    }
    ```

- `GET /{short_code}`
  - Redirects to the original URL.

- `GET /api/v1/stats/{short_code}`
  - Returns click count + metadata.

## Scalability Guidance (for future evolution)

- **Cache-first redirects**: Keep hot key mappings in Redis.
- **Database scaling**: Add read replicas, partition hot tables, and tune indexes.
- **Event-driven analytics**: Emit click events to Kafka, aggregate asynchronously.
- **Abuse controls**: Add IP/user rate limits and malicious URL detection.
- **Observability**: Add tracing, metrics, structured logs, and SLO dashboards.

## Deployment Notes

- Suitable for container deployments (Docker, ECS, Kubernetes).
- Add managed Postgres + Redis for production.
- Add HTTPS ingress, secrets manager, and autoscaling policy.

## Suggested Next Milestones

1. Add custom alias support.
2. Add user accounts and ownership.
3. Add expiration policies and QR code generation.
4. Add background jobs and real-time analytics dashboards.
5. Add Terraform for cloud infra provisioning.

## Important note on GitHub push automation

This repository is prepared and committed locally. To push to your GitHub account, configure a remote and push:

```bash
git remote add origin git@github.com:<your-username>/<repo-name>.git
git push -u origin work
```

If you want, this can be switched to HTTPS remote format as well.

You can also use the helper script:

```bash
./scripts/push_to_github.sh origin https://github.com/adi8hub/SD_Project.git work
```

Parameters are optional and default to:
- remote: `origin`
- URL: `https://github.com/adi8hub/SD_Project.git`
- branch: current branch

## Authenticate this environment (step-by-step)

Before any push can work, this environment must be authenticated to your GitHub account.

### Option A: HTTPS + Personal Access Token (recommended if `gh` is unavailable)

1. Create a token on GitHub:
   - GitHub → **Settings** → **Developer settings** → **Personal access tokens**.
   - Create a token with repository write access.
2. In this repository, set the remote:
   ```bash
   git remote add origin https://github.com/adi8hub/SD_Project.git
   ```
   If `origin` already exists:
   ```bash
   git remote set-url origin https://github.com/adi8hub/SD_Project.git
   ```
3. Push once and use token when prompted:
   ```bash
   git push -u origin work
   ```
   - Username: `adi8hub`
   - Password: paste your GitHub token (not your GitHub password)
4. Optional: cache credentials on your machine:
   ```bash
   git config --global credential.helper store
   ```

### Option B: SSH key (good for long-term use)

1. Generate a key (if needed):
   ```bash
   ssh-keygen -t ed25519 -C "your_email@example.com"
   ```
2. Start agent + add key:
   ```bash
   eval "$(ssh-agent -s)"
   ssh-add ~/.ssh/id_ed25519
   ```
3. Copy and add key to GitHub:
   ```bash
   cat ~/.ssh/id_ed25519.pub
   ```
   - GitHub → **Settings** → **SSH and GPG keys** → **New SSH key**.
4. Set SSH remote and push:
   ```bash
   git remote set-url origin git@github.com:adi8hub/SD_Project.git
   git push -u origin work
   ```

## Troubleshooting GitHub remote setup

If you see:

```text
fatal: not a git repository (or any of the parent directories): .git
```

it means the command is being run outside the local repository folder.

Use this exact sequence:

```bash
# 1) Go to your local project directory
cd /path/to/your/local/SD_Project

# 2) Verify this folder is a git repo
git rev-parse --is-inside-work-tree

# 3) If this prints "true", set the remote
git remote add origin https://github.com/adi8hub/SD_Project.git

# 4) Push your branch (work or main, whichever you use locally)
git push -u origin work
```

If step 2 fails, initialize git in that local folder first:

```bash
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/adi8hub/SD_Project.git
git push -u origin main
```
