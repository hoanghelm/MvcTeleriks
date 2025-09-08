using WIRS.Services.Models;

namespace WIRS.Mvc.ViewModels
{
    public class HomeViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string InstructionContent { get; set; } = string.Empty;
        public List<string> UserPermissions { get; set; } = new();
        public DashboardStatsModel DashboardStats { get; set; } = new();
        public string RoleName { get; set; } = string.Empty;
        public string RoleValue { get; set; } = string.Empty;
        public bool IsHigherAuthority { get; set; }

        public bool CanCreateIncident => UserPermissions.Contains("CREATE_INCIDENT");
        public bool CanViewReports => UserPermissions.Contains("VIEW_REPORTS");
        public bool HasAdminAccess => UserPermissions.Contains("ADMIN_ACCESS");
        public bool HasWorkflowAccess => UserPermissions.Contains("WORKFLOW_ACCESS");
        public bool HasDashboardAccess => UserPermissions.Contains("DASHBOARD_ACCESS");
        public bool HasApprovalAccess => UserPermissions.Contains("APPROVAL_ACCESS");
    }

    public class IncidentActionRequest
    {
        public string IncidentId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }

    public class IncidentActionResponse
    {
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string? Message { get; set; }
        public bool CanEdit { get; set; }
        public bool CanWorkflow { get; set; }
        public bool CanPrint { get; set; }
    }
}