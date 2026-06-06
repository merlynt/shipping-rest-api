namespace Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int UsersId { get; set; }
        public Usuario? Usuario { get; set; } // Relación con el usuario
        public int DistrictsId { get; set; }
    }
}