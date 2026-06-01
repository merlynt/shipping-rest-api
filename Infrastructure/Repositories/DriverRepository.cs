using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly AppDbContext _context;

        public DriverRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Piloto?> GetByUsuarioIdAsync(int usuarioId)
            => await _context.Pilotos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

        /// <inheritdoc />
        /// <remarks>
        /// Envio.PilotoId is nullable (int?) in the real domain model.
        /// The explicit cast to (int?)driverId ensures EF Core generates
        /// a correct parameter-safe WHERE clause.
        /// </remarks>
        public async Task<IEnumerable<Envio>> GetShipmentsByDriverIdAsync(int driverId)
            => await _context.Envios
                .AsNoTracking()
                .Where(e => e.PilotoId == (int?)driverId)   // mandatory security filter
                .Include(e => e.Destinatario)                // Nombre, Apellido, Direccion, Telefono
                .Include(e => e.Estado)                      // Nombre
                .ToListAsync();

    }
}
