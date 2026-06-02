using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class ReturnShipmentDto
    {
        [Required(ErrorMessage = "El motivo de la devolución es obligatorio.")]
        public string Motivo { get; set; } = string.Empty;
    }
}
