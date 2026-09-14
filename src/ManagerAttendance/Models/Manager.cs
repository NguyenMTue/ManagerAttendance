using ManagerAttendance.Enums;

namespace ManagerAttendance.Models;

public class Manager : Employee
{
    public ManagerType ManagerType { get; set; }
    public string ManagedDepartment { get; set; } = string.Empty;
}
