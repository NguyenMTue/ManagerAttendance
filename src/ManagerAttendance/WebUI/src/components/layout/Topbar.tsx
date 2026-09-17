'use client';

import { useAuth } from '@/hooks/useAuth';
import { User, LogOut, Clock, Shield } from 'lucide-react';

export default function Topbar() {
  const { user, logout } = useAuth();

  return (
    <header className="h-16 bg-white border-b border-slate-200 px-6 flex items-center justify-between sticky top-0 z-30 shadow-xs">
      <div className="flex items-center gap-3">
        <div className="bg-blue-600 text-white p-2 rounded-lg font-bold flex items-center gap-2">
          <Clock className="w-5 h-5" />
          <span>ManagerAttendance</span>
        </div>
        <span className="hidden md:inline-block text-xs font-semibold px-2.5 py-1 bg-slate-100 text-slate-600 rounded-full">
          Múi giờ Việt Nam (UTC+7)
        </span>
      </div>

      <div className="flex items-center gap-4">
        {user && (
          <div className="flex items-center gap-3">
            <div className="text-right hidden sm:block">
              <div className="text-sm font-semibold text-slate-800">{user.email}</div>
              <div className="text-xs text-slate-500 flex items-center justify-end gap-1">
                <Shield className="w-3 h-3 text-blue-500" />
                <span>Quyền: <strong className="text-blue-600">{user.role}</strong></span>
              </div>
            </div>

            <div className="w-10 h-10 rounded-full bg-blue-100 text-blue-600 font-bold flex items-center justify-center border border-blue-200">
              {user.email.substring(0, 2).toUpperCase()}
            </div>

            <button
              onClick={logout}
              title="Đăng xuất"
              className="p-2 text-slate-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors cursor-pointer"
            >
              <LogOut className="w-5 h-5" />
            </button>
          </div>
        )}
      </div>
    </header>
  );
}
