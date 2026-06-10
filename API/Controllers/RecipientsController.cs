using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Empresa")]
    public class RecipientsController : ControllerBase
    {
        private readonly IRecipientService _recipientService; // <-- Inyectamos el SERVICIO, no el repositorio

        public RecipientsController(IRecipientService recipientService)
        {
            _recipientService = recipientService;
        }
        /// <summary>
        /// Crea un nuevo destinatario en el sistema.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Empresa.
        /// Registra la información de la persona que recibirá el paquete. Valida que el distrito ingresado exista en el sistema.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create(CreateDestinatarioDto dto)
        {
            try
            {
                // El controlador solo delega el trabajo pesado a la capa de Aplicación
                var destinatario = await _recipientService.CrearDestinatarioAsync(dto);

                return CreatedAtAction(nameof(Create), new { id = destinatario.Id }, destinatario);
            }
            catch (ArgumentException ex)
            {
                // Si la regla de negocio falla (ej. Distrito no existe), capturamos el error
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Edita los datos de un destinatario registrado.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Empresa.
        /// Permite corregir información de contacto o dirección antes de asociarlo a un envío.
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDestinatarioDto dto)
        {
            try
            {
                await _recipientService.ActualizarDestinatarioAsync(id, dto);

                // Cumple con: "Entonces el sistema actualiza recipients y retorna HTTP 200"
                return Ok(new { message = "Destinatario actualizado correctamente." });
            }
            catch (KeyNotFoundException)
            {
                // Cumple con: "Si el recipients.id no existe -> HTTP 404"
                return NotFound(new { message = "El destinatario especificado no existe." });
            }
            catch (ArgumentException ex)
            {
                // Cumple con: "Si el nuevo districts_id no existe -> HTTP 400"
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}