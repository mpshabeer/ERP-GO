
using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

public class UnitApiService : IUnitService
{
    private readonly IUnitApiClient _apiClient;

    public UnitApiService(IUnitApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<List<Unit>> GetAllUnitsAsync() => _apiClient.GetAllUnitsAsync();
    public Task<Unit?> GetUnitByIdAsync(int id) => _apiClient.GetUnitByIdAsync(id);
    public Task<Unit> AddUnitAsync(Unit unit) => _apiClient.AddUnitAsync(unit);
    public Task<Unit> UpdateUnitAsync(Unit unit) => _apiClient.UpdateUnitAsync(unit);
    public Task DeleteUnitAsync(int id) => _apiClient.DeleteUnitAsync(id);
}
