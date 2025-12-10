using Application.Abstractions.Messaging;
using Application.Features.Terminals.Commands.UpdateTerminal;

namespace Application.Features.Terminals.Commands.DeleteTerminal
{
    public class DeleteTerminalCommand : ICommand
    {
        public Guid TerminalId { get; set; }
    }
}
