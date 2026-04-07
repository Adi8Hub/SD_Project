from datetime import datetime

from pydantic import BaseModel, HttpUrl


class CreateShortURLRequest(BaseModel):
    original_url: HttpUrl


class CreateShortURLResponse(BaseModel):
    short_code: str
    short_url: str
    original_url: str


class ShortURLStats(BaseModel):
    short_code: str
    original_url: str
    clicks: int
    created_at: datetime
