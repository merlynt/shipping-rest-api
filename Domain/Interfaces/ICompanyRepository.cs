using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICompanyRepository
    {
        Task<bool> ExisteCodigoAsync(string codigo);
        Task<bool> ExisteDistritoAsync(int distritoId);
        Task<bool> ExisteEmail(string email);
        Task<Empresa> CrearEmpresa(Empresa empresa, Usuario usuario);
        Task<Empresa?> ObtenerPorIdConUsuarioAsync(int id);
        Task ActualizarEmpresaAsync(Empresa empresa);
        Task<Empresa?> ObtenerPorIdAsync(int id);
        Task<List<Empresa>> ObtenerTodasAsync();  
        Task<Empresa?> ObtenerPorIdConDetallesAsync(int id); 
    }
}