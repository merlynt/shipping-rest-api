using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Administrador
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // true: Master Nacional / false: Administrador Local
        public bool EsMaster { get; set; }

        // Relación uno a uno con el sistema de Login
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int DistritoId { get; set; }
        public Distrito Distrito { get; set; } = null!;
    }
}