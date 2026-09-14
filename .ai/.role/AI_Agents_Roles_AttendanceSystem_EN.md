# AI Agents Role Specification - Employee Attendance Management System

This document clearly defines the roles, responsibilities, and scope of operations for each AI Agent during the Backend development of the Attendance Management system. This division ensures that the code strictly adheres to the established technical standards (ASP.NET Core, EF Core, AutoMapper, DI, Repository/UoW) and directory structure.

## 📁 System Directory Structure Allocation
The system uses the following directories; Agents must store files in their assigned locations:
- `Common/`: Contains utility classes, constants, formatters, and helpers.
- `Configuration/`: Contains system configurations, AutoMapper Profiles settings, and Fluent API configurations for the DB.
- `Controllers/`: API Endpoints handling HTTP Request/Response.
- `DTOs/`: Data Transfer Objects (Request/Response) between Client and Server.
- `Enums/`: Defines enumerations (Role, Gender, Department, Band, etc.).
- `Middlewares/`: Contains Global Exception Handler and Logging Middleware.
- `Models/`: Contains Entity classes mapped to the Database.
- `Repositories/`: Contains Interfaces and Implementations for the Repository Pattern and Unit of Work.
- `Services/`: Contains Business Logic following the rule (1 Interface + 1 Service).

---

## 🤖 AI Agents List & Responsibilities

### 1. Agent 1: Database & Domain Expert
- **Objective:** Design a solid data foundation, ensuring the integrity of the object-oriented model and database.
- **Directory Scope:** `Models/`, `Enums/`, `Configuration/` (for Fluent API).
- **Specific Responsibilities:**
  - Define constants and enumerations in the `Enums/` directory (e.g., `RoleType`, `DepartmentType`).
  - Write Entity classes in the `Models/` directory (e.g., `Employee`, `Attendance`).
  - Apply data inheritance structures for Employees (Developer, QA, Manager) by using **TPH (Table-Per-Hierarchy)** in Entity Framework Core.
  - Configure primary keys, foreign keys, indexes, and entity relationships via Fluent API (stored in the `Configuration/` directory).
- **Note:** Do not write DB query logic; focus solely on the schema.

### 2. Agent 2: Data Access Architect
- **Objective:** Encapsulate all database operations, providing a secure data access interface for the Service layer.
- **Directory Scope:** `Repositories/`.
- **Specific Responsibilities:**
  - Define `IGenericRepository<T>` and implement `GenericRepository<T>`.
  - Define and implement specialized repositories: `IEmployeeRepository`, `IAttendanceRepository` (inheriting from `GenericRepository`).
  - Initialize the **Unit of Work** pattern (`IUnitOfWork` and `UnitOfWork`) for centralized transaction management, ensuring data is saved with a single `SaveChangesAsync()` command.

### 3. Agent 3: Business Logic & Mapping Specialist
- **Objective:** Handle all core application logic, ensuring security and proper data flow.
- **Directory Scope:** `Services/`, `DTOs/`, `Configuration/` (AutoMapper Profile).
- **Specific Responsibilities:**
  - Adhere to the **1 Interface + 1 Service** rule (e.g., `IEmployeeService.cs` and `EmployeeService.cs`).
  - Build data transfer objects in `DTOs/` for Requests and Responses (e.g., `EmployeeLoginDto`, `AttendanceRecordDto`).
  - Set up **AutoMapper Profiles** in `Configuration/` to automatically map data between Models and DTOs.
  - Integrate authorization logic (Managers can only view subordinates/within their department).
  - Interact directly with `IUnitOfWork` (injected via DI).

### 4. Agent 4: API & Security Controller
- **Objective:** Receive and return standard HTTP responses, ensuring the system is secure from unauthorized access.
- **Directory Scope:** `Controllers/`, `Common/` (Action Filters).
- **Specific Responsibilities:**
  - Build Controllers inheriting from `ControllerBase`. Route APIs clearly using route attributes (e.g., `[Route("api/[controller]")]`).
  - Use DI to directly inject interfaces from the `Services/` directory.
  - Apply Authentication & Authorization (JWT): tag accurately with `[Authorize(Roles = "...")]` according to 3 privilege levels: Admin, Manager, Employee.
  - Write Action Filters (in `Common/`) to automatically validate input data (ModelState).
  - Integrate Swagger (clearly describing endpoints).

### 5. Agent 5: System Integration & Middleware Engineer
- **Objective:** Assemble components, establish application processing flows, register DI, and handle global errors.
- **Directory Scope:** `Middlewares/`, `Program.cs`.
- **Specific Responsibilities:**
  - Build a **Global Exception Middleware** in `Middlewares/` to automatically catch errors (e.g., `ArgumentNullException`, `Exception`) and return standard JSON, preventing system crashes.
  - Take charge of registering all **Dependency Injection (DI)** in `Program.cs` (AddScoped for Repository, Unit of Work, Services).
  - Configure the connection string with SQL Server.
  - Configure JWT Bearer Token validation, Swagger with token submission features, and register AutoMapper in the Service Collection.

---

## ⚙️ Coordination & Execution Workflow

When starting code implementation, the AI Agents will operate in the following sequence:

1. **Agent 1** creates Base Enums, Models, and DbContext (EF Core).
2. **Agent 2** uses Models to create Repositories & Unit Of Work.
3. **Agent 3** creates DTOs, configures AutoMapper, and implements Services (calling IUnitOfWork).
4. **Agent 4** creates the Controllers structure, calls Service Interfaces, and applies Auth.
5. **Agent 5** sets up `Program.cs` and Middlewares to connect everything together.

*(Note: The Console App UI will be implemented by a specialized Agent after the API is fully completed).*
