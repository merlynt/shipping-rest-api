using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOS;

namespace Application.Services
{
    public interface IShipmentService
    {
        Task<EnvioResponseDto?> CrearEnvioAsync(CreateEnvioDto dto, int empresaId);
        Task<List<EnvioResponseDto>> ObtenerTodosPorEmpresaAsync(int empresaId);

        Task<EnvioResponseDto?> ObtenerPorTrackingAsync(string codigoTracking, int empresaId);
        Task<IEnumerable<DriverShipmentResponseDto>> GetMyShipmentsAsync(int usuarioId);
    }
}
