using Application.Abstractions.Messaging;
using Domain.Enums;
using MediatR;

namespace Application.Features.Responsibles.Commands.UpdateResponsible
{
    public class UpdateResponsibleCommand : ICommand<Unit>
    {
        public Guid ResponsibleId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public List<CorporateRole> Roles { get; set; }
    }
}
