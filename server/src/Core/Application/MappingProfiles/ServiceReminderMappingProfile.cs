using Application.Features.ServiceReminders.Query;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class ServiceReminderMappingProfile : Profile
{
    public ServiceReminderMappingProfile()
    {
        // ServiceReminder to ServiceReminderDTO mapping
        CreateMap<ServiceReminder, ServiceReminderDTO>(MemberList.Destination)
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Name : "Unknown"))
            .ForMember(dest => dest.CurrentMileage, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Mileage : 0))
            .ForMember(dest => dest.MileageVariance, opt => opt.MapFrom(src => src.MeterVariance))
            .ForMember(dest => dest.DaysUntilDue, opt => opt.MapFrom(src => src.DueDate.HasValue ? (int?)(src.DueDate.Value - DateTime.UtcNow).TotalDays : null))
            .ForMember(dest => dest.ServiceTasks, opt => opt.MapFrom(src => MapServiceTasks(src.ServiceSchedule)))
            .ForMember(dest => dest.TotalEstimatedLabourHours, opt => opt.MapFrom(src => CalculateTotalLabourHours(src.ServiceSchedule)))
            .ForMember(dest => dest.TotalEstimatedCost, opt => opt.MapFrom(src => CalculateTotalCost(src.ServiceSchedule)))
            .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => CalculateTaskCount(src.ServiceSchedule)))
            .ForMember(dest => dest.OccurrenceNumber, opt => opt.Ignore()) // Will be calculated in the handler
            .ForMember(dest => dest.ScheduleType, opt => opt.MapFrom(src => src.GetScheduleType()));

        // ServiceTask to ServiceTaskInfoDTO mapping
        CreateMap<ServiceTask, ServiceTaskInfoDTO>()
            .ForMember(dest => dest.ServiceTaskID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.ServiceTaskName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ServiceTaskCategory, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.EstimatedLabourHours, opt => opt.MapFrom(src => src.EstimatedLabourHours))
            .ForMember(dest => dest.EstimatedCost, opt => opt.MapFrom(src => src.EstimatedCost))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => true));
    }

    private static List<ServiceTaskInfoDTO> MapServiceTasks(ServiceSchedule? serviceSchedule)
    {
        if (serviceSchedule?.XrefServiceScheduleServiceTasks == null)
            return [];

        return serviceSchedule.XrefServiceScheduleServiceTasks
            .Select(xssst => new ServiceTaskInfoDTO
            {
                ServiceTaskID = xssst.ServiceTask.ID,
                ServiceTaskName = xssst.ServiceTask.Name,
                ServiceTaskCategory = xssst.ServiceTask.Category,
                EstimatedLabourHours = xssst.ServiceTask.EstimatedLabourHours,
                EstimatedCost = xssst.ServiceTask.EstimatedCost,
                Description = xssst.ServiceTask.Description,
                IsRequired = true
            }).ToList();
    }

    private static double CalculateTotalLabourHours(ServiceSchedule? serviceSchedule)
    {
        if (serviceSchedule?.XrefServiceScheduleServiceTasks == null)
            return 0;

        return serviceSchedule.XrefServiceScheduleServiceTasks.Sum(xssst => xssst.ServiceTask.EstimatedLabourHours);
    }

    private static decimal CalculateTotalCost(ServiceSchedule? serviceSchedule)
    {
        if (serviceSchedule?.XrefServiceScheduleServiceTasks == null)
            return 0;

        return serviceSchedule.XrefServiceScheduleServiceTasks.Sum(xssst => xssst.ServiceTask.EstimatedCost);
    }

    private static int CalculateTaskCount(ServiceSchedule? serviceSchedule)
    {
        return serviceSchedule?.XrefServiceScheduleServiceTasks?.Count ?? 0;
    }
}