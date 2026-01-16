using Application.Features.CustomerAddresses.Commands.AddCustomerAddress;
using Application.Features.CustomerAddresses.Commands.AssignAddressToResponsible;
using Application.Features.CustomerAddresses.Commands.DeleteCustomerAddress;
using Application.Features.CustomerAddresses.Commands.UpdateCustomerAddress;
using Application.Features.CustomerAddresses.DTOs;
using Application.Features.CustomerAddresses.Queries.GetMyAddresses;
using Application.Features.Responsibles.Commands.CreateResponsible;
using Application.Features.Responsibles.Commands.UpdateResponsible;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CustomerController : ApiControllerBase
    {
        // --- ADRES METODLARI ---

        [HttpPost("address")]
        public async Task<IActionResult> AddAddress(AddCustomerAddressCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("address")]
        public async Task<IActionResult> UpdateAddress(UpdateCustomerAddressCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("address/{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            await Mediator.Send(new DeleteCustomerAddressCommand { AddressId = id });
            return NoContent();
        }

        // --- KURUMSAL YÖNETİM (Sadece Corporate Admin) ---

        //[Authorize(Roles = "Admin")] // CorporateRole Enum'ına göre Policy yazılabilir
        [HttpPost("responsible")]
        public async Task<IActionResult> CreateResponsible(CreateResponsibleCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("responsible")]
        public async Task<IActionResult> UpdateResponsible(UpdateResponsibleCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        // --- ATAMA ---
        //[Authorize(Roles = "Admin")]
        [HttpPost("responsible/assign-address")]
        public async Task<IActionResult> AssignAddress(AssignAddressToResponsibleCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Kullanıcının sipariş verebileceği adresleri listeler.
        /// Bireysel kullanıcı için kendi adresleri, Kurumsal Sorumlu için atandığı adresler döner.
        /// </summary>
        [HttpGet("addresses")]
        [ProducesResponseType(typeof(List<AddressSelectionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAddresses()
        {
            // Query'nin içinde property olmadığı için parametre almaz, 
            // kullanıcı bilgisini Token'dan handler içinde çözer.
            var result = await Mediator.Send(new GetMyAddressesQuery());
            return Ok(result);
        }
    }
}
