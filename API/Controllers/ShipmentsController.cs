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
    public class ShipmentsController : ControllerBase
    {
        // 1. Inyectamos la Interfaz del Repositorio
        private readonly IShipmentRepository _shipmentRepo;
        private readonly ITrackingService _trackingService;
        private readonly IUserContext _userContext;

        public ShipmentsController(IShipmentRepository shipmentRepo, ITrackingService trackingService, IUserContext userContext)
        {
            _shipmentRepo = shipmentRepo;
            _trackingService = trackingService;
            _userContext = userContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEnvioDto dto)
        {
            var empresaId = _userContext.GetUserId();
            if (empresaId == 0) return Unauthorized("No se pudo identificar la empresa.");

            // Validación: ¿Existe el destinatario? (Error 404)
            var destinatarioExiste = await _shipmentRepo.ExisteDestinatario(dto.DestinatarioId);
            if (!destinatarioExiste) return NotFound("El destinatario no existe.");

            var envio = new Envio
            {
                Peso = dto.Peso,
                Descripcion = dto.Descripcion,
                DestinatarioId = dto.DestinatarioId,
                EstadoId = 1,
                EmpresaId = empresaId,
                CodigoTracking = _trackingService.GenerarCodigo()
            };

            await _shipmentRepo.Crear(envio);

            // Retorna 201 Created con la ruta para consultar el nuevo envío
            return CreatedAtAction(nameof(GetByTracking), new { codigoTracking = envio.CodigoTracking }, envio);
        }

        [HttpGet]
        public async Task<ActionResult<List<EnvioResponseDto>>> GetAll()
        {
            var empresaId = _userContext.GetUserId();

            // 3. Usamos el repositorio para consultar
            var envios = await _shipmentRepo.ObtenerTodosPorEmpresa(empresaId);

            return envios.Select(e => new EnvioResponseDto
            {
                Id = e.Id,
                CodigoTracking = e.CodigoTracking,
                Peso = e.Peso,
                Descripcion = e.Descripcion,
                EstadoNombre = e.Estado?.Nombre ?? "Sin Estado", // Manejo nulo preventivo
                DestinatarioNombre = $"{e.Destinatario?.Nombre} {e.Destinatario?.Apellido}",
                DestinatarioTelefono = e.Destinatario?.Telefono ?? "",
                DestinatarioDireccion = e.Destinatario?.Direccion ?? ""
            }).ToList();
        }

        [HttpGet("{codigoTracking}")]
        public async Task<ActionResult<EnvioResponseDto>> GetByTracking(string codigoTracking)
        {
            var empresaId = _userContext.GetUserId();

            var envio = await _shipmentRepo.ObtenerPorTracking(codigoTracking, empresaId);

            if (envio == null)
            {
                return NotFound("No se encontró el envío o no le pertenece a su empresa.");
            }

        
            return Ok(new EnvioResponseDto
            {
                Id = envio.Id,
                CodigoTracking = envio.CodigoTracking,
                Peso = envio.Peso,
                Descripcion = envio.Descripcion,
                EstadoNombre = envio.Estado?.Nombre ?? "Sin Estado",
                DestinatarioNombre = $"{envio.Destinatario?.Nombre} {envio.Destinatario?.Apellido}",
                DestinatarioTelefono = envio.Destinatario?.Telefono ?? "",
                DestinatarioDireccion = envio.Destinatario?.Direccion ?? "",
                Evidencias = envio.Evidencias?.Select(e => new EvidenciaDto
                {
                    FirmaUrl = e.FirmaUrl,
                    FotoUrl = e.FotoUrl
                }).ToList() ?? new List<EvidenciaDto>()
            });
        }
    }
}