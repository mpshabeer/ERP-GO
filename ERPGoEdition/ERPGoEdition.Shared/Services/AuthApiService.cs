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

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            // Normally this goes to _apiClient.ChangePasswordAsync(request);
            // Updating Refit Client is next if necessary. For now, assuming Refit is direct or returning true.
            return await _apiClient.ChangePasswordAsync(request);
        }
        catch
        {
            return false;
        }
    }
}
