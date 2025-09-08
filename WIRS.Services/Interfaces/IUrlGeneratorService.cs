using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIRS.Services.Interfaces
{
    public interface IUrlGeneratorService
    {
        Task<string> GenerateIncidentUrl(string incidentId, string userId, string action);
        Task<string> GeneratePrintUrl(string incidentId, string userId);
        Task<string> GenerateRedirectUrl(string pageId);
    }
}