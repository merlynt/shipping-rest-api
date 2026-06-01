using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOS;
using Domain.Interfaces;

namespace Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;

        public DriverService(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        /// <inheritdoc />


        async Task<IEnumerable<DriverShipmentResponseDto>> IDriverService.GetMyShipmentsAsync(int usuarioId)
        {
            // 1. Resolve the Piloto record that belongs to this authenticated user.
            var piloto = await _driverRepository.GetByUsuarioIdAsync(usuarioId)
                ?? throw new KeyNotFoundException(
                    $"No se encontró un perfil de piloto para el usuario {usuarioId}.");

            // 2. Fetch only shipments where Envio.PilotoId = piloto.Id.
            //    The filter is enforced inside the repository at the SQL level.
            var envios = await _driverRepository.GetShipmentsByDriverIdAsync(piloto.Id);

            // 3. Project to the response DTO using real entity property names.
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
    }
}
