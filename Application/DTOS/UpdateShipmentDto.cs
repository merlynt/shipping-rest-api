using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class UpdateShipmentDto
    {
        public decimal? Peso { get; set; }
        public string? Descripcion { get; set; }
        public int? DestinatarioId { get; set; }
    }
}