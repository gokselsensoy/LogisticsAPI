using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RegisterCorporateCustomer;
using Application.Features.Auth.Commands.RegisterFreelancer;
using Application.Features.Auth.Commands.RegisterIndividualCustomer;
using Application.Features.Auth.Commands.RegisterSupplier;
using Application.Features.Auth.Commands.RegisterTransporter;
using Application.Features.Auth.Commands.SelectProfile;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ApiControllerBase
    {
        /// <summary>
        /// Giriş yapmış olan kullanıcının (Token sahibinin) profil detaylarını getirir.
        /// </summary>
        /// <remarks>
        /// Bu endpoint çalışmak için geçerli bir <b>Bearer Token</b> gerektirir.
        /// Token içindeki ProfileId ve ProfileType'a göre ilgili veriyi döner.
        /// </remarks>
        [HttpGet("me")]
        [Authorize] // <--- KRİTİK: Token olmadan buraya girilemez
        [ProducesResponseType(typeof(MyProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyProfile(CancellationToken token)
        {
            var query = new GetMyProfileQuery();

            var response = await Mediator.Send(query, token);

            return Ok(response);
        }

        /// <summary>
        /// Giriş yapıldığında eğer emaile kayıtlı tek bir profil varsa token değiştirilmeden devam edilir.
        /// Birden fazla profil varsa "select-profile" ile profil seçilerek yeni token alınır.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("select-profile")]
        [Authorize]
        public async Task<IActionResult> SelectProfile([FromBody] SelectProfileCommand command)
        {
            // SelectProfileCommand handler'ı yeni bir LoginResponse (Yeni Token) döner.
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("register-transporter")]
        public async Task<IActionResult> RegisterTransporter([FromBody] RegisterTransporterCommand command)
        {
            var res = await Mediator.Send(command);
            return Ok(res);
        }

        [HttpPost("register-supplier")]
        public async Task<IActionResult> RegisterSupplier([FromBody] RegisterSupplierCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(new { CompanyId = id });
        }

        [HttpPost("register-freelancer")]
        public async Task<IActionResult> RegisterFreelancer([FromBody] RegisterFreelancerCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(new { FreelancerId = id });
        }

        [HttpPost("register-corporate")]
        public async Task<IActionResult> RegisterCorporateCustomer([FromBody] RegisterCorporateCustomerCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(new { CustomerId = id });
        }

        [HttpPost("register-individual")]
        public async Task<IActionResult> RegisterIndividualCustomer([FromBody] RegisterIndividualCustomerCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(new { CustomerId = id });
        }
    }
}
