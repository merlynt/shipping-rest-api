using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Envio
    {
        public int Id { get; set; }
        public string CodigoTracking { get; set; } = string.Empty;
        public decimal Peso { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        // Relaciones obligatorias
        public int EstadoId { get; set; }
        public Estado Estado { get; set; } = null!;

        public int DestinatarioId { get; set; }
        public Destinatario Destinatario { get; set; } = null!;

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public int? PilotoId { get; set; }
        public Piloto? Piloto { get; set; }

        public List<Evidencia> Evidencias { get; set; } = new();
    }
}