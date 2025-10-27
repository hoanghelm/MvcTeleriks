using WIRS.Services.Interfaces;
using WIRS.Services.Models;
using WIRS.DataAccess.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace WIRS.Services.Implementations
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ICommonFunDataAccess _commonFunDataAccess;
        private readonly IUserDataAccess _userDataAccess;

        public MaintenanceService(ICommonFunDataAccess commonFunDataAccess, IUserDataAccess userDataAccess)
        {
            _commonFunDataAccess = commonFunDataAccess;
            _userDataAccess = userDataAccess;
        }

        public async Task<IEnumerable<LOBModel>> GetLOBListAsync(string sbaCode)
        {
            var dataSet = await _commonFunDataAccess.Setup_all_sbus(sbaCode);
            var lobList = new List<LOBModel>();

            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    lobList.Add(new LOBModel
                    {
                        SbaCode = row["sba_code"]?.ToString(),
                        SbaName = row["sba_name"]?.ToString(),
                        SbuCode = row["sbu_code"]?.ToString(),
                        SbuName = row["sbu_name"]?.ToString(),
                        InactiveDate = row["inactive_date"]?.ToString(),
                        Uid = row["uid"]?.ToString()
                    });
                }
            }

            return lobList;
        }

        public async Task<LOBModel> GetLOBByUidAsync(string sbaCode, string sbuCode)
        {
            var dataSet = await _commonFunDataAccess.Get_sbu_by_uid(sbaCode, sbuCode);
            
            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                return new LOBModel
                {
                    SbaCode = row["sba_code"]?.ToString(),
                    SbuCode = row["sbu_code"]?.ToString(),
                    SbuName = row["sbu_name"]?.ToString(),
                    InactiveDate = row["inactive_date"]?.ToString(),
                    Uid = row["uid"]?.ToString()
                };
            }

            return null;
        }

        public async Task<ServiceResult> SaveLOBAsync(LOBModel model, string modifiedBy)
        {
            try
            {
                // Check if record exists
                var existingRecord = await GetLOBByUidAsync(model.SbaCode, model.SbuCode);
                string errorCode;

                if (existingRecord != null)
                {
                    // Update existing record
                    errorCode = await _commonFunDataAccess.Update_sbu(
                        model.SbaCode, 
                        model.SbuCode, 
                        model.SbuName, 
                        model.InactiveDate, 
                        modifiedBy);
                }
                else
                {
                    // Insert new record
                    errorCode = await _commonFunDataAccess.Insert_sbu(
                        model.SbaCode, 
                        model.SbuCode, 
                        model.SbuName, 
                        model.InactiveDate, 
                        modifiedBy);
                }

                if (!string.IsNullOrEmpty(errorCode))
                {
                    // You might want to implement error message lookup similar to the original code
                    return new ServiceResult
                    {
                        Success = false,
                        ErrorCode = errorCode,
                        ErrorMessage = $"Error: {errorCode}"
                    };
                }

                return new ServiceResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<string> GenerateLOBCodeAsync(string sbaCode)
        {
            var dataSet = await _commonFunDataAccess.generate_sbu_code(sbaCode);
            
            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                return dataSet.Tables[0].Rows[0][0]?.ToString();
            }

            // Fallback to default generation if needed
            return $"{sbaCode}001";
        }

        // Location Methods
        public async Task<IEnumerable<LocationModel>> GetLocationListAsync(string sbaCode, string sbuCode, string departmentCode)
        {
            var dataSet = await _commonFunDataAccess.get_all_locations(sbaCode, sbuCode, departmentCode);
            var locationList = new List<LocationModel>();

            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    locationList.Add(new LocationModel
                    {
                        SbaCode = row["sba_code"]?.ToString(),
                        SbaName = row["sba_name"]?.ToString(),
                        SbuCode = row["sbu_code"]?.ToString(),
                        SbuName = row["sbu_name"]?.ToString(),
                        DepartmentCode = row["department_code"]?.ToString(),
                        DepartmentName = row["department_name"]?.ToString(),
                        LocationCode = row["location_code"]?.ToString(),
                        LocationName = row["location_name"]?.ToString(),
                        InactiveDate = row["inactive_date"]?.ToString(),
                        Uid = row["uid"]?.ToString()
                    });
                }
            }

            return locationList;
        }

        public async Task<LocationModel> GetLocationByUidAsync(string sbaCode, string sbuCode, string departmentCode, string locationCode)
        {
            var dataSet = await _commonFunDataAccess.Get_location_by_uid(sbaCode, sbuCode, departmentCode, locationCode);
            
            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                return new LocationModel
                {
                    SbaCode = row["sba_code"]?.ToString(),
                    SbaName = row["sba_name"]?.ToString(),
                    SbuCode = row["sbu_code"]?.ToString(),
                    SbuName = row["sbu_name"]?.ToString(),
                    DepartmentCode = row["department_code"]?.ToString(),
                    DepartmentName = row["department_name"]?.ToString(),
                    LocationCode = row["location_code"]?.ToString(),
                    LocationName = row["location_name"]?.ToString(),
                    InactiveDate = row["inactive_date"]?.ToString(),
                    Uid = row["uid"]?.ToString()
                };
            }

            return null;
        }

        public async Task<ServiceResult> SaveLocationAsync(LocationModel model, string modifiedBy)
        {
            try
            {
                // Check if record exists
                var existingRecord = await GetLocationByUidAsync(model.SbaCode, model.SbuCode, model.DepartmentCode, model.LocationCode);
                string errorCode;

                if (existingRecord != null)
                {
                    // Update existing record
                    errorCode = await _commonFunDataAccess.Update_location(
                        model.SbaCode, 
                        model.SbuCode, 
                        model.DepartmentCode,
                        model.LocationCode, 
                        model.LocationName, 
                        model.InactiveDate ?? string.Empty, 
                        modifiedBy);
                }
                else
                {
                    // Insert new record
                    errorCode = await _commonFunDataAccess.Insert_location(
                        model.SbaCode, 
                        model.SbuCode, 
                        model.DepartmentCode,
                        model.LocationCode, 
                        model.LocationName, 
                        model.InactiveDate ?? string.Empty, 
                        modifiedBy);
                }

                if (!string.IsNullOrEmpty(errorCode))
                {
                    return new ServiceResult
                    {
                        Success = false,
                        ErrorCode = errorCode,
                        ErrorMessage = $"Error: {errorCode}"
                    };
                }

                return new ServiceResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<string> GenerateLocationCodeAsync(string sbuCode)
        {
            var dataSet = await _commonFunDataAccess.generate_location_code(sbuCode);
            
            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                return dataSet.Tables[0].Rows[0][0]?.ToString();
            }

            // Fallback to default generation if needed
            return $"{sbuCode}L001";
        }

        // Department Methods
        public async Task<IEnumerable<DepartmentModel>> GetDepartmentListAsync(string sbaCode, string sbuCode, string departmentName = "")
        {
            var dataSet = await _commonFunDataAccess.search_departments(string.Empty, sbaCode, sbuCode, departmentName);
            var departmentList = new List<DepartmentModel>();

            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    departmentList.Add(new DepartmentModel
                    {
                        SbaCode = row["sba_code"]?.ToString(),
                        SbaName = row["sba_name"]?.ToString(),
                        SbuCode = row["sbu_code"]?.ToString(),
                        SbuName = row["sbu_name"]?.ToString(),
                        DepartmentCode = row["department_code"]?.ToString(),
                        DepartmentName = row["department_name"]?.ToString(),
                        InactiveDate = row["inactive_date"]?.ToString(),
                        Uid = row["uid"]?.ToString(),
                        CodeType = row["code_type"]?.ToString()
                    });
                }
            }

            return departmentList;
        }

        public async Task<DepartmentModel> GetDepartmentByUidAsync(string codeType, string sbaCode, string sbuCode, string departmentCode)
        {
            var dataSet = await _commonFunDataAccess.get_department_by_uid(codeType, sbaCode, sbuCode, departmentCode);
            
            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                return new DepartmentModel
                {
                    SbaCode = row["sba_code"]?.ToString(),
                    SbaName = row["sba_name"]?.ToString(),
                    SbuCode = row["sbu_code"]?.ToString(),
                    SbuName = row["sbu_name"]?.ToString(),
                    DepartmentCode = row["department_code"]?.ToString(),
                    DepartmentName = row["department_name"]?.ToString(),
                    InactiveDate = row["inactive_date"]?.ToString(),
                    Uid = row["uid"]?.ToString(),
                    CodeType = row["code_type"]?.ToString()
                };
            }

            return null;
        }

        public async Task<ServiceResult> SaveDepartmentAsync(DepartmentModel model, string modifiedBy)
        {
            try
            {
                // Check if record exists
                var existingRecord = await GetDepartmentByUidAsync(model.CodeType ?? string.Empty, model.SbaCode, model.SbuCode, model.DepartmentCode);
                string errorCode;

                if (existingRecord != null)
                {
                    // Update existing record
                    errorCode = await _commonFunDataAccess.update_department(
                        model.CodeType ?? string.Empty,
                        model.SbaCode, 
                        model.SbuCode, 
                        model.DepartmentCode, 
                        model.DepartmentName, 
                        model.InactiveDate ?? string.Empty, 
                        modifiedBy);
                }
                else
                {
                    // Insert new record
                    errorCode = await _commonFunDataAccess.insert_department(
                        model.CodeType ?? string.Empty,
                        model.SbaCode, 
                        model.SbuCode, 
                        model.DepartmentCode, 
                        model.DepartmentName, 
                        model.InactiveDate ?? string.Empty, 
                        modifiedBy);
                }

                if (!string.IsNullOrEmpty(errorCode))
                {
                    return new ServiceResult
                    {
                        Success = false,
                        ErrorCode = errorCode,
                        ErrorMessage = $"Error: {errorCode}"
                    };
                }

                return new ServiceResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<string> GenerateDepartmentCodeAsync(string sbuCode)
        {
            var dataSet = await _commonFunDataAccess.generate_department_code(sbuCode);
            
            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                return dataSet.Tables[0].Rows[0][0]?.ToString();
            }

            // Fallback to default generation if needed
            return $"{sbuCode}D001";
        }

        // CopyTo List Methods
        public async Task<IEnumerable<CopyToListModel>> GetCopyToListAsync(string sbaCode, string sbuCode, string departmentCode, string locationCode)
        {
            var dataSet = await _userDataAccess.get_all_copyto_list_by_sbu(sbaCode, sbuCode, departmentCode, locationCode);
            var copyToList = new List<CopyToListModel>();

            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    copyToList.Add(new CopyToListModel
                    {
                        SbaCode = row["sba_code"]?.ToString() ?? string.Empty,
                        SbaName = row["sba_name"]?.ToString(),
                        SbuCode = row["sbu_code"]?.ToString() ?? string.Empty,
                        SbuName = row["sbu_name"]?.ToString(),
                        DepartmentCode = row["department_code"]?.ToString() ?? string.Empty,
                        DepartmentName = row["department_name"]?.ToString(),
                        LocationCode = row["location_code"]?.ToString() ?? string.Empty,
                        LocationName = row["location_name"]?.ToString(),
                        UserId = row["user_id"]?.ToString() ?? string.Empty,
                        UserName = row["user_name"]?.ToString() ?? string.Empty,
                        InactiveDate = row["inactive_date"]?.ToString(),
                        Uid = row["uid"]?.ToString()
                    });
                }
            }

            return copyToList;
        }

        public async Task<CopyToListModel> GetCopyToListByUidAsync(string sbaCode, string sbuCode, string departmentCode, string locationCode, string userId)
        {
            var dataSet = await _userDataAccess.get_all_copyto_list_by_uid(sbaCode, sbuCode, departmentCode, locationCode, userId);
            
            if (dataSet?.Tables?[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                return new CopyToListModel
                {
                    SbaCode = row["sba_code"]?.ToString() ?? string.Empty,
                    SbaName = row["sba_name"]?.ToString(),
                    SbuCode = row["sbu_code"]?.ToString() ?? string.Empty,
                    SbuName = row["sbu_name"]?.ToString(),
                    DepartmentCode = row["department_code"]?.ToString() ?? string.Empty,
                    DepartmentName = row["department_name"]?.ToString(),
                    LocationCode = row["location_code"]?.ToString() ?? string.Empty,
                    LocationName = row["location_name"]?.ToString(),
                    UserId = row["user_id"]?.ToString() ?? string.Empty,
                    UserName = row["user_name"]?.ToString() ?? string.Empty,
                    InactiveDate = row["inactive_date"]?.ToString(),
                    Uid = row["uid"]?.ToString()
                };
            }

            return null;
        }

        public async Task<ServiceResult> SaveCopyToListAsync(CopyToListModel model, string modifiedBy)
        {
            try
            {
                // Check if record exists first
                var existingRecord = await GetCopyToListByUidAsync(model.SbaCode, model.SbuCode, model.DepartmentCode, model.LocationCode, model.UserId);
                
                string errorCode;
                
                if (existingRecord == null)
                {
                    // Insert new record
                    errorCode = await _userDataAccess.insert_copyto_list(
                        model.SbaCode,
                        model.SbuCode,
                        model.DepartmentCode,
                        model.LocationCode,
                        model.UserId,
                        model.InactiveDate ?? string.Empty,
                        modifiedBy
                    );
                }
                else
                {
                    // Update existing record
                    errorCode = await _userDataAccess.update_copyto_list(
                        model.SbaCode,
                        model.SbuCode,
                        model.DepartmentCode,
                        model.LocationCode,
                        model.UserId,
                        model.InactiveDate ?? string.Empty,
                        modifiedBy
                    );
                }

                if (!string.IsNullOrEmpty(errorCode))
                {
                    return new ServiceResult
                    {
                        Success = false,
                        ErrorCode = errorCode,
                        ErrorMessage = $"Error saving copy-to list: {errorCode}"
                    };
                }

                return new ServiceResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}