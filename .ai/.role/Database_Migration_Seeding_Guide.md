# Database Migration & Seeding Instruction Guide

This document outlines the approach for Agent 1 and Agent 5 to implement Entity Framework Core Migrations and Data Seeding for the Employee Attendance Management System. 
The goal is to automatically generate the database schema matching the provided SQL script and populate it with initial data upon running the application.

## 📌 1. Database Schema Overview
Based on the provided `db.sql`, the database includes the following key tables:
- **ASP.NET Core Identity Tables:** `AspNetRoles`, `AspNetUsers`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, `AspNetUserTokens`, `AspNetRoleClaims`.
- **Domain Tables:** 
  - `Employees`: Uses Table-Per-Hierarchy (TPH) strategy indicated by the `EmployeeType` discriminator column. Contains common fields (FirstName, LastName, Department) and role-specific fields (Band, TechnicalDirection, ManagerType, CodingSkillsFlag).
  - `AttendanceRecords`: Records employee check-ins and check-outs (ArrivalTime, DepartureTime).

## 📌 2. Agent Responsibilities for Database Creation

### 🎯 Agent 1: Database & Domain Expert
**Task:** Define the EF Core Models and DbContext.
1. **Entity Definition:**
   - Define `Employee` as an abstract base class.
   - Define derived classes: `Developer`, `QA`, and `Manager`.
   - Define `AttendanceRecord`.
   - Integrate ASP.NET Core Identity by having the `ApplicationDbContext` inherit from `IdentityDbContext<IdentityUser>`.
2. **Fluent API Configuration (in `OnModelCreating`):**
   - **Identity:** Ensure base `OnModelCreating(builder)` is called to map Identity tables correctly.
   - **TPH Configuration:** Configure the `Employees` table with a discriminator column `EmployeeType`.
     ```csharp
     builder.Entity<Employee>()
         .HasDiscriminator<string>("EmployeeType")
         .HasValue<Developer>("Developer")
         .HasValue<QA>("QA")
         .HasValue<Manager>("Manager");
     ```
   - **Relationships:** Configure the one-to-many relationship between `Employee` and `AttendanceRecord`.
   - **Default Values:** Map `IsActive` default value to `true` (`1`).

### 🎯 Agent 5: System Integration & Middleware Engineer
**Task:** Implement Automatic Migration and Data Seeding logic.
1. **Migration Execution:**
   - Configure the application in `Program.cs` to apply migrations automatically on startup using a scope:
     ```csharp
     using (var scope = app.Services.CreateScope())
     {
         var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
         dbContext.Database.Migrate(); // Creates DB and Tables if not exist
         
         // Call Seeder
         var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
         var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
         await DatabaseSeeder.SeedDataAsync(dbContext, userManager, roleManager);
     }
     ```
2. **Data Seeder Implementation (`DatabaseSeeder.cs`):**
   - **Seed Roles:** Create default Identity roles (e.g., "Admin", "Manager", "Employee") if they don't exist.
   - **Seed Users (Admin):** Create a default Admin Identity user and assign the "Admin" role.
   - **Seed Domain Data:** 
     - Check if `dbContext.Employees.Any()` is false.
     - If empty, insert sample `Manager`, `Developer`, and `QA` records.
     - Insert sample `AttendanceRecord` entries linked to the seeded employees.

## 📌 3. Execution Steps for Developers
1. Ensure the connection string in `appsettings.json` points to a completely empty or non-existent database.
2. Run EF Core commands to create the initial migration (if not using code-first entirely from scratch with pre-existing scripts):
   ```bash
   dotnet ef migrations add InitialCreate
   ```
3. Run the application (`dotnet run`). The application will:
   - Create the database.
   - Execute the migration to generate tables (matching the structure of `db.sql`).
   - Execute the seeder to insert default roles, an admin account, and sample employee/attendance data.
