using ERPGOAPPLICATION.DTOs;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface IAuthApiClient
{
    [Post("/api/auth/login")]
    Task<LoginResponse> LoginAsync([Body] LoginRequest request);

    [Post("/api/auth/change-password")]
    Task<bool> ChangePasswordAsync([Body] ChangePasswordRequest request);
}
