using System.ComponentModel.DataAnnotations;

namespace Application.DTOS
{
    public class AssignDriverDto
    {
        [Required(ErrorMessage = "El ID del driver es obligatorio")]
        public int PilotoId { get; set; }
    }
}
