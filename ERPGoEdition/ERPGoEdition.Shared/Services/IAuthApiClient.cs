using ERPGOAPPLICATION.DTOs;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface IAuthApiClient
{
    [Post("/api/auth/login")]
    Task<LoginResponse> LoginAsync([Body] LoginRequest request);
}
