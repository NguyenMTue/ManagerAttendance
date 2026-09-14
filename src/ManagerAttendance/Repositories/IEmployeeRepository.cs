using ManagerAttendance.Enums;
using ManagerAttendance.Models;

namespace ManagerAttendance.Repositories;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<IEnumerable<Employee>> GetEmployeesWithDetailsAsync(bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeWithDetailsByIdAsync(int id, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<Employee?> GetByUserIdAsync(string userId, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<Employee?> GetByEmailAsync(string email, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(DepartmentType department, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TDerived>> GetEmployeesByTypeAsync<TDerived>(bool trackChanges = false, CancellationToken cancellationToken = default) where TDerived : Employee;
}
