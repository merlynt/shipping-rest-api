using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class DriverShipmentDetail
    {
        public int Id { get; set; }

        public string CodigoTracking { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string DestinatarioNombre { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;
        
        public decimal Peso { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
    }
}
