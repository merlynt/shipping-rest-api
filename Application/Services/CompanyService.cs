using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CompanyService : ICompanyService
    {

        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<CompanyDto> CrearEmpresaAsync(CreateCompanyDto dto)
        {
            if (await _companyRepository.ExisteCodigoAsync(dto.Codigo))
                throw new InvalidOperationException("El código de empresa ya está registrado.");

            if (!await _companyRepository.ExisteDistritoAsync(dto.DistritoId))
                throw new InvalidOperationException("El distrito especificado no existe.");

            if (await _companyRepository.ExisteEmail(dto.Email))
                throw new InvalidOperationException("El correo electrónico ya está en uso por otro usuario.");

            var nuevoUsuario = new Usuario
            {
                Email = dto.Email,
                Password = dto.Password, 
                Activo = true,
                RolId = 2 
            };

            var nuevaEmpresa = new Empresa
            {
                Codigo = dto.Codigo,
                NombreEmpresa = dto.NombreEmpresa,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Activo = true,
                DistritoId = dto.DistritoId
            };

            var empresaCreada = await _companyRepository.CrearEmpresa(nuevaEmpresa, nuevoUsuario);

            return new CompanyDto
            {
                Id = empresaCreada.Id,
                Codigo = empresaCreada.Codigo,
                NombreEmpresa = empresaCreada.NombreEmpresa,
                Activo = empresaCreada.Activo,
                UsernameCreado = empresaCreada.Usuario.Email 
            };
        }

        public async Task<bool> DesactivarEmpresaAsync(int id)
        {
            var empresa = await _companyRepository.ObtenerPorIdConUsuarioAsync(id);

           
            if (empresa == null)
            {
                throw new KeyNotFoundException($"No se encontró la empresa con el ID {id}.");
            }

            if (!empresa.Activo)
            {
                throw new InvalidOperationException("La empresa ya se encuentra inactiva.");
            }

            empresa.Activo = false;

            if (empresa.Usuario != null)
            {
                empresa.Usuario.Activo = false;
            }

            await _companyRepository.ActualizarEmpresaAsync(empresa);

            return true;
        }

        public async Task<bool> ActivarEmpresaAsync(int id)
        {
            var empresa = await _companyRepository.ObtenerPorIdConUsuarioAsync(id);

            if (empresa == null)
            {
                throw new KeyNotFoundException($"No se encontró la empresa con el ID {id}.");
            }

            if (empresa.Activo)
            {
                throw new InvalidOperationException("La empresa ya se encuentra activa en el sistema.");
            }

            empresa.Activo = true;

            if (empresa.Usuario != null)
            {
                empresa.Usuario.Activo = true;
            }

            await _companyRepository.ActualizarEmpresaAsync(empresa);

            return true;
        }

        public async Task<CompanyDto> ActualizarEmpresaAsync(int id, UpdateCompanyDto dto)
        {
            var empresa = await _companyRepository.ObtenerPorIdAsync(id);

            if (empresa == null)
                throw new KeyNotFoundException($"No se encontró la empresa con el ID {id}.");

            if (!empresa.Activo)
                throw new InvalidOperationException("No se puede actualizar una empresa inactiva.");

            if (!await _companyRepository.ExisteDistritoAsync(dto.DistritoId))
                throw new InvalidOperationException("El distrito especificado no existe.");

            empresa.NombreEmpresa = dto.NombreEmpresa;
            empresa.Telefono = dto.Telefono;
            empresa.Direccion = dto.Direccion;
            empresa.DistritoId = dto.DistritoId;

            await _companyRepository.ActualizarEmpresaAsync(empresa);

            return new CompanyDto
            {
                Id = empresa.Id,
                Codigo = empresa.Codigo,
                NombreEmpresa = empresa.NombreEmpresa,
                Activo = empresa.Activo,
                UsernameCreado = empresa.Usuario?.Email ?? string.Empty
            };
        }

        public async Task<List<CompanyDetailDto>> ObtenerTodasAsync()
        {
            var empresas = await _companyRepository.ObtenerTodasAsync();

            return empresas.Select(e => new CompanyDetailDto
            {
                Id = e.Id,
                Codigo = e.Codigo,
                NombreEmpresa = e.NombreEmpresa,
                Telefono = e.Telefono,
                Direccion = e.Direccion,
                Activo = e.Activo,
                Email = e.Usuario?.Email ?? string.Empty,
                DistritoNombre = e.Distrito?.Nombre ?? string.Empty,
                DepartamentoNombre = e.Distrito?.Departamento?.Nombre ?? string.Empty
            }).ToList();
        }

        public async Task<CompanyDetailDto> ObtenerPorIdAsync(int id)
        {
            var empresa = await _companyRepository.ObtenerPorIdConDetallesAsync(id);

            if (empresa == null)
                throw new KeyNotFoundException($"No se encontró la empresa con el ID {id}.");

            return new CompanyDetailDto
            {
                Id = empresa.Id,
                Codigo = empresa.Codigo,
                NombreEmpresa = empresa.NombreEmpresa,
                Telefono = empresa.Telefono,
                Direccion = empresa.Direccion,
                Activo = empresa.Activo,
                Email = empresa.Usuario?.Email ?? string.Empty,
                DistritoNombre = empresa.Distrito?.Nombre ?? string.Empty,
                DepartamentoNombre = empresa.Distrito?.Departamento?.Nombre ?? string.Empty
            };
        }
    }

}
