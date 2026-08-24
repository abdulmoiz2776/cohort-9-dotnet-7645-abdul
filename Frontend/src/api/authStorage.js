const ACCESS_TOKEN_KEY = 'tm_access_token';
const REFRESH_TOKEN_KEY = 'tm_refresh_token';

export function getAuthTokens() {
  return {
    accessToken: localStorage.getItem(ACCESS_TOKEN_KEY) ?? '',
    refreshToken: localStorage.getItem(REFRESH_TOKEN_KEY) ?? ''
  };
}

export function saveAuthTokens({ accessToken, refreshToken }) {
  if (accessToken) {
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  }

  if (refreshToken) {
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  }
}

export function saveAccessToken(accessToken) {
  if (accessToken) {
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  }
}

export function clearAuthTokens() {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
}
