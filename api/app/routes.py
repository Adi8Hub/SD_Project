from fastapi import APIRouter, Depends, HTTPException
from fastapi.responses import RedirectResponse
from redis import Redis
from sqlalchemy.orm import Session

from app.config import settings
from app.crud import create_short_url, get_by_code, increment_clicks
from app.database import get_db
from app.schemas import CreateShortURLRequest, CreateShortURLResponse, ShortURLStats

router = APIRouter()
cache = Redis.from_url(settings.redis_url, decode_responses=True)


@router.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@router.post("/api/v1/shorten", response_model=CreateShortURLResponse)
def shorten(payload: CreateShortURLRequest, db: Session = Depends(get_db)) -> CreateShortURLResponse:
    row = create_short_url(db, str(payload.original_url), settings.short_code_length)
    cache.setex(f"code:{row.short_code}", 3600, row.original_url)
    return CreateShortURLResponse(
        short_code=row.short_code,
        short_url=f"{settings.base_url}/{row.short_code}",
        original_url=row.original_url,
    )


@router.get("/{short_code}")
def redirect(short_code: str, db: Session = Depends(get_db)) -> RedirectResponse:
    cached = cache.get(f"code:{short_code}")
    if cached:
        row = get_by_code(db, short_code)
        if row:
            increment_clicks(db, row)
        return RedirectResponse(url=cached, status_code=307)

    row = get_by_code(db, short_code)
    if not row:
        raise HTTPException(status_code=404, detail="Short code not found")

    cache.setex(f"code:{short_code}", 3600, row.original_url)
    increment_clicks(db, row)
    return RedirectResponse(url=row.original_url, status_code=307)


@router.get("/api/v1/stats/{short_code}", response_model=ShortURLStats)
def stats(short_code: str, db: Session = Depends(get_db)) -> ShortURLStats:
    row = get_by_code(db, short_code)
    if not row:
        raise HTTPException(status_code=404, detail="Short code not found")

    return ShortURLStats(
        short_code=row.short_code,
        original_url=row.original_url,
        clicks=row.clicks,
        created_at=row.created_at,
    )
