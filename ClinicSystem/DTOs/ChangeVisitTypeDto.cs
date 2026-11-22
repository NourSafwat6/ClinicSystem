using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.API.DTOs
{
    public class ChangeVisitTypeDto
    {
        [Required]
        public string VisitType { get; set; } = null!;
    }
}