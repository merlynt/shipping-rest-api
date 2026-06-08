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

        public ShipmentService(IShipmentRepository shipmentRepository, ITrackingService trackingService)
        {
            _shipmentRepository = shipmentRepository;
            _trackingService = trackingService;
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

        public async Task<List<EnvioResponseDto>> ObtenerTodosPorEmpresaAsync(int empresaId)
        {
            var envios = await _shipmentRepository.ObtenerTodosPorEmpresa(empresaId);

            // El mapeo ahora vive aquí, lejos del controlador
            return envios.Select(e => new EnvioResponseDto
            {
                Id = e.Id,
                CodigoTracking = e.CodigoTracking,
                Peso = e.Peso,
                Descripcion = e.Descripcion,
                EstadoNombre = e.Estado?.Nombre ?? "Sin Estado",
                DestinatarioNombre = $"{e.Destinatario?.Nombre} {e.Destinatario?.Apellido}",
                DestinatarioTelefono = e.Destinatario?.Telefono ?? "",
                DestinatarioDireccion = e.Destinatario?.Direccion ?? ""
            }).ToList();
        }

        public async Task<EnvioResponseDto?> ObtenerPorTrackingAsync(string codigoTracking, int empresaId)
        {
            var envio = await _shipmentRepository.ObtenerPorTracking(codigoTracking, empresaId);

            if (envio == null) return null;

            return new EnvioResponseDto
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
            };
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

        
    }
}
