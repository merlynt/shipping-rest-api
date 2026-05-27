using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Distrito
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Relación: Un distrito pertenece a un departamento
        public int DepartamentoId { get; set; }
        public Departamento Departamento { get; set; } = null!;
    }
}