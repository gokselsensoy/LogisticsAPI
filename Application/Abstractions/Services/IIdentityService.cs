namespace Application.Abstractions.Services
{
    public interface IIdentityService
    {
        Task<Guid?> CreateWorkerUserAsync(string email, string password, string role, CancellationToken cancellationToken);
    }
}
