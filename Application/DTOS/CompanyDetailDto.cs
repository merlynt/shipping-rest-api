using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class CompanyDetailDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DistritoNombre { get; set; } = string.Empty;
        public string DepartamentoNombre { get; set; } = string.Empty;
    }
}