using Application.DTOS;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService )
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Para master y admin depto. Crea empresas y usuario asociado
        /// </summary>
        [HttpPost]
       
        public async Task<IActionResult> Registrar([FromBody] CreateCompanyDto dto)
        {
            try
            {
             
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resultado = await _companyService.CrearEmpresaAsync(dto);

                return CreatedAtAction(nameof(Registrar), new { id = resultado.Id }, resultado);
            }
            catch (InvalidOperationException ex)
            {
          
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Ocurrió un error interno al registrar la empresa y su usuario.",
                    detalle = ex.Message
                });
            }
        }

        /// <summary>
        /// Para Admin Depto. y Maste. Da de baja a una empresa sin eliminarla físicamente (Soft Delete).
        /// </summary>


        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _companyService.DesactivarEmpresaAsync(id);
                return Ok(new
                {
                    mensaje = "La empresa y su usuario asociado han sido dados de baja exitosamente."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Ocurrió un error interno al intentar desactivar la empresa.",
                    detalle = ex.Message
                });
            }
        }

        /// <summary>
        /// Para Admin Master y depto. Reactiva una empresa que fue dada de baja previamente.
        /// </summary>
      
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                await _companyService.ActivarEmpresaAsync(id);

                return Ok(new
                {
                    mensaje = "La empresa y su usuario asociado han sido reactivados exitosamente."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Ocurrió un error interno al intentar reactivar la empresa.",
                    detalle = ex.Message
                });
            }
        }

        /// <summary>
        /// Para Admin Master y Depto. Actualiza los datos de una empresa existente.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Administrador.
        /// No se puede actualizar el código de la empresa (identificador único).
        /// Solo se pueden actualizar: nombre, teléfono, dirección y distrito.
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateCompanyDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resultado = await _companyService.ActualizarEmpresaAsync(id, dto);

                return Ok(new
                {
                    mensaje = "La empresa ha sido actualizada exitosamente.",
                    datos = resultado
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Ocurrió un error interno al actualizar la empresa.",
                    detalle = ex.Message
                });
            }
        }

        /// <summary>
        /// Para Admin Master y Depto. Obtiene la lista de todas las empresas registradas.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Administrador.
        /// Devuelve tanto empresas activas como inactivas.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var empresas = await _companyService.ObtenerTodasAsync();
                return Ok(empresas);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Ocurrió un error interno al obtener la lista de empresas."
                });
            }
        }

        /// <summary>
        /// Para Admin Master y Depto. Obtiene el detalle completo de una empresa específica.
        /// </summary>
        /// <remarks>
        /// **Rol Requerido:** Administrador.
        /// Incluye información del distrito, departamento y usuario asociado.
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var empresa = await _companyService.ObtenerPorIdAsync(id);
                return Ok(empresa);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Ocurrió un error interno al obtener la empresa."
                });
            }
        }
    }

}
