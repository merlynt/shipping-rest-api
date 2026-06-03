using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDriverRepository
    {
        Task<Piloto> Crear(Piloto piloto, Usuario usuario);
        Task<Piloto?> ObtenerPorId(int id);
        Task<Piloto> Actualizar(Piloto piloto);
    }
}