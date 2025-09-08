using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockStatisticsReportDataAccess : IStatisticsReportDataAccess
	{
		public Task<DataSet> GetStatisticsReport(StatisticsReport statisticsBE)
		{
			throw new NotImplementedException();
		}
	}
}