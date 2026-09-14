using ManagerAttendance.DTOs;
using ManagerAttendance.Repositories;

namespace ManagerAttendance.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;

    public AttendanceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<IEnumerable<AttendanceRecordDto>> GetAllAttendanceAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AttendanceRecordDto?> GetAttendanceByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AttendanceRecordDto>> GetAttendanceByEmployeeIdAsync(int employeeId)
    {
        throw new NotImplementedException();
    }

    public Task<AttendanceRecordDto> CheckInAsync(CheckInDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<AttendanceRecordDto?> CheckOutAsync(CheckOutDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAttendanceAsync(int id)
    {
        throw new NotImplementedException();
    }
}
