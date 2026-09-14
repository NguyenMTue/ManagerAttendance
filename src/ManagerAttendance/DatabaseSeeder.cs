using ManagerAttendance.Enums;
using ManagerAttendance.Models;
using Microsoft.AspNetCore.Identity;

namespace ManagerAttendance;

public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // 1. Seed Roles
        string[] roles = new[] { "Admin", "Manager", "Employee" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Identity Users
        // Admin User
        var adminEmail = "admin@attendance.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Manager User
        var managerEmail = "manager@attendance.com";
        var managerUser = await userManager.FindByEmailAsync(managerEmail);
        if (managerUser == null)
        {
            managerUser = new IdentityUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(managerUser, "Manager123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }
        }

        // Developer User
        var devEmail = "developer@attendance.com";
        var devUser = await userManager.FindByEmailAsync(devEmail);
        if (devUser == null)
        {
            devUser = new IdentityUser
            {
                UserName = devEmail,
                Email = devEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(devUser, "Developer123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(devUser, "Employee");
            }
        }

        // QA User
        var qaEmail = "qa@attendance.com";
        var qaUser = await userManager.FindByEmailAsync(qaEmail);
        if (qaUser == null)
        {
            qaUser = new IdentityUser
            {
                UserName = qaEmail,
                Email = qaEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(qaUser, "QaPass123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(qaUser, "Employee");
            }
        }

        // 3. Seed Employee Entities (Domain Data)
        if (!dbContext.Employees.Any())
        {
            var seededManager = new Manager
            {
                UserId = managerUser?.Id ?? string.Empty,
                FirstName = "John",
                LastName = "Manager",
                Email = managerEmail,
                Gender = GenderType.Male,
                Department = DepartmentType.Management,
                Band = BandType.Senior,
                IsActive = true,
                ManagerType = ManagerType.Technical,
                ManagedDepartment = "Software Engineering"
            };

            var seededDeveloper = new Developer
            {
                UserId = devUser?.Id ?? string.Empty,
                FirstName = "Alice",
                LastName = "Developer",
                Email = devEmail,
                Gender = GenderType.Female,
                Department = DepartmentType.Development,
                Band = BandType.Mid,
                IsActive = true,
                TechnicalDirection = "Backend (.NET Core / Microservices)",
                CodingSkillsFlag = "C#, ASP.NET Core, EF Core, SQL Server, Docker"
            };

            var seededQA = new QA
            {
                UserId = qaUser?.Id ?? string.Empty,
                FirstName = "Bob",
                LastName = "Tester",
                Email = qaEmail,
                Gender = GenderType.Male,
                Department = DepartmentType.QA,
                Band = BandType.Junior,
                IsActive = true,
                TestingMethodology = "Automation & Integration Testing",
                AutomationSkills = true
            };

            await dbContext.Employees.AddRangeAsync(seededManager, seededDeveloper, seededQA);
            await dbContext.SaveChangesAsync();
        }

        // 4. Seed Attendance Records
        if (!dbContext.AttendanceRecords.Any())
        {
            var developer = dbContext.Employees.FirstOrDefault(e => e.Email == devEmail);
            var qa = dbContext.Employees.FirstOrDefault(e => e.Email == qaEmail);

            if (developer != null)
            {
                var today = DateTime.UtcNow.Date;
                var record1 = new AttendanceRecord
                {
                    EmployeeId = developer.Id,
                    ArrivalTime = today.AddHours(8).AddMinutes(30),
                    DepartureTime = today.AddHours(17).AddMinutes(30),
                    Status = AttendanceStatus.Present,
                    Notes = "On-time arrival. Worked on feature modules.",
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.AttendanceRecords.AddAsync(record1);
            }

            if (qa != null)
            {
                var today = DateTime.UtcNow.Date;
                var record2 = new AttendanceRecord
                {
                    EmployeeId = qa.Id,
                    ArrivalTime = today.AddHours(9).AddMinutes(15),
                    DepartureTime = today.AddHours(18).AddMinutes(0),
                    Status = AttendanceStatus.Late,
                    Notes = "Traffic delay. Worked late to compensate.",
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.AttendanceRecords.AddAsync(record2);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
