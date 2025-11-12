using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WIRS.Shared.Configuration;
using WIRS.Mvc.ViewModels;
using WIRS.Services.Interfaces;
using WIRS.Shared.Enums;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace WIRS.Mvc.Controllers
{
	[Authorize]
	public class HomeController : BaseController
	{
		private readonly IIncidentService _incidentService;
		private readonly IUrlGeneratorService _urlGenerator;
		private readonly AppSettings _appSettings;

		public HomeController(
			WIRS.Services.Auth.IAuthService authService,
			IIncidentService incidentService,
			IUrlGeneratorService urlGenerator,
			IOptions<AppSettings> appSettings) : base(authService)
		{
			_incidentService = incidentService;
			_urlGenerator = urlGenerator;
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

			if (userSession == null && User.Identity.IsAuthenticated)
			{
				var userId = User.Identity.Name;
				if (!string.IsNullOrEmpty(userId))
				{
					var user = await AuthService.RecreateSessionFromClaimsAsync(userId);
					if (user != null)
					{
						userSession = await GetCurrentUserSessionAsync();
					}
				}
			}

			if (userSession == null)
			{
				TempData["ErrorMessage"] = "Your session has expired. Please login again.";
				return RedirectToAction("Index", "Login");
			}

			var model = new HomeViewModel
			{
				UserId = userSession.UserId,
				UserName = userSession.UserName,
				RoleName = UserRoleExtensions.GetRoleName(userSession.UserRole)
			};

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> GetIncidentData()
		{
			try
			{
				var userSession = await GetCurrentUserSessionAsync();
				if (userSession == null)
				{
					return Json(new { success = false, message = "User session not found" });
				}

				var incidentData = await _incidentService.GetIncidentDataAsync(userSession.UserId, UserRoleExtensions.GetRoleName(userSession.UserRole));

				var response = new
				{
					success = true,
					allIncidents = incidentData.Incidents,
					pendingIncidents = incidentData.PendingIncidents,
					totalCount = incidentData.Incidents.Count,
					pendingCount = incidentData.PendingIncidents.Count
				};

				return Json(response);
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "An error occurred while loading incident data" });
			}
		}

		[HttpPost]
		public async Task<IActionResult> GetGridData([DataSourceRequest] DataSourceRequest request, string type = "pending")
		{
			try
			{
				var userSession = await GetCurrentUserSessionAsync();
				if (userSession == null)
				{
					return Json(new DataSourceResult { Errors = "User session not found" });
				}

				var incidentData = await _incidentService.GetIncidentDataAsync(userSession.UserId, UserRoleExtensions.GetRoleName(userSession.UserRole));
				var incidents = type == "pending" ? incidentData.PendingIncidents : incidentData.Incidents;

				var result = incidents.ToDataSourceResult(request);
				return Json(result);
			}
			catch (Exception ex)
			{
				return Json(new DataSourceResult { Errors = "An error occurred while loading incident data" });
			}
		}
	}
}