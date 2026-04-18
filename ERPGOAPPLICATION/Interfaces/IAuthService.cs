using ERPGOAPPLICATION.DTOs;

namespace ERPGOAPPLICATION.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
}
