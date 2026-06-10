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
            // 1. Obtener el administrador existente
            var administrador = await _adminRepository.GetByIdAsync(id);
            
            if (administrador == null)
            {
                return false;
            }

            // 2. Actualizar propiedades
            administrador.Nombre = dto.Name ?? administrador.Nombre;
            administrador.Apellido = dto.Lastname ?? administrador.Apellido;
            administrador.Direccion = dto.Address ?? administrador.Direccion;
            administrador.Telefono = dto.Phone ?? administrador.Telefono;
            administrador.DistritoId = dto.DistrictsId ?? administrador.DistritoId;

            // 3. Actualizar datos de usuario si vienen en el DTO
            if (administrador.Usuario != null)
            {
                if (!string.IsNullOrEmpty(dto.Email)) 
                    administrador.Usuario.Email = dto.Email;
                
                if (!string.IsNullOrEmpty(dto.Password)) 
                    administrador.Usuario.Password = dto.Password;
            }

            // 4. Persistir los cambios
            await _adminRepository.ActualizarAdministrador(administrador);
            return true;
        }
    }
}