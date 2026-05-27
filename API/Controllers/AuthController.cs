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

            if (usuario.Rol?.Nombre == "Empresa")
            {
                entidadId = await _usuarioRepo.ObtenerIdEmpresaPorUsuario(usuario.Id);
            }
            else if (usuario.Rol?.Nombre == "Piloto")
            {
                entidadId = await _usuarioRepo.ObtenerIdPilotoPorUsuario(usuario.Id);
            }

            // 3. Generamos el Token
            var token = _tokenService.GenerarToken(usuario.Id, usuario.Rol!.Nombre, entidadId);

            return Ok(new { Token = token, Rol = usuario.Rol.Nombre });
        }
    }
}