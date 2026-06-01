using System.Security.Claims;
using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class DriversController : Controller
    {
        private readonly IDriverService _driverService;

        public DriversController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        /// <summary>
        /// Returns all shipments (Envios) assigned to the currently authenticated driver.
        /// </summary>
        /// <remarks>
        /// The driver identity is resolved from <c>ClaimTypes.NameIdentifier</c> in the
        /// JWT — never from a URL or body parameter — so it is impossible for a Piloto
        /// to query another Piloto's Envios.
        ///
        /// Flow:
        ///   JWT.NameIdentifier (usuarioId)
        ///     → Pilotos WHERE UsuarioId = usuarioId   → Piloto.Id
        ///     → Envios  WHERE PilotoId  = Piloto.Id   → filtered list
        /// </remarks>
        /// <response code="200">List of Envios assigned to this driver.</response>
        /// <response code="401">Token missing or invalid.</response>
        /// <response code="404">No Piloto profile linked to the authenticated user.</response>
        [HttpGet("my-shipments")]
        [ProducesResponseType(typeof(IEnumerable<DriverShipmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyShipments()
        {
            // Read the Usuario.Id embedded in the JWT by TokenService.
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
                return Unauthorized(new { mensaje = "Token inválido o claim ausente." });

            try
            {
                var envios = await _driverService.GetMyShipmentsAsync(usuarioId);
                return Ok(envios);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

    }
}
