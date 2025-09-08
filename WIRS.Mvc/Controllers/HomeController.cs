using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WIRS.Shared.Configuration;
using WIRS.Mvc.ViewModels;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Mvc.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IUrlGeneratorService _urlGenerator;
        private readonly IIncidentAccessService _incidentAccessService;
        private readonly IDashboardService _dashboardService;
        private readonly AppSettings _appSettings;

        public HomeController(
            WIRS.Services.Auth.IAuthService authService,
            IUserService userService,
            IUrlGeneratorService urlGenerator,
            IIncidentAccessService incidentAccessService,
            IDashboardService dashboardService,
            IOptions<AppSettings> appSettings) : base(authService)
        {
            _userService = userService;
            _urlGenerator = urlGenerator;
            _incidentAccessService = incidentAccessService;
            _dashboardService = dashboardService;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> Index(string page_id = null)
        {
            if (!string.IsNullOrEmpty(page_id))
            {
                var redirectUrl = await _urlGenerator.GenerateRedirectUrl(page_id);
                return Redirect(redirectUrl);
            }

            await UpdateLastActivityAsync();
            var userSession = await GetCurrentUserSessionAsync();
            if (userSession == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var model = new HomeViewModel
            {
                UserId = userSession.UserId,
                UserName = userSession.UserName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetIncidentData()
        {
            try
            {
                if (!await HasPermissionAsync("VIEW_OWN_INCIDENTS"))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                var userSession = await GetCurrentUserSessionAsync();

                var data = await _userService.GetIncidentDataAsync(userSession.UserId, ((int)userSession.UserRole).ToString());

                return Json(new { 
                    success = true, 
                    incidents = data.Incidents,
                    pendingIncidents = data.PendingIncidents
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetIncidentRedirectUrl(string incidentId, string action = "view")
        {
            try
            {
                var currentUser = await AuthService.GetCurrentUserModelAsync();

                var hasAccess = await _incidentAccessService.ValidateIncidentAccessAsync(
                    incidentId, currentUser.UserId, action);

                if (!hasAccess)
                {
                    return Json(new { success = false, message = "Access denied for this action" });
                }

                var url = await _urlGenerator.GenerateIncidentUrl(incidentId, currentUser.UserId, action);

                return Json(new { success = true, url = url });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPrintUrl(string incidentId)
        {
            try
            {
                var currentUser = await AuthService.GetCurrentUserModelAsync();

                var hasAccess = await _incidentAccessService.ValidateIncidentAccessAsync(
                    incidentId, currentUser.UserId, "print");

                if (!hasAccess)
                {
                    return Json(new { success = false, message = "Access denied for printing" });
                }

                var url = await _urlGenerator.GeneratePrintUrl(incidentId, currentUser.UserId);

                return Json(new { success = true, url = url });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var currentUser = await AuthService.GetCurrentUserModelAsync();

                if (!await AuthService.HasPermissionAsync("VIEW_DASHBOARD"))
                {
                    return Json(new { success = false, message = "Dashboard access denied" });
                }

                var stats = await _dashboardService.GetDashboardStatsAsync(currentUser.UserId, currentUser.UserRole);
                var recentIncidents = await _dashboardService.GetRecentIncidentsAsync(currentUser.UserId, currentUser.UserRole);
                var pendingApprovals = await _dashboardService.GetPendingApprovalsAsync(currentUser.UserId, currentUser.UserRole);

                return Json(new
                {
                    success = true,
                    stats = stats,
                    recentIncidents = recentIncidents.Select(i => new
                    {
                        i.IncidentId,
                        i.IncidentDesc,
                        i.StatusDesc,
                        i.SubmittedOn
                    }),
                    pendingApprovals = pendingApprovals.Select(i => new
                    {
                        i.IncidentId,
                        i.IncidentDesc,
                        i.CreatorName,
                        i.SubmittedOn
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {
            try
            {
                var currentUser = await AuthService.GetCurrentUserModelAsync();

                if (!await AuthService.HasPermissionAsync("SYSTEM_ADMIN"))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                var roles = GetAllRoles();
                return Json(new { success = true, roles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult Administration()
        {
            return View();
        }

        [Authorize(Policy = "ManagementOnly")]
        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var currentUser = await AuthService.GetCurrentUserModelAsync();
            var stats = await _dashboardService.GetDashboardStatsAsync(currentUser.UserId, currentUser.UserRole);
            return View(stats);
        }

        [Authorize(Policy = "WSHOnly")]
        [HttpGet]
        public async Task<IActionResult> WSHDashboard()
        {
            var currentUser = await AuthService.GetCurrentUserModelAsync();
            var pendingApprovals = await _dashboardService.GetPendingApprovalsAsync(currentUser.UserId, currentUser.UserRole);
            return View(pendingApprovals);
        }

        [HttpPost]
        public async Task<IActionResult> CheckAccess(string resourceType, string resourceId, string action)
        {
            try
            {
                var currentUser = await AuthService.GetCurrentUserModelAsync();
                var hasAccess = false;

                switch (resourceType.ToLower())
                {
                    case "incident":
                        hasAccess = await _incidentAccessService.ValidateIncidentAccessAsync(
                            resourceId, currentUser.UserId, action);
                        break;
                    case "module":
                        hasAccess = await AuthService.HasPermissionAsync($"ACCESS_{resourceId.ToUpper()}");
                        break;
                    default:
                        hasAccess = false;
                        break;
                }

                return Json(new { success = true, hasAccess });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private static List<object> GetAllRoles()
        {
            return new List<object>
            {
                new { Code = "1", Name = "Administrator", Value = "Administrator", SortOrder = 9 },
                new { Code = "2", Name = "User", Value = "User", SortOrder = 1 },
                new { Code = "3", Name = "Supervisor", Value = "Supervisor", SortOrder = 2 },
                new { Code = "4", Name = "WSHO", Value = "WSHO", SortOrder = 4 },
                new { Code = "5", Name = "Assistant WSHO", Value = "Assistant WSHO", SortOrder = 3 },
                new { Code = "6", Name = "Management", Value = "Management", SortOrder = 5 },
                new { Code = "7", Name = "Senior Management", Value = "Senior Management", SortOrder = 6 },
                new { Code = "8", Name = "Department Head", Value = "Department Head", SortOrder = 7 },
                new { Code = "9", Name = "HOD", Value = "HOD", SortOrder = 8 }
            };
        }
    }
}