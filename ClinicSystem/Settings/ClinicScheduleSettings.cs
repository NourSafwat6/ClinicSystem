namespace ClinicSystem.API.Settings
{
    public class ClinicScheduleSettings
    {
        public List<DayOfWeek> WorkingDays { get; set; } = new();

        public TimeOnly OpeningTime { get; set; }

        public TimeOnly ClosingTime { get; set; }

        public int SlotIntervalMinutes { get; set; }

        public int BookingWindowDays { get; set; }

        public int ArrivalBeforeMinutes { get; set; }
    }
}