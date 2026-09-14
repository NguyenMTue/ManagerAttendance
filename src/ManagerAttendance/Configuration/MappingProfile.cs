using AutoMapper;
using ManagerAttendance.DTOs;
using ManagerAttendance.Enums;
using ManagerAttendance.Models;

namespace ManagerAttendance.Configuration;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Employee -> EmployeeDto
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.GetType().Name))
            .ForMember(dest => dest.TechnicalDirection, opt => opt.MapFrom(src => src is Developer ? ((Developer)src).TechnicalDirection : null))
            .ForMember(dest => dest.CodingSkillsFlag, opt => opt.MapFrom(src => src is Developer ? ((Developer)src).CodingSkillsFlag : null))
            .ForMember(dest => dest.TestingMethodology, opt => opt.MapFrom(src => src is QA ? ((QA)src).TestingMethodology : null))
            .ForMember(dest => dest.AutomationSkills, opt => opt.MapFrom(src => src is QA ? (bool?)((QA)src).AutomationSkills : null))
            .ForMember(dest => dest.ManagerType, opt => opt.MapFrom(src => src is Manager ? (ManagerType?)((Manager)src).ManagerType : null))
            .ForMember(dest => dest.ManagedDepartment, opt => opt.MapFrom(src => src is Manager ? ((Manager)src).ManagedDepartment : null));

        // Create DTOs -> Entities
        CreateMap<CreateDeveloperDto, Developer>();
        CreateMap<CreateQADto, QA>();
        CreateMap<CreateManagerDto, Manager>();
        CreateMap<UpdateEmployeeDto, Employee>();

        // AttendanceRecord -> AttendanceRecordDto
        CreateMap<AttendanceRecord, AttendanceRecordDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? (src.Employee.FirstName + " " + src.Employee.LastName).Trim() : string.Empty))
            .ForMember(dest => dest.EmployeeEmail, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Email : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateAttendanceDto, AttendanceRecord>();
        CreateMap<UpdateAttendanceDto, AttendanceRecord>();
    }
}
