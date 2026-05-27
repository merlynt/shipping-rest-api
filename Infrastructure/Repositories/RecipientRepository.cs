using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RecipientRepository : IRecipientRepository
    {
        private readonly AppDbContext _context;

        public RecipientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteDistrito(int distritoId)
        {
            //  validación del distrito aquí
            return await _context.Distritos.AnyAsync(d => d.Id == distritoId);
        }

        public async Task<Destinatario> Crear(Destinatario destinatario)
        {
            _context.Destinatarios.Add(destinatario);
            await _context.SaveChangesAsync();
            return destinatario;
        }
    }
}