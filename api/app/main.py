from fastapi import FastAPI

from app.config import settings
from app.database import Base, engine
from app.routes import router

app = FastAPI(title=settings.app_name)


@app.on_event("startup")
def startup() -> None:
    Base.metadata.create_all(bind=engine)


app.include_router(router)
