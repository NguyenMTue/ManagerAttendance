using ManagerAttendance.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagerAttendance.Repositories;

public class AttendanceRepository : GenericRepository<AttendanceRecord>, IAttendanceRepository
{
    public AttendanceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AttendanceRecord>> GetAttendanceByEmployeeIdAsync(int employeeId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(a => a.EmployeeId == employeeId).OrderByDescending(a => a.ArrivalTime);
        return trackChanges 
            ? await query.ToListAsync(cancellationToken) 
            : await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AttendanceRecord>> GetAttendanceByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(a => a.Employee)
                          .Where(a => a.ArrivalTime >= startDate && a.ArrivalTime <= endDate)
                          .OrderByDescending(a => a.ArrivalTime);
        return trackChanges 
            ? await query.ToListAsync(cancellationToken) 
            : await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<AttendanceRecord?> GetTodayAttendanceByEmployeeIdAsync(int employeeId, bool trackChanges = true, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var query = _dbSet.Where(a => a.EmployeeId == employeeId && a.ArrivalTime.Date == today);
        return trackChanges 
            ? await query.FirstOrDefaultAsync(cancellationToken) 
            : await query.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<AttendanceRecord>> GetAttendanceWithEmployeeDetailsAsync(DateTime? date = null, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(a => a.Employee).AsQueryable();
        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            query = query.Where(a => a.ArrivalTime.Date == targetDate);
        }

        query = query.OrderByDescending(a => a.ArrivalTime);

        return trackChanges 
            ? await query.ToListAsync(cancellationToken) 
            : await query.AsNoTracking().ToListAsync(cancellationToken);
    }
}
