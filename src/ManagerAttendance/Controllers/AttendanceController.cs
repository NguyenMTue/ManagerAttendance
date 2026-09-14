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
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    /// <summary>
    /// Get all attendance records (Admin & Manager only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(Summary = "Get All Attendance Records", Description = "Retrieves attendance records for all employees.")]
    [ProducesResponseType(typeof(IEnumerable<AttendanceRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var records = await _attendanceService.GetAllAttendanceAsync();
        return Ok(records);
    }

    /// <summary>
    /// Get attendance records for specific employee
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    [HttpGet("employee/{employeeId}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get Employee Attendance History", Description = "Retrieves attendance history for a specific employee.")]
    [ProducesResponseType(typeof(IEnumerable<AttendanceRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByEmployeeId(int employeeId)
    {
        var records = await _attendanceService.GetAttendanceByEmployeeIdAsync(employeeId);
        return Ok(records);
    }

    /// <summary>
    /// Employee Check-In Endpoint
    /// </summary>
    [HttpPost("check-in")]
    [Authorize]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Employee Check-In", Description = "Records employee check-in timestamp. Automatically sets status Present or Late (after 9:00 AM).")]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
    {
        try
        {
            var record = await _attendanceService.CheckInAsync(dto);
            return Ok(record);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Employee Check-Out Endpoint
    /// </summary>
    [HttpPost("check-out")]
    [Authorize]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Employee Check-Out", Description = "Records employee check-out timestamp for today's attendance record.")]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
    {
        var record = await _attendanceService.CheckOutAsync(dto);
        if (record == null)
        {
            return BadRequest(new { message = "Check-out failed: Today's check-in record was not found for this employee." });
        }

        return Ok(record);
    }

    /// <summary>
    /// Delete Attendance Record (Admin only)
    /// </summary>
    /// <param name="id">Attendance Record ID</param>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete Attendance Record", Description = "Deletes an attendance record by ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _attendanceService.DeleteAttendanceAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Attendance record with Id {id} not found." });
        }

        return NoContent();
    }
}
