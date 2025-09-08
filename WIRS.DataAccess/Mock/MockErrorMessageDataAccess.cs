using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockErrorMessageDataAccess : IErrorMessageDataAccess
	{
		public Task<string> GetErrorMessage(string errorCode)
		{
			throw new NotImplementedException();
		}
	}
}