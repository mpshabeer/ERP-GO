using ERPGOAPPLICATION.DTOs;
using ERPGOAPPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERPGOAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", async ([FromBody] LoginRequest request, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            if (result.IsSuccess)
                return Results.Ok(result);
            return Results.BadRequest(result);
        })
        .WithName("Login");

        group.MapPost("/change-password", async ([FromBody] ChangePasswordRequest request, IAuthService authService) =>
        {
            var success = await authService.ChangePasswordAsync(request);
            if (success) return Results.Ok();
            return Results.BadRequest("Invalid current password or user not found.");
        });
    }
}
