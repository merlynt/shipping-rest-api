using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITokenService
    {
        // Recibe los datos del usuario y genera el string del Token JWT
        string GenerarToken(int usuarioId, string rol, int entidadId);
    }
}