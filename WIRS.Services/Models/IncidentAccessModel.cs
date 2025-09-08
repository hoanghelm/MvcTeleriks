using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIRS.Services.Models
{
    public class IncidentAccessModel
    {
        public string IncidentId { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanWorkflow { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public string AccessLevel { get; set; } = string.Empty;
    }
}