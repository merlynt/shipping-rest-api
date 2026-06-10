using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOS;
using Application.Interfaces;
using Domain.Constanst;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly ITrackingService _trackingService;
        private readonly ICompanyRepository _companyRepository;
        // Inyecta el repositorio de Administrador
        private readonly IAdminRepository _adminRepository;

        public ShipmentService(
            IShipmentRepository shipmentRepository, 
            ITrackingService trackingService, 
            ICompanyRepository companyRepository,
            IAdminRepository adminRepository)  // ← NUEVO
        {
            _shipmentRepository = shipmentRepository;
            _trackingService = trackingService;
            _companyRepository = companyRepository;
            _adminRepository = adminRepository;  // ← NUEVO
        }

        async Task<IEnumerable<DriverShipmentResponseDto>> IShipmentService.GetMyShipmentsAsync(int usuarioId)
        {
            var piloto = await _shipmentRepository.GetByUsuarioIdAsync(usuarioId)
                ?? throw new KeyNotFoundException(
                    $"No se encontró un perfil de piloto para el usuario {usuarioId}.");

            var envios = await _shipmentRepository.GetShipmentsByDriverIdAsync(piloto.Id);

            return envios.Select(e => new DriverShipmentResponseDto
            {
                Id = e.Id,
                CodigoTracking = e.CodigoTracking,
                Estado = e.Estado?.Nombre ?? string.Empty,
                DestinatarioNombre = $"{e.Destinatario?.Nombre} {e.Destinatario?.Apellido}".Trim(),
                Direccion = e.Destinatario?.Direccion ?? string.Empty,
                Telefono = e.Destinatario?.Telefono ?? string.Empty
            });
        }
        public async Task<EnvioResponseDto?> CrearEnvioAsync(CreateEnvioDto dto, int empresaId)
        {
            // 1. Lógica de negocio: Validar destinatario
            var destinatarioExiste = await _shipmentRepository.ExisteDestinatario(dto.DestinatarioId);
            if (!destinatarioExiste)
                return null; // Retornamos null para indicar que falló por destinatario

            // 2. Construcción de la entidad
            var envio = new Envio
            {
                Peso = dto.Peso,
                Descripcion = dto.Descripcion,
                DestinatarioId = dto.DestinatarioId,
                EstadoId = 1, // Estado inicial por defecto
                EmpresaId = empresaId,
                CodigoTracking = _trackingService.GenerarCodigo()
            };

            await _shipmentRepository.Crear(envio);

            // 3. Reutilizamos el método de búsqueda para devolver el DTO completo
            return await ObtenerPorTrackingAsync(envio.CodigoTracking, empresaId);
        }


        private EnvioResponseDto MapearAEnvioResponseDto(Envio e)
        {
            return new EnvioResponseDto
            {
                Id = e.Id,
                CodigoTracking = e.CodigoTracking,
                Peso = e.Peso,
                Descripcion = e.Descripcion,
                EstadoNombre = e.Estado?.Nombre ?? "Sin Estado",
                DestinatarioNombre = $"{e.Destinatario?.Nombre} {e.Destinatario?.Apellido}",
                DestinatarioTelefono = e.Destinatario?.Telefono ?? "",
                DestinatarioDireccion = e.Destinatario?.Direccion ?? "",
                // Auditoría unificada
                FechaAsignacion = e.FechaAsignacion,
                NombrePiloto = e.Piloto?.Nombre ?? "No asignado",
                Evidencias = e.Evidencias?.Select(ev => new EvidenciaDto
                {
                    FirmaUrl = ev.FirmaUrl,
                    FotoUrl = ev.FotoUrl
                }).ToList() ?? new List<EvidenciaDto>()
            };
        }

        public async Task<List<EnvioResponseDto>> ObtenerTodosPorEmpresaAsync(int empresaId)
        {
            var envios = await _shipmentRepository.ObtenerTodosPorEmpresa(empresaId);
            return envios.Select(MapearAEnvioResponseDto).ToList();
        }


        public async Task<EnvioResponseDto?> ObtenerPorTrackingAsync(string codigoTracking, int empresaId)
        {
            var envio = await _shipmentRepository.ObtenerPorTracking(codigoTracking, empresaId);
            // Si es null retorna null, si no, mapea
            return envio == null ? null : MapearAEnvioResponseDto(envio);
        }

        public async Task<bool> EntregarEnvioAsync(string codigoTracking, int usuarioId, DeliverShipmentDto dto)
        {
            var envio = await _shipmentRepository.ObtenerPorTrackingAsync(codigoTracking);

            var piloto = await _shipmentRepository.GetByUsuarioIdAsync(usuarioId);

            if (envio == null)
                throw new KeyNotFoundException($"No se encontró el envío con código: {codigoTracking}");

            if (piloto == null)
                throw new UnauthorizedAccessException("Tu usuario no tiene un perfil de piloto asignado.");

            if (envio.PilotoId != piloto.Id)
                throw new UnauthorizedAccessException("Este envío no te pertenece o no lo tienes asignado.");

            if (envio.EstadoId != EstadosEnvios.EnRuta)
                throw new InvalidOperationException("El envío no está en ruta y no puede ser entregado.");

            envio.EstadoId = EstadosEnvios.Entregado;

            var evidencia = new Evidencia
            {
                EnvioId = envio.Id,
                FirmaUrl = dto.FirmaUrl,
                FotoUrl = dto.FotoUrl
            };

            await _shipmentRepository.GuardarEvidenciaYActualizarEstadoAsync(envio, evidencia);
            return true;
        }

        public async Task<bool> DevolverEnvioAsync(string codigoTracking, int usuarioId, ReturnShipmentDto dto)
        {
            var envio = await _shipmentRepository.ObtenerPorTrackingAsync(codigoTracking);
            var piloto = await _shipmentRepository.GetByUsuarioIdAsync(usuarioId);

            if (envio == null)
                throw new KeyNotFoundException($"No se encontró el envío con código: {codigoTracking}");

            if (piloto == null)
                throw new UnauthorizedAccessException("Tu usuario no tiene un perfil de piloto asignado.");

            if (envio.PilotoId != piloto.Id)
                throw new UnauthorizedAccessException("Este envío no te pertenece.");

            if (envio.EstadoId != Domain.Constanst.EstadosEnvios.EnRuta)
                throw new InvalidOperationException("El envío no está en ruta y no puede ser devuelto.");

            envio.EstadoId = Domain.Constanst.EstadosEnvios.Devolucion;
            envio.MotivoDevolucion = dto.Motivo;

            await _shipmentRepository.ActualizarAsync(envio);

            return true;
        }

        public async Task<EnvioResponseDto?> ActualizarShipmentAsync(int id, UpdateShipmentDto dto)
        {
            var envio = await _shipmentRepository.ObtenerPorId(id);
            if (envio == null) return null;

            if (dto.Peso.HasValue) envio.Peso = dto.Peso.Value;
            if (!string.IsNullOrEmpty(dto.Descripcion)) envio.Descripcion = dto.Descripcion;

            if (dto.DestinatarioId.HasValue)
            {
                var existe = await _shipmentRepository.ExisteDestinatario(dto.DestinatarioId.Value);
                if (!existe) return null;
                envio.DestinatarioId = dto.DestinatarioId.Value;
            }

            await _shipmentRepository.ActualizarAsync(envio);

            // Mapeo completo incluyendo los nuevos campos de auditoría
            return MapearAEnvioResponseDto(envio);
        }

        public async Task<EnvioResponseDto?> CambiarEstadoAsync(int id, UpdateShipmentStatusDto dto)
        {
            var envio = await _shipmentRepository.ObtenerPorId(id);
            if (envio == null) return null;

            var estadosValidos = new[] {
        EstadosEnvios.Recolectado,
        EstadosEnvios.EnBodega,
        EstadosEnvios.EnRuta,
        EstadosEnvios.Entregado,
        EstadosEnvios.Devolucion
    };

            if (!estadosValidos.Contains(dto.EstadoId))
                throw new InvalidOperationException("El estado proporcionado no es válido.");

            envio.EstadoId = dto.EstadoId;
            await _shipmentRepository.ActualizarAsync(envio);

            return MapearAEnvioResponseDto(envio);
        }


        public async Task<EnvioResponseDto?> MarcarEnBodegaAsync(int id)
        {
            var envio = await _shipmentRepository.ObtenerPorId(id);
            if (envio == null) return null;

            if (envio.EstadoId != EstadosEnvios.Recolectado)
                throw new InvalidOperationException("Solo se puede marcar En Bodega un envío que esté Recolectado.");

            envio.EstadoId = EstadosEnvios.EnBodega;
            await _shipmentRepository.ActualizarAsync(envio);

            return MapearAEnvioResponseDto(envio);
        }

        public async Task<DriverShipmentDetail> ObtenerDetalleEnvioParaDriverAsync(int shipmentId, int usuarioId)
        {
            var piloto = await _shipmentRepository.GetByUsuarioIdAsync(usuarioId)
                ?? throw new KeyNotFoundException($"No se encontró un perfil de piloto para el usuario {usuarioId}.");

            var envio = await _shipmentRepository.ObtenerConDetallesAsync(shipmentId)
                ?? throw new KeyNotFoundException("El envío no existe.");

            if (envio.PilotoId != piloto.Id)
            {
                throw new UnauthorizedAccessException("Este envío no te pertenece o no lo tienes asignado.");
            }

     
            return new DriverShipmentDetail
            {
                Id = envio.Id,
                CodigoTracking = envio.CodigoTracking,
                Estado = envio.Estado?.Nombre ?? string.Empty,
                DestinatarioNombre = $"{envio.Destinatario?.Nombre} {envio.Destinatario?.Apellido}".Trim(),
                Direccion = envio.Destinatario?.Direccion ?? string.Empty,
                Telefono = envio.Destinatario?.Telefono ?? string.Empty,
                Peso = envio.Peso,
                Descripcion = envio.Descripcion,
                Distrito = envio.Destinatario?.Distrito?.Nombre ?? string.Empty,
                Departamento = envio.Destinatario?.Distrito?.Departamento?.Nombre ?? string.Empty
            };
        }

        public async Task<List<ShipmentReportDto>> ObtenerReporteAdminPorEmpresaAsync(int empresaId)
        {
            var empresaExiste = await _companyRepository.ObtenerPorIdConUsuarioAsync(empresaId);

            if (empresaExiste == null)
            {
                throw new KeyNotFoundException($"No se encontró ninguna empresa registrada con el ID {empresaId}.");
            }

            var envios = await _shipmentRepository.ObtenerEnviosConDetallesPorEmpresaAsync(empresaId);

            var enviosDto = envios.Select(e => new ShipmentReportDto
            {
                Id = e.Id,
                CodigoTracking = e.CodigoTracking,
                Peso = e.Peso,
                Descripcion = e.Descripcion,
                EstadoNombre = e.Estado?.Nombre ?? "Sin Estado",
                DestinatarioNombre = e.Destinatario?.Nombre ?? "Sin Destinatario",
                EmpresaNombre = e.Empresa?.NombreEmpresa ?? "Empresa Desconocida",
                PilotoNombre = e.Piloto?.Nombre ?? "No se ha asignado",
                MotivoDevolucion = e.MotivoDevolucion ?? "No aplica",
            }).ToList();

            return enviosDto;
        }

        public async Task<List<ShipmentReportDto>> ObtenerReporteAdminPorEstadoAsync(int? statusId)
        {
            var envios = await _shipmentRepository.ObtenerReportePorEstadoAsync(statusId);

            return envios.Select(e => new ShipmentReportDto
            {
                Id = e.Id,
                CodigoTracking = e.CodigoTracking,
                Peso = e.Peso,
                Descripcion = e.Descripcion,
                EstadoNombre = e.Estado?.Nombre ?? "Sin Estado",
                DestinatarioNombre = e.Destinatario?.Nombre ?? "Sin Destinatario",
                EmpresaNombre = e.Empresa?.NombreEmpresa ?? "Empresa Desconocida",
                PilotoNombre = e.Piloto?.Nombre ?? "No se ha asignado",
                MotivoDevolucion = e.MotivoDevolucion ?? "N/A"
            }).ToList();
        }

        public async Task<List<ShipmentAdminDto>> ObtenerTodosAdminAsync(int usuarioId)
        {
            // Obtener el admin logeado
            var admin = await _adminRepository.ObtenerPorUsuarioIdAsync(usuarioId)
                ?? throw new UnauthorizedAccessException("No se encontró un perfil de administrador para este usuario.");

            // Si es Master, pasar null (ver todos)
            // Si es Admin Departamental, pasar su DistritoId
            int? distritoId = admin.EsMaster ? null : admin.DistritoId;

            var envios = await _shipmentRepository.ObtenerTodosAdminAsync(distritoId);

            return envios.Select(e => new ShipmentAdminDto
            {
                Id = e.Id,
                CodigoTracking = e.CodigoTracking,
                EmpresaNombre = e.Empresa?.NombreEmpresa ?? "Desconocida",
                DestinatarioNombre = $"{e.Destinatario?.Nombre} {e.Destinatario?.Apellido}".Trim(),
                EstadoNombre = e.Estado?.Nombre ?? "Sin Estado",
                PilotoNombre = e.Piloto?.Nombre ?? "No asignado",
                Peso = e.Peso,
                Descripcion = e.Descripcion,
                FechaAsignacion = e.FechaAsignacion,
                MotivoDevolucion = e.MotivoDevolucion
            }).ToList();
        }
    }
}
