'use client';

import { useState } from 'react';
import { useAttendance } from '@/hooks/useAttendance';
import { Search, Trash2, Calendar } from 'lucide-react';

export default function AttendanceManagementPage() {
  const { allAttendanceQuery, deleteAttendanceMutation } = useAttendance();
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('All');

  const records = allAttendanceQuery.data || [];

  const filteredRecords = records.filter((r) => {
    const matchesSearch =
      r.employeeName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      r.employeeEmail.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'All' || r.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  const handleDelete = async (id: number) => {
    if (confirm(`Bạn có chắc chắn muốn xóa lượt chấm công #${id} không?`)) {
      await deleteAttendanceMutation.mutateAsync(id);
    }
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Quản Lý Điểm Danh Toàn Công Ty</h1>
        <p className="text-sm text-slate-500 mt-1">
          Theo dõi lượt Check-In / Check-Out của tất cả nhân viên trong toàn hệ thống.
        </p>
      </div>

      {/* Filter Bar */}
      <div className="bg-white p-4 rounded-xl border border-slate-200 shadow-xs flex flex-col sm:flex-row items-center gap-4">
        <div className="relative flex-1 w-full">
          <Search className="w-5 h-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Tìm kiếm theo Tên hoặc Email nhân viên..."
            className="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:outline-hidden"
          />
        </div>

        <div className="flex items-center gap-2 w-full sm:w-auto">
          <Calendar className="w-5 h-5 text-slate-400" />
          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="py-2 px-3 border border-slate-300 rounded-lg text-sm bg-white"
          >
            <option value="All">Tất cả Trạng thái</option>
            <option value="Present">Đúng giờ (Present)</option>
            <option value="Late">Đi muộn (Late)</option>
          </select>
        </div>
      </div>

      {/* Data Table */}
      <div className="bg-white rounded-xl border border-slate-200 shadow-xs overflow-hidden">
        {allAttendanceQuery.isLoading ? (
          <div className="p-8 text-center text-slate-500">Đang tải danh sách điểm danh...</div>
        ) : filteredRecords.length === 0 ? (
          <div className="p-8 text-center text-slate-500">Không tìm thấy bản ghi điểm danh nào.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm text-slate-600">
              <thead className="bg-slate-50 text-slate-700 text-xs font-semibold uppercase border-b border-slate-200">
                <tr>
                  <th className="px-6 py-4">Mã số</th>
                  <th className="px-6 py-4">Tên Nhân viên</th>
                  <th className="px-6 py-4">Email</th>
                  <th className="px-6 py-4">Giờ vào (Check-In)</th>
                  <th className="px-6 py-4">Giờ ra (Check-Out)</th>
                  <th className="px-6 py-4">Trạng thái</th>
                  <th className="px-6 py-4 text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200">
                {filteredRecords.map((rec) => (
                  <tr key={rec.id} className="hover:bg-slate-50 transition-colors">
                    <td className="px-6 py-4 font-mono text-slate-400">#{rec.id}</td>
                    <td className="px-6 py-4 font-semibold text-slate-900">{rec.employeeName}</td>
                    <td className="px-6 py-4 text-slate-600">{rec.employeeEmail}</td>
                    <td className="px-6 py-4 font-mono text-slate-700">
                      {new Date(rec.arrivalTime).toLocaleString('vi-VN')}
                    </td>
                    <td className="px-6 py-4 font-mono text-slate-700">
                      {rec.departureTime ? new Date(rec.departureTime).toLocaleString('vi-VN') : '—'}
                    </td>
                    <td className="px-6 py-4">
                      <span
                        className={`inline-flex items-center px-2.5 py-1 rounded-full text-xs font-semibold ${
                          rec.status === 'Present'
                            ? 'bg-emerald-100 text-emerald-800'
                            : rec.status === 'Late'
                            ? 'bg-amber-100 text-amber-800'
                            : 'bg-slate-100 text-slate-700'
                        }`}
                      >
                        {rec.status === 'Present' ? 'Đúng giờ' : rec.status === 'Late' ? 'Đi muộn' : rec.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      <button
                        onClick={() => handleDelete(rec.id)}
                        title="Xóa bản ghi điểm danh"
                        className="p-1.5 text-rose-600 hover:bg-rose-50 rounded-lg cursor-pointer"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
