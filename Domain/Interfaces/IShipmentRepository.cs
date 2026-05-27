using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IShipmentRepository
    {
        Task<List<Envio>> ObtenerTodosPorEmpresa(int empresaId);
        Task<Envio> Crear(Envio envio);

        Task<Envio?> ObtenerPorTracking(string codigoTracking, int empresaId);

        Task<bool> ExisteDestinatario(int id);
    }
}