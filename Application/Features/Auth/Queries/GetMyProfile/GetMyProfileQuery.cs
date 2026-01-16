using Application.Features.Auth.DTOs;
using Application.Shared.ResultModels;
using MediatR;

namespace Application.Features.Auth.Queries.GetMyProfile
{
    public class GetMyProfileQuery : IRequest<MyProfileDto>
    {
    }
}
