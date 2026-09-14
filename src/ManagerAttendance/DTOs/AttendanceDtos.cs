using ManagerAttendance.Enums;

namespace ManagerAttendance.DTOs;

public class AttendanceRecordDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public DateTime ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CheckInDto
{
    public int EmployeeId { get; set; }
    public string? Notes { get; set; }
}

public class CheckOutDto
{
    public int EmployeeId { get; set; }
    public string? Notes { get; set; }
}

public class CreateAttendanceDto
{
    public int EmployeeId { get; set; }
    public DateTime ArrivalTime { get; set; } = DateTime.UtcNow;
    public DateTime? DepartureTime { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string? Notes { get; set; }
}

public class UpdateAttendanceDto
{
    public DateTime ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
