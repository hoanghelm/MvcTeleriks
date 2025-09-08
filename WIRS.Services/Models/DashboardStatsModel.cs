using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIRS.Services.Models
{
    public class DashboardStatsModel
    {
        public int TotalIncidents { get; set; }
        public int PendingIncidents { get; set; }
        public int CompletedIncidents { get; set; }
        public int OverdueIncidents { get; set; }
        public int IncidentsThisMonth { get; set; }
        public List<IncidentTypeStat> IncidentsByType { get; set; } = new();
        public List<IncidentStatusStat> IncidentsByStatus { get; set; } = new();
    }

    public class IncidentTypeStat
    {
        public string IncidentType { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class IncidentStatusStat
    {
        public string Status { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}