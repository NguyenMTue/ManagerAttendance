using ManagerAttendance.Enums;
using ManagerAttendance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        // 2. Ensure Accounts for Each Role
        // 2.1 Admin Role Account
        var adminEmail = "admin@attendance.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 2.2 Manager Role Account (IT Department Manager)
        var managerEmail = "manager.it@attendance.com";
        var managerUser = await userManager.FindByEmailAsync(managerEmail);
        if (managerUser == null)
        {
            managerUser = new IdentityUser { UserName = managerEmail, Email = managerEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(managerUser, "Manager123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }
        }

        // 2.3 Regular Employee Role Accounts (IT Department Staff)
        var itEmployeeSeedData = new[]
        {
            new { Email = "dev.backend@attendance.com", Password = "Employee123!", FirstName = "Nguyen", LastName = "Van A", Type = "Developer", Tech = "C# / ASP.NET Core", Skills = "C#, EF Core, SQL Server", Band = BandType.Senior },
            new { Email = "dev.frontend@attendance.com", Password = "Employee123!", FirstName = "Tran", LastName = "Thi B", Type = "Developer", Tech = "React / TypeScript", Skills = "React, Redux, Tailwind", Band = BandType.Mid },
            new { Email = "dev.fullstack@attendance.com", Password = "Employee123!", FirstName = "Le", LastName = "Van C", Type = "Developer", Tech = "Fullstack (.NET + Vue)", Skills = "ASP.NET Core, Vue.js, Docker", Band = BandType.Senior },
            new { Email = "qa.automation@attendance.com", Password = "Employee123!", FirstName = "Pham", LastName = "Thi D", Type = "QA", Tech = "Playwright / Selenium", Skills = "C#, Playwright, CI/CD", Band = BandType.Mid },
            new { Email = "qa.manual@attendance.com", Password = "Employee123!", FirstName = "Hoang", LastName = "Van E", Type = "QA", Tech = "API & UI Manual Testing", Skills = "Postman, Swagger, TestRail", Band = BandType.Junior }
        };

        var createdUserMap = new Dictionary<string, IdentityUser>();

        foreach (var empSeed in itEmployeeSeedData)
        {
            var user = await userManager.FindByEmailAsync(empSeed.Email);
            if (user == null)
            {
                user = new IdentityUser { UserName = empSeed.Email, Email = empSeed.Email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, empSeed.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Employee");
                }
            }
            createdUserMap[empSeed.Email] = user;
        }

        // 3. Seed IT Department Employees (Domain Entities)
        if (!await dbContext.Employees.AnyAsync(e => e.Department == DepartmentType.IT))
        {
            // Seed IT Manager Entity
            var itManagerEntity = new Manager
            {
                UserId = managerUser?.Id ?? string.Empty,
                FirstName = "Trinh",
                LastName = "Quoc Truong",
                Email = managerEmail,
                Gender = GenderType.Male,
                Department = DepartmentType.IT,
                Band = BandType.Lead,
                IsActive = true,
                ManagerType = ManagerType.Technical,
                ManagedDepartment = "IT Department"
            };
            await dbContext.Employees.AddAsync(itManagerEntity);

            // Seed IT Regular Employee Entities
            foreach (var seed in itEmployeeSeedData)
            {
                var user = createdUserMap[seed.Email];
                Employee empEntity;

                if (seed.Type == "Developer")
                {
                    empEntity = new Developer
                    {
                        UserId = user.Id,
                        FirstName = seed.FirstName,
                        LastName = seed.LastName,
                        Email = seed.Email,
                        Gender = seed.FirstName.Contains("Thi") ? GenderType.Female : GenderType.Male,
                        Department = DepartmentType.IT,
                        Band = seed.Band,
                        IsActive = true,
                        TechnicalDirection = seed.Tech,
                        CodingSkillsFlag = seed.Skills
                    };
                }
                else
                {
                    empEntity = new QA
                    {
                        UserId = user.Id,
                        FirstName = seed.FirstName,
                        LastName = seed.LastName,
                        Email = seed.Email,
                        Gender = seed.FirstName.Contains("Thi") ? GenderType.Female : GenderType.Male,
                        Department = DepartmentType.IT,
                        Band = seed.Band,
                        IsActive = true,
                        TestingMethodology = seed.Tech,
                        AutomationSkills = seed.Tech.Contains("Playwright")
                    };
                }

                await dbContext.Employees.AddAsync(empEntity);
            }

            await dbContext.SaveChangesAsync();
        }

        // 4. Seed Attendance Records
        // - Yesterday: Every IT department employee has an attendance record.
        // - Today: NO ONE has checked in today.
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var today = DateTime.UtcNow.Date;

        // Remove any today records to ensure zero check-ins today
        var todayRecords = await dbContext.AttendanceRecords
            .Where(a => a.ArrivalTime >= today)
            .ToListAsync();

        if (todayRecords.Any())
        {
            dbContext.AttendanceRecords.RemoveRange(todayRecords);
            await dbContext.SaveChangesAsync();
        }

        // Ensure every IT employee has a yesterday attendance record
        var itEmployees = await dbContext.Employees
            .Where(e => e.Department == DepartmentType.IT)
            .ToListAsync();

        foreach (var emp in itEmployees)
        {
            var hasYesterdayRecord = await dbContext.AttendanceRecords
                .AnyAsync(a => a.EmployeeId == emp.Id && a.ArrivalTime >= yesterday && a.ArrivalTime < today);

            if (!hasYesterdayRecord)
            {
                var isLate = emp.Id % 2 == 0;
                var arrivalTime = isLate 
                    ? yesterday.AddHours(9).AddMinutes(15)  // 9:15 AM (Late)
                    : yesterday.AddHours(8).AddMinutes(30); // 8:30 AM (Present)

                var departureTime = yesterday.AddHours(17).AddMinutes(30); // 5:30 PM

                var attendanceRecord = new AttendanceRecord
                {
                    EmployeeId = emp.Id,
                    ArrivalTime = arrivalTime,
                    DepartureTime = departureTime,
                    Status = isLate ? AttendanceStatus.Late : AttendanceStatus.Present,
                    Notes = isLate ? "Check-in 15m late due to traffic." : "Full day worked. Checked out on time.",
                    CreatedAt = yesterday
                };

                await dbContext.AttendanceRecords.AddAsync(attendanceRecord);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
