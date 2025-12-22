using Application.Features.Schedules.Commands.GenerateSchedule.DTOs;
using Application.Features.Schedules.Commands.GenerateSchedule.Services;
using Hangfire;

namespace WebApi.Extensions
{
    public static class HangfireJobsExtension
    {
        public static void RegisterRecurringJobs(this IApplicationBuilder app)
        {
            var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<IScheduleGeneratorService>(
                "daily-schedule-generator",
                service => service.GenerateSchedulesAsync(
                    new GenerateScheduleRequest
                    {
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(60)
                    },
                    CancellationToken.None),
                Cron.Daily(3)); // Her gece 03:00

            // Başka jobların olursa yine recurringJobManager değişkenini kullanarak ekle
        }
    }
}
