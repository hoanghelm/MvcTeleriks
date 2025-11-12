using System.Data;
using Microsoft.Extensions.Caching.Memory;
using WIRS.DataAccess.Interfaces;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Services.Implementations
{
    public class MasterDataService : IMasterDataService
    {
        private readonly ICommonFunDataAccess _commonFunDataAccess;
        private readonly IUserCredentialsDataAccess _userCredentialsDataAccess;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);

        public MasterDataService(ICommonFunDataAccess commonFunDataAccess, IUserCredentialsDataAccess userCredentialsDataAccess, IMemoryCache cache)
        {
            _commonFunDataAccess = commonFunDataAccess;
            _userCredentialsDataAccess = userCredentialsDataAccess;
            _cache = cache;
        }

        public async Task<List<LookupItem>> GetUserRoles()
        {
            return await _cache.GetOrCreateAsync("UserRoles", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
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
            }) ?? new List<LookupItem>();
        }

        public async Task<List<LookupItem>> GetSectors()
        {
            return await _cache.GetOrCreateAsync("Lookup_SBA", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
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
                                Description = row["lookup_value"]?.ToString() ?? string.Empty,
                            });
                        }
                    }

                    return result;
                }
                catch
                {
                    return new List<LookupItem>();
                }
            }) ?? new List<LookupItem>();
        }

        public async Task<List<LookupItem>> GetLOBsBySector(string sectorCode)
        {
            var cacheKey = $"LOBs_{sectorCode}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
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
            }) ?? new List<LookupItem>();
        }

        public async Task<List<LookupItem>> GetDepartmentsByLOB(string sectorCode, string lobCode)
        {
            var cacheKey = $"Departments_{sectorCode}_{lobCode}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
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
            }) ?? new List<LookupItem>();
        }

        public async Task<List<LookupItem>> GetLocations()
        {
            return await _cache.GetOrCreateAsync("Lookup_LOCATION", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
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
            }) ?? new List<LookupItem>();
        }

        public async Task<List<LookupItem>> GetLocations(string sectorCode, string lobCode, string deptCode)
        {
            var cacheKey = $"Locations_{sectorCode}_{lobCode}_{deptCode}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
                try
                {
                    var result = new List<LookupItem>();
                    var dataSet = await _commonFunDataAccess.get_active_locations(sectorCode, lobCode, deptCode);

                    if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            result.Add(new LookupItem
                            {
                                Code = row["location_code"]?.ToString() ?? string.Empty,
                                Value = row["location_name"]?.ToString() ?? string.Empty
                            });
                        }
                    }

                    return result;
                }
                catch
                {
                    return new List<LookupItem>();
                }
            }) ?? new List<LookupItem>();
        }

        public async Task<List<LookupItem>> GetAccountStatuses()
        {
            return await _cache.GetOrCreateAsync("Lookup_Account Status", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
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
            }) ?? new List<LookupItem>();
        }

        public async Task<List<LookupItem>> GetLookup(string type)
        {
            var cacheKey = $"Lookup_{type}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
                try
                {
                    var result = new List<LookupItem>();
                    var dataSet = await _commonFunDataAccess.GetLookUpType(type);

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
            }) ?? new List<LookupItem>();
        }
    }
}