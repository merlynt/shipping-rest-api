using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdminDto dto)
        {
            try
            {
                var admin = await _adminService.CrearAdministradorAsync(dto);

                return CreatedAtAction(
                    nameof(Create),
                    new { id = admin.Id },
                    admin);
            }
            catch (InvalidOperationException ex)
                when (ex.Message == "EMAIL_EXISTE")
            {
                return Conflict("El email ya existe.");
            }
            catch (ArgumentException ex)
                when (ex.Message == "DISTRITO_NO_EXISTE")
            {
                return BadRequest("El distrito no existe.");
            }
        }
    }
}