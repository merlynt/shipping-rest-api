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
        /// Obtiene el historial de envíos. Puede filtrar por empresa o por estado a nivel global.
        /// </summary>
        /// <remarks>
        /// **Rol requerido:** Admin Master
        /// 
        /// - Si envías `companies_id`: Retorna los envíos específicos de esa empresa.
        /// - Si no envías `companies_id`: Retorna un reporte global.
        /// - Si además envías `status_id`: Filtra el reporte global por ese estado exacto.
        /// </remarks>
        /// <param name="companiesId">El ID de la empresa a consultar (Opcional)</param>
        /// <param name="statusId">El ID del estado para filtrar el reporte global (Opcional)</param>
        /// <response code="200">Retorna la lista de envíos según los filtros aplicados</response>
        /// <response code="404">Si la empresa proporcionada no existe</response>
       
        [Authorize(Policy = "SoloAdminMaster")]
        [HttpGet("shipments")]
        public async Task<IActionResult> GetShipmentsReport([FromQuery(Name = "companies_id")] int? companiesId,[FromQuery(Name = "status_id")] int? statusId)
        {
            try
            {
               
                if (companiesId.HasValue)
                {
                    var reporteEmpresa = await _shipmentService.ObtenerReporteAdminPorEmpresaAsync(companiesId.Value);
                    return Ok(reporteEmpresa);
                }
                var reporteEstados = await _shipmentService.ObtenerReporteAdminPorEstadoAsync(statusId);
                return Ok(reporteEstados);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno.", detalle = ex.Message });
            }
        }
    }
}
