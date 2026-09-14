using ManagerAttendance.Enums;
using ManagerAttendance.Models;
using ManagerAttendance.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace ManagerAttendance.Tests.Repositories;

[TestFixture]
public class GenericRepositoryTests
{
    private ApplicationDbContext _context;
    private GenericRepository<AttendanceRecord> _repository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new GenericRepository<AttendanceRecord>(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task AddAsync_And_GetByIdAsync_ShouldWorkCorrectly()
    {
        // Arrange
        var record = new AttendanceRecord
        {
            EmployeeId = 1,
            ArrivalTime = DateTime.UtcNow,
            Status = AttendanceStatus.Present,
            Notes = "Test note"
        };

        // Act
        await _repository.AddAsync(record);
        await _context.SaveChangesAsync();

        var fetched = await _repository.GetByIdAsync(record.Id);

        // Assert
        fetched.ShouldNotBeNull();
        fetched.EmployeeId.ShouldBe(1);
        fetched.Notes.ShouldBe("Test note");
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        // Arrange
        await _repository.AddAsync(new AttendanceRecord { EmployeeId = 1, ArrivalTime = DateTime.UtcNow });
        await _repository.AddAsync(new AttendanceRecord { EmployeeId = 2, ArrivalTime = DateTime.UtcNow });
        await _context.SaveChangesAsync();

        // Act
        var all = await _repository.GetAllAsync();

        // Assert
        all.Count().ShouldBe(2);
    }

    [Test]
    public async Task Remove_ShouldDeleteEntityFromDatabase()
    {
        // Arrange
        var record = new AttendanceRecord { EmployeeId = 5, ArrivalTime = DateTime.UtcNow };
        await _repository.AddAsync(record);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(record);
        await _context.SaveChangesAsync();

        var fetched = await _repository.GetByIdAsync(record.Id);

        // Assert
        fetched.ShouldBeNull();
    }
}
