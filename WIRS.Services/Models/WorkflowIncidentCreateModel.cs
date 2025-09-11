namespace WIRS.Services.Models
{
    public class WorkflowIncidentCreateModel
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
    }

    public class IncidentTypeModel
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class InjuredPersonModel
    {
        public string EmpNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NricFinNo { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Age { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public string RaceOther { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string EmploymentTypeOther { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    public class EyewitnessModel
    {
        public string EmpNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
    }
}