using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDriverRepository
    {
        /// <summary>
        /// Returns the Piloto linked to the given usuarioId, or null if not found.
        /// </summary>
        Task<Piloto?> GetByUsuarioIdAsync(int usuarioId);

        /// <summary>
        /// Returns all shipments where Envio.PilotoId = driverId,
        /// including Destinatario and Estado navigation properties.
        /// </summary>
        Task<IEnumerable<Envio>> GetShipmentsByDriverIdAsync(int driverId);

    }
}
