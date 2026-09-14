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

namespace ManagerAttendance.Tests.Services;

[TestFixture]
public class AttendanceServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IEmployeeRepository> _employeeRepoMock;
    private Mock<IAttendanceRepository> _attendanceRepoMock;
    private Mock<ILogger<AttendanceService>> _loggerMock;
    private Mock<IMapper> _mapperMock;
    private AttendanceService _attendanceService;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _employeeRepoMock = new Mock<IEmployeeRepository>();
        _attendanceRepoMock = new Mock<IAttendanceRepository>();
        _loggerMock = new Mock<ILogger<AttendanceService>>();
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock.Setup(u => u.Employees).Returns(_employeeRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.AttendanceRecords).Returns(_attendanceRepoMock.Object);

        // Setup Mapper Mappings
        _mapperMock.Setup(m => m.Map<AttendanceRecordDto>(It.IsAny<AttendanceRecord>()))
            .Returns((AttendanceRecord rec) => new AttendanceRecordDto
            {
                Id = rec.Id,
                EmployeeId = rec.EmployeeId,
                ArrivalTime = rec.ArrivalTime,
                DepartureTime = rec.DepartureTime,
                Status = rec.Status.ToString(),
                Notes = rec.Notes
            });

        _attendanceService = new AttendanceService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task CheckInAsync_WhenEmployeeDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange
        _employeeRepoMock.Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((Employee?)null);

        var dto = new CheckInDto { EmployeeId = 999 };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await _attendanceService.CheckInAsync(dto);
        });
    }

    [Test]
    public async Task CheckInAsync_WhenAlreadyCheckedInToday_ShouldReturnExistingRecord()
    {
        // Arrange
        var employee = new Developer { Id = 1, FirstName = "John", LastName = "Doe" };
        var existingRecord = new AttendanceRecord
        {
            Id = 100,
            EmployeeId = 1,
            Employee = employee,
            ArrivalTime = DateTime.UtcNow.Date.AddHours(8),
            Status = AttendanceStatus.Present
        };

        _employeeRepoMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(employee);

        _attendanceRepoMock.Setup(r => r.GetTodayAttendanceByEmployeeIdAsync(1, true, default))
            .ReturnsAsync(existingRecord);

        var dto = new CheckInDto { EmployeeId = 1 };

        // Act
        var result = await _attendanceService.CheckInAsync(dto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(100);
        result.EmployeeId.ShouldBe(1);

        _attendanceRepoMock.Verify(r => r.AddAsync(It.IsAny<AttendanceRecord>(), default), Times.Never);
    }

    [Test]
    public async Task CheckInAsync_WhenFirstCheckIn_ShouldCreateNewAttendanceRecord()
    {
        // Arrange
        var employee = new Developer { Id = 2, FirstName = "Alice", LastName = "Smith" };

        _employeeRepoMock.Setup(r => r.GetByIdAsync(2, default))
            .ReturnsAsync(employee);

        _attendanceRepoMock.Setup(r => r.GetTodayAttendanceByEmployeeIdAsync(2, true, default))
            .ReturnsAsync((AttendanceRecord?)null);

        _attendanceRepoMock.Setup(r => r.AddAsync(It.IsAny<AttendanceRecord>(), default))
            .Callback<AttendanceRecord, CancellationToken>((rec, token) => rec.Id = 50)
            .Returns(Task.CompletedTask);

        _attendanceRepoMock.Setup(r => r.GetByIdAsync(50, default))
            .ReturnsAsync(new AttendanceRecord { Id = 50, EmployeeId = 2, Employee = employee, Status = AttendanceStatus.Present });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var dto = new CheckInDto { EmployeeId = 2, Notes = "First check in" };

        // Act
        var result = await _attendanceService.CheckInAsync(dto);

        // Assert
        result.ShouldNotBeNull();
        result.EmployeeId.ShouldBe(2);

        _attendanceRepoMock.Verify(r => r.AddAsync(It.IsAny<AttendanceRecord>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Test]
    public async Task CheckOutAsync_WhenTodayRecordExists_ShouldUpdateDepartureTime()
    {
        // Arrange
        var todayRecord = new AttendanceRecord
        {
            Id = 77,
            EmployeeId = 3,
            ArrivalTime = DateTime.UtcNow.Date.AddHours(8),
            DepartureTime = null,
            Notes = "Check in note"
        };

        _attendanceRepoMock.Setup(r => r.GetTodayAttendanceByEmployeeIdAsync(3, true, default))
            .ReturnsAsync(todayRecord);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var dto = new CheckOutDto { EmployeeId = 3, Notes = "Leaving early" };

        // Act
        var result = await _attendanceService.CheckOutAsync(dto);

        // Assert
        result.ShouldNotBeNull();
        result.DepartureTime.ShouldNotBeNull();

        _attendanceRepoMock.Verify(r => r.Update(todayRecord), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Test]
    public async Task CheckOutAsync_WhenNoCheckInRecord_ShouldReturnNull()
    {
        // Arrange
        _attendanceRepoMock.Setup(r => r.GetTodayAttendanceByEmployeeIdAsync(4, true, default))
            .ReturnsAsync((AttendanceRecord?)null);

        var dto = new CheckOutDto { EmployeeId = 4 };

        // Act
        var result = await _attendanceService.CheckOutAsync(dto);

        // Assert
        result.ShouldBeNull();
        _attendanceRepoMock.Verify(r => r.Update(It.IsAny<AttendanceRecord>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
