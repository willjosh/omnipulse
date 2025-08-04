// Native JavaScript Cookie Implementation
const TOKEN_KEY = "jwt_token";
const REFRESH_TOKEN_KEY = "refresh_token";
const USER_COOKIE_NAME = "auth_user";

interface CookieOptions {
  expires?: number; // days
  secure?: boolean;
  sameSite?: "strict" | "lax" | "none";
  path?: string;
  httpOnly?: boolean; // Note: httpOnly can't be set from client-side JS
}

class CookieManager {
  // Set cookie with options
  setCookie(name: string, value: string, options: CookieOptions = {}): boolean {
    try {
      if (typeof window === "undefined") return false;

      const {
        expires = 1, // days
        secure = process.env.NODE_ENV === "production",
        sameSite = "strict",
        path = "/",
        httpOnly = false, // Note: httpOnly can't be set from client-side JS
      } = options;

      let cookieString = `${encodeURIComponent(name)}=${encodeURIComponent(value)}`;

      // Add expiration
      if (expires) {
        const date = new Date();
        date.setTime(date.getTime() + expires * 24 * 60 * 60 * 1000);
        cookieString += `; expires=${date.toUTCString()}`;
      }

      // Add path
      if (path) {
        cookieString += `; path=${path}`;
      }

      // Add secure flag
      if (secure) {
        cookieString += `; secure`;
      }

      // Add sameSite
      if (sameSite) {
        cookieString += `; samesite=${sameSite}`;
      }

      document.cookie = cookieString;
      return true;
    } catch (error) {
      console.error("Error setting cookie:", error);
      return false;
    }
  }

  // Get cookie value
  getCookie(name: string): string | null {
    try {
      if (typeof window === "undefined") return null;

      const nameEQ = encodeURIComponent(name) + "=";
      const cookies = document.cookie.split(";");

      for (let cookie of cookies) {
        let c = cookie.trim();
        if (c.indexOf(nameEQ) === 0) {
          return decodeURIComponent(c.substring(nameEQ.length));
        }
      }
      return null;
    } catch (error) {
      console.error("Error getting cookie:", error);
      return null;
    }
  }

  // Delete cookie
  deleteCookie(name: string, path: string = "/"): boolean {
    try {
      if (typeof window === "undefined") return false;

      document.cookie = `${encodeURIComponent(name)}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=${path};`;
      return true;
    } catch (error) {
      console.error("Error deleting cookie:", error);
      return false;
    }
  }

  // Check if cookies are available
  areCookiesAvailable(): boolean {
    try {
      if (typeof window === "undefined") return false;

      const testKey = "__cookie_test__";
      this.setCookie(testKey, "test", { expires: 1 });
      const available = this.getCookie(testKey) === "test";
      this.deleteCookie(testKey);
      return available;
    } catch (error) {
      return false;
    }
  }
}

const cookieManager = new CookieManager();

// Cookie configuration for security
const cookieOptions: CookieOptions = {
  expires: 1, // 1 day for access token
  secure: process.env.NODE_ENV === "production",
  sameSite: "strict",
  path: "/",
};

const refreshCookieOptions: CookieOptions = {
  expires: 7, // 7 days for refresh token
  secure: process.env.NODE_ENV === "production",
  sameSite: "strict",
  path: "/",
};

