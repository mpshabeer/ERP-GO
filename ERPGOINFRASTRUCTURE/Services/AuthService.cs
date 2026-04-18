using ERPGOAPPLICATION.DTOs;
using ERPGOAPPLICATION.Interfaces;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERPGOINFRASTRUCTURE.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Email || u.Email == request.Email);
        
        if (user == null || !user.IsActive)
        {
            return new LoginResponse { IsSuccess = false, Message = "Invalid Credentials or Inactive user" };
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        
        if (!isPasswordValid)
        {
            return new LoginResponse { IsSuccess = false, Message = "Invalid Credentials" };
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new LoginResponse
        {
            IsSuccess = true,
            Token = token,
            Message = "Login Successful",
            UserId = user.Id,
            FullName = user.FullName,
            Username = user.Username
        };
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if(user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    private string GenerateJwtToken(ERPGODomain.Entities.User user)
    {
        var securityKeyVal = _configuration["Jwt:Key"] ?? "super_secret_fallback_key_that_is_at_least_32_bytes_long_123456$";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyVal));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "ERPGOAPI",
            audience: _configuration["Jwt:Audience"] ?? "ERPGOClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
