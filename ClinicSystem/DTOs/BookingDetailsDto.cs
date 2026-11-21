namespace ClinicSystem.API.DTOs
{
    public class BookingDetailsDto
    {
        public Guid Id { get; set; }
        public  DateOnly SlotDate { get; set; }
        public TimeOnly SlotTime { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string  Phone { get; set; } = string.Empty;
        public string  VisitType { get; set; } = string.Empty;
        public  int Price { get; set; }
        public  int DurationMin { get; set; }
        public string  Status { get; set; } = string.Empty;
    }
}
