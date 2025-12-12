using Application.Abstractions.Messaging;
using Domain.Enums;

namespace Application.Features.Workers.Commands.CreateWorker
{
    public class CreateWorkerCommand : ICommand<Guid>
    {
        // Kişisel Bilgiler
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; } // Admin belirleyebilir veya Random üretilebilir

        // Şirket İçi Bilgiler
        public Guid DepartmentId { get; set; }
        public List<WorkerRole> Roles { get; set; }
    }
}
