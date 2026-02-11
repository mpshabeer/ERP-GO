
using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IUnitService
{
    Task<List<Unit>> GetAllUnitsAsync();
    Task<Unit?> GetUnitByIdAsync(int id);
    Task<Unit> AddUnitAsync(Unit unit);
    Task<Unit> UpdateUnitAsync(Unit unit);
    Task DeleteUnitAsync(int id);
}
