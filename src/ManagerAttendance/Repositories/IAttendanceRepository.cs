using ManagerAttendance.Models;

namespace ManagerAttendance.Repositories;

public interface IAttendanceRepository : IGenericRepository<AttendanceRecord>
{
    Task<IEnumerable<AttendanceRecord>> GetAttendanceByEmployeeIdAsync(int employeeId, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<AttendanceRecord>> GetAttendanceByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<AttendanceRecord?> GetTodayAttendanceByEmployeeIdAsync(int employeeId, bool trackChanges = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<AttendanceRecord>> GetAttendanceWithEmployeeDetailsAsync(DateTime? date = null, bool trackChanges = false, CancellationToken cancellationToken = default);
}
