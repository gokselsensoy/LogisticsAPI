using Application.Shared.ResultModels;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyCode;
public class VerifyCodeCommand : IRequest<Result>
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
