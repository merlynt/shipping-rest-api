using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorCredenciales(string email, string password)
        {
            // Busca al usuario y trae de una vez su Rol
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<int> ObtenerIdEmpresaPorUsuario(int usuarioId)
        {
            var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);
            return empresa?.Id ?? 0;
        }

        public async Task<int> ObtenerIdPilotoPorUsuario(int usuarioId)
        {
            var piloto = await _context.Pilotos.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
            return piloto?.Id ?? 0;
        }
    }
}