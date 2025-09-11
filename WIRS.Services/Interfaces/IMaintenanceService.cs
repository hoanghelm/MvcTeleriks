using WIRS.Services.Models;
using System.Threading.Tasks;

namespace WIRS.Services.Interfaces
{
    public interface IMaintenanceService
    {
        // LOB Methods
        Task<IEnumerable<LOBModel>> GetLOBListAsync(string sbaCode);
        Task<LOBModel> GetLOBByUidAsync(string sbaCode, string sbuCode);
        Task<ServiceResult> SaveLOBAsync(LOBModel model, string modifiedBy);
        Task<string> GenerateLOBCodeAsync(string sbaCode);

        // Location Methods
        Task<IEnumerable<LocationModel>> GetLocationListAsync(string sbaCode, string sbuCode, string departmentCode);
        Task<LocationModel> GetLocationByUidAsync(string sbaCode, string sbuCode, string departmentCode, string locationCode);
        Task<ServiceResult> SaveLocationAsync(LocationModel model, string modifiedBy);
        Task<string> GenerateLocationCodeAsync(string sbuCode);

        // Department Methods
        Task<IEnumerable<DepartmentModel>> GetDepartmentListAsync(string sbaCode, string sbuCode, string departmentName = "");
        Task<DepartmentModel> GetDepartmentByUidAsync(string codeType, string sbaCode, string sbuCode, string departmentCode);
        Task<ServiceResult> SaveDepartmentAsync(DepartmentModel model, string modifiedBy);
        Task<string> GenerateDepartmentCodeAsync(string sbuCode);

        // CopyTo List Methods
        Task<IEnumerable<CopyToListModel>> GetCopyToListAsync(string sbaCode, string sbuCode, string departmentCode, string locationCode);
        Task<CopyToListModel> GetCopyToListByUidAsync(string sbaCode, string sbuCode, string departmentCode, string locationCode, string userId);
        Task<ServiceResult> SaveCopyToListAsync(CopyToListModel model, string modifiedBy);
    }
}