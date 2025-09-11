using WIRS.Services.Models;

namespace WIRS.Mvc.ViewModels
{
    public class IncidentViewViewModel
    {
        public WorkflowIncidentDetailModel? Incident { get; set; }
        public List<IncidentStagePermissionModel> StagePermissions { get; set; } = new();
        public List<IncidentWorkflowModel> WorkflowHistory { get; set; } = new();
        public List<IncidentAttachmentModel> Attachments { get; set; } = new();
        
        public List<LookupItem> SbaOptions { get; set; } = new();
        public List<LookupItem> SbuOptions { get; set; } = new();
        public List<LookupItem> DivisionOptions { get; set; } = new();
        public List<LookupItem> DepartmentOptions { get; set; } = new();
        public List<LookupItem> LocationOptions { get; set; } = new();
        public List<LookupItem> IncidentTypeOptions { get; set; } = new();
        public List<LookupItem> InjuredCaseTypeOptions { get; set; } = new();
        
        public string CurrentStage { get; set; } = string.Empty;
        public string CurrentStageDescription { get; set; } = string.Empty;
        public bool CanEdit { get; set; }
        public bool CanWorkflow { get; set; }
        public bool CanViewPartA { get; set; }
        public bool CanViewPartB { get; set; }
        public bool CanViewPartC { get; set; }
        public bool CanEditPartA { get; set; }
        public bool CanEditPartB { get; set; }
        public bool CanEditPartC { get; set; }
        
        public WorkflowActionViewModel WorkflowAction { get; set; } = new();
        public string ValidationMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
    }

    public class WorkflowActionViewModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string NextStage { get; set; } = string.Empty;
        public List<string> AvailableActions { get; set; } = new();
        
        public WorkflowIncidentPartCModel? PartCData { get; set; }
    }

    public class IncidentSearchViewModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public string SbaCode { get; set; } = string.Empty;
        public string SbuCode { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string IncDateFrom { get; set; } = string.Empty;
        public string IncDateTo { get; set; } = string.Empty;
        
        public List<LookupItem> SbaOptions { get; set; } = new();
        public List<LookupItem> SbuOptions { get; set; } = new();
        public List<LookupItem> DivisionOptions { get; set; } = new();
        
        public List<IncidentSummaryModel> SearchResults { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageSize { get; set; } = 20;
        public int CurrentPage { get; set; } = 1;
    }

    public class IncidentSummaryModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public string IncidentDateTime { get; set; } = string.Empty;
        public string SbuName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string IncidentDesc { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusDesc { get; set; } = string.Empty;
        public string SubmittedOn { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }
}