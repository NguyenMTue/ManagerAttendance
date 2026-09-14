namespace ManagerAttendance.DTOs;

public class AttendanceRecordDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
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
