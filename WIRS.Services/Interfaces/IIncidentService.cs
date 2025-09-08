using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.Services.Models;

namespace WIRS.Services.Interfaces
{
    public interface IIncidentService
    {
        Task<IncidentDataModel> GetIncidentDataAsync(string userId, string userRole);
        Task<bool> CanUserEditIncident(string incidentId, string userId);
        Task<bool> CanUserWorkflowIncident(string incidentId, string userId);
    }
}