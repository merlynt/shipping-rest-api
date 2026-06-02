using System.Security.Claims;
using Application.DTOS;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly IUserContext _userContext;

        public ShipmentsController(IShipmentService shipmentService, IUserContext userContext)
        {
            _shipmentService = shipmentService;
            _userContext = userContext;
        }

        [HttpPost]
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Create(CreateEnvioDto dto)
        {
            var empresaId = _userContext.GetUserId();
            if (empresaId == 0) return Unauthorized("No se pudo identificar la empresa.");

            var nuevoEnvioDto = await _shipmentService.CrearEnvioAsync(dto, empresaId);

            if (nuevoEnvioDto == null)
                return NotFound("El destinatario no existe o los datos son inválidos.");

            return CreatedAtAction(nameof(GetByTracking), new { codigoTracking = nuevoEnvioDto.CodigoTracking }, nuevoEnvioDto);
        }

        [HttpGet]
        [Authorize(Roles = "Empresa")]
        public async Task<ActionResult<List<EnvioResponseDto>>> GetAll()
        {
            var empresaId = _userContext.GetUserId();
            var envios = await _shipmentService.ObtenerTodosPorEmpresaAsync(empresaId);

            return Ok(envios);
        }

        [HttpGet("{codigoTracking}")]
        [Authorize(Roles = "Empresa")]
        public async Task<ActionResult<EnvioResponseDto>> GetByTracking(string codigoTracking)
        {
            var empresaId = _userContext.GetUserId();
            var envio = await _shipmentService.ObtenerPorTrackingAsync(codigoTracking, empresaId);

            if (envio == null)
            {
                return NotFound("No se encontró el envío o no le pertenece a su empresa.");
            }

            return Ok(envio);
        }

        /// <summary>
        /// Obtiene la lista de envíos asignados al piloto (conductor) autenticado.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Piloto.
        /// Este método extrae el ID del usuario desde los claims del token JWT para buscar su perfil de conductor y sus envíos en ruta.
        /// </remarks>

        [HttpGet("my-shipments")]

        [Authorize(Roles = "Piloto")]
        public async Task<IActionResult> GetMyShipments()
        {
  
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
                return Unauthorized(new { message = "Invalid or missing token claim." });

            try
            {
                var shipments = await _shipmentService.GetMyShipmentsAsync(usuarioId);
                return Ok(shipments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Para rol piloto y marca un envío como entregado usando su Código
        /// </summary>
        [HttpPatch("{codigoTracking}/deliver")]
        [Authorize(Roles = "Piloto")]
        public async Task<IActionResult> Deliver(string codigoTracking, [FromBody] DeliverShipmentDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var driverId))
                return Unauthorized(new { message = "Token inválido o ausente." });

            try
            {
                await _shipmentService.EntregarEnvioAsync(codigoTracking, driverId, dto);

                return Ok(new { message = $"Envío {codigoTracking} marcado como entregado con éxito." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}