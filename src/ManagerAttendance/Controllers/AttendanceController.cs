using System.Security.Claims;
using ManagerAttendance.Common;
using ManagerAttendance.DTOs;
using ManagerAttendance.Repositories;
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
    private readonly IUnitOfWork _unitOfWork;

    public AttendanceController(IAttendanceService attendanceService, IUnitOfWork unitOfWork)
    {
        _attendanceService = attendanceService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Helper method to resolve current logged-in user's Employee ID
    /// </summary>
    private async Task<int?> GetCurrentEmployeeIdAsync()
    {
        var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
        if (!string.IsNullOrEmpty(employeeIdClaim) && int.TryParse(employeeIdClaim, out int empId))
        {
            return empId;
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            var emp = await _unitOfWork.Employees.GetByUserIdAsync(userId);
            if (emp != null) return emp.Id;
        }

        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            var emp = await _unitOfWork.Employees.GetByEmailAsync(email);
            if (emp != null) return emp.Id;
        }

        return null;
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
    /// Get attendance history for the authenticated logged-in user
    /// </summary>
    [HttpGet("my-history")]
    [Authorize]
    [SwaggerOperation(Summary = "Get Current Employee Attendance History", Description = "Retrieves attendance history for the authenticated employee.")]
    [ProducesResponseType(typeof(IEnumerable<AttendanceRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyHistory()
    {
        var empId = await GetCurrentEmployeeIdAsync();
        if (!empId.HasValue)
        {
            return BadRequest(new { message = "Không tìm thấy thông tin nhân viên cho tài khoản đăng nhập hiện tại." });
        }

        var records = await _attendanceService.GetAttendanceByEmployeeIdAsync(empId.Value);
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
    /// Employee Check-In Endpoint (For Logged-in User)
    /// </summary>
    [HttpPost("check-in")]
    [Authorize]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Employee Check-In", Description = "Records check-in timestamp for the authenticated employee. Automatically sets status Present or Late (after 9:00 AM).")]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInDto? dto)
    {
        dto ??= new CheckInDto();

        var empId = await GetCurrentEmployeeIdAsync();
        if (!empId.HasValue)
        {
            return BadRequest(new { message = "Không tìm thấy thông tin nhân viên liên kết với tài khoản hiện tại." });
        }

        dto.EmployeeId = empId.Value;

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
    /// Employee Check-Out Endpoint (For Logged-in User)
    /// </summary>
    [HttpPost("check-out")]
    [Authorize]
    [ValidationFilter]
    [SwaggerOperation(Summary = "Employee Check-Out", Description = "Records check-out timestamp for the authenticated employee's today record.")]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutDto? dto)
    {
        dto ??= new CheckOutDto();

        var empId = await GetCurrentEmployeeIdAsync();
        if (!empId.HasValue)
        {
            return BadRequest(new { message = "Không tìm thấy thông tin nhân viên liên kết với tài khoản hiện tại." });
        }

        dto.EmployeeId = empId.Value;

        var record = await _attendanceService.CheckOutAsync(dto);
        if (record == null)
        {
            return BadRequest(new { message = "Check-out thất bại: Không tìm thấy lượt Check-in hôm nay cho tài khoản này." });
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
