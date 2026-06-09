using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/reports")] // Define la primera parte de la URL
    public class ReportsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ReportsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        /// <summary>
        /// Obtiene el historial de envíos (shipments) filtrado por empresa.
        /// </summary>
        /// <remarks>
        /// **Rol requerido:** Admin Master
        /// 
        /// Retorna un reporte aplanado con los envíos. Si la empresa no tiene envíos, retorna una lista vacía (HTTP 200). Si la empresa no existe en el sistema, retorna un HTTP 404.
        /// </remarks>
        /// <param name="companiesId">El ID de la empresa a consultar</param>
        /// <response code="200">Retorna la lista de envíos filtrados</response>
        /// <response code="404">Si el companies_id proporcionado no existe en la base de datos</response>
        [HttpGet("shipments")] // Completa la ruta a /api/reports/shipments
        public async Task<IActionResult> GetShipmentsReport([FromQuery(Name = "companies_id")] int companiesId)
        {
            try
            {
                // Llamamos al método que construimos en el servicio
                var reporte = await _shipmentService.ObtenerReporteAdminPorEmpresaAsync(companiesId);

                // Devuelve HTTP 200 OK con el array de resultados
                return Ok(reporte);
            }
            catch (KeyNotFoundException ex)
            {
                // Si el servicio detectó que la empresa no existe, atrapa la excepción y retorna 404
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Cualquier otro error de ejecución
                return StatusCode(500, new
                {
                    error = "Ocurrió un error interno al generar el reporte de envíos.",
                    detalle = ex.Message
                });
            }
        }
    }
}
