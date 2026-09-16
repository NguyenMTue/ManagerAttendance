'use client';

import { useState } from 'react';
import { useEmployees } from '@/hooks/useEmployees';
import { FileSpreadsheet, Upload, CheckCircle2, AlertTriangle, Play, XCircle } from 'lucide-react';
import { ExcelImportResultDto } from '@/types';

export default function ImportExcelPage() {
  const { importExcelMutation } = useEmployees();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [dryRunResult, setDryRunResult] = useState<ExcelImportResultDto | null>(null);
  const [activeTab, setActiveTab] = useState<'valid' | 'errors'>('valid');
  const [importSuccessMsg, setImportSuccessMsg] = useState<string | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setSelectedFile(e.target.files[0]);
      setDryRunResult(null);
      setImportSuccessMsg(null);
    }
  };

  const handleDryRun = async () => {
    if (!selectedFile) return;
    setImportSuccessMsg(null);
    try {
      const result = await importExcelMutation.mutateAsync({ file: selectedFile, dryRun: true });
      setDryRunResult(result);
      if (result.errorCount > 0) {
        setActiveTab('errors');
      } else {
        setActiveTab('valid');
      }
    } catch {
      alert('Kiểm tra Dry-Run thất bại!');
    }
  };

  const handleOfficialImport = async () => {
    if (!selectedFile) return;
    try {
      const result = await importExcelMutation.mutateAsync({ file: selectedFile, dryRun: false });
      setImportSuccessMsg(`Đã thêm thành công ${result.successCount} nhân viên vào Cơ sở dữ liệu!`);
      setSelectedFile(null);
      setDryRunResult(null);
    } catch {
      alert('Lưu dữ liệu thất bại!');
    }
  };

  return (
    <div className="space-y-8 max-w-6xl mx-auto">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Nhập Danh Sách Nhân Viên Từ Excel (.xlsx)</h1>
        <p className="text-sm text-slate-500 mt-1">
          Hỗ trợ kiểm tra lỗi dữ liệu tự động (Dry-Run Preview) trước khi lưu chính thức vào Hệ thống.
        </p>
      </div>

      {importSuccessMsg && (
        <div className="p-4 bg-emerald-50 border border-emerald-200 text-emerald-800 rounded-xl flex items-center gap-3">
          <CheckCircle2 className="w-5 h-5 text-emerald-600" />
          <span className="font-semibold text-sm">{importSuccessMsg}</span>
        </div>
      )}

      {/* File Upload Zone */}
      <div className="bg-white p-8 rounded-2xl border border-slate-200 shadow-xs text-center">
        <div className="w-16 h-16 bg-blue-50 text-blue-600 rounded-2xl flex items-center justify-center mx-auto mb-4">
          <FileSpreadsheet className="w-8 h-8" />
        </div>
        <h3 className="text-lg font-bold text-slate-900">Chọn hoặc Kéo thả File Excel</h3>
        <p className="text-sm text-slate-500 mt-1 mb-6">Định dạng chấp nhận: .xlsx, .xls</p>

        <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
          <label className="px-6 py-3 bg-slate-100 hover:bg-slate-200 text-slate-700 font-semibold rounded-xl cursor-pointer transition-colors flex items-center gap-2">
            <Upload className="w-5 h-5" />
            <span>{selectedFile ? selectedFile.name : 'Duyệt File trên Máy...'}</span>
            <input type="file" accept=".xlsx, .xls" onChange={handleFileChange} className="hidden" />
          </label>

          {selectedFile && (
            <button
              onClick={handleDryRun}
              disabled={importExcelMutation.isPending}
              className="px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-xl shadow-md transition-colors flex items-center gap-2 cursor-pointer disabled:opacity-50"
            >
              <Play className="w-5 h-5" />
              <span>{importExcelMutation.isPending ? 'Đang kiểm tra...' : 'Bắt đầu Kiểm tra (Dry-Run)'}</span>
            </button>
          )}
        </div>
      </div>

      {/* Dry-Run Result Preview */}
      {dryRunResult && (
        <div className="bg-white rounded-2xl border border-slate-200 shadow-md p-6 space-y-6">
          {/* Result Cards */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div className="bg-slate-50 p-4 rounded-xl border border-slate-200">
              <div className="text-xs font-semibold text-slate-500 uppercase">Tổng Số Dòng</div>
              <div className="text-2xl font-bold text-slate-900 mt-1">{dryRunResult.totalRows}</div>
            </div>

            <div className="bg-emerald-50 p-4 rounded-xl border border-emerald-200">
              <div className="text-xs font-semibold text-emerald-700 uppercase">Hợp Lệ Thêm Mới</div>
              <div className="text-2xl font-bold text-emerald-700 mt-1">{dryRunResult.successCount}</div>
            </div>

            <div className="bg-amber-50 p-4 rounded-xl border border-amber-200">
              <div className="text-xs font-semibold text-amber-700 uppercase">Dòng Phát Hiện Lỗi</div>
              <div className="text-2xl font-bold text-amber-700 mt-1">{dryRunResult.errorCount}</div>
            </div>
          </div>

          {/* Action Confirmation Bar */}
          <div className="p-4 bg-blue-50 border border-blue-200 rounded-xl flex flex-col sm:flex-row items-center justify-between gap-4">
            <div className="text-sm text-blue-900">
              {dryRunResult.errorCount > 0 ? (
                <span>
                  Phát hiện <strong className="text-amber-700">{dryRunResult.errorCount} dòng lỗi</strong>. Bạn có muốn bỏ qua các dòng lỗi và lưu <strong className="text-emerald-700">{dryRunResult.successCount} nhân viên hợp lệ</strong> vào Database không?
                </span>
              ) : (
                <span>
                  Tất cả <strong className="text-emerald-700">{dryRunResult.successCount} dòng đều hợp lệ</strong>! Sẵn sàng thêm vào Cơ sở dữ liệu.
                </span>
              )}
            </div>

            <div className="flex items-center gap-3">
              <button
                onClick={() => setDryRunResult(null)}
                className="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 text-sm font-medium hover:bg-slate-100 cursor-pointer"
              >
                Hủy
              </button>
              <button
                onClick={handleOfficialImport}
                disabled={importExcelMutation.isPending || dryRunResult.successCount === 0}
                className="px-5 py-2 bg-emerald-600 hover:bg-emerald-700 text-white text-sm font-bold rounded-lg shadow-xs cursor-pointer disabled:opacity-50"
              >
                {importExcelMutation.isPending ? 'Đang lưu...' : '⚡ Tiến hành Lưu vào Database'}
              </button>
            </div>
          </div>

          {/* Preview Tabs */}
          <div>
            <div className="flex border-b border-slate-200">
              <button
                onClick={() => setActiveTab('valid')}
                className={`px-4 py-3 text-sm font-semibold border-b-2 transition-colors cursor-pointer ${
                  activeTab === 'valid'
                    ? 'border-emerald-600 text-emerald-600'
                    : 'border-transparent text-slate-500 hover:text-slate-700'
                }`}
              >
                🟢 Nhân Viên Hợp Lệ ({dryRunResult.importedEmployees.length})
              </button>

              <button
                onClick={() => setActiveTab('errors')}
                className={`px-4 py-3 text-sm font-semibold border-b-2 transition-colors cursor-pointer ${
                  activeTab === 'errors'
                    ? 'border-amber-600 text-amber-600'
                    : 'border-transparent text-slate-500 hover:text-slate-700'
                }`}
              >
                🔴 Danh Sách Dòng Lỗi ({dryRunResult.errors.length})
              </button>
            </div>

            <div className="pt-4">
              {activeTab === 'valid' ? (
                <div className="overflow-x-auto">
                  <table className="w-full text-left text-sm text-slate-600">
                    <thead className="bg-slate-50 text-slate-700 text-xs font-semibold uppercase border-b border-slate-200">
                      <tr>
                        <th className="px-4 py-3">Họ và Tên</th>
                        <th className="px-4 py-3">Email</th>
                        <th className="px-4 py-3">Loại NV</th>
                        <th className="px-4 py-3">Phòng ban</th>
                        <th className="px-4 py-3">Cấp bậc</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-200">
                      {dryRunResult.importedEmployees.map((emp, i) => (
                        <tr key={i} className="hover:bg-slate-50">
                          <td className="px-4 py-3 font-semibold text-slate-900">{emp.firstName} {emp.lastName}</td>
                          <td className="px-4 py-3">{emp.email}</td>
                          <td className="px-4 py-3 font-medium text-blue-600">{emp.employeeType}</td>
                          <td className="px-4 py-3">{emp.department}</td>
                          <td className="px-4 py-3">{emp.band}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-left text-sm text-slate-600">
                    <thead className="bg-amber-50 text-amber-900 text-xs font-semibold uppercase border-b border-amber-200">
                      <tr>
                        <th className="px-4 py-3">Dòng Excel</th>
                        <th className="px-4 py-3">Cột Bị Lỗi</th>
                        <th className="px-4 py-3">Chi Tiết Lỗi</th>
                        <th className="px-4 py-3">Dữ Liệu Nhập Sai</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-200">
                      {dryRunResult.errors.map((err, i) => (
                        <tr key={i} className="hover:bg-amber-50/50">
                          <td className="px-4 py-3 font-bold text-amber-800">
                            {err.rowIndex > 0 ? `Dòng ${err.rowIndex}` : 'Hệ thống'}
                          </td>
                          <td className="px-4 py-3 font-semibold text-slate-900">{err.fieldName}</td>
                          <td className="px-4 py-3 text-red-600">{err.errorMessage}</td>
                          <td className="px-4 py-3 font-mono text-slate-500">{err.rawData || '—'}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
