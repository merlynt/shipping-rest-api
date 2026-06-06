using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICompanyRepository
    {
        Task<bool> ExisteCode(string code);
        Task<bool> ExisteEmail(string email);
        Task<bool> ExisteDistrito(int distritoId);
        Task<Company> RegistrarEmpresa(Company empresa, Usuario usuario);
    }
}