using Application.Abstractions.Services;
using Application.Shared.ResultModels;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyCode;
public class VerifyCodeCommandHandler : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly IIdentityService _identityService;

    public VerifyCodeCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _identityService.VerifyCodeAsync(request.Email, request.Code, cancellationToken);
            return Result.Success("Kod başarıyla doğrulandı.");
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}