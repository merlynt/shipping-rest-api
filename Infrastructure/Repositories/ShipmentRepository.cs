using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    // Aquí implementamos el contrato del Domain y usamos Entity Framework
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly AppDbContext _context;

        public ShipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Envio>> ObtenerTodosPorEmpresa(int empresaId)
        {
            return await _context.Envios
                .Where(e => e.EmpresaId == empresaId)
                .Include(e => e.Estado)
                .Include(e => e.Destinatario)
                .ToListAsync();
        }

        public async Task<Envio> Crear(Envio envio)
        {
            _context.Envios.Add(envio);
            await _context.SaveChangesAsync();
            return envio; // Entity Framework le asigna el ID automáticamente al hacer SaveChanges
        }

        public async Task<Envio?> ObtenerPorTracking(string codigoTracking, int empresaId)
        {
            return await _context.Envios
                .Include(e => e.Evidencias) 
                .Include(e => e.Destinatario)
                .Include(e => e.Estado)
                .FirstOrDefaultAsync(e => e.CodigoTracking == codigoTracking && e.EmpresaId == empresaId);
        }

        public async Task<bool> ExisteDestinatario(int id)
        {
            // Usamos el _context que ya deberías tener inyectado en el repositorio
            return await _context.Destinatarios.AnyAsync(d => d.Id == id);
        }

        public async Task<Piloto?> GetByUsuarioIdAsync(int usuarioId)
            => await _context.Pilotos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        public async Task<IEnumerable<Envio>> GetShipmentsByDriverIdAsync(int driverId)
            => await _context.Envios
                .AsNoTracking()
                .Where(e => e.PilotoId == (int?)driverId)  
                .Include(e => e.Destinatario)               
                .Include(e => e.Estado)                     
                .ToListAsync();
        public async Task<Envio?> ObtenerPorIdAsync(int id)
        {
            return await _context.Envios.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task GuardarEvidenciaYActualizarEstadoAsync(Envio envio, Evidencia evidencia)
        {
            _context.Envios.Update(envio);
            await _context.Evidencias.AddAsync(evidencia);

            await _context.SaveChangesAsync();
        }

        public async Task<Envio?> ObtenerPorTrackingAsync(string codigoTracking)
        {
            return await _context.Envios.FirstOrDefaultAsync(e => e.CodigoTracking == codigoTracking);
        }
        public async Task ActualizarAsync(Envio envio)
        {
            _context.Envios.Update(envio);
            await _context.SaveChangesAsync();
        }
    }
}