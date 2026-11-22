using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.API.DTOs
{
    public class RescheduleBookingDto
    {
        [Required]
        public DateOnly? SlotDate { get; set; }

        [Required]
        public TimeOnly? SlotTime { get; set; }
    }
}