namespace WIRS.Services.Models
{
    public class IncidentModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public DateTime IncidentDateTime { get; set; }
        public string SbuName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string IncidentDesc { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public DateTime SubmittedOn { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}