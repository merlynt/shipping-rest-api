using System.ComponentModel.DataAnnotations;

namespace Application.DTOS
{
    public class UpdateDriverDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El vehículo es obligatorio")]
        public string Vehiculo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de licencia es obligatorio")]
        public string NumeroLicencia { get; set; } = string.Empty;
    }
}
