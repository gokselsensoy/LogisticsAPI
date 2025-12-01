namespace Application.Abstractions.Services
{
    public interface IIdentityService
    {
        Task<Guid?> CreateUserAsync(string email, string password, string role, CancellationToken cancellationToken);
    }
}
