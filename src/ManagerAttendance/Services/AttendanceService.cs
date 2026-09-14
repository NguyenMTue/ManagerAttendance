using AutoMapper;
using ManagerAttendance.DTOs;
using ManagerAttendance.Enums;
using ManagerAttendance.Models;
using ManagerAttendance.Repositories;

namespace ManagerAttendance.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AttendanceService> _logger;

    public AttendanceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AttendanceService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AttendanceRecordDto>> GetAllAttendanceAsync()
    {
        var records = await _unitOfWork.AttendanceRecords.GetAttendanceWithEmployeeDetailsAsync();
        return _mapper.Map<IEnumerable<AttendanceRecordDto>>(records);
    }

    public async Task<AttendanceRecordDto?> GetAttendanceByIdAsync(int id)
    {
        var record = await _unitOfWork.AttendanceRecords.GetByIdAsync(id);
        if (record == null) return null;
        return _mapper.Map<AttendanceRecordDto>(record);
    }

    public async Task<IEnumerable<AttendanceRecordDto>> GetAttendanceByEmployeeIdAsync(int employeeId)
    {
        var records = await _unitOfWork.AttendanceRecords.GetAttendanceByEmployeeIdAsync(employeeId);
        return _mapper.Map<IEnumerable<AttendanceRecordDto>>(records);
    }

    public async Task<AttendanceRecordDto> CheckInAsync(CheckInDto dto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(dto.EmployeeId);
        if (employee == null)
        {
            throw new ArgumentException($"Employee with Id {dto.EmployeeId} does not exist.");
        }

        var todayRecord = await _unitOfWork.AttendanceRecords.GetTodayAttendanceByEmployeeIdAsync(dto.EmployeeId);
        if (todayRecord != null)
        {
            _logger.LogInformation("Employee Id {EmployeeId} has already checked in today.", dto.EmployeeId);
            return _mapper.Map<AttendanceRecordDto>(todayRecord);
        }

        var now = DateTime.UtcNow;
        var status = (now.TimeOfDay > new TimeSpan(9, 0, 0)) 
            ? AttendanceStatus.Late 
            : AttendanceStatus.Present;

        var record = new AttendanceRecord
        {
            EmployeeId = dto.EmployeeId,
            ArrivalTime = now,
            Status = status,
            Notes = dto.Notes,
            CreatedAt = now
        };

        await _unitOfWork.AttendanceRecords.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Employee Id {EmployeeId} checked in at {ArrivalTime} with status {Status}.", dto.EmployeeId, now, status);

        var savedRecord = await _unitOfWork.AttendanceRecords.GetByIdAsync(record.Id);
        return _mapper.Map<AttendanceRecordDto>(savedRecord ?? record);
    }

    public async Task<AttendanceRecordDto?> CheckOutAsync(CheckOutDto dto)
    {
        var todayRecord = await _unitOfWork.AttendanceRecords.GetTodayAttendanceByEmployeeIdAsync(dto.EmployeeId, trackChanges: true);
        if (todayRecord == null)
        {
            _logger.LogWarning("CheckOut failed: No today's check-in record found for Employee Id {EmployeeId}.", dto.EmployeeId);
            return null;
        }

        todayRecord.DepartureTime = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(dto.Notes))
        {
            todayRecord.Notes = string.IsNullOrEmpty(todayRecord.Notes) 
                ? dto.Notes 
                : $"{todayRecord.Notes} | CheckOut Note: {dto.Notes}";
        }

        _unitOfWork.AttendanceRecords.Update(todayRecord);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Employee Id {EmployeeId} checked out at {DepartureTime}.", dto.EmployeeId, todayRecord.DepartureTime);

        return _mapper.Map<AttendanceRecordDto>(todayRecord);
    }

    public async Task<bool> DeleteAttendanceAsync(int id)
    {
        var record = await _unitOfWork.AttendanceRecords.GetByIdAsync(id);
        if (record == null)
        {
            _logger.LogWarning("Delete attendance failed: Record with Id {Id} not found.", id);
            return false;
        }

        _unitOfWork.AttendanceRecords.Remove(record);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Deleted attendance record with Id: {Id}", id);
        return true;
    }
}
