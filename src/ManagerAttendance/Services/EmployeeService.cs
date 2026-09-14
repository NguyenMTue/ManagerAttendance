using ManagerAttendance.DTOs;
using ManagerAttendance.Repositories;

namespace ManagerAttendance.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<EmployeeDto> CreateDeveloperAsync(CreateDeveloperDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<EmployeeDto> CreateQAAsync(CreateQADto dto)
    {
        throw new NotImplementedException();
    }

    public Task<EmployeeDto> CreateManagerAsync(CreateManagerDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteEmployeeAsync(int id)
    {
        throw new NotImplementedException();
    }
}
