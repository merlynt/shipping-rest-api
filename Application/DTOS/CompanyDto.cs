using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string NombreEmpresa { get; set; } = string.Empty;
        public bool Activo { get; set; }

        public string UsernameCreado { get; set; } = string.Empty;
    }
}
