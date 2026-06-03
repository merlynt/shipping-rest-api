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

        public async Task<Piloto> Crear(Piloto piloto, Usuario usuario)
        {
            // Usamos una transacción para garantizar que ambas entidades
            // (Usuario + Piloto) se guarden juntas o ninguna se guarde
            using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Vinculamos el Piloto al Usuario recién creado
            piloto.UsuarioId = usuario.Id;
            _context.Pilotos.Add(piloto);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return piloto;
        }

        public async Task<Piloto?> ObtenerPorId(int id)
        {
            return await _context.Pilotos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Piloto> Actualizar(Piloto piloto)
        {
            _context.Pilotos.Update(piloto);
            await _context.SaveChangesAsync();
            return piloto;
        }
    }
}