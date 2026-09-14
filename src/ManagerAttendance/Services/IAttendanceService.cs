using ManagerAttendance.DTOs;

namespace ManagerAttendance.Services;

public interface IAttendanceService
{
    Task<IEnumerable<AttendanceRecordDto>> GetAllAttendanceAsync();
    Task<AttendanceRecordDto?> GetAttendanceByIdAsync(int id);
    Task<IEnumerable<AttendanceRecordDto>> GetAttendanceByEmployeeIdAsync(int employeeId);
    Task<AttendanceRecordDto> CheckInAsync(CheckInDto dto);
    Task<AttendanceRecordDto?> CheckOutAsync(CheckOutDto dto);
    Task<bool> DeleteAttendanceAsync(int id);
}
