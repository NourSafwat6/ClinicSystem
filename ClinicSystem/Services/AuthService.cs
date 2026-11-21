using ClinicSystem.API.Data;
using ClinicSystem.API.Entities;
using ClinicSystem.API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ClinicSystem.API.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Staff> _passwordHasher;

        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<Staff>();
        }


        public async Task<Staff?> ValidateStaffAsync(LoginRequestDto request)
        {
            var staff = await _context.Staffs
                .FirstOrDefaultAsync(s => s.Email == request.Email);

            if (staff == null || !staff.IsActive)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                staff,
                staff.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
                return null;

            return staff;
        }
        private string GenerateJwtToken(Staff staff)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT Secret Key is missing.");

            var issuer = jwtSettings["Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer is missing.");

            var audience = jwtSettings["Audience"]
                ?? throw new InvalidOperationException("JWT Audience is missing.");

            var expiryMinutes = int.Parse(
                jwtSettings["ExpiryMinutes"] ?? "60"
            );

            var claims = new[]
            {
    new Claim(JwtRegisteredClaimNames.Sub, staff.Id.ToString()),
    new Claim(JwtRegisteredClaimNames.Email, staff.Email),
    new Claim(ClaimTypes.Name, staff.Name),
    new Claim(ClaimTypes.Role, "Staff")
};

            var key = new SymmetricSecurityKey(
                Convert.FromBase64String(secretKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var staff = await ValidateStaffAsync(request);

            if (staff == null)
                return null;

            var token = GenerateJwtToken(staff);

            return new LoginResponseDto
            {
                Token = token,
                Name = staff.Name,
                Email = staff.Email
            };
        }
    }
}