using ClinicSystem.API.Data;
using ClinicSystem.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.API.Services
{
    public class VisitTypeService
    {
        private readonly AppDbContext _context;

        public VisitTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VisitTypeDto>> GetVisitTypesAsync()
        {
            return await _context.VisitTypes
                .AsNoTracking()
                .Where(v => v.PatientCanBook)
                .OrderBy(v => v.Sort)
                .Select(v => new VisitTypeDto
                {
                    Code = v.Code,
                    NameAr = v.NameAr,
                    DurationMin = v.DurationMin,
                    Price = v.Price
                })
                .ToListAsync();
        }
    }
}