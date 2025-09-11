using WIRS.Services.Models;

namespace WIRS.Services.Interfaces
{
    public interface IMasterDataService
    {
        Task<List<LookupItem>> GetUserRoles();
        Task<List<LookupItem>> GetSectors();
        Task<List<LookupItem>> GetLOBsBySector(string sectorCode);
        Task<List<LookupItem>> GetDepartmentsByLOB(string sectorCode, string lobCode);
        Task<List<LookupItem>> GetLocations();
        Task<List<LookupItem>> GetAccountStatuses();
    }
}