using Application.Abstractions.Messaging;
using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ClientType { get; set; }
    }
}
