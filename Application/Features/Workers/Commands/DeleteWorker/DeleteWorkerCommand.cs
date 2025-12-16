using Application.Abstractions.Messaging;
using Domain.Enums;

namespace Application.Features.Workers.Commands.DeleteWorker
{
    public class DeleteWorkerCommand : ICommand
    {
        public Guid WorkerId { get; set; }
    }
}
