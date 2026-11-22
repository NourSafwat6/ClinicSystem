using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.API.DTOs
{
    public class CreateDeskBookingDto
    {
        [Required]
        public string PatientName { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

        [Required]
        public string VisitType { get; set; } = null!;

        [Required]
        public DateOnly? SlotDate { get; set; }

        [Required]
        public TimeOnly? SlotTime { get; set; }
    }
}