using ManagerAttendance.DTOs;
using ManagerAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerAttendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll()
    {
        var records = await _attendanceService.GetAllAttendanceAsync();
        return Ok(records);
    }

    [HttpGet("employee/{employeeId}")]
    [Authorize]
    public async Task<IActionResult> GetByEmployeeId(int employeeId)
    {
        var records = await _attendanceService.GetAttendanceByEmployeeIdAsync(employeeId);
        return Ok(records);
    }

    [HttpPost("check-in")]
    [Authorize]
    public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
    {
        var record = await _attendanceService.CheckInAsync(dto);
        return Ok(record);
    }

    [HttpPost("check-out")]
    [Authorize]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
    {
        var record = await _attendanceService.CheckOutAsync(dto);
        if (record == null) return BadRequest("Check-in record not found for today.");
        return Ok(record);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _attendanceService.DeleteAttendanceAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
