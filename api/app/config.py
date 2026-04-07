from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    app_name: str = "Scalable URL Shortener"
    env: str = "development"
    database_url: str
    redis_url: str
    base_url: str
    short_code_length: int = 7

    model_config = SettingsConfigDict(env_file="../.env.example", extra="ignore")


settings = Settings()
