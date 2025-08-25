using ConsultationService.Infrastructure.BackgroundServices.Quartz.Interfaces;
using Quartz;

namespace ConsultationService.Infrastructure.BackgroundServices.Quartz.Schedules;

public class ConsultationTemplateStatusJobSchedule : IQuartzJobSchedule
{
    public void Configure(QuartzOptions options)
    {
        // throw new NotImplementedException();
    }
}