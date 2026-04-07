# Scalable URL Shortener - Architecture

## Core Components
- **API Service (FastAPI)**: Provides REST endpoints for URL creation, redirect, and analytics lookup.
- **PostgreSQL**: Durable storage for canonical mappings and click counters.
- **Redis**: Low-latency cache for hot short-code lookups.

## Request Flow
1. Client sends `POST /api/v1/shorten` with a long URL.
2. API generates a unique short code and writes the mapping to PostgreSQL.
3. API stores the mapping in Redis with a TTL.
4. User opens `/{short_code}`.
5. API checks Redis first; on miss, resolves from PostgreSQL and refreshes Redis.
6. API increments click count and issues a redirect.

## Scale Strategy
- Horizontal API replicas behind a load balancer.
- Redis handles read-heavy traffic and protects PostgreSQL.
- PostgreSQL can be expanded with read replicas and partitioning.
- Add background workers for analytics/event streaming if traffic grows.

## Production Evolution
- Add Kafka for event streams and asynchronous analytics.
- Introduce consistent hash routing for cache locality.
- Add rate limiting, abuse detection, and observability stack (Prometheus + Grafana + OpenTelemetry).
