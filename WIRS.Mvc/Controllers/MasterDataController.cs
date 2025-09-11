using Microsoft.AspNetCore.Mvc;
using WIRS.Services.Auth;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Mvc.Controllers
{
    public class MasterDataController : BaseController
    {
        private readonly IMasterDataService _masterDataService;

        public MasterDataController(IAuthService authService, IMasterDataService masterDataService) : base(authService)
        {
            _masterDataService = masterDataService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {
            try
            {
                var roles = await _masterDataService.GetUserRoles();
                return Json(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving user roles", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSectors()
        {
            try
            {
                var sectors = await _masterDataService.GetSectors();
                return Json(new { success = true, data = sectors });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving sectors", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLOBs(string sectorCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sectorCode))
                {
                    return Json(new { success = false, message = "Sector code is required" });
                }

                var lobs = await _masterDataService.GetLOBsBySector(sectorCode);
                return Json(new { success = true, data = lobs });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving LOBs", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments(string sectorCode, string lobCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sectorCode) || string.IsNullOrWhiteSpace(lobCode))
                {
                    return Json(new { success = false, message = "Both sector and LOB codes are required" });
                }

                var departments = await _masterDataService.GetDepartmentsByLOB(sectorCode, lobCode);
                return Json(new { success = true, data = departments });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving departments", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            try
            {
                var locations = await _masterDataService.GetLocations();
                return Json(new { success = true, data = locations });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving locations", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountStatuses()
        {
            try
            {
                var statuses = await _masterDataService.GetAccountStatuses();
                return Json(new { success = true, data = statuses });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving account statuses", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetMasterData([FromBody] MasterDataRequest request)
        {
            try
            {
                var result = new MasterDataResponse();

                if (request.IncludeUserRoles)
                {
                    result.UserRoles = await _masterDataService.GetUserRoles();
                }

                if (request.IncludeSectors)
                {
                    result.Sectors = await _masterDataService.GetSectors();
                }

                if (request.IncludeLocations)
                {
                    result.Locations = await _masterDataService.GetLocations();
                }

                if (!string.IsNullOrWhiteSpace(request.SectorCode))
                {
                    result.LOBs = await _masterDataService.GetLOBsBySector(request.SectorCode);

                    if (!string.IsNullOrWhiteSpace(request.LOBCode))
                    {
                        result.Departments = await _masterDataService.GetDepartmentsByLOB(request.SectorCode, request.LOBCode);
                    }
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving master data", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetYesNoOptions()
        {
            var options = new List<LookupItem>
            {
                new LookupItem { Code = "1", Value = "Yes" },
                new LookupItem { Code = "0", Value = "No" }
            };
            return Json(options);
        }

        [HttpGet]
        public IActionResult GetOvertimeOptions()
        {
            var options = new List<LookupItem>
            {
                new LookupItem { Code = "Y", Value = "Yes" },
                new LookupItem { Code = "N", Value = "No" }
            };
            return Json(options);
        }

        [HttpGet]
        public IActionResult GetJobRelatedOptions()
        {
            var options = new List<LookupItem>
            {
                new LookupItem { Code = "Y", Value = "Yes" },
                new LookupItem { Code = "N", Value = "No" }
            };
            return Json(options);
        }
    }
}