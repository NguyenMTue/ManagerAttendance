using ManagerAttendance.Enums;

namespace ManagerAttendance.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public GenderType Gender { get; set; }
    public DepartmentType Department { get; set; }
    public BandType Band { get; set; }
    public bool IsActive { get; set; }
    public string EmployeeType { get; set; } = string.Empty;

    // Derived specific fields
    public string? TechnicalDirection { get; set; }
    public string? CodingSkillsFlag { get; set; }
    public string? TestingMethodology { get; set; }
    public bool? AutomationSkills { get; set; }
    public ManagerType? ManagerType { get; set; }
    public string? ManagedDepartment { get; set; }
}

public class CreateDeveloperDto
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public GenderType Gender { get; set; }
    public DepartmentType Department { get; set; }
    public BandType Band { get; set; }
    public string TechnicalDirection { get; set; } = string.Empty;
    public string CodingSkillsFlag { get; set; } = string.Empty;
}

public class CreateQADto
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public GenderType Gender { get; set; }
    public DepartmentType Department { get; set; }
    public BandType Band { get; set; }
    public string TestingMethodology { get; set; } = string.Empty;
    public bool AutomationSkills { get; set; }
}

public class CreateManagerDto
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public GenderType Gender { get; set; }
    public DepartmentType Department { get; set; }
    public BandType Band { get; set; }
    public ManagerType ManagerType { get; set; }
    public string ManagedDepartment { get; set; } = string.Empty;
}

public class UpdateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public GenderType Gender { get; set; }
    public DepartmentType Department { get; set; }
    public BandType Band { get; set; }
    public bool IsActive { get; set; }
}
