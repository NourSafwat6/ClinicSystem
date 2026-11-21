namespace ClinicSystem.API.DTOs
{
    public class VisitTypeDto
    {
        public string Code { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public int DurationMin { get; set; }
        public decimal Price { get; set; }
    }
}