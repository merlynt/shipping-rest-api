using System.ComponentModel.DataAnnotations;

namespace Application.DTOS
{
    public class UpdateCompanyDto
    {
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no puede tener más de 200 caracteres.")]
        public string NombreEmpresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(500, ErrorMessage = "La dirección no puede tener más de 500 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un distrito válido.")]
        public int DistritoId { get; set; }
    }
}