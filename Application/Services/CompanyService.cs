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
    }

}
