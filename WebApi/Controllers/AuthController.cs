using Application.Features.Company.Commands.RegisterFreelancer;
using Application.Features.Company.Commands.RegisterSupplier;
using Application.Features.Company.Commands.RegisterTransporter;
using MediatR;
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

        // ... Diğerleri (Corporate, Individual) ...
    }
}
