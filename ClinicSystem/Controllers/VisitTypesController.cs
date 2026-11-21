using ClinicSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/visit-types")]
    public class VisitTypesController : ControllerBase
    {
        private readonly VisitTypeService _visitTypeService;

        public VisitTypesController(VisitTypeService visitTypeService)
        {
            _visitTypeService = visitTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVisitTypes()
        {
            var visitTypes = await _visitTypeService.GetVisitTypesAsync();

            return Ok(visitTypes);
        }
    }
}