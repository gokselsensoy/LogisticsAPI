using Application.Abstractions.Messaging;
using Application.Features.Workers.Commands.CreateWorker;
using Domain.Enums;

namespace Application.Features.Workers.Commands.UpdateWorker
{
    public class UpdateWorkerCommand : ICommand
    {
        public Guid WorkerId { get; set; }

        public string FullName { get; set; }
        public string Phone { get; set; }

        public Guid DepartmentId { get; set; }
        public List<WorkerRole> Roles { get; set; }
    }
}
