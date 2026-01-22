using Application.Shared.ResultModels;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Result>
{
    public string Email { get; set; } = null!;
}
