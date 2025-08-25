using Microsoft.Extensions.Options;
using Quartz;

namespace ConsultationService.Infrastructure.BackgroundServices.Quartz.Interfaces;

public interface IQuartzJobSchedule : IConfigureOptions<QuartzOptions>
{
    
}