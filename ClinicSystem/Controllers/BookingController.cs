using ClinicSystem.API.DTOs;
using ClinicSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize(Roles = "Staff")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingsController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var staffIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(staffIdClaim, out var staffId))
            {
                return Unauthorized(new
                {
                    message = "Invalid staff identity."
                });
            }

            var result = await _bookingService
                .CancelBookingAsync(id, staffId);

            if (!result.Success)
            {
                if (result.Error == "Booking not found.")
                    return NotFound(new { message = result.Error });

                return BadRequest(new { message = result.Error });
            }

            return Ok(new
            {
                message = "Booking cancelled successfully."
            });
        }
        [HttpPatch("{id:guid}/reschedule")]
        public async Task<IActionResult> RescheduleBooking(
Guid id, [FromBody] RescheduleBookingDto dto)
        {
            var staffIdClaim = User.FindFirstValue(
     ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(staffIdClaim, out var staffId))
            {
                return Unauthorized(new
                {
                    message = "Invalid staff identity."
                });
            }

            var result = await _bookingService
                .RescheduleBookingAsync(id, dto, staffId);

            if (!result.Success)
            {
                if (result.Error == "Booking not found.")
                {
                    return NotFound(new { message = result.Error });
                }

                return BadRequest(new { message = result.Error });
            }

            return Ok(new
            {
                message = "Booking rescheduled successfully."
            });
        }

        [HttpPatch("{id:guid}/visit-type")]
        public async Task<IActionResult> ChangeVisitType(
Guid id, [FromBody] ChangeVisitTypeDto dto)
        {
            var staffIdClaim = User.FindFirstValue(
    ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(staffIdClaim, out var staffId))
            {
                return Unauthorized(new
                {
                    message = "Invalid staff identity."
                });
            }
            var result = await _bookingService
            .ChangeVisitTypeAsync(id, dto,staffId);

            if (!result.Success)
            {
                if (result.Error == "Booking not found.")
                {
                    return NotFound(new { message = result.Error });
                }

                return BadRequest(new { message = result.Error });
            }

            return Ok(new
            {
                message = "Visit type changed successfully."
            });
        }
        [HttpPost("desk")]
        public async Task<IActionResult> CreateDeskBooking(
[FromBody] CreateDeskBookingDto dto)
        {
            var staffIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(staffIdClaim, out var staffId))
            {
                return Unauthorized(new
                {
                    message = "Invalid staff identity."
                });
            }

            var result = await _bookingService
                .CreateDeskBookingAsync(dto, staffId);

            if (!result.BookingId.HasValue)
            {
                return BadRequest(new
                {
                    message = result.Error
                });
            }

            return StatusCode(201, new
            {
                bookingId = result.BookingId,
                message = "Desk booking created successfully."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();

            return Ok(bookings);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateBooking(
            [FromBody] CreateBookingDto dto)
        {
            var result = await _bookingService.CreateBookingAsync(dto);

            if (result.Error != null)
                return BadRequest(new { message = result.Error });

            return StatusCode(201, new
            {
                bookingId = result.BookingId,
                message = "Booking created successfully"
            });
        }
    }
}