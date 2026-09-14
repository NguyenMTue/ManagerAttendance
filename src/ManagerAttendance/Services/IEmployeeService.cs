using ManagerAttendance.DTOs;

namespace ManagerAttendance.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
    Task<EmployeeDto> CreateDeveloperAsync(CreateDeveloperDto dto);
    Task<EmployeeDto> CreateQAAsync(CreateQADto dto);
    Task<EmployeeDto> CreateManagerAsync(CreateManagerDto dto);
    Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);
    Task<bool> DeleteEmployeeAsync(int id);
}
