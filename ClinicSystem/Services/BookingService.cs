using ClinicSystem.API.Data;
using ClinicSystem.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.API.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookingDetailsDto?> GetBookingByIdAsync(Guid id)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BookingDetailsDto
                {
                    Id = b.Id,
                    SlotDate = b.SlotDate,
                    SlotTime = b.SlotTime,
                    PatientName = b.PatientName,
                    Phone = b.Phone,
                    VisitType = b.VisitType,
                    Price = b.Price,
                    DurationMin = b.DurationMin,
                    Status = b.Status
                })
                .FirstOrDefaultAsync();
        }
    }
}