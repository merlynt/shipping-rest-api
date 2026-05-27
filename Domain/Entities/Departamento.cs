using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Departamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Relación: Un departamento tiene muchos distritos
        public ICollection<Distrito> Distritos { get; set; } = new List<Distrito>();
    }
}