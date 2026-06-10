using Application.DTOS;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepo; // ¡Cebolla pura!
        private readonly ITokenService _tokenService;

        public AuthController(IUsuarioRepository usuarioRepo, ITokenService tokenService)
        {
            _usuarioRepo = usuarioRepo;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // 1. Validamos credenciales mediante el repositorio
            var usuario = await _usuarioRepo.ObtenerPorCredenciales(dto.Email, dto.Password);

            if (usuario == null) return Unauthorized("Credenciales incorrectas.");

            int entidadId = 0;
            bool esMaster = false; // 👈 1. Agregamos esta variable

            if (usuario.Rol?.Nombre == "Empresa")
            {
                entidadId = await _usuarioRepo.ObtenerIdEmpresaPorUsuario(usuario.Id);
            }
            else if (usuario.Rol?.Nombre == "Piloto")
            {
                entidadId = await _usuarioRepo.ObtenerIdPilotoPorUsuario(usuario.Id);
            }
            // 👇 2. AGREGAMOS EL BLOQUE DEL ADMINISTRADOR 👇
            else if (usuario.Rol?.Nombre == "Administrador" || usuario.Rol?.Nombre == "Admin")
            {
                // Llamamos al método nuevo que creaste en UsuarioRepository
                var datosAdmin = await _usuarioRepo.ObtenerDatosAdminPorUsuario(usuario.Id);

                entidadId = datosAdmin.Id;
                esMaster = datosAdmin.EsMaster; // Extraemos el true/false de la base de datos
            }

            // 👇 3. Le pasamos esMaster al TokenService 👇
            var token = _tokenService.GenerarToken(usuario.Id, usuario.Rol!.Nombre, entidadId, esMaster);

            return Ok(new { Token = token, Rol = usuario.Rol.Nombre });
        }
    }
}