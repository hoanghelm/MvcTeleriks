using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockUserCredentialsDataAccess : IUserCredentialsDataAccess
	{
		public Task<DataSet> GetAllUserRole()
		{
			throw new NotImplementedException();
		}

		public Task<string> GetUser(UserCredentials userCredentials)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetUserRoleName(string userRoleCode)
		{
			throw new NotImplementedException();
		}

		public Task<(DataSet accessList, string errorCode)> GetUsersAccessSBU(string userId, string userRole, string sbaCode)
		{
			throw new NotImplementedException();
		}

		public Task InactiveUsers(UserCredentials userCredentials)
		{
			throw new NotImplementedException();
		}

		public Task ResetPassword(UserCredentials userCredentials)
		{
			throw new NotImplementedException();
		}

		public Task<string> SaveUsers(UserCredentials userCredentials)
		{
			throw new NotImplementedException();
		}

		public Task<DataSet> SeachUsers(UserCredentials userCredentials, string loginId)
		{
			throw new NotImplementedException();
		}

		public Task<string> UpdateUsers(UserCredentials userCredentials)
		{
			throw new NotImplementedException();
		}

		public Task ValidateUserExists(UserCredentials userCredentials)
		{
			throw new NotImplementedException();
		}

		public Task<string> ValidateUserResign(UserCredentials userCredentials)
		{
			throw new NotImplementedException();
		}

		public Task<string> ValidatUserName(UserCredentials userCredentials)
		{
			throw new NotImplementedException();
		}
	}
}