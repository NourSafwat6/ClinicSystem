using ClinicSystem.API.Data;
using ClinicSystem.API.DTOs;
using ClinicSystem.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.API.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;
        private readonly AvailabilityService _availabilityService;

        public BookingService(
            AppDbContext context,
            AvailabilityService availabilityService)
        {
            _context = context;
            _availabilityService = availabilityService;
        }

        public async Task<List<BookingDetailsDto>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .AsNoTracking()
                .OrderBy(b => b.SlotDate)
                .ThenBy(b => b.SlotTime)
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
                .ToListAsync();
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

        public async Task<(Guid? BookingId, string? Error)>
            CreateBookingAsync(CreateBookingDto dto)
        {
            if (!dto.SlotDate.HasValue ||
                !dto.SlotTime.HasValue)
            {
                return (null, "Date and time are required.");
            }

            var date = dto.SlotDate.Value;
            var time = dto.SlotTime.Value;

            var visitType = await _context.VisitTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(v =>
                    v.Code == dto.VisitType &&
                    v.PatientCanBook &&
                    v.Code == "kashf");

            if (visitType == null)
            {
                return (null, "Invalid online visit type.");
            }

            var availableSlots = await _availabilityService
                .GetAvailableSlotsAsync(date);

            if (!availableSlots.Contains(time))
            {
                return (null, "This slot is not available.");
            }

            var existingBooking = await _context.Bookings
                .AnyAsync(b =>
                    b.Phone == dto.Phone &&
                    b.SlotDate == date &&
                    b.Status != "cancelled");

            if (existingBooking)
            {
                return (null,
                    "This phone number already has a booking on this day.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                PatientName = dto.PatientName.Trim(),
                Phone = dto.Phone,
                SlotDate = date,
                SlotTime = time,
                VisitType = visitType.Code,
                BookedType = visitType.Code,
                Price = (int)visitType.Price,
                DurationMin = visitType.DurationMin,
                Status = "confirmed",
                Source = "online",
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Bookings.Add(booking);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is
                    Microsoft.Data.SqlClient.SqlException sqlEx &&
                (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return (null,
                    "A conflicting booking already exists.");
            }

            return (booking.Id, null);

        }
        public async Task<(bool Success, string? Error)>
CancelBookingAsync(Guid id, Guid staffId)
        {
            var staff = await _context.Staffs
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == staffId && s.IsActive);

            if (staff == null)
            {
                return (false, "Invalid staff account.");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return (false, "Booking not found.");
            }

            if (booking.Status == "cancelled")
            {
                return (false, "Booking is already cancelled.");
            }

            var bookingChange = new BookingChange
            {
                BookingId = booking.Id,
                Action = "cancel",
                OldType = booking.VisitType,
                OldPrice = booking.Price,
                OldSlotDate = booking.SlotDate,
                OldSlotTime = booking.SlotTime,
                Reason = "Booking cancelled by staff.",
                ChangedBy = staff.Id,
                ChangedByName = staff.Name,
                ChangedAt = DateTimeOffset.UtcNow
            };

            booking.Status = "cancelled";

            _context.BookingChanges.Add(bookingChange);

            await _context.SaveChangesAsync();

            return (true, null);
        }
        public async Task<(bool Success, string? Error)> RescheduleBookingAsync(
Guid id, RescheduleBookingDto dto , Guid staffId)
        {
            if (!dto.SlotDate.HasValue || !dto.SlotTime.HasValue)
            {
                return (false, "Date and time are required.");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return (false, "Booking not found.");
            }

            if (booking.Status == "cancelled")
            {
                return (false, "Cancelled booking cannot be rescheduled.");
            }

            var staff = await _context.Staffs
    .AsNoTracking()
    .FirstOrDefaultAsync(s => s.Id == staffId && s.IsActive);

            if (staff == null)
            {
                return (false, "Invalid staff account.");
            }

            var newDate = dto.SlotDate.Value;
            var newTime = dto.SlotTime.Value;

            if (booking.SlotDate == newDate &&
                booking.SlotTime == newTime)
            {
                return (false, "Please select a different slot.");
            }

            var availableSlots = await _availabilityService
                .GetAvailableSlotsAsync(newDate);

            if (!availableSlots.Contains(newTime))
            {
                return (false, "This slot is not available.");
            }

            var existingBooking = await _context.Bookings.AnyAsync(b =>
                b.Id != id &&
                b.Phone == booking.Phone &&
                b.SlotDate == newDate &&
                b.Status != "cancelled");

            if (existingBooking)
            {
                return (false, "This phone number already has a booking on this day.");
            }

            var change = new BookingChange
            {
                BookingId = booking.Id,
                Action = "reschedule",
                OldSlotDate = booking.SlotDate,
                OldSlotTime = booking.SlotTime,
                NewSlotDate = newDate,
                NewSlotTime = newTime,
                Reason = "Booking rescheduled by staff.",
                ChangedBy = staff.Id,
                ChangedByName = staff.Name,
                ChangedAt = DateTimeOffset.UtcNow
            };

            _context.BookingChanges.Add(change);

            booking.SlotDate = newDate;
            booking.SlotTime = newTime;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx &&
                (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return (false, "A conflicting booking already exists.");
            }

            return (true, null);
        }
        public async Task<(bool Success, string? Error)> ChangeVisitTypeAsync(
Guid id, ChangeVisitTypeDto dto , Guid staffId)
        {
            var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return (false, "Booking not found.");
            }

            if (booking.Status == "cancelled")
            {
                return (false, "Cancelled booking cannot be updated.");
            }

            var visitType = await _context.VisitTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Code == dto.VisitType);

            if (visitType == null)
            {
                return (false, "Invalid visit type.");
            }

            if (booking.VisitType == visitType.Code)
            {
                return (false, "Please select a different visit type.");
            }

            var staff = await _context.Staffs
    .AsNoTracking()
    .FirstOrDefaultAsync(s => s.Id == staffId && s.IsActive);

            if (staff == null)
            {
                return (false, "Invalid staff account.");
            }

            var oldType = booking.VisitType;
            var oldPrice = booking.Price;

            booking.VisitType = visitType.Code;
            booking.Price = (int)visitType.Price;
            booking.DurationMin = visitType.DurationMin;

            _context.BookingChanges.Add(new BookingChange
            {
                BookingId = booking.Id,
                Action = "change_type",
                OldType = oldType,
                NewType = booking.VisitType,
                OldPrice = oldPrice,
                NewPrice = booking.Price,
                Reason = "Visit type changed by staff.",
                ChangedBy = staff.Id,
                ChangedByName = staff.Name,
                ChangedAt = DateTimeOffset.UtcNow
            });

            await _context.SaveChangesAsync();

            return (true, null);
        }
        public async Task<(Guid? BookingId, string? Error)> CreateDeskBookingAsync(
CreateDeskBookingDto dto, Guid staffId)
        {
            if (!dto.SlotDate.HasValue || !dto.SlotTime.HasValue)
            {
                return (null, "Date and time are required.");
            }

            var date = dto.SlotDate.Value;
            var time = dto.SlotTime.Value;

            var staff = await _context.Staffs
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == staffId && s.IsActive);

            if (staff == null)
            {
                return (null, "Invalid staff account.");
            }

            var visitType = await _context.VisitTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Code == dto.VisitType);

            if (visitType == null)
            {
                return (null, "Invalid visit type.");
            }

            var availableSlots = await _availabilityService
                .GetAvailableSlotsAsync(date);

            if (!availableSlots.Contains(time))
            {
                return (null, "This slot is not available.");
            }

            var existingBooking = await _context.Bookings.AnyAsync(b =>
                b.Phone == dto.Phone &&
                b.SlotDate == date &&
                b.Status != "cancelled");

            if (existingBooking)
            {
                return (null, "This phone number already has a booking on this day.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                PatientName = dto.PatientName.Trim(),
                Phone = dto.Phone,
                SlotDate = date,
                SlotTime = time,
                VisitType = visitType.Code,
                BookedType = visitType.Code,
                Price = (int)visitType.Price,
                DurationMin = visitType.DurationMin,
                Status = "confirmed",
                Source = "staff",
                CreatedBy = staff.Id,
                CreatedByName = staff.Name,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Bookings.Add(booking);

            var bookingChange = new BookingChange
            {
                BookingId = booking.Id,
                Action = "create",
                NewType = booking.VisitType,
                NewPrice = booking.Price,
                NewSlotDate = booking.SlotDate,
                NewSlotTime = booking.SlotTime,
                Reason = "Booking created by staff.",
                ChangedBy = staff.Id,
                ChangedByName = staff.Name,
                ChangedAt = DateTimeOffset.UtcNow
            };

            _context.BookingChanges.Add(bookingChange);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx &&
                (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return (null, "A conflicting booking already exists.");
            }

            return (booking.Id, null);
        }
        public async Task<List<BookingHistoryDto>> GetBookingHistoryAsync(Guid bookingId)
        {
            return await _context.BookingChanges
                .AsNoTracking()
                .Where(c => c.BookingId == bookingId)
                .OrderByDescending(c => c.ChangedAt)
                .Select(c => new BookingHistoryDto
                {
                    Id = c.Id,
                    BookingId = c.BookingId,
                    Action = c.Action,
                    OldType = c.OldType,
                    NewType = c.NewType,
                    OldPrice = c.OldPrice,
                    NewPrice = c.NewPrice,
                    OldSlotDate = c.OldSlotDate,
                    NewSlotDate = c.NewSlotDate,
                    OldSlotTime = c.OldSlotTime,
                    NewSlotTime = c.NewSlotTime,
                    Reason = c.Reason,
                    ChangedBy = c.ChangedBy,
                    ChangedByName = c.ChangedByName,
                    ChangedAt = c.ChangedAt
                })
                .ToListAsync();
        }
    }
}