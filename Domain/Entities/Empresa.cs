using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;

        // SOFT DELETE: Permite dar de baja sin eliminar el registro físico
        public bool Activo { get; set; } = true;

        // Relación uno a uno con el sistema de Login
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int DistritoId { get; set; }
        public Distrito Distrito { get; set; } = null!;
    }
}