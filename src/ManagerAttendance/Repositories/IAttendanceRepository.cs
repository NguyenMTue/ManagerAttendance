using ManagerAttendance.Models;

namespace ManagerAttendance.Repositories;

public interface IAttendanceRepository : IGenericRepository<AttendanceRecord>
{
    Task<IEnumerable<AttendanceRecord>> GetAttendanceByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<AttendanceRecord>> GetAttendanceByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<AttendanceRecord?> GetTodayAttendanceByEmployeeIdAsync(int employeeId);
}
