using ERPGOAPPLICATION.DTOs;
using ERPGOAPPLICATION.Interfaces;

namespace ERPGoEdition.Shared.Services;

public class AuthApiService : IAuthService
{
    private readonly IAuthApiClient _apiClient;

    public AuthApiService(IAuthApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try 
        {
            return await _apiClient.LoginAsync(request);
        }
        catch (Exception ex)
        {
            return new LoginResponse 
            { 
                IsSuccess = false, 
                Message = $"API Error: {ex.Message}" 
            };
        }
    }
}
