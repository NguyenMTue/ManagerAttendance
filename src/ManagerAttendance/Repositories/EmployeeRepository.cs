using ManagerAttendance.Enums;
using ManagerAttendance.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagerAttendance.Repositories;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> GetEmployeesWithDetailsAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(e => e.AttendanceRecords.OrderByDescending(a => a.ArrivalTime));
        return trackChanges 
            ? await query.ToListAsync(cancellationToken) 
            : await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetEmployeeWithDetailsByIdAsync(int id, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(e => e.AttendanceRecords.OrderByDescending(a => a.ArrivalTime));
        return trackChanges 
            ? await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken) 
            : await query.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee?> GetByUserIdAsync(string userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        return trackChanges 
            ? await query.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken) 
            : await query.AsNoTracking().FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
    }

    public async Task<Employee?> GetByEmailAsync(string email, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        return trackChanges 
            ? await query.FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower(), cancellationToken) 
            : await query.AsNoTracking().FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(DepartmentType department, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.Department == department);
        return trackChanges 
            ? await query.ToListAsync(cancellationToken) 
            : await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TDerived>> GetEmployeesByTypeAsync<TDerived>(bool trackChanges = false, CancellationToken cancellationToken = default) where TDerived : Employee
    {
        var query = _context.Set<TDerived>().AsQueryable();
        return trackChanges 
            ? await query.ToListAsync(cancellationToken) 
            : await query.AsNoTracking().ToListAsync(cancellationToken);
    }
}
