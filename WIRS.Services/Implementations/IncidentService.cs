using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.DataAccess.Interfaces;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Services.Implementations
{
    public class IncidentService : IIncidentService
    {
        private readonly IUserDataAccess _userDataAccess;
        private readonly IWorkflowIncidentDataAccess _workflowIncidentDataAccess;
        private readonly IDataMapperService _dataMapper;

        public IncidentService(
            IUserDataAccess userDataAccess, 
            IWorkflowIncidentDataAccess workflowIncidentDataAccess,
            IDataMapperService dataMapper)
        {
            _userDataAccess = userDataAccess;
            _workflowIncidentDataAccess = workflowIncidentDataAccess;
            _dataMapper = dataMapper;
        }

        public async Task<IncidentDataModel> GetIncidentDataAsync(string userId, string userRole)
        {
            try
            {
                var dataset = await _userDataAccess.GetInfoByUserID(userId, userRole);

                var incidents = _dataMapper.MapDataSetToIncidents(dataset, 0);
                var pendingIncidents = _dataMapper.MapDataSetToIncidents(dataset, 1);

                return await Task.FromResult(new IncidentDataModel
                {
                    Incidents = incidents,
                    PendingIncidents = pendingIncidents
                });
            }
            catch
            {
                return new IncidentDataModel();
            }
        }

        public async Task<bool> CanUserEditIncident(string incidentId, string userId)
        {
            try
            {
                var dataset = await _workflowIncidentDataAccess.validate_user_to_edit_inc(incidentId, userId, string.Empty);
                return dataset?.Tables?.Count > 0 && dataset.Tables[0].Rows.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CanUserWorkflowIncident(string incidentId, string userId)
        {
            try
            {
                var dataset = await _workflowIncidentDataAccess.validate_workflowuser(incidentId, userId);
                return dataset?.Tables?.Count > 0 && dataset.Tables[0].Rows.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}