namespace Application.Features.Schedules.Commands.GenerateSchedule.DTOs
{
    public class GenerateScheduleRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? SpecificWorkerId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
