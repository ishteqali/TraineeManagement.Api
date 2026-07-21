using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Shared.Data;
using TraineeManagement.Shared.Models;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(existingUser => existingUser.Username == request.Username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }
            int expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes");
            string tokenString = GenerateJwtToken(user, expiryMinutes);
            return new LoginResponse
            {
                Token = tokenString,
                ExpiresIn = expiryMinutes * 60,
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }

        public string GenerateJwtToken(User user, int expiryMinutes)
        {
            string jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing from configuration.");
            SymmetricSecurityKey? securityKey = new(System.Text.Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials? credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[]? claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            JwtSecurityToken? token = new(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryMinutes")),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}