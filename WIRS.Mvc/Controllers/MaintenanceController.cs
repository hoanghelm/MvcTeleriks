using Microsoft.AspNetCore.Mvc;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;
using System.Threading.Tasks;

namespace WIRS.Mvc.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IMasterDataService _masterDataService;

        public MaintenanceController(IMaintenanceService maintenanceService, IMasterDataService masterDataService)
        {
            _maintenanceService = maintenanceService;
            _masterDataService = masterDataService;
        }

        public IActionResult LOB()
        {
            return View();
        }

        public IActionResult Locations()
        {
            return View();
        }

        public IActionResult Department()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetLOBList(string sbaCode)
        {
            try
            {
                var result = await _maintenanceService.GetLOBListAsync(sbaCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetLOBByUid(string sbaCode, string sbuCode)
        {
            try
            {
                var result = await _maintenanceService.GetLOBByUidAsync(sbaCode, sbuCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveLOB([FromBody] LOBModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                var currentUser = HttpContext.Session.GetString("UserId") ?? "System";
                
                var result = await _maintenanceService.SaveLOBAsync(model, currentUser);
                
                if (result.Success)
                {
                    return Json(new { success = true, message = "Record(s) have been saved." });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateLOBCode(string sbaCode)
        {
            try
            {
                var result = await _maintenanceService.GenerateLOBCodeAsync(sbaCode);
                return Json(new { success = true, code = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetLocationList(string sbaCode, string sbuCode, string departmentCode)
        {
            try
            {
                var result = await _maintenanceService.GetLocationListAsync(sbaCode, sbuCode, departmentCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetLocationByUid(string sbaCode, string sbuCode, string departmentCode, string locationCode)
        {
            try
            {
                var result = await _maintenanceService.GetLocationByUidAsync(sbaCode, sbuCode, departmentCode, locationCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveLocation([FromBody] LocationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                var currentUser = HttpContext.Session.GetString("UserId") ?? "System";
                
                var result = await _maintenanceService.SaveLocationAsync(model, currentUser);
                
                if (result.Success)
                {
                    return Json(new { success = true, message = "Record(s) have been saved." });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateLocationCode(string sbuCode)
        {
            try
            {
                var result = await _maintenanceService.GenerateLocationCodeAsync(sbuCode);
                return Json(new { success = true, code = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDepartmentList(string sbaCode, string sbuCode, string departmentName = "")
        {
            try
            {
                var result = await _maintenanceService.GetDepartmentListAsync(sbaCode, sbuCode, departmentName);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDepartmentByUid(string codeType, string sbaCode, string sbuCode, string departmentCode)
        {
            try
            {
                var result = await _maintenanceService.GetDepartmentByUidAsync(codeType, sbaCode, sbuCode, departmentCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDepartment([FromBody] DepartmentModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                var currentUser = HttpContext.Session.GetString("UserId") ?? "System";
                
                var result = await _maintenanceService.SaveDepartmentAsync(model, currentUser);
                
                if (result.Success)
                {
                    return Json(new { success = true, message = "Record(s) have been saved." });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateDepartmentCode(string sbuCode)
        {
            try
            {
                var result = await _maintenanceService.GenerateDepartmentCodeAsync(sbuCode);
                return Json(new { success = true, code = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult CopyTo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetCopyToList(string sbaCode, string sbuCode, string departmentCode, string locationCode)
        {
            try
            {
                var result = await _maintenanceService.GetCopyToListAsync(sbaCode, sbuCode, departmentCode, locationCode);
                return Json(new { 
                    success = true, 
                    data = result,
                    totalCount = result.Count()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCopyToListByUid(string sbaCode, string sbuCode, string departmentCode, string locationCode, string userId)
        {
            try
            {
                var result = await _maintenanceService.GetCopyToListByUidAsync(sbaCode, sbuCode, departmentCode, locationCode, userId);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCopyToList([FromBody] CopyToListModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                var currentUser = HttpContext.Session.GetString("UserId") ?? "System";
                
                var result = await _maintenanceService.SaveCopyToListAsync(model, currentUser);
                
                if (result.Success)
                {
                    return Json(new { success = true, message = "Record(s) have been saved." });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCopyToList([FromBody] CopyToListModel model)
        {
            try
            {
                model.InactiveDate = DateTime.Now.ToString("yyyy-MM-dd");
                
                var currentUser = HttpContext.Session.GetString("UserId") ?? "System";
                
                var result = await _maintenanceService.SaveCopyToListAsync(model, currentUser);
                
                if (result.Success)
                {
                    return Json(new { success = true, message = "Record has been inactivated." });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}