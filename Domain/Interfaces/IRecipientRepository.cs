using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRecipientRepository
    {
        Task<bool> ExisteDistrito(int distritoId);
        Task<Destinatario> Crear(Destinatario destinatario);
    }
}