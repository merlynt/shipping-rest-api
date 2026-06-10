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
           
            administrador.Usuario = usuario;
            _context.Administradores.Add(administrador);
            await _context.SaveChangesAsync();

            var administradorCreado = await _context.Administradores
                .Include(a => a.Distrito)
                .Include(a => a.Usuario)
                    .ThenInclude(u => u.Rol)
                .FirstAsync(a => a.Id == administrador.Id);

          
            return administradorCreado;
        }

        public async Task<Administrador?> GetByIdAsync(int id)
        {
            
            return await _context.Administradores
                .Include(a => a.Distrito)
                .Include(a => a.Usuario)
                    .ThenInclude(u => u.Rol) 
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task ActualizarAdministrador(Administrador administrador)
        {
           
            var adminExistente = await _context.Administradores
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == administrador.Id);

            if (adminExistente != null)
            {
                
                adminExistente.Nombre = administrador.Nombre;
                adminExistente.Apellido = administrador.Apellido;
                adminExistente.Direccion = administrador.Direccion;
                adminExistente.Telefono = administrador.Telefono;
                adminExistente.EsMaster = administrador.EsMaster;
                adminExistente.DistritoId = administrador.DistritoId;

               
                if (adminExistente.Usuario != null && administrador.Usuario != null)
                {
                    adminExistente.Usuario.Email = administrador.Usuario.Email;
                    adminExistente.Usuario.RolId = administrador.Usuario.RolId;
                    adminExistente.Usuario.Activo = administrador.Usuario.Activo;
                }

                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Administrador>> ObtenerTodosAsync()
        {
            return await _context.Administradores
                .Include(a => a.Distrito)
                .Include(a => a.Usuario)
                    .ThenInclude(u => u.Rol)
                .ToListAsync();
        }
        public async Task EliminarAdministrador(int id)
        {
            var adminExistente = await _context.Administradores
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (adminExistente != null)
            {
                // Solo ejecuta las instrucciones de borrado, sin preguntar por qué
                if (adminExistente.Usuario != null)
                {
                    _context.Usuarios.Remove(adminExistente.Usuario);
                }

                _context.Administradores.Remove(adminExistente);
                await _context.SaveChangesAsync();
            }
        }


    }
}