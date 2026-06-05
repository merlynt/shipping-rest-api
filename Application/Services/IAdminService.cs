using Application.DTOS;
using Domain.Entities;

namespace Application.Services
{
    public interface IAdminService
    {
        Task<Administrador> CrearAdministradorAsync(CreateAdminDto dto);
    }
}