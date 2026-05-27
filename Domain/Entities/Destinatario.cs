using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Destinatario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int DistritoId { get; set; }
        public Distrito Distrito { get; set; } = null!;
    }
}