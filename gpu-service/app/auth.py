import os
from fastapi import Depends, HTTPException, status
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer

_bearer = HTTPBearer(auto_error=False)


def require_api_key(credentials: HTTPAuthorizationCredentials | None = Depends(_bearer)) -> None:
    api_key = os.getenv("API_KEY", "")
    if not api_key:
        raise RuntimeError("API_KEY environment variable is not set")
    if credentials is None or credentials.credentials != api_key:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid or missing API key")
