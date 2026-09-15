using ManagerAttendance.Common;
using ManagerAttendance.DTOs;
using ManagerAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ManagerAttendance.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Get all employees (Admin & Manager only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(Summary = "Get All Employees", Description = "Retrieves all employees with detailed attendance records.")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    /// <param name="id">Employee ID</param>
    [HttpGet("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get Employee by ID", Description = "Retrieves a single employee by unique identifier.")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound(new { message = $"Employee with Id {id} not found." });
        }

        return Ok(employee);
    }

    /// <summary>
    /// Create Developer Employee (Admin & Manager only)
    /// </summary>
    [HttpPost("developer")]
    [Authorize(Roles = "Admin,Manager")]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Create Developer Employee", Description = "Creates a new Developer employee using TPH inheritance.")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDeveloper([FromBody] CreateDeveloperDto dto)
    {
        var result = await _employeeService.CreateDeveloperAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Create QA Employee (Admin & Manager only)
    /// </summary>
    [HttpPost("qa")]
    [Authorize(Roles = "Admin,Manager")]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Create QA Employee", Description = "Creates a new QA employee using TPH inheritance.")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateQA([FromBody] CreateQADto dto)
    {
        var result = await _employeeService.CreateQAAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Create Manager Employee (Admin & Manager only)
    /// </summary>
    [HttpPost("manager")]
    [Authorize(Roles = "Admin,Manager")]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Create Manager Employee", Description = "Creates a new Manager employee using TPH inheritance.")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateManager([FromBody] CreateManagerDto dto)
    {
        var result = await _employeeService.CreateManagerAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update Employee Info (Admin & Manager only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Update Employee", Description = "Updates employee information by ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var success = await _employeeService.UpdateEmployeeAsync(id, dto);
        if (!success)
        {
            return NotFound(new { message = $"Employee with Id {id} not found." });
        }

        return NoContent();
    }

    /// <summary>
    /// Promote Employee Band & Department (Admin & Manager only)
    /// </summary>
    [HttpPut("{id}/promote")]
    [Authorize(Roles = "Admin,Manager")]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Promote Employee", Description = "Promotes employee band level and optional department.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Promote(int id, [FromBody] PromoteEmployeeDto dto)
    {
        var success = await _employeeService.PromoteEmployeeAsync(id, dto);
        if (!success)
        {
            return NotFound(new { message = $"Employee with Id {id} not found." });
        }

        return NoContent();
    }

    /// <summary>
    /// Update Employee Active Status (Activate / Deactivate / Terminate) (Admin & Manager only)
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Manager")]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Update Employee Active Status", Description = "Updates active status for employee (e.g. Terminated/Resigned).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateEmployeeStatusDto dto)
    {
        var success = await _employeeService.UpdateEmployeeStatusAsync(id, dto);
        if (!success)
        {
            return NotFound(new { message = $"Employee with Id {id} not found." });
        }

        return NoContent();
    }

    /// <summary>
    /// Delete Employee (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete Employee", Description = "Deletes an employee record by ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _employeeService.DeleteEmployeeAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Employee with Id {id} not found." });
        }

        return NoContent();
    }
}
