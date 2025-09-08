using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockIncidentDataAccess : IIncidentDataAccess
	{
		public Task GetRegisteredIncidents(Incident incidents)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetRegisteredIncidentsByLink(Incident incidents)
		{
			throw new NotImplementedException();
		}

		public Task<DataSet> GetSelCCEmailListByIncidents(string incidentId, string recipienttype)
		{
			throw new NotImplementedException();
		}

		public Task<(string IncidentDateTime, string SubmittedDateTime)> GetSubmittedDate(string incident_ID)
		{
			throw new NotImplementedException();
		}

		public Task<DataSet> GetToEmailDistributionBySBU(Incident incidents)
		{
			throw new NotImplementedException();
		}

		public Task<DataSet> GetWSHOEmailListBySBU(Incident incident)
		{
			throw new NotImplementedException();
		}

		public Task<(string incident_ID, string error_Code)> SaveIncidents(Incident incidents)
		{
			throw new NotImplementedException();
		}

		public Task<DataSet> SearchIncidents(User user, Incident incidents)
		{
			throw new NotImplementedException();
		}

		public Task<DataSet> SearchVerfiedIncidents(User user, Incident incidents)
		{
			throw new NotImplementedException();
		}

		public Task<(string incident_ID, string error_Code)> UpdateIncidents(Incident incidents)
		{
			throw new NotImplementedException();
		}
	}
}