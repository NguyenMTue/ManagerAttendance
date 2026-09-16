'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';
import { LayoutDashboard, CalendarCheck, Users, FileSpreadsheet } from 'lucide-react';

export default function Sidebar() {
  const pathname = usePathname();
  const { user } = useAuth();

  const isManagerOrAdmin = user?.role === 'Admin' || user?.role === 'Manager';

  const navItems = [
    {
      name: 'Dashboard Tổng quan',
      href: '/dashboard',
      icon: LayoutDashboard,
      show: true,
    },
    {
      name: 'Chấm công của Tôi',
      href: '/my-attendance',
      icon: CalendarCheck,
      show: true,
    },
    {
      name: 'Quản lý Điểm danh',
      href: '/attendance-management',
      icon: CalendarCheck,
      show: isManagerOrAdmin,
    },
    {
      name: 'Quản lý Nhân sự',
      href: '/employees',
      icon: Users,
      show: isManagerOrAdmin,
    },
    {
      name: 'Nhập từ Excel',
      href: '/employees/import-excel',
      icon: FileSpreadsheet,
      show: isManagerOrAdmin,
    },
  ];

  return (
    <aside className="w-64 bg-slate-900 text-slate-300 min-h-[calc(100vh-4rem)] p-4 flex flex-col gap-2 shadow-lg">
      <div className="px-3 py-2 text-xs font-semibold text-slate-400 uppercase tracking-wider">
        Menu Hệ thống
      </div>

      <nav className="flex flex-col gap-1">
        {navItems
          .filter((item) => item.show)
          .map((item) => {
            const Icon = item.icon;
            const isActive = pathname === item.href;
            return (
              <Link
                key={item.href}
                href={item.href}
                className={`flex items-center gap-3 px-3 py-2.5 rounded-lg font-medium text-sm transition-colors ${
                  isActive
                    ? 'bg-blue-600 text-white shadow-xs'
                    : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                }`}
              >
                <Icon className="w-5 h-5" />
                <span>{item.name}</span>
              </Link>
            );
          })}
      </nav>
    </aside>
  );
}
