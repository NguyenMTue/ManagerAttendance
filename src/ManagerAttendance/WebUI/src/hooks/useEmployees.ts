'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/lib/axios';
import {
  EmployeeDto,
  CreateDeveloperDto,
  CreateQADto,
  CreateManagerDto,
  PromoteEmployeeDto,
  UpdateEmployeeStatusDto,
  ExcelImportResultDto,
} from '@/types';

export function useEmployees() {
  const queryClient = useQueryClient();

  const employeesQuery = useQuery<EmployeeDto[]>({
    queryKey: ['employees'],
    queryFn: async () => {
      const res = await apiClient.get<EmployeeDto[]>('/employee');
      return res.data;
    },
  });

  const createDeveloperMutation = useMutation({
    mutationFn: async (dto: CreateDeveloperDto) => {
      const res = await apiClient.post<EmployeeDto>('/employee/developer', dto);
      return res.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
    },
  });

  const createQAMutation = useMutation({
    mutationFn: async (dto: CreateQADto) => {
      const res = await apiClient.post<EmployeeDto>('/employee/qa', dto);
      return res.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
    },
  });

  const createManagerMutation = useMutation({
    mutationFn: async (dto: CreateManagerDto) => {
      const res = await apiClient.post<EmployeeDto>('/employee/manager', dto);
      return res.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
    },
  });

  const promoteMutation = useMutation({
    mutationFn: async ({ id, dto }: { id: number; dto: PromoteEmployeeDto }) => {
      await apiClient.put(`/employee/${id}/promote`, dto);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
    },
  });

  const updateStatusMutation = useMutation({
    mutationFn: async ({ id, dto }: { id: number; dto: UpdateEmployeeStatusDto }) => {
      await apiClient.put(`/employee/${id}/status`, dto);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
    },
  });

  const deleteEmployeeMutation = useMutation({
    mutationFn: async (id: number) => {
      await apiClient.delete(`/employee/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
    },
  });

  const importExcelMutation = useMutation({
    mutationFn: async ({ file, dryRun }: { file: File; dryRun: boolean }) => {
      const formData = new FormData();
      formData.append('file', file);
      const res = await apiClient.post<ExcelImportResultDto>(
        `/employee/import-excel?dryRun=${dryRun}`,
        formData,
        {
          headers: { 'Content-Type': 'multipart/form-data' },
        }
      );
      return res.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
    },
  });

  return {
    employeesQuery,
    createDeveloperMutation,
    createQAMutation,
    createManagerMutation,
    promoteMutation,
    updateStatusMutation,
    deleteEmployeeMutation,
    importExcelMutation,
  };
}
