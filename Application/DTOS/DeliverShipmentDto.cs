using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class DeliverShipmentDto
    {
        [Required(ErrorMessage = "La firma es obligatoria.")]
        public string FirmaUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "La foto es obligatoria.")]
        public string FotoUrl { get; set; } = string.Empty;
    }
}
