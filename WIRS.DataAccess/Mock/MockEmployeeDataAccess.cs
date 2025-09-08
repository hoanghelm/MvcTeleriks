using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
    public class MockEmployeeDataAccess : IEmployeeDataAccess
    {
		public Task<DataSet> GetEmployeeByName(Employee employee)
		{
			throw new NotImplementedException();
		}

		public Task<(string emailAddress, string userRoleName, string errorCode)> GetEmployeeEmailAddress(Employee employee)
		{
			throw new NotImplementedException();
		}

		public void ThrowNotImplemented(string methodName)
        {
            throw new NotImplementedException($"MockEmployeeDataAccess.{methodName} is not implemented. This method is not used in the current services.");
        }

		public Task<string> ValidateEmployee(Employee employee)
		{
			throw new NotImplementedException();
		}

		public Task<string> ValidateNextUser(string empNo)
		{
			throw new NotImplementedException();
		}
	}
}