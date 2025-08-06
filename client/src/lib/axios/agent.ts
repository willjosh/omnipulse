import axios from "axios";
import { tokenStorage } from "@/lib/auth/tokenStorage";

const sleep = (delay: number) => {
  return new Promise(resolve => {
    setTimeout(resolve, delay);
  });
};

const baseURL =
  process.env.NODE_ENV === "production"
    ? "https://omnipulse-backend.wonderfulsky-7bfd34c0.australiaeast.azurecontainerapps.io"
    : "http://localhost:5100";

export const agent = axios.create({ baseURL });

// Request interceptor to add JWT token
agent.interceptors.request.use(
  config => {
    const token = tokenStorage.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  },
);

// Response interceptor for authentication errors
agent.interceptors.response.use(
  async response => {
    try {
      // await sleep(1000);
      return response;
    } catch (error) {
      console.log(error);
      return Promise.reject(error);
    }
  },
  async error => {
    // Handle 401 Unauthorized errors
    if (error.response?.status === 401) {
      // Check if we have a refresh token and try to refresh
      const refreshToken = tokenStorage.getRefreshToken();
      if (refreshToken) {
        try {
          // Note: You'll need to implement the refresh token endpoint
          // For now, we'll just clear tokens and redirect
          // const response = await axios.post(`${baseURL}/api/auth/refresh`, {
          //   refreshToken: refreshToken
          // });
          // if (response.data.token) {
          //   tokenStorage.setToken(response.data.token);
          //   if (response.data.refreshToken) {
          //     tokenStorage.setRefreshToken(response.data.refreshToken);
          //   }
          //   // Retry the original request
          //   return agent.request(error.config);
          // }
        } catch (refreshError) {
          console.error("Token refresh failed:", refreshError);
        }
      }

      // If refresh failed or no refresh token, clear everything and redirect
      tokenStorage.clear();
      window.location.href = "/login";
    }
    return Promise.reject(error);
  },
);
