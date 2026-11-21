using ClinicSystem.API.Data;
using ClinicSystem.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.API.Development
{
    public static class StaffSeeder
    {
        public static async Task SeedAsync(
        AppDbContext context,
        IConfiguration configuration)
        {
            var email = configuration["TestStaff:Email"];
            var name = configuration["TestStaff:Name"];
            var password = configuration["TestStaff:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "TestStaff settings are missing.");
            }

            var exists = await context.Staffs
                .AnyAsync(s => s.Email == email);

            if (exists)
                return;

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var passwordHasher = new PasswordHasher<Staff>();

            staff.PasswordHash = passwordHasher.HashPassword(
                staff, password);

            context.Staffs.Add(staff);
            await context.SaveChangesAsync();
        }
    }
}