namespace Application.Abstractions.Services
{
    public interface IIdentityService
    {
        Task<Guid?> CreateWorkerUserAsync(string email, string password, CancellationToken cancellationToken);
    }
}
