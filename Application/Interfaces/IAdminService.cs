using Application.DTOS;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        Task<Administrador> CrearAdministradorAsync(CreateAdminDto dto);
        Task<bool> ActualizarAdministradorAsync(int id, UpdateAdminDto dto);
    }
}