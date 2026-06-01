using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class DriverShipmentResponseDto
    {
        /// <summary>Shipment unique identifier.</summary>
        public int Id { get; set; }

        /// <summary>Tracking code.</summary>
        public string CodigoTracking { get; set; } = string.Empty;

        /// <summary>Current status name — from Estado.Nombre.</summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>Recipient full name — Destinatario.Nombre + Apellido.</summary>
        public string DestinatarioNombre { get; set; } = string.Empty;

        /// <summary>Delivery address — Destinatario.Direccion.</summary>
        public string Direccion { get; set; } = string.Empty;

        /// <summary>Recipient phone — Destinatario.Telefono.</summary>
        public string Telefono { get; set; } = string.Empty;

    }
}
