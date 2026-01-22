using Application.Abstractions.Services;
using Application.Shared.ResultModels;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;
public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ForgotPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _identityService.ForgotPasswordAsync(request.Email, cancellationToken);
            return Result.Success("Eğer kayıtlıysa, mail adresinize kod gönderildi.");
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

