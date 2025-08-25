using UserService.Application.Workers;

namespace UserService.Application.DependencyInjection.Extensions;

public static class QuartzExtensions
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            // Đăng ký job
            var jobKey = new JobKey("GenerateCarePlanJob");
            q.AddJob<GenerateCareplanJob>(opts => opts.WithIdentity(jobKey));

            // Trigger chạy mỗi ngày lúc 12h đêm (giờ VN)
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("GenerateCarePlanJob-trigger")
                .WithSchedule(CronScheduleBuilder
                    .DailyAtHourAndMinute(0, 0) // 0 giờ 0 phút = 12h đêm
                    .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                )
            );
        });

        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });

        return services;
    }
}
