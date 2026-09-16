'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { apiClient } from '@/lib/axios';
import { LoginDto, RegisterDto, AuthResponseDto, User } from '@/types';

export function useAuth() {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem('jwt_token');
    const userInfoStr = localStorage.getItem('user_info');
    if (token && userInfoStr) {
      try {
        setUser(JSON.parse(userInfoStr));
      } catch {
        setUser(null);
      }
    }
    setIsLoading(false);
  }, []);

  const login = async (dto: LoginDto) => {
    const response = await apiClient.post<AuthResponseDto>('/auth/login', dto);
    const data = response.data;
    
    const userObj: User = {
      id: data.userId,
      email: data.email,
      role: data.role as User['role'],
      employeeId: data.employeeId,
    };

    localStorage.setItem('jwt_token', data.token);
    localStorage.setItem('user_info', JSON.stringify(userObj));
    setUser(userObj);
    router.push('/dashboard');
    return data;
  };

  const register = async (dto: RegisterDto) => {
    const response = await apiClient.post<AuthResponseDto>('/auth/register', dto);
    const data = response.data;
    
    const userObj: User = {
      id: data.userId,
      email: data.email,
      role: data.role as User['role'],
      employeeId: data.employeeId,
    };

    localStorage.setItem('jwt_token', data.token);
    localStorage.setItem('user_info', JSON.stringify(userObj));
    setUser(userObj);
    router.push('/dashboard');
    return data;
  };

  const logout = () => {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_info');
    setUser(null);
    router.push('/login');
  };

  return {
    user,
    isLoading,
    login,
    register,
    logout,
    isAuthenticated: !!user,
  };
}
