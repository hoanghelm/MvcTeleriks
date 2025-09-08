using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.Services.Models;

namespace WIRS.Services.Interfaces
{
    public interface IIncidentAccessService
    {
        Task<IncidentAccessModel> GetIncidentAccessAsync(string incidentId, string userId, string userRole);
        Task<bool> ValidateIncidentAccessAsync(string incidentId, string userId, string action);
        Task<List<string>> GetAccessibleIncidentStatusesAsync(string userId, string userRole);
    }
}