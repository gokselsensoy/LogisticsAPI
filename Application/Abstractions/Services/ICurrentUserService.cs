namespace Application.Abstractions.Services
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        Guid AppUserId { get; }
        string? ClientId { get; }
        Guid? ProfileId { get; }
        Guid? CompanyId { get; }
        string? ProfileType { get; }
        List<string> Roles { get; }
    }
}
