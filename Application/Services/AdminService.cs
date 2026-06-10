using Application.DTOS;
using Application.Interfaces; // Ahora apunta a la nueva carpeta de interfaces
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<Administrador> CrearAdministradorAsync(CreateAdminDto dto)
        {
            var emailExiste = await _adminRepository.ExisteEmail(dto.Email);
            if (emailExiste) throw new InvalidOperationException("EMAIL_EXISTE");

            var distritoExiste = await _adminRepository.ExisteDistrito(dto.DistritoId);
            if (!distritoExiste) throw new ArgumentException("DISTRITO_NO_EXISTE");

            var usuario = new Usuario
            {
                Email = dto.Email,
                Password = dto.Password,
                RolId = 1 
            };

            var administrador = new Administrador
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                EsMaster = dto.EsMaster,
                DistritoId = dto.DistritoId
            };

            return await _adminRepository.CrearAdministrador(administrador, usuario);
        }

        public async Task<bool> ActualizarAdministradorAsync(int id, UpdateAdminDto dto)
        {
            var administrador = await _adminRepository.GetByIdAsync(id);
            
            if (administrador == null)
            {
                return false;
            }

            administrador.Nombre = dto.Name ?? administrador.Nombre;
            administrador.Apellido = dto.Lastname ?? administrador.Apellido;
            administrador.Direccion = dto.Address ?? administrador.Direccion;
            administrador.Telefono = dto.Phone ?? administrador.Telefono;
            administrador.DistritoId = dto.DistrictsId ?? administrador.DistritoId;

            if (administrador.Usuario != null)
            {
                if (!string.IsNullOrEmpty(dto.Email)) 
                    administrador.Usuario.Email = dto.Email;
                
                if (!string.IsNullOrEmpty(dto.Password)) 
                    administrador.Usuario.Password = dto.Password;
            }

            await _adminRepository.ActualizarAdministrador(administrador);
            return true;
        }

        public async Task<List<AdminResponseDto>> ObtenerTodosAsync()
        {
            var admins = await _adminRepository.ObtenerTodosAsync();

            var adminsDto = admins.Select(a => new AdminResponseDto
            {
                Id = a.Id,
                NombreCompleto = $"{a.Nombre} {a.Apellido}",
                Telefono = a.Telefono,
                EsMaster = a.EsMaster,
                Email = a.Usuario?.Email ?? "Sin correo",
                Rol = a.Usuario?.Rol?.Nombre ?? "Sin rol",
                Distrito = a.Distrito?.Nombre ?? "Sin distrito"
            }).ToList();

            return adminsDto;
        }

        public async Task EliminarAdministradorAsync(int id)
        {
            
            var adminExistente = await _adminRepository.GetByIdAsync(id);

            if (adminExistente == null)
            {
                throw new KeyNotFoundException("El administrador no existe en la base de datos.");
            }

            if (adminExistente.EsMaster)
            {
                throw new InvalidOperationException("No está permitido eliminar a un Administrador Master Nacional.");
            }

            await _adminRepository.EliminarAdministrador(id);
        }
    }
}