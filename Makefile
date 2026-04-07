.PHONY: up down logs test lint format

up:
	docker compose up --build

down:
	docker compose down

logs:
	docker compose logs -f api

test:
	cd api && pytest -q

lint:
	cd api && ruff check app

format:
	cd api && ruff format app
