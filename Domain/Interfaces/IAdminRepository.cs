using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAdminRepository
    {
        Task<bool> ExisteEmail(string email);

        Task<bool> ExisteDistrito(int distritoId);

        Task<Administrador> CrearAdministrador(
            Administrador administrador,
            Usuario usuario);
    }
}