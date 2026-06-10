using Application.DTOS;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class DriversController : ControllerBase
    {
        private readonly IDriverRepository _driverRepo;
        private readonly IShipmentRepository _shipmentRepo;

        private const int ROL_PILOTO_ID = 3;
        private const int ESTADO_EN_RUTA_ID = 3;

        public DriversController(IDriverRepository driverRepo, IShipmentRepository shipmentRepo)
        {
            _driverRepo = driverRepo;
            _shipmentRepo = shipmentRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDriverDto dto)
        {
            var usuario = new Usuario
            {
                Email = dto.Email,
                Password = dto.Password,
                RolId = ROL_PILOTO_ID
            };

            var piloto = new Piloto
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Vehiculo = dto.Vehiculo,
                NumeroLicencia = dto.NumeroLicencia
            };

            var creado = await _driverRepo.Crear(piloto, usuario);

            var response = new DriverResponseDto
            {
                Id = creado.Id,
                Nombre = creado.Nombre,
                Telefono = creado.Telefono,
                Vehiculo = creado.Vehiculo,
                NumeroLicencia = creado.NumeroLicencia,
                Email = dto.Email
            };

            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDriverDto dto)
        {
            var piloto = await _driverRepo.ObtenerPorId(id);

            if (piloto == null)
                return NotFound($"No se encontró el driver con ID {id}.");

            piloto.Nombre = dto.Nombre;
            piloto.Telefono = dto.Telefono;
            piloto.Vehiculo = dto.Vehiculo;
            piloto.NumeroLicencia = dto.NumeroLicencia;

            var actualizado = await _driverRepo.Actualizar(piloto);

            return Ok(new DriverResponseDto
            {
                Id = actualizado.Id,
                Nombre = actualizado.Nombre,
                Telefono = actualizado.Telefono,
                Vehiculo = actualizado.Vehiculo,
                NumeroLicencia = actualizado.NumeroLicencia,
                Email = actualizado.Usuario?.Email ?? ""
            });
        }

        [HttpPatch("shipments/{shipmentId}/assign")]
        public async Task<IActionResult> AssignShipment(int shipmentId, [FromBody] AssignDriverDto dto)
        {
            var envio = await _shipmentRepo.ObtenerPorId(shipmentId);
            if (envio == null)
                return NotFound($"No se encontró el envío con ID {shipmentId}.");

            if (envio.PilotoId != null || envio.EstadoId == ESTADO_EN_RUTA_ID)
                return Conflict("El envío ya tiene un driver asignado o ya está en ruta.");

            var pilotoExiste = await _shipmentRepo.ExistePiloto(dto.PilotoId);
            if (!pilotoExiste)
                return NotFound($"No se encontró el driver con ID {dto.PilotoId}.");

            envio.PilotoId = dto.PilotoId;
            envio.EstadoId = ESTADO_EN_RUTA_ID;
            envio.FechaAsignacion = DateTime.Now;

            var actualizado = await _shipmentRepo.Actualizar(envio);

            return Ok(new
            {
                actualizado.Id,
                actualizado.CodigoTracking,
                Estado = "En Ruta",
                PilotoAsignado = dto.PilotoId
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var piloto = await _driverRepo.ObtenerPorId(id);
            if (piloto == null) return NotFound();

            return Ok(new DriverResponseDto
            {
                Id = piloto.Id,
                Nombre = piloto.Nombre,
                Telefono = piloto.Telefono,
                Vehiculo = piloto.Vehiculo,
                NumeroLicencia = piloto.NumeroLicencia,
                Email = piloto.Usuario?.Email ?? ""
            });
        }
    }
}