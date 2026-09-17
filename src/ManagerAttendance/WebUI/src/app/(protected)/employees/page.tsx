'use client';

import { useState } from 'react';
import { useEmployees } from '@/hooks/useEmployees';
import { Plus, Search, Trash2, ArrowUpRight, UserCheck, UserX } from 'lucide-react';
import { EmployeeDto } from '@/types';

export default function EmployeesPage() {
  const {
    employeesQuery,
    createDeveloperMutation,
    createQAMutation,
    createManagerMutation,
    promoteMutation,
    updateStatusMutation,
    deleteEmployeeMutation,
  } = useEmployees();

  const [searchTerm, setSearchTerm] = useState('');
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [empType, setEmpType] = useState<'Developer' | 'QA' | 'Manager'>('Developer');

  // Form states
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [techDirection, setTechDirection] = useState('Backend');
  const [codingSkills, setCodingSkills] = useState('C#, SQL');
  const [testingMethod, setTestingMethod] = useState('Automation');
  const [autoSkills, setAutoSkills] = useState(true);
  const [managerType, setManagerType] = useState('Technical');
  const [managedDept, setManagedDept] = useState('Software Engineering');

  const employees = employeesQuery.data || [];

  const filteredEmployees = employees.filter(
    (e) =>
      e.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      e.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      e.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleCreateSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (empType === 'Developer') {
        await createDeveloperMutation.mutateAsync({
          firstName,
          lastName,
          email,
          technicalDirection: techDirection,
          codingSkillsFlag: codingSkills,
        });
      } else if (empType === 'QA') {
        await createQAMutation.mutateAsync({
          firstName,
          lastName,
          email,
          testingMethodology: testingMethod,
          automationSkills: autoSkills,
        });
      } else {
        await createManagerMutation.mutateAsync({
          firstName,
          lastName,
          email,
          managerType: managerType as any,
          managedDepartment: managedDept,
        });
      }
      setShowCreateModal(false);
      resetForm();
    } catch {
      alert('Tạo nhân viên thất bại!');
    }
  };

  const resetForm = () => {
    setFirstName('');
    setLastName('');
    setEmail('');
  };

  const handlePromote = async (emp: EmployeeDto) => {
    const newBand = prompt('Nhập Cấp bậc mới (Junior, Mid, Senior, Lead, Principal):', emp.band || 'Senior');
    if (newBand) {
      try {
        await promoteMutation.mutateAsync({ id: emp.id, dto: { band: newBand as any } });
        alert(`Đã thăng chức cho nhân viên ${emp.firstName} ${emp.lastName} lên cấp ${newBand}!`);
      } catch (err: unknown) {
        const errorObj = err as { response?: { data?: { message?: string } } };
        const msg = errorObj.response?.data?.message || 'Thăng chức thất bại! Vui lòng kiểm tra lại cấp bậc (Junior, Mid, Senior, Lead, Principal).';
        alert(msg);
      }
    }
  };

  const handleToggleStatus = async (emp: EmployeeDto) => {
    const newStatus = !emp.isActive;
    const actionText = newStatus ? 'Kích hoạt' : 'Vô hiệu hóa / Sa thải';
    const reason = prompt(`Xác nhận ${actionText} tài khoản ${emp.firstName} ${emp.lastName}? Nhập lý do:`, actionText);
    if (reason !== null) {
      try {
        await updateStatusMutation.mutateAsync({
          id: emp.id,
          dto: { isActive: newStatus, reason: reason || actionText },
        });
        alert(`Đã ${actionText} thành công tài khoản của ${emp.firstName} ${emp.lastName}!`);
      } catch (err: unknown) {
        const errorObj = err as { response?: { data?: { message?: string } } };
        const msg = errorObj.response?.data?.message || 'Điều chỉnh trạng thái thất bại!';
        alert(msg);
      }
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản nhân viên này không?')) {
      await deleteEmployeeMutation.mutateAsync(id);
    }
  };

  return (
    <div className="space-y-8">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Quản Lý Nhân Sự</h1>
          <p className="text-sm text-slate-500 mt-1">Danh sách nhân viên, thăng chức và tạo mới tài khoản đa hình.</p>
        </div>

        <button
          onClick={() => setShowCreateModal(true)}
          className="px-4 py-2.5 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-xl shadow-xs flex items-center gap-2 transition-colors cursor-pointer"
        >
          <Plus className="w-5 h-5" />
          <span>Tạo Nhân Viên Mới</span>
        </button>
      </div>

      {/* Filter Bar */}
      <div className="bg-white p-4 rounded-xl border border-slate-200 shadow-xs flex items-center gap-4">
        <div className="relative flex-1">
          <Search className="w-5 h-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Tìm kiếm theo Tên, Họ hoặc Email nhân viên..."
            className="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:outline-hidden"
          />
        </div>
      </div>

      {/* Directory Table */}
      <div className="bg-white rounded-xl border border-slate-200 shadow-xs overflow-hidden">
        {employeesQuery.isLoading ? (
          <div className="p-8 text-center text-slate-500">Đang tải danh sách nhân viên...</div>
        ) : filteredEmployees.length === 0 ? (
          <div className="p-8 text-center text-slate-500">Không tìm thấy nhân viên phù hợp.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm text-slate-600">
              <thead className="bg-slate-50 text-slate-700 text-xs font-semibold uppercase border-b border-slate-200">
                <tr>
                  <th className="px-6 py-4">ID</th>
                  <th className="px-6 py-4">Họ và Tên</th>
                  <th className="px-6 py-4">Email</th>
                  <th className="px-6 py-4">Loại NV</th>
                  <th className="px-6 py-4">Phòng ban</th>
                  <th className="px-6 py-4">Cấp bậc (Band)</th>
                  <th className="px-6 py-4">Trạng thái</th>
                  <th className="px-6 py-4 text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200">
                {filteredEmployees.map((emp) => (
                  <tr key={emp.id} className="hover:bg-slate-50 transition-colors">
                    <td className="px-6 py-4 font-mono text-slate-400">#{emp.id}</td>
                    <td className="px-6 py-4 font-semibold text-slate-900">{emp.firstName} {emp.lastName}</td>
                    <td className="px-6 py-4 text-slate-600">{emp.email}</td>
                    <td className="px-6 py-4">
                      <span className="px-2.5 py-1 bg-blue-100 text-blue-800 text-xs font-semibold rounded-md">
                        {emp.employeeType}
                      </span>
                    </td>
                    <td className="px-6 py-4">{emp.department}</td>
                    <td className="px-6 py-4 font-medium text-slate-800">{emp.band}</td>
                    <td className="px-6 py-4">
                      <span
                        className={`inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold ${
                          emp.isActive ? 'bg-emerald-100 text-emerald-800' : 'bg-red-100 text-red-800'
                        }`}
                      >
                        {emp.isActive ? <UserCheck className="w-3 h-3" /> : <UserX className="w-3 h-3" />}
                        {emp.isActive ? 'Hoạt động' : 'Vô hiệu hóa'}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right space-x-2">
                      <button
                        onClick={() => handlePromote(emp)}
                        title="Thăng chức Cấp bậc"
                        className="p-1.5 text-blue-600 hover:bg-blue-50 rounded-lg cursor-pointer"
                      >
                        <ArrowUpRight className="w-4 h-4" />
                      </button>

                      <button
                        onClick={() => handleToggleStatus(emp)}
                        title={emp.isActive ? 'Vô hiệu hóa / Sa thải' : 'Kích hoạt lại'}
                        className="p-1.5 text-amber-600 hover:bg-amber-50 rounded-lg cursor-pointer"
                      >
                        {emp.isActive ? <UserX className="w-4 h-4" /> : <UserCheck className="w-4 h-4" />}
                      </button>

                      <button
                        onClick={() => handleDelete(emp.id)}
                        title="Xóa nhân viên"
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

      {/* Dynamic Create Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-slate-900/50 backdrop-blur-xs flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-2xl max-w-lg w-full p-6 shadow-2xl space-y-6">
            <h2 className="text-xl font-bold text-slate-900">Tạo Nhân Viên Mới</h2>

            <form onSubmit={handleCreateSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Loại Nhân Viên (TPH)</label>
                <select
                  value={empType}
                  onChange={(e) => setEmpType(e.target.value as any)}
                  className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                >
                  <option value="Developer">Developer (Lập trình viên)</option>
                  <option value="QA">QA (Kiểm thử viên)</option>
                  <option value="Manager">Manager (Quản lý)</option>
                </select>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Họ & Tên đệm</label>
                  <input
                    type="text"
                    required
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Tên</label>
                  <input
                    type="text"
                    required
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Email</label>
                <input
                  type="email"
                  required
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                />
              </div>

              {/* Dynamic Conditional Fields */}
              {empType === 'Developer' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-slate-700 mb-1">Hướng Kỹ Thuật</label>
                    <input
                      type="text"
                      value={techDirection}
                      onChange={(e) => setTechDirection(e.target.value)}
                      className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-slate-700 mb-1">Kỹ Năng Lập Trình</label>
                    <input
                      type="text"
                      value={codingSkills}
                      onChange={(e) => setCodingSkills(e.target.value)}
                      className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                    />
                  </div>
                </>
              )}

              {empType === 'QA' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-slate-700 mb-1">Phương Pháp Testing</label>
                    <input
                      type="text"
                      value={testingMethod}
                      onChange={(e) => setTestingMethod(e.target.value)}
                      className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                    />
                  </div>
                  <div className="flex items-center gap-2 pt-2">
                    <input
                      type="checkbox"
                      id="autoSkills"
                      checked={autoSkills}
                      onChange={(e) => setAutoSkills(e.target.checked)}
                      className="w-4 h-4 text-blue-600 rounded-xs"
                    />
                    <label htmlFor="autoSkills" className="text-sm font-medium text-slate-700">
                      Có Kỹ Năng Automation Testing
                    </label>
                  </div>
                </>
              )}

              {empType === 'Manager' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-slate-700 mb-1">Loại Quản Lý</label>
                    <select
                      value={managerType}
                      onChange={(e) => setManagerType(e.target.value)}
                      className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                    >
                      <option value="Technical">Technical Manager</option>
                      <option value="Project">Project Manager</option>
                      <option value="Operations">Operations Manager</option>
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-slate-700 mb-1">Phòng Ban Quản Lý</label>
                    <input
                      type="text"
                      value={managedDept}
                      onChange={(e) => setManagedDept(e.target.value)}
                      className="w-full p-2.5 border border-slate-300 rounded-lg text-sm"
                    />
                  </div>
                </>
              )}

              <div className="flex justify-end gap-3 pt-4 border-t border-slate-100">
                <button
                  type="button"
                  onClick={() => setShowCreateModal(false)}
                  className="px-4 py-2 border border-slate-300 rounded-lg text-sm text-slate-700 hover:bg-slate-50 cursor-pointer"
                >
                  Hủy
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-semibold cursor-pointer"
                >
                  Tạo Nhân Viên
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
