using ERPGOAPPLICATION.DTOs;
using ERPGOAPPLICATION.Interfaces;

namespace ERPGOINFRASTRUCTURE.Services;

public class AuthService : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // TODO: Implement actual logic (Check DB, Hash password, etc.)
        // valid user: admin/admin for basic test
        
        await Task.Delay(100); // Simulate DB call

        if (request.Email == "admin" && request.Password == "admin")
        {
            return new LoginResponse
            {
                IsSuccess = true,
                Token = "fake-jwt-token-for-direct-mode",
                Message = "Login Successful (Direct)"
            };
        }

        return new LoginResponse
        {
            IsSuccess = false,
            Message = "Invalid Credentials"
        };
    }
}
