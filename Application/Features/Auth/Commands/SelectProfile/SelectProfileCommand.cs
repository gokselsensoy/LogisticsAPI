using Application.Abstractions.Messaging;
using Application.Features.Auth.DTOs;
using Application.Shared.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.SelectProfile
{
    public class SelectProfileCommand : ICommand<LoginResponse>
    {
        public Guid ProfileId { get; set; }
        public string ProfileType { get; set; }
        public string ClientId { get; set; }
    }
}
