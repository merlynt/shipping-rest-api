using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;


namespace Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        // Busca al usuario por sus credenciales incluyendo su Rol
        Task<Usuario?> ObtenerPorCredenciales(string email, string password);

        // Métodos para buscar el ID de la entidad según su rol
        Task<int> ObtenerIdEmpresaPorUsuario(int usuarioId);
        Task<int> ObtenerIdPilotoPorUsuario(int usuarioId);

        // Agrega esta línea al final de tu IUsuarioRepository
        Task<(int Id, bool EsMaster)> ObtenerDatosAdminPorUsuario(int usuarioId);
    }
}