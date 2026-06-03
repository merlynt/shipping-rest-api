using System.ComponentModel.DataAnnotations;

namespace Application.DTOS
{
    public class CreateDriverDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El vehículo es obligatorio")]
        public string Vehiculo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de licencia es obligatorio")]
        public string NumeroLicencia { get; set; } = string.Empty;

        // Credenciales de acceso al sistema
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;
    }
}
