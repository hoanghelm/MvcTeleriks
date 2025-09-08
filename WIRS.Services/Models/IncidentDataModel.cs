namespace WIRS.Services.Models
{
    public class IncidentDataModel
    {
        public List<IncidentModel> Incidents { get; set; } = new List<IncidentModel>();
        public List<IncidentModel> PendingIncidents { get; set; } = new List<IncidentModel>();
    }
}