using System.Data;
using WIRS.DataAccess.Interfaces;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Services.Implementations
{
    public class MasterDataService : IMasterDataService
    {
        private readonly ICommonFunDataAccess _commonFunDataAccess;
        private readonly IUserCredentialsDataAccess _userCredentialsDataAccess;

        public MasterDataService(ICommonFunDataAccess commonFunDataAccess, IUserCredentialsDataAccess userCredentialsDataAccess)
        {
            _commonFunDataAccess = commonFunDataAccess;
            _userCredentialsDataAccess = userCredentialsDataAccess;
        }

        public async Task<List<LookupItem>> GetUserRoles()
        {
            try
            {
                var result = new List<LookupItem>();
                var dataSet = await _userCredentialsDataAccess.GetAllUserRole();

                if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        result.Add(new LookupItem
                        {
                            Code = row["user_role_code"]?.ToString() ?? string.Empty,
                            Value = row["user_role_name"]?.ToString() ?? string.Empty
                        });
                    }
                }

                return result;
            }
            catch
            {
                return new List<LookupItem>();
            }
        }

        public async Task<List<LookupItem>> GetSectors()
        {
            try
            {
                var result = new List<LookupItem>();
                var dataSet = await _commonFunDataAccess.GetLookUpType("SBA");

                if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        result.Add(new LookupItem
                        {
                            Code = row["lookup_code"]?.ToString() ?? string.Empty,
                            Value = row["lookup_value"]?.ToString() ?? string.Empty,
                            Description = row["lookup_desc"]?.ToString()
                        });
                    }
                }

                return result;
            }
            catch
            {
                return new List<LookupItem>();
            }
        }

        public async Task<List<LookupItem>> GetLOBsBySector(string sectorCode)
        {
            try
            {
                var result = new List<LookupItem>();
                var dataSet = await _commonFunDataAccess.Get_sbu_by_uid(sectorCode, string.Empty);

                if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        result.Add(new LookupItem
                        {
                            Code = row["sbu_code"]?.ToString() ?? string.Empty,
                            Value = row["sbu_name"]?.ToString() ?? string.Empty
                        });
                    }
                }

                return result;
            }
            catch
            {
                return new List<LookupItem>();
            }
        }

        public async Task<List<LookupItem>> GetDepartmentsByLOB(string sectorCode, string lobCode)
        {
            try
            {
                var result = new List<LookupItem>();
                var dataSet = await _commonFunDataAccess.get_active_departments(string.Empty, sectorCode, lobCode);

                if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        result.Add(new LookupItem
                        {
                            Code = row["department_code"]?.ToString() ?? string.Empty,
                            Value = row["department_name"]?.ToString() ?? string.Empty
                        });
                    }
                }

                return result;
            }
            catch
            {
                return new List<LookupItem>();
            }
        }

        public async Task<List<LookupItem>> GetLocations()
        {
            try
            {
                var result = new List<LookupItem>();
                var dataSet = await _commonFunDataAccess.GetLookUpType("LOCATION");

                if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        result.Add(new LookupItem
                        {
                            Code = row["lookup_code"]?.ToString() ?? string.Empty,
                            Value = row["lookup_value"]?.ToString() ?? string.Empty
                        });
                    }
                }

                return result;
            }
            catch
            {
                return new List<LookupItem>();
            }
        }

        public async Task<List<LookupItem>> GetAccountStatuses()
        {
            try
            {
                var result = new List<LookupItem>();
                var dataSet = await _commonFunDataAccess.GetLookUpType("Account Status");

                if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        result.Add(new LookupItem
                        {
                            Code = row["lookup_code"]?.ToString() ?? string.Empty,
                            Value = row["lookup_value"]?.ToString() ?? string.Empty
                        });
                    }
                }

                return result;
            }
            catch
            {
                return new List<LookupItem>();
            }
        }
    }
}