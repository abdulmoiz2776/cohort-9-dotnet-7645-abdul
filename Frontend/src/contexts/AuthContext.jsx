import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import api from '../api/httpClient';
import { clearAuthTokens, getAuthTokens, saveAuthTokens } from '../api/authStorage';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadCurrentUser = async () => {
    try {
      const response = await api.get('/api/users/me');
      setUser(response.data);
      setIsAuthenticated(true);
    } catch (exception) {
      clearAuthTokens();
      setUser(null);
      setIsAuthenticated(false);
    }
  };

  useEffect(() => {
    const tokens = getAuthTokens();

    if (tokens.accessToken) {
      loadCurrentUser().finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, []);

  const login = async ({ email, password, rememberMe }) => {
    setError('');
    const response = await api.post('/api/auth/login', { email, password, rememberMe });

    if (!response.data.succeeded) {
      throw new Error(response.data.message || 'Login failed.');
    }

    saveAuthTokens({
      accessToken: response.data.accessToken,
      refreshToken: response.data.refreshToken
    });

    setIsAuthenticated(true);
    await loadCurrentUser();
  };

  const register = async (payload) => {
    setError('');
    const response = await api.post('/api/auth/register', payload);

    if (!response.data.succeeded) {
      throw new Error(response.data.message || 'Registration failed.');
    }

    return response.data;
  };

  const logout = async () => {
    const tokens = getAuthTokens();

    if (tokens.refreshToken) {
      try {
        await api.post('/api/auth/logout', { refreshToken: tokens.refreshToken });
      } catch {
        // ignore logout errors, clear local state anyway
      }
    }

    clearAuthTokens();
    setUser(null);
    setIsAuthenticated(false);
  };

  const verifyEmail = async ({ userId, token }) => {
    const response = await api.get('/api/auth/verify-email', {
      params: { userId, token }
    });
    return response.data;
  };

  const updateProfile = async (payload) => {
    const response = await api.put('/api/users/me', payload);
    setUser(response.data);
    return response.data;
  };

  const changePassword = async (payload) => {
    const response = await api.post('/api/auth/change-password', payload);
    return response.data;
  };

  const getSessions = async () => {
    const response = await api.get('/api/auth/sessions');
    return response.data;
  };

  const revokeSession = async (sessionId) => {
    const response = await api.delete(`/api/auth/sessions/${sessionId}`);
    return response.data;
  };

  const refreshSession = async () => {
    const tokens = getAuthTokens();
    if (!tokens.refreshToken) {
      throw new Error('Refresh token missing.');
    }

    const response = await api.post('/api/auth/refresh', { refreshToken: tokens.refreshToken });

    saveAuthTokens({ accessToken: response.data.accessToken, refreshToken: response.data.refreshToken });

    return response.data;
  };

  const value = useMemo(
    () => ({
      user,
      isAuthenticated,
      loading,
      error,
      login,
      logout,
      register,
      verifyEmail,
      updateProfile,
      changePassword,
      getSessions,
      revokeSession,
      refreshSession
    }),
    [user, isAuthenticated, loading, error]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider.');
  }
  return context;
};
