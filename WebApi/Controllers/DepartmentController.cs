using Application.Features.Departments.Commands.CreateDepartment;
using Application.Features.Departments.Commands.DeleteDepartment;
using Application.Features.Departments.Commands.UpdateDepartment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/departments")]
    public class DepartmentController : ApiControllerBase
    {

        // POST api/departments
        [HttpPost]
        // Policy: İstersen buraya [Authorize(Policy = "AdminOrOwner")] gibi bir kısıt koyabilirsin.
        // Ama Handler içinde zaten rol kontrolü yaptığımız için burada zorunlu değil (Çift dikiş güvenlik).
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
        {
            var departmentId = await Mediator.Send(command);
            return Ok(new { Id = departmentId });
        }

        // PUT api/departments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentCommand command)
        {
            // URL'deki ID ile Body'deki ID'nin eşleştiğinden emin olalım (Güvenlik)
            if (id != command.DepartmentId)
            {
                return BadRequest("URL'deki ID ile Body'deki ID uyuşmuyor.");
            }

            await Mediator.Send(command);
            return Ok(new { Message = "Departman başarıyla güncellendi." });
        }

        // DELETE api/departments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteDepartmentCommand { DepartmentId = id });
            return Ok(new { Message = "Departman silindi (Soft Delete)." });
        }
    }
}
