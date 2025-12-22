using Application.Features.Schedules.Commands.GenerateSchedule.DTOs;

namespace Application.Features.Schedules.Commands.GenerateSchedule.Services
{
    public interface IScheduleGeneratorService
    {
        Task GenerateSchedulesAsync(GenerateScheduleRequest request, CancellationToken token);
    }
}
