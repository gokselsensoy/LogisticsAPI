using Application.Features.Departments.Commands.CreateDepartment;
using Application.Features.Departments.Commands.DeleteDepartment;
using Application.Features.Departments.Commands.UpdateDepartment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/departments")]
    [ApiController]
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class DepartmentController : ControllerBase
    {
        private readonly ISender _sender;

        public DepartmentController(ISender sender)
        {
            _sender = sender;
        }

        // POST api/departments
        [HttpPost]
        // Policy: İstersen buraya [Authorize(Policy = "AdminOrOwner")] gibi bir kısıt koyabilirsin.
        // Ama Handler içinde zaten rol kontrolü yaptığımız için burada zorunlu değil (Çift dikiş güvenlik).
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
        {
            var departmentId = await _sender.Send(command);
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

            await _sender.Send(command);
            return Ok(new { Message = "Departman başarıyla güncellendi." });
        }

        // DELETE api/departments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sender.Send(new DeleteDepartmentCommand { DepartmentId = id });
            return Ok(new { Message = "Departman silindi (Soft Delete)." });
        }
    }
}
