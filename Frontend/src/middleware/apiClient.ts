import axios, { AxiosInstance, AxiosResponse } from 'axios';

// Create configured axios instance
const createApiClient = (): AxiosInstance => {
  const client = axios.create({
    baseURL: 'https://localhost:64341/api',
    headers: { 'Content-Type': 'application/json' },
    timeout: 10000,
  });

  // Request interceptor - attach JWT token
  client.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => Promise.reject(error)
  );

  // Response interceptor - handle auth errors globally
  client.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        window.location.href = '/login';
      }
      return Promise.reject(error);
    }
  );

  return client;
};

export const apiClient = createApiClient();

// Helper to extract error message
export const getErrorMessage = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    return error.response?.data?.error ?? error.message;
  }
  return 'An unexpected error occurred';
};
