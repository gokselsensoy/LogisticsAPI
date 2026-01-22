using Application.Abstractions.Services;
using Application.Shared.ResultModels;
using MediatR;
using System.Text.RegularExpressions;

namespace Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService; // Token'dan ID okumak için

    public ChangePasswordCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. Kullanıcı ID'sini Token'dan al (Güvenlik)
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
            return Result.Failure("Oturum bilgisi alınamadı.");

        // 2. Parolalar eşleşiyor mu?
        if (request.NewPassword != request.ConfirmNewPassword)
            return Result.Failure("Yeni parolalar birbiriyle uyuşmuyor.");

        // 3. Yeni parola kurallara uyuyor mu? (Regex logic'ini ValidatePassword metoduna taşıdık)
        var validationResult = ValidatePassword(request.NewPassword);
        if (!validationResult.Succeeded)
            return validationResult;

        try
        {
            // 4. Servis çağrısı
            await _identityService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);
            return Result.Success("Parolanız başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    // ResetPassword'deki validasyon mantığının aynısı (DRY prensibi için Shared'a da alınabilir ama şimdilik burada kalsın)
    private Result ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6) return Result.Failure("Password must be at least 6 characters long.");
        if (!Regex.IsMatch(password, "[A-Z]")) return Result.Failure("Password must contain at least one uppercase letter.");
        if (!Regex.IsMatch(password, "[a-z]")) return Result.Failure("Password must contain at least one lowercase letter.");
        if (!Regex.IsMatch(password, "[0-9]")) return Result.Failure("Password must contain at least one number.");
        if (!Regex.IsMatch(password, "[^a-zA-Z0-9]")) return Result.Failure("Password must contain at least one special character.");

        return Result.Success();
    }
}