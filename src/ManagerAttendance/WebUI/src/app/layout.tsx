import './globals.css';
import QueryProvider from '@/providers/QueryProvider';
import { ReactNode } from 'react';

export const metadata = {
  title: 'Hệ thống Quản lý Điểm danh Nhân viên - ManagerAttendance',
  description: 'Hệ thống quản lý điểm danh, chấm công và quản lý nhân sự chuyên nghiệp.',
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="vi">
      <body className="antialiased bg-slate-50 text-slate-900 min-h-screen">
        <QueryProvider>{children}</QueryProvider>
      </body>
    </html>
  );
}
