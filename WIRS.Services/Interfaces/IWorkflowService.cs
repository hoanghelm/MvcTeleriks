using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.Services.Models;

namespace WIRS.Services.Interfaces
{
    public interface IWorkflowService
    {
        Task<(string incidentId, string errorCode)> CreateIncidentAsync(WorkflowIncidentCreateModel model, string userId);
        Task<WorkflowIncidentDetailModel?> GetIncidentByIdAsync(string incidentId, string userId);
        Task<bool> CanUserEditIncidentAsync(string incidentId, string userId, string changeMode = "");
        Task<bool> CanUserWorkflowIncidentAsync(string incidentId, string userId);
        Task<string> UpdateIncidentAsync(WorkflowIncidentUpdateModel model, string userId);
        Task<string> SubmitIncidentPartCAsync(WorkflowIncidentPartCModel model, string userId);
        Task<DataSet> GetIncidentWorkflowsAsync(string incidentId, string status = "");
        Task<List<IncidentStagePermissionModel>> GetIncidentStagePermissionsAsync(string incidentId, string userId);
        Task<DataSet> SearchIncidentsAsync(string userId, string userRoleCode, IncidentSearchModel searchCriteria);
    }
}