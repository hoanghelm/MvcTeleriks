using System;
using System.Data;
using System.Threading.Tasks;
using WIRS.DataAccess.Entities;

namespace WIRS.DataAccess.Interfaces
{
    public interface IIncidentDataAccess
    {
        Task GetRegisteredIncidents(Incident incidents);

        Task<(string incident_ID, string error_Code)> SaveIncidents(Incident incidents);

        Task<(string incident_ID, string error_Code)> UpdateIncidents(Incident incidents);

        Task<DataSet> SearchIncidents(User user, Incident incidents);

        Task<DataSet> SearchVerfiedIncidents(User user, Incident incidents);

        Task<string> GetRegisteredIncidentsByLink(Incident incidents);

        Task<DataSet> GetToEmailDistributionBySBU(Incident incidents);

        Task<DataSet> GetSelCCEmailListByIncidents(string incidentId, string recipienttype);

        Task<DataSet> GetWSHOEmailListBySBU(Incident incident);

        Task<(string IncidentDateTime, string SubmittedDateTime)> GetSubmittedDate(string incident_ID);
    }
}