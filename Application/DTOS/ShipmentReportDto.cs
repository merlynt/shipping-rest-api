using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class ShipmentReportDto
    {
        public int Id { get; set; } 
        public string CodigoTracking { get; set; } = string.Empty;
        public decimal Peso { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string EstadoNombre { get; set; } = string.Empty;
        public string DestinatarioNombre { get; set; } = string.Empty;
        public string EmpresaNombre { get; set; } = string.Empty;

        public string? PilotoNombre { get; set; }

        public string? MotivoDevolucion { get; set; }
    }
}
