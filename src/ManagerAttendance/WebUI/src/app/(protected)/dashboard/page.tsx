'use client';

import { useAttendance } from '@/hooks/useAttendance';
import { useEmployees } from '@/hooks/useEmployees';
import { Users, UserCheck, UserX, Clock, TrendingUp } from 'lucide-react';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Legend,
} from 'recharts';

export default function DashboardPage() {
  const { allAttendanceQuery } = useAttendance();
  const { employeesQuery } = useEmployees();

  const employees = employeesQuery.data || [];
  const attendanceRecords = allAttendanceQuery.data || [];

  const totalEmployees = employees.length;
  const activeEmployees = employees.filter((e) => e.isActive).length;
  const todayCheckedIn = attendanceRecords.filter((r) => r.status !== 'Absent').length;
  const todayLate = attendanceRecords.filter((r) => r.status === 'Late').length;

  // Department distribution data for Pie Chart
  const deptMap: Record<string, number> = {};
  employees.forEach((e) => {
    const dept = e.department || 'Other';
    deptMap[dept] = (deptMap[dept] || 0) + 1;
  });

  const pieData = Object.keys(deptMap).map((dept) => ({
    name: dept,
    value: deptMap[dept],
  }));

  const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899'];

  // Status overview bar chart data
  const statusCounts = {
    'Đúng giờ (Present)': attendanceRecords.filter((r) => r.status === 'Present').length,
    'Đi muộn (Late)': todayLate,
  };

  const barData = [
    { name: 'Đúng giờ', count: statusCounts['Đúng giờ (Present)'] },
    { name: 'Đi muộn', count: statusCounts['Đi muộn (Late)'] },
  ];

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Dashboard Thống Kê Tổng Quan</h1>
        <p className="text-sm text-slate-500 mt-1">
          Theo dõi tình hình điểm danh, nhân sự và xu hướng làm việc theo thời gian thực (Múi giờ Việt Nam UTC+7).
        </p>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white p-6 rounded-xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Tổng Nhân viên</div>
            <div className="text-3xl font-bold text-slate-900 mt-2">{totalEmployees}</div>
            <div className="text-xs text-emerald-600 mt-1 flex items-center gap-1 font-medium">
              <TrendingUp className="w-3.5 h-3.5" /> {activeEmployees} đang hoạt động
            </div>
          </div>
          <div className="w-12 h-12 bg-blue-50 text-blue-600 rounded-xl flex items-center justify-center">
            <Users className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Đã Check-In</div>
            <div className="text-3xl font-bold text-emerald-600 mt-2">{todayCheckedIn}</div>
            <div className="text-xs text-slate-500 mt-1 font-medium">Hôm nay</div>
          </div>
          <div className="w-12 h-12 bg-emerald-50 text-emerald-600 rounded-xl flex items-center justify-center">
            <UserCheck className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Đi Muộn (Late)</div>
            <div className="text-3xl font-bold text-amber-600 mt-2">{todayLate}</div>
            <div className="text-xs text-amber-700 mt-1 font-medium">Sau 09:00 AM</div>
          </div>
          <div className="w-12 h-12 bg-amber-50 text-amber-600 rounded-xl flex items-center justify-center">
            <Clock className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Vắng / Chưa CheckIn</div>
            <div className="text-3xl font-bold text-slate-400 mt-2">
              {Math.max(0, activeEmployees - todayCheckedIn)}
            </div>
            <div className="text-xs text-slate-500 mt-1 font-medium">Chờ cập nhật</div>
          </div>
          <div className="w-12 h-12 bg-slate-100 text-slate-500 rounded-xl flex items-center justify-center">
            <UserX className="w-6 h-6" />
          </div>
        </div>
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Bar Chart */}
        <div className="bg-white p-6 rounded-xl border border-slate-200 shadow-xs">
          <h2 className="text-lg font-bold text-slate-900 mb-4">Tỷ Lệ Điểm Danh (Đúng giờ vs Đi muộn)</h2>
          <div className="h-72 w-full">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={barData}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#e2e8f0" />
                <XAxis dataKey="name" />
                <YAxis allowDecimals={false} />
                <Tooltip />
                <Bar dataKey="count" fill="#3b82f6" radius={[6, 6, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Pie Chart */}
        <div className="bg-white p-6 rounded-xl border border-slate-200 shadow-xs">
          <h2 className="text-lg font-bold text-slate-900 mb-4">Phân Bổ Nhân Viên Theo Phòng Ban</h2>
          <div className="h-72 w-full">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={pieData}
                  cx="50%"
                  cy="50%"
                  innerRadius={60}
                  outerRadius={90}
                  paddingAngle={5}
                  dataKey="value"
                >
                  {pieData.map((_, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>
    </div>
  );
}
