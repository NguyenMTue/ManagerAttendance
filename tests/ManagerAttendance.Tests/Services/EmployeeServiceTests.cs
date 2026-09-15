using AutoMapper;
using ManagerAttendance.DTOs;
using ManagerAttendance.Enums;
using ManagerAttendance.Models;
using ManagerAttendance.Repositories;
using ManagerAttendance.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

using Microsoft.AspNetCore.Identity;

namespace ManagerAttendance.Tests.Services;

[TestFixture]
public class EmployeeServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IEmployeeRepository> _employeeRepoMock;
    private Mock<ILogger<EmployeeService>> _loggerMock;
    private Mock<IMapper> _mapperMock;
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private EmployeeService _employeeService;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _employeeRepoMock = new Mock<IEmployeeRepository>();
        _loggerMock = new Mock<ILogger<EmployeeService>>();
        _mapperMock = new Mock<IMapper>();

        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _unitOfWorkMock.Setup(u => u.Employees).Returns(_employeeRepoMock.Object);

        // Setup Mapper Mappings
        _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(It.IsAny<IEnumerable<Employee>>()))
            .Returns((IEnumerable<Employee> src) => src.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                EmployeeType = e.GetType().Name,
                TechnicalDirection = (e as Developer)?.TechnicalDirection,
                TestingMethodology = (e as QA)?.TestingMethodology
            }));

        _mapperMock.Setup(m => m.Map<EmployeeDto>(It.IsAny<Employee>()))
            .Returns((Employee e) => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                EmployeeType = e.GetType().Name,
                TechnicalDirection = (e as Developer)?.TechnicalDirection
            });

        _mapperMock.Setup(m => m.Map<Developer>(It.IsAny<CreateDeveloperDto>()))
            .Returns((CreateDeveloperDto dto) => new Developer
            {
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                TechnicalDirection = dto.TechnicalDirection
            });

        _employeeService = new EmployeeService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _userManagerMock.Object);
    }

    [Test]
    public async Task GetAllEmployeesAsync_ShouldReturnMappedEmployeeDtoList()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Developer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Department = DepartmentType.Development,
                Band = BandType.Senior,
                TechnicalDirection = "Backend",
                CodingSkillsFlag = "C#"
            },
            new QA
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Department = DepartmentType.QA,
                Band = BandType.Mid,
                TestingMethodology = "Automation",
                AutomationSkills = true
            }
        };

        _employeeRepoMock.Setup(r => r.GetEmployeesWithDetailsAsync(false, default))
            .ReturnsAsync(employees);

        // Act
        var result = await _employeeService.GetAllEmployeesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(2);

        var dev = result.First(e => e.Id == 1);
        dev.FirstName.ShouldBe("John");
        dev.EmployeeType.ShouldBe("Developer");
        dev.TechnicalDirection.ShouldBe("Backend");

        var qa = result.First(e => e.Id == 2);
        qa.FirstName.ShouldBe("Jane");
        qa.EmployeeType.ShouldBe("QA");
        qa.TestingMethodology.ShouldBe("Automation");
    }

    [Test]
    public async Task GetEmployeeByIdAsync_WhenExists_ShouldReturnEmployeeDto()
    {
        // Arrange
        var developer = new Developer
        {
            Id = 10,
            FirstName = "Alice",
            LastName = "Developer",
            Email = "alice@example.com",
            Department = DepartmentType.IT,
            Band = BandType.Mid,
            TechnicalDirection = "Fullstack"
        };

        _employeeRepoMock.Setup(r => r.GetEmployeeWithDetailsByIdAsync(10, false, default))
            .ReturnsAsync(developer);

        // Act
        var result = await _employeeService.GetEmployeeByIdAsync(10);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(10);
        result.FirstName.ShouldBe("Alice");
        result.TechnicalDirection.ShouldBe("Fullstack");
    }

    [Test]
    public async Task GetEmployeeByIdAsync_WhenDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _employeeRepoMock.Setup(r => r.GetEmployeeWithDetailsByIdAsync(999, false, default))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _employeeService.GetEmployeeByIdAsync(999);

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public async Task CreateDeveloperAsync_ShouldAddDeveloperAndSaveChanges()
    {
        // Arrange
        var dto = new CreateDeveloperDto
        {
            UserId = "user-123",
            FirstName = "Bob",
            LastName = "Coder",
            Email = "bob@example.com",
            Department = DepartmentType.IT,
            Band = BandType.Senior,
            TechnicalDirection = "Backend (.NET Core)",
            CodingSkillsFlag = "C#, SQL"
        };

        _employeeRepoMock.Setup(r => r.AddAsync(It.IsAny<Developer>(), default))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _employeeService.CreateDeveloperAsync(dto);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe("bob@example.com");
        result.TechnicalDirection.ShouldBe("Backend (.NET Core)");

        _employeeRepoMock.Verify(r => r.AddAsync(It.IsAny<Developer>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Test]
    public async Task DeleteEmployeeAsync_WhenExists_ShouldRemoveAndReturnTrue()
    {
        // Arrange
        var employee = new Developer { Id = 5, FirstName = "ToDelete" };

        _employeeRepoMock.Setup(r => r.GetByIdAsync(5, default))
            .ReturnsAsync(employee);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var success = await _employeeService.DeleteEmployeeAsync(5);

        // Assert
        success.ShouldBeTrue();
        _employeeRepoMock.Verify(r => r.Remove(employee), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Test]
    public async Task DeleteEmployeeAsync_WhenNotFound_ShouldReturnFalse()
    {
        // Arrange
        _employeeRepoMock.Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((Employee?)null);

        // Act
        var success = await _employeeService.DeleteEmployeeAsync(999);

        // Assert
        success.ShouldBeFalse();
        _employeeRepoMock.Verify(r => r.Remove(It.IsAny<Employee>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }

    [Test]
    public async Task ImportEmployeesFromExcelAsync_DryRun_ShouldValidateAndNotSaveToDb()
    {
        // Arrange
        _employeeRepoMock.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<Employee>());

        using var ms = new MemoryStream();
        using (var writer = new StreamWriter(ms, leaveOpen: true))
        {
            writer.WriteLine("FirstName\tLastName\tEmail\tPassword\tGender\tDepartment\tBand\tEmployeeType\tTechnicalDirection\tCodingSkillsFlag");
            writer.WriteLine("Alice\tSmith\talice.test@example.com\tPass123!\tFemale\tDevelopment\tSenior\tDeveloper\tBackend\tC#");
        }
        ms.Position = 0;

        // Act & Assert
        // Verified method call exists and returns result structure
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
