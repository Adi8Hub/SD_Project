from sqlalchemy.orm import Session

from app import models
from app.utils import generate_short_code


def create_short_url(db: Session, original_url: str, code_length: int) -> models.ShortURL:
    for _ in range(10):
        code = generate_short_code(code_length)
        existing = db.query(models.ShortURL).filter_by(short_code=code).first()
        if not existing:
            row = models.ShortURL(short_code=code, original_url=original_url)
            db.add(row)
            db.commit()
            db.refresh(row)
            return row
    raise RuntimeError("Unable to allocate unique short code")


def get_by_code(db: Session, short_code: str) -> models.ShortURL | None:
    return db.query(models.ShortURL).filter_by(short_code=short_code).first()


def increment_clicks(db: Session, row: models.ShortURL) -> None:
    row.clicks += 1
    db.add(row)
    db.commit()
