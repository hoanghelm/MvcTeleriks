using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.DataAccess.Interfaces;
using WIRS.Services.Auth;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Services.Implementations
{
    public class IncidentAccessService : IIncidentAccessService
    {
        private readonly IAuthService _authService;
        private readonly IUserDataAccess _userDataAccess;

        public IncidentAccessService(IAuthService authService, IUserDataAccess userDataAccess)
        {
            _authService = authService;
            _userDataAccess = userDataAccess;
        }

        public async Task<IncidentAccessModel> GetIncidentAccessAsync(string incidentId, string userId, string userRole)
        {
            return await Task.FromResult(new IncidentAccessModel
            {
                IncidentId = incidentId,
                CanView = await _authService.CanViewIncidentAsync(incidentId),
                CanEdit = await _authService.CanEditIncidentAsync(incidentId),
                CanWorkflow = await _authService.HasPermissionAsync("WORKFLOW_INCIDENTS"),
                CanDelete = await _authService.HasPermissionAsync("DELETE_INCIDENTS"),
                CanPrint = await _authService.HasPermissionAsync("PRINT_INCIDENTS"),
                AccessLevel = await DetermineAccessLevel(incidentId)
            });
        }

        public async Task<bool> ValidateIncidentAccessAsync(string incidentId, string userId, string action)
        {
            try
            {
                return action.ToLower() switch
                {
                    "view" => await _authService.CanViewIncidentAsync(incidentId),
                    "edit" => await _authService.CanEditIncidentAsync(incidentId),
                    "workflow" => await _authService.HasPermissionAsync("WORKFLOW_INCIDENTS"),
                    "delete" => await _authService.HasPermissionAsync("DELETE_INCIDENTS"),
                    "print" => await _authService.HasPermissionAsync("PRINT_INCIDENTS"),
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> GetAccessibleIncidentStatusesAsync(string userId, string userRole)
        {
            var statuses = new List<string>();

            if (await _authService.HasPermissionAsync("VIEW_ALL_INCIDENTS"))
            {
                statuses.AddRange(new[] { "1", "2", "3", "4", "5" });
            }

            if (await _authService.HasPermissionAsync("SYSTEM_ADMIN"))
            {
                statuses.AddRange(new[] { "6", "7" });
            }

            return await Task.FromResult(statuses.Distinct().ToList());
        }

        private async Task<string> DetermineAccessLevel(string incidentId)
        {
            if (await _authService.HasPermissionAsync("DELETE_INCIDENTS"))
                return "FULL_ACCESS";

            if (await _authService.HasPermissionAsync("WORKFLOW_INCIDENTS"))
                return "WORKFLOW_ACCESS";

            if (await _authService.CanEditIncidentAsync(incidentId))
                return "EDIT_ACCESS";

            if (await _authService.CanViewIncidentAsync(incidentId))
                return "VIEW_ACCESS";

            return "NO_ACCESS";
        }
    }

    public class PageRedirectParameters
    {
        public string IncidentId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string ModifyDate { get; set; } = string.Empty;
    }
}