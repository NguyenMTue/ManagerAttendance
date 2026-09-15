using AutoMapper;
using ManagerAttendance.DTOs;
using ManagerAttendance.Models;
using ManagerAttendance.Repositories;

namespace ManagerAttendance.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<EmployeeService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _unitOfWork.Employees.GetEmployeesWithDetailsAsync();
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetEmployeeWithDetailsByIdAsync(id);
        if (employee == null) return null;
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> CreateDeveloperAsync(CreateDeveloperDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            dto.UserId = Guid.NewGuid().ToString();
        }
        var developer = _mapper.Map<Developer>(dto);
        await _unitOfWork.Employees.AddAsync(developer);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Created Developer employee with Id: {Id}", developer.Id);
        return _mapper.Map<EmployeeDto>(developer);
    }

    public async Task<EmployeeDto> CreateQAAsync(CreateQADto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            dto.UserId = Guid.NewGuid().ToString();
        }
        var qa = _mapper.Map<QA>(dto);
        await _unitOfWork.Employees.AddAsync(qa);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Created QA employee with Id: {Id}", qa.Id);
        return _mapper.Map<EmployeeDto>(qa);
    }

    public async Task<EmployeeDto> CreateManagerAsync(CreateManagerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            dto.UserId = Guid.NewGuid().ToString();
        }
        var manager = _mapper.Map<Manager>(dto);
        await _unitOfWork.Employees.AddAsync(manager);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Created Manager employee with Id: {Id}", manager.Id);
        return _mapper.Map<EmployeeDto>(manager);
    }

    public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Update failed: Employee with Id {Id} not found.", id);
            return false;
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Gender = dto.Gender;
        employee.Department = dto.Department;
        employee.Band = dto.Band;
        employee.IsActive = dto.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        if (employee is Developer dev)
        {
            if (!string.IsNullOrEmpty(dto.TechnicalDirection)) dev.TechnicalDirection = dto.TechnicalDirection;
            if (!string.IsNullOrEmpty(dto.CodingSkillsFlag)) dev.CodingSkillsFlag = dto.CodingSkillsFlag;
        }
        else if (employee is QA qa)
        {
            if (!string.IsNullOrEmpty(dto.TestingMethodology)) qa.TestingMethodology = dto.TestingMethodology;
            if (dto.AutomationSkills.HasValue) qa.AutomationSkills = dto.AutomationSkills.Value;
        }
        else if (employee is Manager mgr)
        {
            if (dto.ManagerType.HasValue) mgr.ManagerType = dto.ManagerType.Value;
            if (!string.IsNullOrEmpty(dto.ManagedDepartment)) mgr.ManagedDepartment = dto.ManagedDepartment;
        }

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated employee with Id: {Id}", id);
        return true;
    }

    public async Task<bool> PromoteEmployeeAsync(int id, PromoteEmployeeDto dto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Promote failed: Employee with Id {Id} not found.", id);
            return false;
        }

        employee.Band = dto.Band;
        if (dto.Department.HasValue)
        {
            employee.Department = dto.Department.Value;
        }
        employee.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Promoted employee Id {Id} to Band: {Band}, Department: {Department}", id, employee.Band, employee.Department);
        return true;
    }

    public async Task<bool> UpdateEmployeeStatusAsync(int id, UpdateEmployeeStatusDto dto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Update status failed: Employee with Id {Id} not found.", id);
            return false;
        }

        employee.IsActive = dto.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated status for employee Id {Id}. IsActive: {IsActive}", id, dto.IsActive);
        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Delete failed: Employee with Id {Id} not found.", id);
            return false;
        }

        _unitOfWork.Employees.Remove(employee);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Deleted employee with Id: {Id}", id);
        return true;
    }
}
