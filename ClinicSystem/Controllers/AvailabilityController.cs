using ClinicSystem.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/availability")]
    public class AvailabilityController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;

        public AvailabilityController(
            AvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [AllowAnonymous]
        [HttpGet("days")]
        public async Task<IActionResult> GetAvailableDays()
        {
            var days = await _availabilityService
            .GetAvailableDaysAsync();

            return Ok(days);
        }

        [AllowAnonymous]
        [HttpGet("slots")]
        public async Task<IActionResult> GetAvailableSlots(
            [FromQuery] DateOnly date)
        {
            var slots = await _availabilityService
                .GetAvailableSlotsAsync(date);

            return Ok(slots);
        }
    }
}