using Application.Shared.ResultModels;
using MediatR;

namespace Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<Result>
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }
}