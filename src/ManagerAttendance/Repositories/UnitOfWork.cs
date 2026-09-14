using ManagerAttendance.Models;

namespace ManagerAttendance.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IEmployeeRepository Employees { get; }
    public IAttendanceRepository AttendanceRecords { get; }

    public UnitOfWork(ApplicationDbContext context, IEmployeeRepository employees, IAttendanceRepository attendanceRecords)
    {
        _context = context;
        Employees = employees;
        AttendanceRecords = attendanceRecords;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
