using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOS
{
    public class CreateEnvioDto
    {
        [Required]
        [Range(0.1, 1000.0, ErrorMessage = "El peso debe ser mayor a 0")]
        public decimal Peso { get; set; }

        [Required(ErrorMessage = "La descripción es necesaria")]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public int DestinatarioId { get; set; }
    }
}