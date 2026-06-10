using Application.DTOS;
using Application.Interfaces; // <-- CORREGIDO: Ahora apunta a la nueva ubicación
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
                return CreatedAtAction(nameof(Create), new { id = admin.Id }, admin);
            }
            catch (InvalidOperationException ex) when (ex.Message == "EMAIL_EXISTE")
            {
                return Conflict("El email ya existe.");
            }
            catch (ArgumentException ex) when (ex.Message == "DISTRITO_NO_EXISTE")
            {
                return BadRequest("El distrito no existe.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminDto dto)
        {
            try
            {
                var fueActualizado = await _adminService.ActualizarAdministradorAsync(id, dto);

                if (!fueActualizado)
                {
                    return NotFound(new { message = $"No se encontró el administrador con ID {id}" });
                }

                return Ok(new { message = "Administrador actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al actualizar el administrador.", details = ex.Message });
            }
        }

        [Authorize(Policy = "SoloAdminMaster")]
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var admins = await _adminService.ObtenerTodosAsync();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno.", detalle = ex.Message });
            }
        }

        [Authorize(Policy = "SoloAdminMaster")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAdmin(int id)
        {
            try
            {
                await _adminService.EliminarAdministradorAsync(id);
                return Ok(new { mensaje = "Administrador eliminado correctamente del sistema." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al intentar eliminar.", detalle = ex.Message });
            }
        }
    }
}