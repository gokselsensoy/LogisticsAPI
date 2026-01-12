namespace Application.Abstractions.Services
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        Guid AppUserId { get; }
        string? ClientId { get; }
        Guid? ProfileId { get; } // WorkerId, ResponsibleId, FreelancerId, IndividualId
        Guid? CompanyId { get; } // CustomerResponsiblelar için CorporateCustomerId
        string? ProfileType { get; }
        List<string> Roles { get; }
    }
}
