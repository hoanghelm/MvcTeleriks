using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.Services.Models;

namespace WIRS.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsModel> GetDashboardStatsAsync(string userId, string userRole);
        Task<List<IncidentModel>> GetRecentIncidentsAsync(string userId, string userRole, int count = 5);
        Task<List<IncidentModel>> GetPendingApprovalsAsync(string userId, string userRole);
    }
}