import { apiClient } from '../middleware/apiClient';
import { User } from '../types';

interface LoginRequest { username: string; password: string; }
interface RegisterRequest { username: string; email: string; password: string; }

export const authService = {
  async login(data: LoginRequest): Promise<User> {
    const res = await apiClient.post<User>('/auth/login', data);
    return res.data;
  },

  async register(data: RegisterRequest): Promise<User> {
    const res = await apiClient.post<User>('/auth/register', data);
    return res.data;
  },
};
