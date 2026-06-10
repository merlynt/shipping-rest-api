using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAdminRepository
    {
        Task<bool> ExisteEmail(string email);
        Task<bool> ExisteDistrito(int distritoId);
        Task<Administrador> CrearAdministrador(Administrador administrador, Usuario usuario);
       
        Task<Administrador?> GetByIdAsync(int id);
        Task ActualizarAdministrador(Administrador administrador);
        Task<List<Administrador>> ObtenerTodosAsync();
        Task EliminarAdministrador(int id);
    }
}