export const tokenStorage = {
  // Check if cookies are supported
  isSupported: (): boolean => cookieManager.areCookiesAvailable(),

  // Store JWT token
  setToken: (token: string): boolean => {
    if (!tokenStorage.isSupported()) {
      console.warn("Cookies not supported, falling back to memory storage");
      if (typeof window !== "undefined") {
        sessionStorage.setItem(TOKEN_KEY, token);
      }
      return true;
    }
    return cookieManager.setCookie(TOKEN_KEY, token, cookieOptions);
  },

  // Get JWT token
  getToken: (): string | null => {
    if (!tokenStorage.isSupported()) {
      if (typeof window !== "undefined") {
        return sessionStorage.getItem(TOKEN_KEY);
      }
      return null;
    }
    return cookieManager.getCookie(TOKEN_KEY);
  },

  // Store refresh token
  setRefreshToken: (refreshToken: string): boolean => {
    if (!tokenStorage.isSupported()) {
      if (typeof window !== "undefined") {
        sessionStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
      }
      return true;
    }
    return cookieManager.setCookie(
      REFRESH_TOKEN_KEY,
      refreshToken,
      refreshCookieOptions,
    );
  },

  // Get refresh token
  getRefreshToken: (): string | null => {
    if (!tokenStorage.isSupported()) {
      if (typeof window !== "undefined") {
        return sessionStorage.getItem(REFRESH_TOKEN_KEY);
      }
      return null;
    }
    return cookieManager.getCookie(REFRESH_TOKEN_KEY);
  },

  // Remove tokens (logout)
  clearTokens: (): boolean => {
    if (!tokenStorage.isSupported()) {
      if (typeof window !== "undefined") {
        sessionStorage.removeItem(TOKEN_KEY);
        sessionStorage.removeItem(REFRESH_TOKEN_KEY);
      }
      return true;
    }

    const success1 = cookieManager.deleteCookie(TOKEN_KEY);
    const success2 = cookieManager.deleteCookie(REFRESH_TOKEN_KEY);
    return success1 && success2;
  },

  // Check if token exists and is valid
  isAuthenticated: (): boolean => {
    const token = tokenStorage.getToken();
    if (!token) return false;

    try {
      // Basic JWT structure validation
      const parts = token.split(".");
      if (parts.length !== 3) return false;

      // Decode payload to check expiration
      const payload = JSON.parse(atob(parts[1]));
      const currentTime = Math.floor(Date.now() / 1000);

      return payload.exp > currentTime;
    } catch (error) {
      console.error("Token validation error:", error);
      return false;
    }
  },

  // Get token payload (decode JWT)
  getTokenPayload: (): any | null => {
    const token = tokenStorage.getToken();
    if (!token) return null;

    try {
      const parts = token.split(".");
      if (parts.length !== 3) return null;

      return JSON.parse(atob(parts[1]));
    } catch (error) {
      console.error("Error decoding token:", error);
      return null;
    }
  },

  // Get token expiration time
  getTokenExpiration: (): number | null => {
    const payload = tokenStorage.getTokenPayload();
    return payload ? payload.exp * 1000 : null; // Convert to milliseconds
  },

  // Check if token will expire soon (within specified minutes)
  willExpireSoon: (minutes: number = 5): boolean => {
    const expiration = tokenStorage.getTokenExpiration();
    if (!expiration) return true;

    const now = Date.now();
    const timeUntilExpiry = expiration - now;
    return timeUntilExpiry < minutes * 60 * 1000;
  },

  // Set user data in localStorage (for UI display purposes only)
  setUser: (user: any): boolean => {
    try {
      if (typeof window !== "undefined") {
        localStorage.setItem(USER_COOKIE_NAME, JSON.stringify(user));
        return true;
      }
      return false;
    } catch (error) {
      console.error("Error storing user data:", error);
      return false;
    }
  },

  // Get user data from localStorage
  getUser: (): any | null => {
    try {
      if (typeof window === "undefined") return null;

      const userStr = localStorage.getItem(USER_COOKIE_NAME);
      if (userStr) {
        return JSON.parse(userStr);
      }
      return null;
    } catch (error) {
      console.error("Failed to parse stored user data:", error);
      return null;
    }
  },

  // Remove user data from localStorage
  removeUser: (): boolean => {
    try {
      if (typeof window !== "undefined") {
        localStorage.removeItem(USER_COOKIE_NAME);
        return true;
      }
      return false;
    } catch (error) {
      console.error("Error removing user data:", error);
      return false;
    }
  },

  // Clear all auth data
  clear: (): boolean => {
    const tokensCleared = tokenStorage.clearTokens();
    const userCleared = tokenStorage.removeUser();
    return tokensCleared && userCleared;
  },
};
