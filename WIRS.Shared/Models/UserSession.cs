using WIRS.Shared.Enums;

namespace WIRS.Shared.Models
{
    public class UserSession
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public string SbaName { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission) || UserRole.IsAdmin();
        }

        public bool CanViewIncident(string incidentId)
        {
            return HasPermission("VIEW_ALL_INCIDENTS") || 
                   HasPermission($"VIEW_INCIDENT_{incidentId}") ||
                   UserRole.IsManagement();
        }

        public bool CanEditIncident(string incidentId)
        {
            return HasPermission("EDIT_ALL_INCIDENTS") || 
                   HasPermission($"EDIT_INCIDENT_{incidentId}") ||
                   UserRole.IsAdmin();
        }
    }
}