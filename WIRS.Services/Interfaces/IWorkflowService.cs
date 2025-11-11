using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.Services.Models;

namespace WIRS.Services.Interfaces
{
    public interface IWorkflowService
    {
        Task<(string incidentId, string errorCode)> CreateIncidentAsync(IncidentCreateModel model, string userId);
        Task<WorkflowIncidentDetailModel?> GetIncidentByIdAsync(string incidentId, string userId);
        Task<bool> CanUserEditIncidentAsync(string incidentId, string userId, string changeMode = "");
        Task<bool> CanUserWorkflowIncidentAsync(string incidentId, string userId);
        Task<string> UpdateIncidentAsync(WorkflowIncidentUpdateModel model, string userId);
        Task<string> SubmitPartBAsync(PartBSubmitModel model, string userId);
        Task<string> ClosePartBAsync(PartBSubmitModel model, string userId);
        Task<string> SavePartCAsync(PartCSubmitModel model, string userId);
        Task<string> SubmitPartCAsync(PartCSubmitModel model, string userId);
        Task<string> ClosePartCAsync(PartCCloseModel model, string userId);
        Task<string> SubmitPartDAsync(PartDSubmitModel model, string userId);
        Task<string> RevertPartDToWSHOAsync(string incidentId, string comments, string wshoId, string userId);
        Task<string> SubmitPartEAsync(string incidentId, string comments, string hodId, List<string> emailToList, List<CopyToPersonModel> additionalCopyToList, string userId);
        Task<string> RevertPartEToWSHOAsync(string incidentId, string comments, string wshoId, List<string> emailToList, List<CopyToPersonModel> additionalCopyToList, string userId);
        Task<string> SubmitPartFAsync(string incidentId, string comments, string riskAssessmentReview, string wshoId, List<Microsoft.AspNetCore.Http.IFormFile> attachments, List<Microsoft.AspNetCore.Http.IFormFile> riskAttachments, string userId);
        Task<DataSet> GetIncidentWorkflowsAsync(string incidentId, string status = "");
        Task<List<IncidentStagePermissionModel>> GetIncidentStagePermissionsAsync(string incidentId, string userId);
        Task<DataSet> SearchIncidentsAsync(string userId, string userRoleCode, IncidentSearchModel searchCriteria);
    }
}