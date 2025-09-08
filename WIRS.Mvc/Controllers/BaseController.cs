using Microsoft.AspNetCore.Mvc;
using WIRS.Services.Auth;
using WIRS.Shared.Models;

namespace WIRS.Mvc.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IAuthService AuthService;

        protected BaseController(IAuthService authService)
        {
            AuthService = authService;
        }

        protected async Task<UserSession?> GetCurrentUserSessionAsync()
        {
            return await AuthService.GetCurrentUserAsync();
        }

        protected async Task<bool> HasPermissionAsync(string permission)
        {
            return await AuthService.HasPermissionAsync(permission);
        }

        protected async Task<bool> CanViewIncidentAsync(string incidentId)
        {
            return await AuthService.CanViewIncidentAsync(incidentId);
        }

        protected async Task<bool> CanEditIncidentAsync(string incidentId)
        {
            return await AuthService.CanEditIncidentAsync(incidentId);
        }

        protected async Task UpdateLastActivityAsync()
        {
            await AuthService.UpdateLastActivityAsync();
        }

        protected IActionResult Unauthorized(string message = "You don't have permission to access this resource")
        {
            return StatusCode(403, new { message });
        }
    }
}