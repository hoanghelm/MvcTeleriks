using System.Data;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockPrintDataAccess : IPrintDataAccess
	{
		public Task<DataSet> PrintIncident(string incidentID)
		{
			throw new NotImplementedException();
		}

		public Task<DataSet> PrintIncidentType(string incidentID)
		{
			throw new NotImplementedException();
		}
	}
}