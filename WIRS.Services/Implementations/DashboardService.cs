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
    public class DashboardService : IDashboardService
    {
        private readonly IUserService _userService;
        private readonly IUserDataAccess _userDataAccess;
        private readonly IAuthService _authService;
        private readonly IDataMapperService _dataMapper;

        public DashboardService(
            IUserService userService,
            IUserDataAccess userDataAccess,
            IAuthService authService,
            IDataMapperService dataMapper)
        {
            _userService = userService;
            _userDataAccess = userDataAccess;
            _authService = authService;
            _dataMapper = dataMapper;
        }

        public async Task<DashboardStatsModel> GetDashboardStatsAsync(string userId, string userRole)
        {
            if (!await _authService.HasPermissionAsync("VIEW_DASHBOARD"))
            {
                return new DashboardStatsModel();
            }

            try
            {
                var dataset = await _userDataAccess.GetInfoByUserID(userId, userRole);

                var allIncidents = _dataMapper.MapDataSetToIncidents(dataset, 0);
                var pendingIncidents = _dataMapper.MapDataSetToIncidents(dataset, 1);

                var stats = new DashboardStatsModel
                {
                    TotalIncidents = allIncidents.Count,
                    PendingIncidents = pendingIncidents.Count,
                    CompletedIncidents = allIncidents.Count(i => i.Status == "5"),
                    OverdueIncidents = pendingIncidents.Count(i => i.SubmittedOn < DateTime.Now.AddDays(-7)),
                    IncidentsThisMonth = allIncidents.Count(i => i.SubmittedOn >= DateTime.Now.Date.AddDays(-30)),
                    IncidentsByType = allIncidents
                        .GroupBy(i => i.IncidentDesc)
                        .Select(g => new IncidentTypeStat { IncidentType = g.Key, Count = g.Count() })
                        .ToList(),
                    IncidentsByStatus = allIncidents
                        .GroupBy(i => new { i.Status, i.StatusDesc })
                        .Select(g => new IncidentStatusStat
                        {
                            Status = g.Key.Status,
                            StatusDescription = g.Key.StatusDesc,
                            Count = g.Count()
                        })
                        .ToList()
                };

                return stats;
            }
            catch
            {
                return new DashboardStatsModel();
            }
        }

        public async Task<List<IncidentModel>> GetRecentIncidentsAsync(string userId, string userRole, int count = 5)
        {
            var data = await _userService.GetIncidentDataAsync(userId, userRole);
            return data.Incidents
                .OrderByDescending(i => i.SubmittedOn)
                .Take(count)
                .ToList();
        }

        public async Task<List<IncidentModel>> GetPendingApprovalsAsync(string userId, string userRole)
        {
            var data = await _userService.GetIncidentDataAsync(userId, userRole);

            return data.PendingIncidents
                .ToList();
        }
    }
}