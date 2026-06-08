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

        
        [HttpPost]

        /// <summary>
        /// Para master y admin depto. Crea empresas y usuario asociado
        /// </summary>
       
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
    }
}
