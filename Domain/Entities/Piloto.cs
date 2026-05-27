using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Piloto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public string NumeroLicencia { get; set; } = string.Empty;

        // Relación uno a uno con el sistema de Login
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}