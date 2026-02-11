
using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface IUnitApiClient
{
    [Get("/api/units")]
    Task<List<Unit>> GetAllUnitsAsync();

    [Get("/api/units/{id}")]
    Task<Unit?> GetUnitByIdAsync(int id);

    [Post("/api/units")]
    Task<Unit> AddUnitAsync([Body] Unit unit);

    [Put("/api/units")]
    Task<Unit> UpdateUnitAsync([Body] Unit unit);

    [Delete("/api/units/{id}")]
    Task DeleteUnitAsync(int id);
}
