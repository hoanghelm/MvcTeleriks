namespace WIRS.Services.Models
{
    public class PartCSubmitModel
    {
        public string IncidentId { get; set; }
        public string IsNegligent { get; set; }
        public string NegligentComments { get; set; }
        public string NeedsRiskAssessmentReview { get; set; }
        public string RiskAssessmentComments { get; set; }
        public string WhatHappenedAndWhy { get; set; }
        public string RecommendedActions { get; set; }
        public string AdditionalComments { get; set; }
        public string CwshoId { get; set; }
        public List<PersonInterviewedModel> PersonsInterviewed { get; set; }
        public List<InjuryDetailModel> InjuryDetails { get; set; }
        public List<MedicalCertificateModel> MedicalCertificates { get; set; }
        public List<string> IncidentClassList { get; set; }
        public List<string> IncidentAgentList { get; set; }
        public List<string> UnsafeConditionsList { get; set; }
        public List<string> UnsafeActsList { get; set; }
        public List<string> ContributingFactorsList { get; set; }
        public string SubmitterName { get; set; }
        public string SubmitterEmpId { get; set; }
        public string SubmitterDesignation { get; set; }
    }

    public class PartCCloseModel
    {
        public string IncidentId { get; set; }
        public string AdditionalComments { get; set; }
        public string CwshoId { get; set; }
        public PartCSubmitModel PartCData { get; set; }
    }

    public class PersonInterviewedModel
    {
        public string Name { get; set; }
        public string EmployeeNo { get; set; }
        public string Designation { get; set; }
        public string ContactNo { get; set; }
    }

    //public class InjuryDetailModel
    //{
    //    public string InjuredPersonId { get; set; }
    //    public string InjuredPersonName { get; set; }
    //    public List<string> NatureOfInjury { get; set; }
    //    public List<string> HeadNeckTorso { get; set; }
    //    public List<string> UpperLimbs { get; set; }
    //    public List<string> LowerLimbs { get; set; }
    //    public string Description { get; set; }
    //}

    public class MedicalCertificateModel
    {
        public string InjuredPersonId { get; set; }
        public string InjuredPersonName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int NumberOfDays { get; set; }
        public string AttachmentPath { get; set; }
        public bool HasAttachment { get; set; }
    }
}