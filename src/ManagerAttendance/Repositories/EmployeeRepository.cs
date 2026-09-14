using ManagerAttendance.Enums;
using ManagerAttendance.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagerAttendance.Repositories;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> GetEmployeesWithDetailsAsync()
    {
        return await _dbSet.Include(e => e.AttendanceRecords).ToListAsync();
    }

    public async Task<Employee?> GetEmployeeWithDetailsByIdAsync(int id)
    {
        return await _dbSet.Include(e => e.AttendanceRecords)
                           .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetByUserIdAsync(string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.UserId == userId);
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(DepartmentType department)
    {
        return await _dbSet.Where(e => e.Department == department).ToListAsync();
    }
}
