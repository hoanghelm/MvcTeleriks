using WIRS.Services.Models;
using WIRS.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace WIRS.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> ValidateUserAsync(string userId, string password);
        Task<bool> ValidateSSO(string loginId, string digest);
        Task SignInUserAsync(UserModel user, HttpContext context);
        Task SignOutAsync(HttpContext context);
        Task<UserSession?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<UserModel> GetCurrentUserModelAsync();
        Task<string> GetCurrentUserIdAsync();
        Task<string> GetCurrentUserRoleAsync();
        Task<List<string>> GetCurrentUserPermissionsAsync();
        Task<bool> HasPermissionAsync(string permission);
        Task<bool> CanViewIncidentAsync(string incidentId);
        Task<bool> CanEditIncidentAsync(string incidentId);
        Task UpdateLastActivityAsync();
        Task<UserSession?> RecreateSessionFromClaimsAsync(string userId);
    }
}