using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteEmail(string email)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExisteDistrito(int distritoId)
        {
            return await _context.Distritos
                .AnyAsync(d => d.Id == distritoId);
        }

        public async Task<Administrador> CrearAdministrador(
            Administrador administrador,
            Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            administrador.UsuarioId = usuario.Id;

            _context.Administradores.Add(administrador);
            await _context.SaveChangesAsync();

            return administrador;
        }
    }
}