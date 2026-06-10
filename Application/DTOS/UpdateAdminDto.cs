namespace Application.DTOS
{
    public class UpdateAdminDto
    {
        // Datos del perfil (Admins)
        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int? DistrictsId { get; set; }

        // Datos de credenciales (Users)
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}