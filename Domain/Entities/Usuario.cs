using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Relación: Un usuario tiene un rol (Admin, Empresa o Piloto)
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;
    }
}