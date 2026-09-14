using ManagerAttendance.Enums;
using ManagerAttendance.Models;

namespace ManagerAttendance.Repositories;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<IEnumerable<Employee>> GetEmployeesWithDetailsAsync();
    Task<Employee?> GetEmployeeWithDetailsByIdAsync(int id);
    Task<Employee?> GetByUserIdAsync(string userId);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(DepartmentType department);
}
