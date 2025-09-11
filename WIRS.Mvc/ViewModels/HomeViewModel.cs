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
	}
}