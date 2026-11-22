using ClinicSystem.API.Data;
using ClinicSystem.API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.API.Services
{
    public class AvailabilityService
    {
        private readonly AppDbContext _context;
        private readonly ClinicScheduleSettings _schedule;

        public AvailabilityService(
            AppDbContext context,
            IOptions<ClinicScheduleSettings> schedule)
        {
            _context = context;
            _schedule = schedule.Value;
        }

        private DateTime GetEgyptNow()
        {
            var egyptTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, egyptTimeZone);
        }

        public async Task<List<DateOnly>> GetAvailableDaysAsync()
        {
            var today = DateOnly.FromDateTime(GetEgyptNow());

            var availableDays = new List<DateOnly>();

            for (int i = 0; i < _schedule.BookingWindowDays; i++)
            {
                var date = today.AddDays(i);

                if (!_schedule.WorkingDays.Contains(date.DayOfWeek))
                {
                    continue;
                }

                var slots = await GetAvailableSlotsAsync(date);

                if (slots.Count > 0)
                {
                    availableDays.Add(date);
                }
            }

            return availableDays;
        }

        public async Task<List<TimeOnly>> GetAvailableSlotsAsync(DateOnly date)
        {
            var now = GetEgyptNow();
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);

            var availableSlots = new List<TimeOnly>();

            if (date < today ||
                date >= today.AddDays(_schedule.BookingWindowDays))
            {
                return availableSlots;
            }

            if (!_schedule.WorkingDays.Contains(date.DayOfWeek))
            {
                return availableSlots;
            }

            if (_schedule.SlotIntervalMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "Invalid slot interval.");
            }

            var bookedSlots = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.SlotDate == date &&
                            b.Status != "cancelled")
                .Select(b => b.SlotTime)
                .ToListAsync();

            for (var time = _schedule.OpeningTime;
                 time < _schedule.ClosingTime;
                 time = time.AddMinutes(_schedule.SlotIntervalMinutes))
            {
                if (date == today && time <= currentTime)
                {
                    continue;
                }

                if (bookedSlots.Contains(time))
                {
                    continue;
                }

                availableSlots.Add(time);
            }

            return availableSlots;
        }
    }
}