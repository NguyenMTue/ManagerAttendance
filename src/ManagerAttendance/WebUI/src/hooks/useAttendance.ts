'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/lib/axios';
import { AttendanceRecordDto, CheckInDto, CheckOutDto } from '@/types';

export function useAttendance() {
  const queryClient = useQueryClient();

  const myHistoryQuery = useQuery<AttendanceRecordDto[]>({
    queryKey: ['attendance', 'my-history'],
    queryFn: async () => {
      const res = await apiClient.get<AttendanceRecordDto[]>('/attendance/my-history');
      return res.data;
    },
  });

  const allAttendanceQuery = useQuery<AttendanceRecordDto[]>({
    queryKey: ['attendance', 'all'],
    queryFn: async () => {
      const res = await apiClient.get<AttendanceRecordDto[]>('/attendance');
      return res.data;
    },
  });

  const checkInMutation = useMutation({
    mutationFn: async (dto: CheckInDto) => {
      const res = await apiClient.post<AttendanceRecordDto>('/attendance/check-in', dto);
      return res.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attendance'] });
    },
  });

  const checkOutMutation = useMutation({
    mutationFn: async (dto: CheckOutDto) => {
      const res = await apiClient.post<AttendanceRecordDto>('/attendance/check-out', dto);
      return res.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attendance'] });
    },
  });

  const deleteAttendanceMutation = useMutation({
    mutationFn: async (id: number) => {
      await apiClient.delete(`/attendance/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attendance'] });
    },
  });

  return {
    myHistoryQuery,
    allAttendanceQuery,
    checkInMutation,
    checkOutMutation,
    deleteAttendanceMutation,
  };
}
