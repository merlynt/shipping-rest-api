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
                .Include(e => e.Piloto)
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
                .Include(e => e.Piloto)
                .Include(e => e.Estado)
                .FirstOrDefaultAsync(e => e.CodigoTracking == codigoTracking && e.EmpresaId == empresaId);
        }

        public async Task<bool> ExisteDestinatario(int id)
        {
            // Usamos el _context que ya deberías tener inyectado en el repositorio
            return await _context.Destinatarios.AnyAsync(d => d.Id == id);
        }

        public async Task<Envio?> ObtenerPorId(int id)
        {
            return await _context.Envios
                .Include(e => e.Estado)
                .Include(e => e.Piloto)
                .Include(e => e.Destinatario)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Envio> Actualizar(Envio envio)
        {
            _context.Envios.Update(envio);
            await _context.SaveChangesAsync();
            return envio;
        }

        public async Task<bool> ExistePiloto(int pilotoId)
        {
            return await _context.Pilotos.AnyAsync(p => p.Id == pilotoId);
        }

        public async Task<Piloto?> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Pilotos
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<Envio>> GetShipmentsByDriverIdAsync(int driverId)
        {
            return await _context.Envios
                .Include(e => e.Estado)
                .Include(e => e.Destinatario)
                .Where(e => e.PilotoId == driverId)
                .ToListAsync();
        }

        public async Task<Envio?> ObtenerPorTrackingAsync(string codigoTracking)
        {
            return await _context.Envios
                .Include(e => e.Estado)
                .Include(e => e.Destinatario)
                .FirstOrDefaultAsync(e => e.CodigoTracking == codigoTracking);
        }

        public async Task<Envio?> ObtenerPorIdAsync(int id)
        {
            return await _context.Envios
                .Include(e => e.Estado)
                .Include(e => e.Destinatario)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task GuardarEvidenciaYActualizarEstadoAsync(Envio envio, Evidencia evidencia)
        {
            _context.Evidencias.Add(evidencia);
            _context.Envios.Update(envio);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Envio envio)
        {
            _context.Envios.Update(envio);
            await _context.SaveChangesAsync();
        }

        public async Task<Envio?> ObtenerConDetallesAsync(int id)
        {
            return await _context.Envios
                .Include(e => e.Estado)
                .Include(e => e.Destinatario)
                    .ThenInclude(d => d.Distrito)
                        .ThenInclude(dist => dist.Departamento)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Envio>> ObtenerEnviosConDetallesPorEmpresaAsync(int empresaId)
        {
            return await _context.Envios
                .Where(e => e.EmpresaId == empresaId)
                .Include(e => e.Estado)
                .Include(e => e.Destinatario)
                .Include(e => e.Empresa)
                .Include(e => e.Piloto)  
                .ToListAsync();
        }

        // Ruta: Infrastructure/Repositories/ShipmentRepository.cs

        public async Task<List<Envio>> ObtenerReportePorEstadoAsync(int? statusId)
        {
            var query = _context.Envios
                .Include(e => e.Estado)
                .Include(e => e.Destinatario)
                .Include(e => e.Empresa)
                .Include(e => e.Piloto)
                .AsQueryable();

            if (statusId.HasValue)
            {
                query = query.Where(e => e.EstadoId == statusId.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Obtiene todos los envíos - Solo para Admin
        /// </summary>
        public async Task<List<Envio>> ObtenerTodosAdminAsync(int? distritoId = null)
        {
            var query = _context.Envios
                .IgnoreQueryFilters()
                .Include(e => e.Estado)
                .Include(e => e.Destinatario)
                .Include(e => e.Empresa)
                .Include(e => e.Piloto)
                .AsQueryable();

            // Si distritoId tiene valor, filtra por empresas en ese distrito
            // Si es null, devuelve todos (para Super Admin)
            if (distritoId.HasValue)
            {
                query = query.Where(e => e.Destinatario != null && e.Destinatario.DistritoId == distritoId.Value);
            }

            return await query.OrderByDescending(e => e.Id).ToListAsync();
        }
    }
}