using ManagerAttendance.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagerAttendance.Repositories;

public class AttendanceRepository : GenericRepository<AttendanceRecord>, IAttendanceRepository
{
    public AttendanceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AttendanceRecord>> GetAttendanceByEmployeeIdAsync(int employeeId)
    {
        return await _dbSet.Where(a => a.EmployeeId == employeeId).ToListAsync();
    }

    public async Task<IEnumerable<AttendanceRecord>> GetAttendanceByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet.Where(a => a.ArrivalTime >= startDate && a.ArrivalTime <= endDate).ToListAsync();
    }

    public async Task<AttendanceRecord?> GetTodayAttendanceByEmployeeIdAsync(int employeeId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.ArrivalTime.Date == today);
    }
}
