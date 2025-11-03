using System.Data;

namespace WIRS.Services.Models
{
    public class WorkflowIncidentDetailModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public string IncidentDateTime { get; set; } = string.Empty;
        public string IncidentTime { get; set; } = string.Empty;
        public string IncidentDate { get; set; } = string.Empty;
        public string SbaCode { get; set; } = string.Empty;
        public string SbuCode { get; set; } = string.Empty;
        public string SbuName { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string ExactLocation { get; set; } = string.Empty;
        public string IncidentDesc { get; set; } = string.Empty;
        public string SuperiorName { get; set; } = string.Empty;
        public string SuperiorEmpNo { get; set; } = string.Empty;
        public string SuperiorDesignation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreationDate { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifyDate { get; set; } = string.Empty;
        public int? AnyEyewitness { get; set; }
        public string DamageDescription { get; set; } = string.Empty;
        public string IsWorkingOvertime { get; set; } = string.Empty;
        public string IsJobrelated { get; set; } = string.Empty;
        public string ExaminedHospitalClinicName { get; set; } = string.Empty;
        public string OfficialWorkingHrs { get; set; } = string.Empty;
        public string InjuredCaseType { get; set; } = string.Empty;
        public string Negligent { get; set; } = string.Empty;
        public string NegligentComments { get; set; } = string.Empty;
        public string RecommendActionDesc { get; set; } = string.Empty;
        public string RiskAssessmentReview { get; set; } = string.Empty;
        public string RiskAssessmentReviewDesc { get; set; } = string.Empty;
        public string RiskAssessmentReviewComments { get; set; } = string.Empty;
        public string WhatHappenedAndWhyComments { get; set; } = string.Empty;
        public List<IncidentTypeModel> IncidentTypes { get; set; } = new();
        public List<InjuredPersonModel> InjuredPersons { get; set; } = new();
        public List<EyewitnessModel> Eyewitnesses { get; set; } = new();
        public List<IncidentWorkflowModel> Workflows { get; set; } = new();
        public List<IncidentAttachmentModel> Attachments { get; set; } = new();
        public bool CanEdit { get; set; }
        public bool CanWorkflow { get; set; }
        public List<IncidentStagePermissionModel> StagePermissions { get; set; } = new();
    }

    public class WorkflowIncidentUpdateModel : IncidentCreateModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Negligent { get; set; } = string.Empty;
        public string NegligentComments { get; set; } = string.Empty;
        public string RecommendActionDesc { get; set; } = string.Empty;
        public string RiskAssessmentReview { get; set; } = string.Empty;
        public string RiskAssessmentReviewDesc { get; set; } = string.Empty;
        public string RiskAssessmentReviewComments { get; set; } = string.Empty;
        public string WhatHappenedAndWhyComments { get; set; } = string.Empty;
    }

    public class WorkflowIncidentPartCModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public List<InjuryDetailModel> InjuryDetails { get; set; } = new();
        public List<IntervieweeModel> Interviewees { get; set; } = new();
        public List<CauseAnalysisModel> CauseAnalysis { get; set; } = new();
        public List<MedicalLeaveModel> MedicalLeaves { get; set; } = new();
        public List<IncidentAttachmentModel> Attachments { get; set; } = new();
        public List<IncidentWorkflowModel> Workflows { get; set; } = new();
    }

    public class IncidentWorkflowModel
    {
        public string WorkflowId { get; set; } = string.Empty;
        public string IncidentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string ActionDate { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
    }

    public class IncidentAttachmentModel
    {
        public string AttachmentId { get; set; } = string.Empty;
        public string IncidentId { get; set; } = string.Empty;
        public string AttachmentType { get; set; } = string.Empty;
        public string ReferenceCode { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;
        public string UploadedDate { get; set; } = string.Empty;
    }

    public class InjuryDetailModel
    {
        public string InjuredPersonId { get; set; }
        public string InjuredPersonName { get; set; }
        public List<string> NatureOfInjury { get; set; }
        public List<string> HeadNeckTorso { get; set; }
        public List<string> UpperLimbs { get; set; }
        public List<string> LowerLimbs { get; set; }
        public string Description { get; set; }
    }

    public class IntervieweeModel
    {
        public string IntervieweeId { get; set; } = string.Empty;
        public string EmpNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string InterviewDate { get; set; } = string.Empty;
        public string Statement { get; set; } = string.Empty;
    }

    public class CauseAnalysisModel
    {
        public string CauseId { get; set; } = string.Empty;
        public string CauseType { get; set; } = string.Empty;
        public string CauseCategory { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;
    }

    public class MedicalLeaveModel
    {
        public string MedicalLeaveId { get; set; } = string.Empty;
        public string InjuredId { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string MedicalCertificate { get; set; } = string.Empty;
    }

    public class IncidentStagePermissionModel
    {
        public string Stage { get; set; } = string.Empty;
        public string StageDescription { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool IsCurrentStage { get; set; }
        public string RequiredRole { get; set; } = string.Empty;
    }

    public class IncidentSearchModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public string Sba { get; set; } = string.Empty;
        public string Sbu { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string IncDateFrom { get; set; } = string.Empty;
        public string IncDateTo { get; set; } = string.Empty;
        public DataTable? SearchFilters { get; set; }
    }
}