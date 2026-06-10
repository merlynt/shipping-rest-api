using Application.DTOS;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyDto> CrearEmpresaAsync(CreateCompanyDto dto);
        Task<bool> DesactivarEmpresaAsync(int id);
        Task<bool> ActivarEmpresaAsync(int id);
        Task<CompanyDto> ActualizarEmpresaAsync(int id, UpdateCompanyDto dto);
        Task<List<CompanyDetailDto>> ObtenerTodasAsync(); 
        Task<CompanyDetailDto> ObtenerPorIdAsync(int id);  
    }
}