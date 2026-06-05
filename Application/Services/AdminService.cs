using Application.DTOS;
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
            // Validar email duplicado
            var emailExiste = await _adminRepository.ExisteEmail(dto.Email);

            if (emailExiste)
            {
                throw new InvalidOperationException("EMAIL_EXISTE");
            }

            // Validar distrito
            var distritoExiste = await _adminRepository.ExisteDistrito(dto.DistritoId);

            if (!distritoExiste)
            {
                throw new ArgumentException("DISTRITO_NO_EXISTE");
            }

            // Crear usuario
            var usuario = new Usuario
            {
                Email = dto.Email,
                Password = dto.Password,
                RolId = 1 // Administrador
            };

            // Crear administrador
            var administrador = new Administrador
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                EsMaster = dto.EsMaster,
                DistritoId = dto.DistritoId
            };

            return await _adminRepository.CrearAdministrador(
                administrador,
                usuario);
        }
    }
}