using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RegisterCorporateCustomer;
using Application.Features.Auth.Commands.RegisterFreelancer;
using Application.Features.Auth.Commands.RegisterIndividualCustomer;
using Application.Features.Auth.Commands.RegisterSupplier;
using Application.Features.Auth.Commands.RegisterTransporter;
using Application.Features.Auth.Commands.SelectProfile;
using Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
        {
            var response = await _sender.Send(command);
            return Ok(response);
        }

        [HttpPost("select-profile")]
        [Authorize]
        public async Task<IActionResult> SelectProfile([FromBody] SelectProfileCommand command)
        {
            // SelectProfileCommand handler'ı yeni bir LoginResponse (Yeni Token) döner.
            var response = await _sender.Send(command);
            return Ok(response);
        }

        // 1. Transporter Kayıt
        [HttpPost("register-transporter")]
        public async Task<IActionResult> RegisterTransporter([FromBody] RegisterTransporterCommand command)
        {
            var id = await _sender.Send(command);
            return Ok(new { CompanyId = id });
        }

        // 2. Supplier Kayıt
        [HttpPost("register-supplier")]
        public async Task<IActionResult> RegisterSupplier([FromBody] RegisterSupplierCommand command)
        {
            var id = await _sender.Send(command);
            return Ok(new { CompanyId = id });
        }

        // 3. Freelancer Kayıt
        [HttpPost("register-freelancer")]
        public async Task<IActionResult> RegisterFreelancer([FromBody] RegisterFreelancerCommand command)
        {
            var id = await _sender.Send(command);
            return Ok(new { FreelancerId = id });
        }

        [HttpPost("register-corporate")]
        public async Task<IActionResult> RegisterCorporateCustomer([FromBody] RegisterCorporateCustomerCommand command)
        {
            var id = await _sender.Send(command);
            return Ok(new { CustomerId = id });
        }

        [HttpPost("register-individual")]
        public async Task<IActionResult> RegisterIndividualCustomer([FromBody] RegisterIndividualCustomerCommand command)
        {
            var id = await _sender.Send(command);
            return Ok(new { CustomerId = id });
        }
    }
}
