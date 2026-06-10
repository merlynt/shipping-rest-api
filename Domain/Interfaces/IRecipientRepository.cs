using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRecipientRepository
    {
        Task<bool> ExisteDistrito(int distritoId);
        Task<Destinatario> Crear(Destinatario destinatario);

        Task<Destinatario?> ObtenerPorId(int id);
        Task<Destinatario> Actualizar(Destinatario destinatario);
    }
}