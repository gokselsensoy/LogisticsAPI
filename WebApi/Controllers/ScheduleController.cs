using Application.Features.Schedules.Commands.AddPatternItem;
using Application.Features.Schedules.Commands.AddScheduleAllocation;
using Application.Features.Schedules.Commands.CreateDailySchedule;
using Application.Features.Schedules.Commands.CreateWeeklyPattern;
using Application.Features.Schedules.Commands.DeleteDailySchedule;
using Application.Features.Schedules.Commands.DeleteWeeklyPattern;
using Application.Features.Schedules.Commands.RemoveScheduleAllocation;
using Application.Features.Schedules.Commands.UpdateDailyShiftTimes;
using Application.Features.Schedules.Commands.UpdateWeeklyPattern;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ISender _sender;

        public ScheduleController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("weekly")]
        public async Task<IActionResult> CreateWeeklyPattern(CreateWeeklyPatternCommand command)
        {
            return Ok(await _sender.Send(command));
        }

        [HttpPut("weekly")]
        public async Task<IActionResult> UpdateWeeklyPattern(UpdateWeeklyPatternCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }

        [HttpDelete("weekly/{id}")]
        public async Task<IActionResult> DeleteWeeklyPattern(Guid id)
        {
            await _sender.Send(new DeleteWeeklyPatternCommand { Id = id });
            return NoContent();
        }

        [HttpPost("weekly/item")]
        public async Task<IActionResult> AddPatternItem(AddPatternItemCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }

        // ==========================================
        // DAILY SCHEDULE (GÜNLÜK VARDİYA) İŞLEMLERİ
        // ==========================================

        [HttpPost("daily")]
        public async Task<IActionResult> CreateDailySchedule(CreateDailyScheduleCommand command)
        {
            return Ok(await _sender.Send(command));
        }

        [HttpPut("daily/times")]
        public async Task<IActionResult> UpdateDailyTimes(UpdateDailyShiftTimesCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }

        [HttpDelete("daily/{id}")]
        public async Task<IActionResult> DeleteDailySchedule(Guid id)
        {
            // DeleteDailyScheduleCommand oluşturup çağırdığını varsayıyorum
            await _sender.Send(new DeleteDailyScheduleCommand { Id = id });
            return NoContent();
        }

        // ==========================================
        // ALLOCATION (GÖREV ATAMA) İŞLEMLERİ
        // ==========================================

        [HttpPost("daily/allocation")]
        public async Task<IActionResult> AddAllocation(AddScheduleAllocationCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }

        [HttpDelete("daily/allocation")]
        public async Task<IActionResult> RemoveAllocation(RemoveScheduleAllocationCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }
    }
}
