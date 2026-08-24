import axios from 'axios';
import { clearAuthTokens, getAuthTokens, saveAuthTokens, saveAccessToken } from './authStorage';

const baseURL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7097';

const api = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json'
  }
});

let isRefreshing = false;
let refreshSubscribers = [];

const notifySubscribers = (token) => {
  refreshSubscribers.forEach((callback) => callback(token));
  refreshSubscribers = [];
};

const addRefreshSubscriber = (callback) => {
  refreshSubscribers.push(callback);
};

api.interceptors.request.use((config) => {
  const tokens = getAuthTokens();

  if (tokens.accessToken && config.headers) {
    config.headers.Authorization = `Bearer ${tokens.accessToken}`;
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (
      error.response?.status === 401 &&
      originalRequest &&
      !originalRequest._retry &&
      !originalRequest.url?.includes('/api/auth/refresh')
    ) {
      originalRequest._retry = true;

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          addRefreshSubscriber((token) => {
            if (token) {
              originalRequest.headers.Authorization = `Bearer ${token}`;
              resolve(api(originalRequest));
            } else {
              reject(error);
            }
          });
        });
      }

      isRefreshing = true;
      const { refreshToken } = getAuthTokens();

      if (!refreshToken) {
        clearAuthTokens();
        isRefreshing = false;
        notifySubscribers(null);
        return Promise.reject(error);
      }

      try {
        const { data } = await axios.post(
          `${baseURL}/api/auth/refresh`,
          { refreshToken },
          { baseURL, headers: { 'Content-Type': 'application/json' } }
        );

        saveAuthTokens({ accessToken: data.accessToken, refreshToken: data.refreshToken });
        saveAccessToken(data.accessToken);

        api.defaults.headers.common.Authorization = `Bearer ${data.accessToken}`;
        originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;

        notifySubscribers(data.accessToken);

        return api(originalRequest);
      } catch (refreshError) {
        clearAuthTokens();
        notifySubscribers(null);
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

export default api;
