"use client";
import React, { createContext, useContext, useEffect, useState } from "react";
import { AuthState, AuthUser } from "../types/authType";
import { tokenStorage } from "@/lib/auth/tokenStorage";

interface AuthContextType extends AuthState {
  login: (token: string, refreshToken: string, user: AuthUser) => void;
  logout: () => void;
  setLoading: (loading: boolean) => void;
  refreshToken: () => Promise<boolean>;
  getToken: () => string | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [state, setState] = useState<AuthState>({
    user: null,
    token: null,
    isAuthenticated: false,
    isLoading: true,
  });

  // Initialize auth state on mount
  useEffect(() => {
    initializeAuth();
  }, []);

  const initializeAuth = () => {
    try {
      const isAuth = tokenStorage.isAuthenticated();
      setIsAuthenticated(isAuth);

      if (isAuth) {
        const payload = tokenStorage.getTokenPayload();
        const storedUser = tokenStorage.getUser();

        if (payload && storedUser) {
          setUser(storedUser);
        } else {
          logout();
        }
      }
    } catch (error) {
      console.error("Auth initialization error:", error);
      logout();
    } finally {
      setLoading(false);
    }
  };

  const setUser = (user: AuthUser) => {
    setState(prev => ({ ...prev, user }));
  };

  const setIsAuthenticated = (isAuthenticated: boolean) => {
    setState(prev => ({ ...prev, isAuthenticated }));
  };

  const setLoading = (loading: boolean) => {
    setState(prev => ({ ...prev, isLoading: loading }));
  };

  const login = (token: string, refreshToken: string, user: AuthUser) => {
    // Store tokens in cookies
    tokenStorage.setToken(token);
    tokenStorage.setRefreshToken(refreshToken);
    tokenStorage.setUser(user);

    setState({ user, token, isAuthenticated: true, isLoading: false });
  };

  const logout = () => {
    tokenStorage.clear();

    setState({
      user: null,
      token: null,
      isAuthenticated: false,
      isLoading: false,
    });
  };

  const refreshToken = async (): Promise<boolean> => {
    try {
      const refreshTokenValue = tokenStorage.getRefreshToken();
      if (!refreshTokenValue) {
        logout();
        return false;
      }

      // Note: You'll need to implement the refresh token endpoint
      // For now, we'll just return false and logout
      // const response = await fetch('/api/auth/refresh', {
      //   method: 'POST',
      //   headers: {
      //     'Content-Type': 'application/json',
      //   },
      //   body: JSON.stringify({ refreshToken: refreshTokenValue }),
      // });

      // const data = await response.json();

      // if (response.ok && data.token) {
      //   tokenStorage.setToken(data.token);
      //   if (data.refreshToken) {
      //     tokenStorage.setRefreshToken(data.refreshToken);
      //   }
      //   return true;
      // } else {
      //   logout();
      //   return false;
      // }

      // For now, just logout if refresh is needed
      logout();
      return false;
    } catch (error) {
      console.error("Token refresh error:", error);
      logout();
      return false;
    }
  };

  // Auto-refresh token when it's about to expire
  useEffect(() => {
    if (!state.isAuthenticated) return;

    const checkTokenExpiry = () => {
      const payload = tokenStorage.getTokenPayload();
      if (!payload) {
        logout();
        return;
      }

      const currentTime = Math.floor(Date.now() / 1000);
      const timeUntilExpiry = payload.exp - currentTime;

      // Refresh token 5 minutes before expiry
      if (timeUntilExpiry < 300 && timeUntilExpiry > 0) {
        refreshToken();
      } else if (timeUntilExpiry <= 0) {
        logout();
      }
    };

    // Check every minute
    const interval = setInterval(checkTokenExpiry, 60000);

    return () => clearInterval(interval);
  }, [state.isAuthenticated]);

  const getToken = (): string | null => {
    return tokenStorage.getToken();
  };

  const value: AuthContextType = {
    ...state,
    login,
    logout,
    setLoading,
    refreshToken,
    getToken,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuthContext() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuthContext must be used within an AuthProvider");
  }
  return context;
}
