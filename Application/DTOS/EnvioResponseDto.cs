using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class EnvioResponseDto
    {
        // Datos identificadores
        public int Id { get; set; }
        public string CodigoTracking { get; set; } = string.Empty;
        public decimal Peso { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        public string EstadoNombre { get; set; } = string.Empty; // "Recolectado", "En ruta", etc.

        // Datos del Destinatario (Relación - Lo que la empresa necesita saber para contactar)
        public string DestinatarioNombre { get; set; } = string.Empty;
        public string DestinatarioTelefono { get; set; } = string.Empty;
        public string DestinatarioDireccion { get; set; } = string.Empty;

        // Datos adicionales clave para la auditoría y tracking
        public DateTime? FechaAsignacion { get; set; }
        public string? NombrePiloto { get; set; }

        public List<EvidenciaDto> Evidencias { get; set; } = new();
    }
}
