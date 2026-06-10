using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        /// <summary>
        /// Crea una nueva orden de envío y genera su código de tracking automáticamente.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Empresa.
        /// El envío queda asociado al usuario Empresa que hace la petición e inicia en estado "Recolectado".
        /// </remarks>
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



        /// <summary>
        /// Para Empresa. Obtiene el historial completo de todos los envíos realizados.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Empresa.
        /// El sistema filtra automáticamente los resultados para mostrar únicamente los envíos que le pertenecen a la empresa autenticada.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Empresa")]
        public async Task<ActionResult<List<EnvioResponseDto>>> GetAll()
        {
            var empresaId = _userContext.GetUserId();
            var envios = await _shipmentService.ObtenerTodosPorEmpresaAsync(empresaId);

            return Ok(envios);
        }

        /// <summary>
        /// Para Admin. Obtiene la lista de TODOS los envíos del sistema.
        /// - Super Admin (EsMaster = true): Ve TODOS los envíos
        /// - Admin Departamental (EsMaster = false): Ve solo envíos de su distrito
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Administrador.
        /// El filtrado se realiza automáticamente según el tipo de administrador.
        /// </remarks>
        [HttpGet("admin/all")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObtenerTodosAdmin()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
                    return Unauthorized(new { error = "Token inválido o ausente." });

                // ← PASAR el usuarioId aquí
                var envios = await _shipmentService.ObtenerTodosAdminAsync(usuarioId);
                
                return Ok(new
                {
                    total = envios.Count,
                    datos = envios
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Ocurrió un error interno al obtener la lista de envíos."
                });
            }
        }

        /// <summary>
        /// Para Empresa. Rastrea el estado y detalle de un envío específico mediante su código de seguimiento.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Empresa.
        /// Si el paquete ya fue entregado, este endpoint también retornará las rutas de las evidencias (foto y firma).
        /// </remarks>
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
        /// Para Piloto. Obtiene la lista de envíos asignados al piloto (conductor) autenticado.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Piloto.
        /// Este método extrae el ID del usuario desde los claims del token JWT para buscar su perfil de conductor y sus envíos en ruta.
        /// </remarks>

        [HttpGet("assigned-shipments")]
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
        /// Para Piloto. Marca un envío como entregado usando su Código
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Piloto.
        /// </remarks>
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

        [HttpPatch("{codigoTracking}/return")]
        [Authorize(Roles = "Piloto")]

        public async Task<IActionResult> ReturnShipment(string codigoTracking, [FromBody] ReturnShipmentDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
                return Unauthorized(new { message = "Token inválido o ausente." });

            try
            {
                await _shipmentService.DevolverEnvioAsync(codigoTracking, usuarioId, dto);
                return Ok(new { message = "El envío ha sido marcado como Devolución exitosamente." });
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
                return UnprocessableEntity(new { message = ex.Message });
            }

        }

        /// <summary>
        /// Para Piloto. Obtiene el detalle completo de un envío asignado por su ID.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Piloto.
        /// </remarks>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Piloto")] // Asegúrate de usar el nombre exacto de tu rol
        public async Task<IActionResult> GetShipmentDetail(int id)
        {
            var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
                return Unauthorized(new { message = "Token inválido o ausente." });

            try
            {
                var result = await _shipmentService.ObtenerDetalleEnvioParaDriverAsync(id, usuarioId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Para Admin. Edita los datos de un shipment por ID (dirección, peso, etc.)
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Administrador.
        /// </remarks>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShipmentDto dto)
        {
            try
            {
                var resultado = await _shipmentService.ActualizarShipmentAsync(id, dto);
                if (resultado == null)
                    return NotFound(new { message = "Envío o destinatario no encontrado." });

                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Para Admin. Cambia el estado de un shipment (Recolectado, En bodega, En ruta, Entregado, Devolución)
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Administrador.
        /// </remarks>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateShipmentStatusDto dto)
        {
            try
            {
                var resultado = await _shipmentService.CambiarEstadoAsync(id, dto);
                if (resultado == null)
                    return NotFound(new { message = "Envío no encontrado." });

                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Para Admin. Marca un shipment como "En Bodega" (solo Administrador departamental)
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Administrador.
        /// </remarks>
        [HttpPatch("{id}/warehouse")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> MarkAsWarehouse(int id)
        {
            try
            {
                var resultado = await _shipmentService.MarcarEnBodegaAsync(id);
                if (resultado == null)
                    return NotFound(new { message = "Envío no encontrado." });

                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

      
    }
}
