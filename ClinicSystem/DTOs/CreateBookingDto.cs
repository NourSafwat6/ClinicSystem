using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.API.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^01[0125][0-9]{8}$")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string VisitType { get; set; } = string.Empty;

        public DateOnly? SlotDate { get; set; }
        public TimeOnly? SlotTime { get; set; }
    }
}