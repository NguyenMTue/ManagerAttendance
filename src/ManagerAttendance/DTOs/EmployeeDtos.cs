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
    public DateTime CreatedAt { get; set; }
    public string EmployeeType { get; set; } = string.Empty;

    // Developer Specific
    public string? TechnicalDirection { get; set; }
    public string? CodingSkillsFlag { get; set; }

    // QA Specific
    public string? TestingMethodology { get; set; }
    public bool? AutomationSkills { get; set; }

    // Manager Specific
    public ManagerType? ManagerType { get; set; }
    public string? ManagedDepartment { get; set; }
}

public class CreateDeveloperDto
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public GenderType Gender { get; set; } = GenderType.Male;
    public DepartmentType Department { get; set; } = DepartmentType.Development;
    public BandType Band { get; set; } = BandType.Junior;
    public string TechnicalDirection { get; set; } = string.Empty;
    public string CodingSkillsFlag { get; set; } = string.Empty;
}

public class CreateQADto
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public GenderType Gender { get; set; } = GenderType.Male;
    public DepartmentType Department { get; set; } = DepartmentType.QA;
    public BandType Band { get; set; } = BandType.Junior;
    public string TestingMethodology { get; set; } = string.Empty;
    public bool AutomationSkills { get; set; }
}

public class CreateManagerDto
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public GenderType Gender { get; set; } = GenderType.Male;
    public DepartmentType Department { get; set; } = DepartmentType.Management;
    public BandType Band { get; set; } = BandType.Senior;
    public ManagerType ManagerType { get; set; } = ManagerType.Technical;
    public string ManagedDepartment { get; set; } = string.Empty;
}

public class UpdateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public GenderType Gender { get; set; }
    public DepartmentType Department { get; set; }
    public BandType Band { get; set; }
    public bool IsActive { get; set; } = true;

    // Derived Specific (Optional)
    public string? TechnicalDirection { get; set; }
    public string? CodingSkillsFlag { get; set; }
    public string? TestingMethodology { get; set; }
    public bool? AutomationSkills { get; set; }
    public ManagerType? ManagerType { get; set; }
    public string? ManagedDepartment { get; set; }
}

public class PromoteEmployeeDto
{
    public BandType Band { get; set; }
    public DepartmentType? Department { get; set; }
}

public class UpdateEmployeeStatusDto
{
    public bool IsActive { get; set; }
    public string? Reason { get; set; }
}

public class ExcelImportResultDto
{
    public bool IsDryRun { get; set; }
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<ExcelRowErrorDto> Errors { get; set; } = new();
    public List<EmployeeDto> ImportedEmployees { get; set; } = new();
}

public class ExcelRowErrorDto
{
    public int RowIndex { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string RawData { get; set; } = string.Empty;
}
