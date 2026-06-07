using Application.DTOS;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> RegistrarEmpresaAsync(CreateCompanyDto dto);
    }
}