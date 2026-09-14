using ManagerAttendance.DTOs;
using ManagerAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerAttendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    [HttpPost("developer")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDeveloper([FromBody] CreateDeveloperDto dto)
    {
        var result = await _employeeService.CreateDeveloperAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("qa")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateQA([FromBody] CreateQADto dto)
    {
        var result = await _employeeService.CreateQAAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("manager")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateManager([FromBody] CreateManagerDto dto)
    {
        var result = await _employeeService.CreateManagerAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var success = await _employeeService.UpdateEmployeeAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _employeeService.DeleteEmployeeAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
