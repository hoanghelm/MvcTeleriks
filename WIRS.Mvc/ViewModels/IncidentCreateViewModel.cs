using WIRS.Services.Models;

namespace WIRS.Mvc.ViewModels
{
    public class IncidentCreateViewModel
    {
        public string IncidentDateTime { get; set; } = string.Empty;
        public string IncidentTime { get; set; } = string.Empty;
        public string IncidentDate { get; set; } = string.Empty;
        public string SbaCode { get; set; } = string.Empty;
        public string SbuCode { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ExactLocation { get; set; } = string.Empty;
        public string IncidentDesc { get; set; } = string.Empty;
        public string SuperiorName { get; set; } = string.Empty;
        public string SuperiorEmpNo { get; set; } = string.Empty;
        public string SuperiorDesignation { get; set; } = string.Empty;
        public List<IncidentTypeModel> IncidentTypes { get; set; } = new();
        public List<InjuredPersonModel> InjuredPersons { get; set; } = new();
        public List<EyewitnessModel> Eyewitnesses { get; set; } = new();
        public int? AnyEyewitness { get; set; }
        public string DamageDescription { get; set; } = string.Empty;
        public string IsWorkingOvertime { get; set; } = string.Empty;
        public string IsJobrelated { get; set; } = string.Empty;
        public string ExaminedHospitalClinicName { get; set; } = string.Empty;
        public string OfficialWorkingHrs { get; set; } = string.Empty;
        public string InjuredCaseType { get; set; } = string.Empty;
        
        public List<LookupItem> SbaOptions { get; set; } = new();
        public List<LookupItem> SbuOptions { get; set; } = new();
        public List<LookupItem> DivisionOptions { get; set; } = new();
        public List<LookupItem> DepartmentOptions { get; set; } = new();
        public List<LookupItem> LocationOptions { get; set; } = new();
        public List<LookupItem> IncidentTypeOptions { get; set; } = new();
        public List<LookupItem> InjuredCaseTypeOptions { get; set; } = new();
        public List<LookupItem> YesNoOptions { get; set; } = new();
        public List<LookupItem> OvertimeOptions { get; set; } = new();
        public List<LookupItem> JobRelatedOptions { get; set; } = new();
        
        public bool IsReadOnly { get; set; }
        public string ValidationMessage { get; set; } = string.Empty;
    }
}