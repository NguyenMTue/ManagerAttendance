using AutoMapper;
using ExcelDataReader;
using ManagerAttendance.DTOs;
using ManagerAttendance.Enums;
using ManagerAttendance.Models;
using ManagerAttendance.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ManagerAttendance.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeService> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<EmployeeService> logger,
        UserManager<IdentityUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _unitOfWork.Employees.GetEmployeesWithDetailsAsync();
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetEmployeeWithDetailsByIdAsync(id);
        if (employee == null) return null;
        return _mapper.Map<EmployeeDto>(employee);
    }

    private async Task<string> EnsureUserCreatedAsync(string email, string roleName, string defaultPassword = "Employee123!")
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return existingUser.Id;
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createRes = await _userManager.CreateAsync(user, defaultPassword);
        if (createRes.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, roleName);
            return user.Id;
        }

        _logger.LogError("Failed to create IdentityUser for {Email}: {Errors}", email, string.Join(", ", createRes.Errors.Select(e => e.Description)));
        return Guid.NewGuid().ToString();
    }

    public async Task<EmployeeDto> CreateDeveloperAsync(CreateDeveloperDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            dto.UserId = await EnsureUserCreatedAsync(dto.Email, "Employee");
        }
        var developer = _mapper.Map<Developer>(dto);
        await _unitOfWork.Employees.AddAsync(developer);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Created Developer employee with Id: {Id}", developer.Id);
        return _mapper.Map<EmployeeDto>(developer);
    }

    public async Task<EmployeeDto> CreateQAAsync(CreateQADto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            dto.UserId = await EnsureUserCreatedAsync(dto.Email, "Employee");
        }
        var qa = _mapper.Map<QA>(dto);
        await _unitOfWork.Employees.AddAsync(qa);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Created QA employee with Id: {Id}", qa.Id);
        return _mapper.Map<EmployeeDto>(qa);
    }

    public async Task<EmployeeDto> CreateManagerAsync(CreateManagerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            dto.UserId = await EnsureUserCreatedAsync(dto.Email, "Manager");
        }
        var manager = _mapper.Map<Manager>(dto);
        await _unitOfWork.Employees.AddAsync(manager);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Created Manager employee with Id: {Id}", manager.Id);
        return _mapper.Map<EmployeeDto>(manager);
    }

    public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Update failed: Employee with Id {Id} not found.", id);
            return false;
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Gender = dto.Gender;
        employee.Department = dto.Department;
        employee.Band = dto.Band;
        employee.IsActive = dto.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        if (employee is Developer dev)
        {
            if (!string.IsNullOrEmpty(dto.TechnicalDirection)) dev.TechnicalDirection = dto.TechnicalDirection;
            if (!string.IsNullOrEmpty(dto.CodingSkillsFlag)) dev.CodingSkillsFlag = dto.CodingSkillsFlag;
        }
        else if (employee is QA qa)
        {
            if (!string.IsNullOrEmpty(dto.TestingMethodology)) qa.TestingMethodology = dto.TestingMethodology;
            if (dto.AutomationSkills.HasValue) qa.AutomationSkills = dto.AutomationSkills.Value;
        }
        else if (employee is Manager mgr)
        {
            if (dto.ManagerType.HasValue) mgr.ManagerType = dto.ManagerType.Value;
            if (!string.IsNullOrEmpty(dto.ManagedDepartment)) mgr.ManagedDepartment = dto.ManagedDepartment;
        }

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated employee with Id: {Id}", id);
        return true;
    }

    public async Task<bool> PromoteEmployeeAsync(int id, PromoteEmployeeDto dto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Promote failed: Employee with Id {Id} not found.", id);
            return false;
        }

        employee.Band = dto.Band;
        if (dto.Department.HasValue)
        {
            employee.Department = dto.Department.Value;
        }
        employee.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Promoted employee Id {Id} to Band: {Band}, Department: {Department}", id, employee.Band, employee.Department);
        return true;
    }

    public async Task<bool> UpdateEmployeeStatusAsync(int id, UpdateEmployeeStatusDto dto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Update status failed: Employee with Id {Id} not found.", id);
            return false;
        }

        employee.IsActive = dto.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated status for employee Id {Id}. IsActive: {IsActive}", id, dto.IsActive);
        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Delete failed: Employee with Id {Id} not found.", id);
            return false;
        }

        _unitOfWork.Employees.Remove(employee);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Deleted employee with Id: {Id}", id);
        return true;
    }

    public async Task<ExcelImportResultDto> ImportEmployeesFromExcelAsync(Stream excelStream, bool isDryRun, CancellationToken cancellationToken = default)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var result = new ExcelImportResultDto
        {
            IsDryRun = isDryRun
        };

        var existingEmployees = await _unitOfWork.Employees.GetAllAsync();
        var existingEmailsInDb = existingEmployees.Select(e => e.Email.ToLowerInvariant()).ToHashSet();

        var emailsInFile = new HashSet<string>();
        var validEntities = new List<(Employee Entity, string Password, string Role)>();

        using var reader = ExcelReaderFactory.CreateReader(excelStream);
        if (!reader.Read())
        {
            result.Errors.Add(new ExcelRowErrorDto
            {
                RowIndex = 0,
                FieldName = "File",
                ErrorMessage = "File Excel rỗng hoặc không có dữ liệu."
            });
            return result;
        }

        // Header mapping dictionary (colIndex -> HeaderKey)
        var headerMap = new Dictionary<int, string>();
        for (int col = 0; col < reader.FieldCount; col++)
        {
            var headerName = reader.GetValue(col)?.ToString()?.Trim() ?? string.Empty;
            var normalized = NormalizeHeaderKey(headerName);
            if (!string.IsNullOrEmpty(normalized))
            {
                headerMap[col] = normalized;
            }
        }

        int rowIndex = 1; // Row 1 was Header
        while (reader.Read())
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowIndex++;

            // Extract row values into dictionary
            var rowValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool hasAnyValue = false;

            for (int col = 0; col < reader.FieldCount; col++)
            {
                if (headerMap.TryGetValue(col, out var key))
                {
                    var rawVal = reader.GetValue(col)?.ToString()?.Trim() ?? string.Empty;
                    rowValues[key] = rawVal;
                    if (!string.IsNullOrEmpty(rawVal)) hasAnyValue = true;
                }
            }

            if (!hasAnyValue) continue; // Skip completely empty rows

            result.TotalRows++;
            var rowErrors = new List<ExcelRowErrorDto>();

            // 1. Validate FirstName & LastName
            rowValues.TryGetValue("FirstName", out var firstName);
            rowValues.TryGetValue("LastName", out var lastName);

            if (string.IsNullOrWhiteSpace(firstName))
            {
                rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "FirstName", ErrorMessage = "Họ và tên đệm không được để trống." });
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "LastName", ErrorMessage = "Tên không được để trống." });
            }

            // 2. Validate Email
            rowValues.TryGetValue("Email", out var email);
            email = email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "Email", ErrorMessage = "Email không được để trống." });
            }
            else
            {
                var lowerEmail = email.ToLowerInvariant();
                if (!email.Contains("@") || !email.Contains("."))
                {
                    rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "Email", ErrorMessage = "Email không hợp lệ.", RawData = email });
                }
                else if (emailsInFile.Contains(lowerEmail))
                {
                    rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "Email", ErrorMessage = "Email trùng lặp trong cùng file Excel.", RawData = email });
                }
                else if (existingEmailsInDb.Contains(lowerEmail))
                {
                    rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "Email", ErrorMessage = "Email đã tồn tại trong cơ sở dữ liệu hệ thống.", RawData = email });
                }
                else
                {
                    emailsInFile.Add(lowerEmail);
                }
            }

            // 3. Validate Password (Default to Employee123! if empty/missing)
            rowValues.TryGetValue("Password", out var password);
            password = password?.Trim();
            if (string.IsNullOrWhiteSpace(password))
            {
                password = "Employee123!";
            }
            else if (password.Length < 6)
            {
                rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "Password", ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.", RawData = password });
            }

            // 4. Validate Gender
            rowValues.TryGetValue("Gender", out var genderStr);
            if (!TryParseGender(genderStr, out GenderType gender))
            {
                rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "Gender", ErrorMessage = $"Giới tính '{genderStr}' không hợp lệ (hợp lệ: Male, Female, Other).", RawData = genderStr ?? string.Empty });
            }

            // 5. Validate Department
            rowValues.TryGetValue("Department", out var deptStr);
            if (!TryParseDepartment(deptStr, out DepartmentType dept))
            {
                rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "Department", ErrorMessage = $"Phòng ban '{deptStr}' không hợp lệ.", RawData = deptStr ?? string.Empty });
            }

            // 6. Validate Band
            rowValues.TryGetValue("Band", out var bandStr);
            if (!TryParseBand(bandStr, out BandType band))
            {
                rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "Band", ErrorMessage = $"Cấp bậc '{bandStr}' không hợp lệ.", RawData = bandStr ?? string.Empty });
            }

            // 7. Validate EmployeeType
            rowValues.TryGetValue("EmployeeType", out var empType);
            if (string.IsNullOrWhiteSpace(empType))
            {
                rowErrors.Add(new ExcelRowErrorDto { RowIndex = rowIndex, FieldName = "EmployeeType", ErrorMessage = "Loại nhân viên không được để trống (hợp lệ: Developer, QA, Manager)." });
            }

            if (rowErrors.Any())
            {
                result.Errors.AddRange(rowErrors);
                continue;
            }

            // Create Entity based on EmployeeType
            Employee entity;
            string roleName = "Employee";

            var empTypeLower = empType!.Trim().ToLowerInvariant();
            if (empTypeLower == "developer" || empTypeLower == "dev")
            {
                rowValues.TryGetValue("TechnicalDirection", out var tech);
                rowValues.TryGetValue("CodingSkillsFlag", out var skills);

                entity = new Developer
                {
                    FirstName = firstName!,
                    LastName = lastName!,
                    Email = email!,
                    Gender = gender,
                    Department = dept,
                    Band = band,
                    IsActive = true,
                    TechnicalDirection = string.IsNullOrWhiteSpace(tech) ? "Backend" : tech,
                    CodingSkillsFlag = string.IsNullOrWhiteSpace(skills) ? "C#" : skills
                };
            }
            else if (empTypeLower == "qa")
            {
                rowValues.TryGetValue("TestingMethodology", out var method);
                rowValues.TryGetValue("AutomationSkills", out var autoStr);

                bool autoSkills = autoStr?.ToLowerInvariant() == "true" || autoStr == "1";

                entity = new QA
                {
                    FirstName = firstName!,
                    LastName = lastName!,
                    Email = email!,
                    Gender = gender,
                    Department = dept,
                    Band = band,
                    IsActive = true,
                    TestingMethodology = string.IsNullOrWhiteSpace(method) ? "Manual" : method,
                    AutomationSkills = autoSkills
                };
            }
            else if (empTypeLower == "manager" || empTypeLower == "mgr")
            {
                rowValues.TryGetValue("ManagerType", out var mgrTypeStr);
                rowValues.TryGetValue("ManagedDepartment", out var managedDept);

                if (!TryParseManagerType(mgrTypeStr, out ManagerType mgrType))
                {
                    mgrType = ManagerType.Technical;
                }

                roleName = "Manager";
                entity = new Manager
                {
                    FirstName = firstName!,
                    LastName = lastName!,
                    Email = email!,
                    Gender = gender,
                    Department = dept,
                    Band = band,
                    IsActive = true,
                    ManagerType = mgrType,
                    ManagedDepartment = string.IsNullOrWhiteSpace(managedDept) ? "Software Engineering" : managedDept
                };
            }
            else
            {
                result.Errors.Add(new ExcelRowErrorDto
                {
                    RowIndex = rowIndex,
                    FieldName = "EmployeeType",
                    ErrorMessage = $"Loại nhân viên '{empType}' không hợp lệ (chấp nhận: Developer, QA, Manager).",
                    RawData = empType ?? string.Empty
                });
                continue;
            }

            validEntities.Add((entity, password!, roleName));
        }

        result.ErrorCount = result.Errors.Select(e => e.RowIndex).Distinct().Count();
        result.SuccessCount = validEntities.Count;

        if (isDryRun)
        {
            _logger.LogInformation("Dry-Run Excel Import completed. Total: {Total}, Valid: {Success}, Errors: {ErrorCount}", result.TotalRows, result.SuccessCount, result.ErrorCount);
            result.ImportedEmployees = validEntities.Select(v => _mapper.Map<EmployeeDto>(v.Entity)).ToList();
            return result;
        }

        // If not dry-run, save valid entities to Database asynchronously
        foreach (var item in validEntities)
        {
            var existingUser = await _userManager.FindByEmailAsync(item.Entity.Email);
            if (existingUser != null)
            {
                item.Entity.UserId = existingUser.Id;
                await _unitOfWork.Employees.AddAsync(item.Entity);
                result.ImportedEmployees.Add(_mapper.Map<EmployeeDto>(item.Entity));
            }
            else
            {
                var user = new IdentityUser
                {
                    UserName = item.Entity.Email,
                    Email = item.Entity.Email,
                    EmailConfirmed = true
                };

                var createRes = await _userManager.CreateAsync(user, item.Password);
                if (createRes.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, item.Role);
                    item.Entity.UserId = user.Id;
                    await _unitOfWork.Employees.AddAsync(item.Entity);
                    result.ImportedEmployees.Add(_mapper.Map<EmployeeDto>(item.Entity));
                }
                else
                {
                    var errStr = string.Join("; ", createRes.Errors.Select(e => e.Description));
                    result.Errors.Add(new ExcelRowErrorDto
                    {
                        RowIndex = 0,
                        FieldName = "IdentityUser",
                        ErrorMessage = $"Không thể tạo tài khoản cho {item.Entity.Email}: {errStr}"
                    });
                }
            }
        }

        if (result.ImportedEmployees.Any())
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully bulk imported {Count} employees to database from Excel.", result.ImportedEmployees.Count);
        }

        return result;
    }

    private static string NormalizeHeaderKey(string header)
    {
        if (string.IsNullOrWhiteSpace(header)) return string.Empty;
        var cleaned = header.Trim().ToLowerInvariant().Replace("_", "").Replace(" ", "").Replace("-", "");

        if (cleaned.Contains("firstname") || cleaned.Contains("họvàtênđệm") || cleaned.Contains("họvàtênlót")) return "FirstName";
        if (cleaned.Contains("lastname") || cleaned == "tên") return "LastName";
        if (cleaned.Contains("email")) return "Email";
        if (cleaned.Contains("password") || cleaned.Contains("mậtkhẩu")) return "Password";
        if (cleaned.Contains("gender") || cleaned.Contains("giớitính")) return "Gender";
        if (cleaned.Contains("department") && !cleaned.Contains("managed")) return "Department";
        if (cleaned == "phòngban") return "Department";
        if (cleaned.Contains("band") || cleaned.Contains("cấpbậc")) return "Band";
        if (cleaned.Contains("employeetype") || cleaned.Contains("loạinhânviên") || cleaned == "role") return "EmployeeType";
        if (cleaned.Contains("managertype") && !cleaned.Contains("title")) return "ManagerType";
        if (cleaned.Contains("technical") || cleaned.Contains("chuyênmôn") || cleaned.Contains("kỹthuật")) return "TechnicalDirection";
        if (cleaned.Contains("coding") || cleaned.Contains("kỹnănglậptrình") || cleaned.Contains("kỹnăngcode")) return "CodingSkillsFlag";
        if (cleaned.Contains("testing") || cleaned.Contains("kiểmthử")) return "TestingMethodology";
        if (cleaned.Contains("automation") || cleaned.Contains("tựđộnghóa")) return "AutomationSkills";
        if (cleaned.Contains("managertitle") || cleaned.Contains("loạiquảnlý")) return "ManagerType";
        if (cleaned.Contains("manageddepartment") || cleaned.Contains("phòngbanquảnlý")) return "ManagedDepartment";

        return string.Empty;
    }

    private static bool TryParseGender(string? val, out GenderType gender)
    {
        gender = GenderType.Male;
        if (string.IsNullOrWhiteSpace(val)) return true; // default
        if (Enum.TryParse<GenderType>(val, true, out gender)) return true;

        var lower = val.Trim().ToLowerInvariant();
        if (lower.Contains("nam") || lower == "male" || lower == "1") { gender = GenderType.Male; return true; }
        if (lower.Contains("nữ") || lower == "female" || lower == "2") { gender = GenderType.Female; return true; }
        if (lower.Contains("khác") || lower == "other" || lower == "3") { gender = GenderType.Other; return true; }

        return false;
    }

    private static bool TryParseDepartment(string? val, out DepartmentType dept)
    {
        dept = DepartmentType.Development;
        if (string.IsNullOrWhiteSpace(val)) return true; // default
        if (Enum.TryParse<DepartmentType>(val, true, out dept)) return true;

        var lower = val.Trim().ToLowerInvariant();
        if (lower.Contains("dev") || lower.Contains("phát triển")) { dept = DepartmentType.Development; return true; }
        if (lower.Contains("qa") || lower.Contains("kiểm thử")) { dept = DepartmentType.QA; return true; }
        if (lower.Contains("manage") || lower.Contains("quản lý")) { dept = DepartmentType.Management; return true; }
        if (lower.Contains("hr") || lower.Contains("nhân sự")) { dept = DepartmentType.HR; return true; }
        if (lower.Contains("it") || lower.Contains("công nghệ")) { dept = DepartmentType.IT; return true; }
        if (lower.Contains("finance") || lower.Contains("tài chính")) { dept = DepartmentType.Finance; return true; }
        if (lower.Contains("sales") || lower.Contains("bán hàng")) { dept = DepartmentType.Sales; return true; }
        if (lower.Contains("marketing") || lower.Contains("thị trường")) { dept = DepartmentType.Marketing; return true; }

        return false;
    }

    private static bool TryParseBand(string? val, out BandType band)
    {
        band = BandType.Junior;
        if (string.IsNullOrWhiteSpace(val)) return true; // default
        if (Enum.TryParse<BandType>(val, true, out band)) return true;

        var lower = val.Trim().ToLowerInvariant();
        if (lower.Contains("intern") || lower == "0") { band = BandType.Junior; return true; }
        if (lower.Contains("junior") || lower == "1") { band = BandType.Junior; return true; }
        if (lower.Contains("mid") || lower == "2") { band = BandType.Mid; return true; }
        if (lower.Contains("senior") || lower == "3") { band = BandType.Senior; return true; }
        if (lower.Contains("lead") || lower == "4") { band = BandType.Lead; return true; }
        if (lower.Contains("principal") || lower == "5") { band = BandType.Principal; return true; }

        return false;
    }

    private static bool TryParseManagerType(string? val, out ManagerType mgrType)
    {
        mgrType = ManagerType.Technical;
        if (string.IsNullOrWhiteSpace(val)) return true; // default
        if (Enum.TryParse<ManagerType>(val, true, out mgrType)) return true;

        var lower = val.Trim().ToLowerInvariant();
        if (lower.Contains("tech") || lower == "1") { mgrType = ManagerType.Technical; return true; }
        if (lower.Contains("proj") || lower == "2") { mgrType = ManagerType.Project; return true; }
        if (lower.Contains("oper") || lower == "3") { mgrType = ManagerType.Operations; return true; }
        if (lower.Contains("gen") || lower == "4") { mgrType = ManagerType.General; return true; }

        return false;
    }
}
