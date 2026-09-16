'use client';

import { useState } from 'react';
import { useAttendance } from '@/hooks/useAttendance';
import { Clock, LogIn, LogOut, CheckCircle, AlertCircle, FileText } from 'lucide-react';

export default function MyAttendancePage() {
  const { myHistoryQuery, checkInMutation, checkOutMutation } = useAttendance();
  const [notes, setNotes] = useState('');
  const [msg, setMsg] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const history = myHistoryQuery.data || [];
  const todayRecord = history[0]; // Recent record

  const handleCheckIn = async () => {
    setMsg(null);
    try {
      await checkInMutation.mutateAsync({ notes });
      setMsg({ type: 'success', text: 'Chấm công đầu ngày (Check-In) thành công!' });
      setNotes('');
    } catch (err: unknown) {
      const errorObj = err as { response?: { data?: { message?: string } } };
      const errText = errorObj.response?.data?.message || 'Check-in thất bại!';
      setMsg({ type: 'error', text: errText });
    }
  };

  const handleCheckOut = async () => {
    setMsg(null);
    try {
      await checkOutMutation.mutateAsync({ notes });
      setMsg({ type: 'success', text: 'Kết thúc ca làm việc (Check-Out) thành công!' });
      setNotes('');
    } catch (err: unknown) {
      const errorObj = err as { response?: { data?: { message?: string } } };
      const errText = errorObj.response?.data?.message || 'Check-out thất bại!';
      setMsg({ type: 'error', text: errText });
    }
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Chấm Công Cá Nhân</h1>
        <p className="text-sm text-slate-500 mt-1">
          Thực hiện Check-In / Check-Out ca làm việc và xem lịch sử điểm danh của bản thân.
        </p>
      </div>

      {msg && (
        <div
          className={`p-4 rounded-xl border flex items-center gap-3 ${
            msg.type === 'success'
              ? 'bg-emerald-50 border-emerald-200 text-emerald-800'
              : 'bg-red-50 border-red-200 text-red-800'
          }`}
        >
          {msg.type === 'success' ? <CheckCircle className="w-5 h-5" /> : <AlertCircle className="w-5 h-5" />}
          <span className="text-sm font-medium">{msg.text}</span>
        </div>
      )}

      {/* Hero Check-In / Check-Out Widget */}
      <div className="bg-white p-8 rounded-2xl border border-slate-200 shadow-md max-w-2xl mx-auto">
        <div className="text-center mb-6">
          <div className="inline-flex items-center gap-2 px-4 py-1.5 bg-blue-50 text-blue-700 rounded-full text-xs font-semibold mb-3">
            <Clock className="w-4 h-4" /> Giờ chuẩn Việt Nam (UTC+7)
          </div>
          <h2 className="text-3xl font-extrabold text-slate-900">Bảng Chấm Công Hàng Ngày</h2>
          <p className="text-sm text-slate-500 mt-1">Giờ vào làm tiêu chuẩn: 09:00 AM (Check-in sau 09:00 AM sẽ tính Đi muộn)</p>
        </div>

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Ghi chú lượt chấm công (Không bắt buộc)</label>
            <div className="relative">
              <FileText className="w-5 h-5 absolute left-3 top-3 text-slate-400" />
              <input
                type="text"
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                placeholder="VD: Đi gặp khách hàng, Làm online từ xa..."
                className="w-full pl-10 pr-4 py-2.5 border border-slate-300 rounded-xl text-sm focus:ring-2 focus:ring-blue-500 focus:outline-hidden"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pt-2">
            <button
              onClick={handleCheckIn}
              disabled={checkInMutation.isPending}
              className="py-4 bg-emerald-600 hover:bg-emerald-700 text-white font-bold rounded-xl shadow-md transition-all flex items-center justify-center gap-2 cursor-pointer disabled:opacity-50"
            >
              <LogIn className="w-5 h-5" />
              <span>{checkInMutation.isPending ? 'Đang xử lý...' : '🟢 Check-In Đầu Nguyện'}</span>
            </button>

            <button
              onClick={handleCheckOut}
              disabled={checkOutMutation.isPending}
              className="py-4 bg-rose-600 hover:bg-rose-700 text-white font-bold rounded-xl shadow-md transition-all flex items-center justify-center gap-2 cursor-pointer disabled:opacity-50"
            >
              <LogOut className="w-5 h-5" />
              <span>{checkOutMutation.isPending ? 'Đang xử lý...' : '🔴 Check-Out Ra Về'}</span>
            </button>
          </div>
        </div>
      </div>

      {/* History Table */}
      <div className="bg-white rounded-xl border border-slate-200 shadow-xs overflow-hidden">
        <div className="p-6 border-b border-slate-200">
          <h3 className="text-lg font-bold text-slate-900">Lịch Sử Điểm Danh Của Bản Thân</h3>
        </div>

        {myHistoryQuery.isLoading ? (
          <div className="p-8 text-center text-slate-500">Đang tải lịch sử điểm danh...</div>
        ) : history.length === 0 ? (
          <div className="p-8 text-center text-slate-500">Chưa có dữ liệu điểm danh.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm text-slate-600">
              <thead className="bg-slate-50 text-slate-700 text-xs font-semibold uppercase border-b border-slate-200">
                <tr>
                  <th className="px-6 py-4">Tên Nhân viên</th>
                  <th className="px-6 py-4">Giờ vào (Check-In)</th>
                  <th className="px-6 py-4">Giờ ra (Check-Out)</th>
                  <th className="px-6 py-4">Trạng thái</th>
                  <th className="px-6 py-4">Ghi chú</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200">
                {history.map((rec) => (
                  <tr key={rec.id} className="hover:bg-slate-50 transition-colors">
                    <td className="px-6 py-4 font-medium text-slate-900">{rec.employeeName}</td>
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
                    <td className="px-6 py-4 text-slate-500">{rec.notes || '—'}</td>
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
