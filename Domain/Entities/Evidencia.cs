using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Evidencia
    {
        public int Id { get; set; }
        public string FirmaUrl { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = string.Empty;

        public int EnvioId { get; set; }
        public Envio Envio { get; set; } = null!;
    }
}