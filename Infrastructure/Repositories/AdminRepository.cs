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
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExisteDistrito(int distritoId)
        {
            return await _context.Distritos.AnyAsync(d => d.Id == distritoId);
        }

        public async Task<Administrador> CrearAdministrador(Administrador administrador, Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            administrador.UsuarioId = usuario.Id;

            _context.Administradores.Add(administrador);
            await _context.SaveChangesAsync();

            return administrador;
        }

        // --- NUEVOS MÉTODOS PARA EDICIÓN ---

        public async Task<Administrador?> GetByIdAsync(int id)
        {
            // Incluimos el Usuario para poder modificarlo cuando el admin sea actualizado
            return await _context.Administradores
                .Include(a => a.Usuario) 
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task ActualizarAdministrador(Administrador administrador)
        {
            // Entity Framework rastrea los cambios realizados en el objeto 'administrador'
            // y en su propiedad de navegación 'Usuario'
            _context.Administradores.Update(administrador);
            await _context.SaveChangesAsync();
        }
    }
}