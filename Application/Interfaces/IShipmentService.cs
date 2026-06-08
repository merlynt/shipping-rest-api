using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOS;

namespace Application.Interfaces
{
    public interface IShipmentService
    {
        Task<EnvioResponseDto?> CrearEnvioAsync(CreateEnvioDto dto, int empresaId);
        Task<List<EnvioResponseDto>> ObtenerTodosPorEmpresaAsync(int empresaId);

        Task<EnvioResponseDto?> ObtenerPorTrackingAsync(string codigoTracking, int empresaId);
        Task<IEnumerable<DriverShipmentResponseDto>> GetMyShipmentsAsync(int usuarioId);
        Task<bool> EntregarEnvioAsync(string codigoTracking, int driverId, DeliverShipmentDto dto);

        Task<bool> DevolverEnvioAsync(string codigoTracking, int usuarioId, ReturnShipmentDto dto);

        Task<DriverShipmentDetail> ObtenerDetalleEnvioParaDriverAsync(int shipmentId, int usuarioId);

    }
}
