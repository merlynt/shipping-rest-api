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
    }

}
