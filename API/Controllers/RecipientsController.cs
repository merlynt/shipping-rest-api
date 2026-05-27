using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Application.DTOS;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Empresa")]
    public class RecipientsController : ControllerBase
    {
        private readonly IRecipientRepository _recipientRepo; // <-- Inyectamos el repositorio

        public RecipientsController(IRecipientRepository recipientRepo)
        {
            _recipientRepo = recipientRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDestinatarioDto dto)
        {
            // 1. Delegamos la validación a la infraestructura mediante la interfaz
            var distritoExiste = await _recipientRepo.ExisteDistrito(dto.DistritoId);
            if (!distritoExiste) return BadRequest("El distrito no existe");

            var destinatario = new Destinatario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Email = dto.Email,
                DistritoId = dto.DistritoId
            };

            // 2. Delegamos el guardado
            await _recipientRepo.Crear(destinatario);

            // Retornamos 201 Created
            return CreatedAtAction(nameof(Create), new { id = destinatario.Id }, destinatario);
        }
    }
}