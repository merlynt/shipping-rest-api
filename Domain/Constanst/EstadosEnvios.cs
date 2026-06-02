using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Constanst
{
    public static class EstadosEnvios
    {
        public const int Recolectado = 1;
        public const int EnBodega = 2;
        public const int EnRuta = 3;
        public const int Entregado = 4;
        public const int Devolucion = 5;
    }
}
