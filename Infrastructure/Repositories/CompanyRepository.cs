using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExisteCodigoAsync(string codigo)
        {
            return await _context.Empresas.AnyAsync(e => e.Codigo == codigo);
        }

        public async Task<bool> ExisteDistritoAsync(int distritoId)
        {
            return await _context.Distritos.AnyAsync(d => d.Id == distritoId);
        }

        public async Task<bool> ExisteEmail(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<Empresa> CrearEmpresa(Empresa empresa, Usuario usuario)
        {
            empresa.Usuario = usuario;

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            var empresaCreada = await _context.Empresas
                .Include(e => e.Distrito)
                .Include(e => e.Usuario)
                    .ThenInclude(u => u.Rol)
                .FirstAsync(e => e.Id == empresa.Id);

            return empresaCreada;
        }
        public async Task<Empresa?> ObtenerPorIdConUsuarioAsync(int id)
        {
            return await _context.Empresas
                .IgnoreQueryFilters()
                .Include(e => e.Usuario) 
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task ActualizarEmpresaAsync(Empresa empresa)
        {
            _context.Empresas.Update(empresa);
            await _context.SaveChangesAsync();
        }

    }
}
