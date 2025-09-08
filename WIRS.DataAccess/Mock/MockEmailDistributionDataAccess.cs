using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockEmailDistributionDataAccess : IEmailDistributionDataAccess
	{
		public Task<string> GetEmailUsers(EmailDistribution emailBE)
		{
			throw new NotImplementedException();
		}

		public Task<string> SaveEmailUsers(EmailDistribution emailBE)
		{
			throw new NotImplementedException();
		}

		public Task<DataSet> SeachEmailUsers(EmailDistribution emailBE)
		{
			throw new NotImplementedException();
		}

		public Task<string> UpdateEmailUsers(EmailDistribution emailBE)
		{
			throw new NotImplementedException();
		}
	}
}