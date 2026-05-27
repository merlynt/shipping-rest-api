using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TrackingService : ITrackingService
    {
        public string GenerarCodigo()
        {
            // Mantenemos tu lógica pero centralizada
            return $"NX-{DateTime.Now.Ticks.ToString().Substring(10)}";
        }
    }
}