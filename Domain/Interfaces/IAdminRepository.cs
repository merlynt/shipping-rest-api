using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAdminRepository
    {
        Task<bool> ExisteEmail(string email);
        Task<bool> ExisteDistrito(int distritoId);
        Task<Administrador> CrearAdministrador(Administrador administrador, Usuario usuario);
        
        // Estos son los que faltaban para el AdminService
        Task<Administrador?> GetByIdAsync(int id);
        Task ActualizarAdministrador(Administrador administrador);
    }
}