namespace Application.DTOS
{
    public class DriverResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public string NumeroLicencia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
