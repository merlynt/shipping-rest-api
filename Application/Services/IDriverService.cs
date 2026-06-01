using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOS;

namespace Application.Services
{
    public interface IDriverService
    {
        /// <summary>
        /// Returns the shipments assigned to the authenticated driver.
        /// Receives the usuarioId extracted from the JWT — never a raw driverId
        /// from the request so callers cannot spoof another driver's data.
        /// </summary>
        Task<IEnumerable<DriverShipmentResponseDto>> GetMyShipmentsAsync(int usuarioId);

    }
}
