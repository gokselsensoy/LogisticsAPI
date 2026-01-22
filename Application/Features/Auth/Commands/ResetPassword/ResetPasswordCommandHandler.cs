using Application.Abstractions.Services;
using Application.Shared.ResultModels;
using MediatR;
using System.Text.RegularExpressions;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            return Result.Failure("Şifreler birbiriyle uyuşmuyor.");
        }

        // 2. Parola Güçlülük Kontrolü (Traffic Saving)
        // IdentityApi'ye hiç gitmeden burada önünü kesiyoruz.
        var passwordCheck = ValidatePassword(request.NewPassword);
        if (!passwordCheck.Succeeded)
        {
            return passwordCheck; // Hata mesajını direkt dönüyoruz
        }

        try
        {
            await _identityService.ResetPasswordAsync(request.Email, request.Code, request.NewPassword, cancellationToken);
            return Result.Success("Şifreniz başarıyla değiştirildi. Giriş yapabilirsiniz.");
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private Result ValidatePassword(string password)
    {
        // Rule 1: Min 6 chars
        if (string.IsNullOrEmpty(password) || password.Length < 6)
            return Result.Failure("Password must be at least 6 characters long.");

        // Rule 2: At least 1 Uppercase
        if (!Regex.IsMatch(password, "[A-Z]"))
            return Result.Failure("Password must contain at least one uppercase letter.");

        // Rule 3: At least 1 Lowercase
        if (!Regex.IsMatch(password, "[a-z]"))
            return Result.Failure("Password must contain at least one lowercase letter.");

        // Rule 4: At least 1 Number
        if (!Regex.IsMatch(password, "[0-9]"))
            return Result.Failure("Password must contain at least one number.");

        // Rule 5: At least 1 Special Char
        if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            return Result.Failure("Password must contain at least one special character (e.g., ! @ # ?).");

        return Result.Success();
    }
}
