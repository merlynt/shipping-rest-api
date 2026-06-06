namespace Application.DTOS
{
    public class CreateCompanyDto
    {
        public string Code { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int DistrictsId { get; set; }
    }
}