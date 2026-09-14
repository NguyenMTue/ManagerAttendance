namespace ManagerAttendance.Repositories;

public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    IAttendanceRepository AttendanceRecords { get; }
    Task<int> SaveChangesAsync();
}
