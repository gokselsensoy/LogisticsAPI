using Application.Shared.ResultModels;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Result>
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}