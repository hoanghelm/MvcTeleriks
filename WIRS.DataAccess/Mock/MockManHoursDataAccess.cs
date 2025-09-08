using System.Data;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockManHoursDataAccess : IManHoursDataAccess
	{
		public Task<DataSet> GetManHoursByYear(int YearFrom, int MonthFrom, int YearTo, int MonthTo, string sbu, int lastentrydate, string userid, string role)
		{
			throw new NotImplementedException();
		}

		public Task<string> SaveManHours(DataTable manhrsdt)
		{
			throw new NotImplementedException();
		}
	}
}