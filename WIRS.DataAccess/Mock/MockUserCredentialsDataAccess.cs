using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockUserCredentialsDataAccess : IUserCredentialsDataAccess
	{
		public async Task<DataSet> GetAllUserRole()
		{
			var dataSet = new DataSet();
			var table = new DataTable();
			table.Columns.Add("user_role_code", typeof(string));
			table.Columns.Add("user_role_name", typeof(string));
			
			table.Rows.Add("1", "System Administrator");
			table.Rows.Add("2", "Manager");
			table.Rows.Add("3", "User");
			table.Rows.Add("4", "Guest");
			
			dataSet.Tables.Add(table);
			return await Task.FromResult(dataSet);
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

		public async Task InactiveUsers(UserCredentials userCredentials)
		{
			// Mock inactive operation - do nothing for mock
			await Task.CompletedTask;
		}

		public async Task ResetPassword(UserCredentials userCredentials)
		{
			// Mock reset password operation - do nothing for mock
			await Task.CompletedTask;
		}

		public async Task<string> SaveUsers(UserCredentials userCredentials)
		{
			return await Task.FromResult(string.Empty);
		}

		public Task<DataSet> SeachUsers(UserCredentials userCredentials, string loginId)
		{
			throw new NotImplementedException();
		}

		public async Task<string> UpdateUsers(UserCredentials userCredentials)
		{
			// Mock successful update
			return await Task.FromResult(string.Empty);
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

		public async Task<DataSet> SearchUsers(string currentUserId, string sector, string lob, string userId, string userName, string userRole)
		{
			var dataSet = new DataSet();
			var table = new DataTable();
			
			// Define table structure
			table.Columns.Add("userid", typeof(string));
			table.Columns.Add("username", typeof(string));
			table.Columns.Add("userrole", typeof(string));
			table.Columns.Add("userrolename", typeof(string));
			table.Columns.Add("email", typeof(string));
			table.Columns.Add("accountstatus", typeof(string));
			table.Columns.Add("accountstatusname", typeof(string));
			table.Columns.Add("lastlogindate", typeof(DateTime));
			table.Columns.Add("inactivedate", typeof(DateTime));
			table.Columns.Add("sectorname", typeof(string));
			table.Columns.Add("lobname", typeof(string));
			
			// Add mock data
			table.Rows.Add("12345678", "John Doe", "1", "System Administrator", "john.doe@company.com", "1", "Active", DateTime.Now.AddDays(-1), DBNull.Value, "IT", "Technology");
			table.Rows.Add("87654321", "Jane Smith", "2", "Manager", "jane.smith@company.com", "1", "Active", DateTime.Now.AddDays(-3), DBNull.Value, "HR", "Human Resources");
			table.Rows.Add("11111111", "Bob Johnson", "3", "User", "bob.johnson@company.com", "1", "Active", DateTime.Now.AddDays(-7), DBNull.Value, "Finance", "Accounting");
			table.Rows.Add("22222222", "Alice Brown", "2", "Manager", "alice.brown@company.com", "2", "Inactive", DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-10), "IT", "Security");
			table.Rows.Add("33333333", "Charlie Wilson", "3", "User", "charlie.wilson@company.com", "1", "Active", DateTime.Now.AddDays(-2), DBNull.Value, "Operations", "Facilities");
			
			// Apply basic filtering (simplified)
			if (!string.IsNullOrEmpty(userId))
			{
				var filteredRows = table.AsEnumerable().Where(row => row["userid"].ToString().Contains(userId));
				if (filteredRows.Any())
				{
					table = filteredRows.CopyToDataTable();
				}
				else
				{
					table.Clear();
				}
			}
			
			if (!string.IsNullOrEmpty(userName))
			{
				var filteredRows = table.AsEnumerable().Where(row => row["username"].ToString().ToLower().Contains(userName.ToLower()));
				if (filteredRows.Any())
				{
					table = filteredRows.CopyToDataTable();
				}
				else
				{
					table.Clear();
				}
			}
			
			dataSet.Tables.Add(table);
			return await Task.FromResult(dataSet);
		}

		public async Task<UserCredentials?> GetUserCredentialsWithAccess(string userId)
		{
			// Mock user data - simulate database lookup
			var mockUsers = new Dictionary<string, UserCredentials>
			{
				["12345678"] = new UserCredentials
				{
					UserId = "12345678",
					UserName = "John Doe",
					Email = "john.doe@company.com",
					UserRole = "1",
					AccountStatus = "1",
					InactiveDate = "",
					Creator = "admin",
					Modifiedby = "admin",
					CreationDate = DateTime.Now.AddMonths(-6),
					LastModifyDate = DateTime.Now.AddDays(-10)
				},
				["87654321"] = new UserCredentials
				{
					UserId = "87654321",
					UserName = "Jane Smith",
					Email = "jane.smith@company.com",
					UserRole = "2",
					AccountStatus = "1",
					InactiveDate = "",
					Creator = "admin",
					Modifiedby = "john.doe",
					CreationDate = DateTime.Now.AddMonths(-3),
					LastModifyDate = DateTime.Now.AddDays(-5)
				},
				["22222222"] = new UserCredentials
				{
					UserId = "22222222",
					UserName = "Alice Brown",
					Email = "alice.brown@company.com",
					UserRole = "2",
					AccountStatus = "2",
					InactiveDate = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd"),
					Creator = "admin",
					Modifiedby = "jane.smith",
					CreationDate = DateTime.Now.AddMonths(-8),
					LastModifyDate = DateTime.Now.AddDays(-10)
				}
			};
			
			if (mockUsers.TryGetValue(userId, out var user))
			{
				// Create mock user access data
				var userAccessTable = new DataTable();
				userAccessTable.Columns.Add("ua_user_role_code", typeof(string));
				userAccessTable.Columns.Add("user_role_name", typeof(string));
				userAccessTable.Columns.Add("sba_code", typeof(string));
				userAccessTable.Columns.Add("sba_value", typeof(string));
				userAccessTable.Columns.Add("sbu_code", typeof(string));
				userAccessTable.Columns.Add("sbu_value", typeof(string));
				userAccessTable.Columns.Add("department_code", typeof(string));
				userAccessTable.Columns.Add("department_value", typeof(string));
				userAccessTable.Columns.Add("location_code", typeof(string));
				userAccessTable.Columns.Add("location_value", typeof(string));
				
				// Add mock access records based on user
				switch (userId)
				{
					case "12345678": // John Doe - System Admin
						userAccessTable.Rows.Add("1", "System Administrator", "IT", "Information Technology", "TECH", "Technology", "IT01", "IT Support", "HQ", "Headquarters");
						userAccessTable.Rows.Add("1", "System Administrator", "SEC", "Security", "SEC01", "Cyber Security", "SEC01", "Security Operations", "HQ", "Headquarters");
						break;
					case "87654321": // Jane Smith - Manager
						userAccessTable.Rows.Add("2", "Manager", "HR", "Human Resources", "HR01", "Human Resources", "HR01", "HR Operations", "HQ", "Headquarters");
						break;
					case "22222222": // Alice Brown - Inactive Manager
						userAccessTable.Rows.Add("2", "Manager", "IT", "Information Technology", "SEC", "Security", "SEC01", "Security Operations", "HQ", "Headquarters");
						break;
				}
				
				user.UserAccess = userAccessTable;
				return await Task.FromResult(user);
			}
			
			return await Task.FromResult<UserCredentials?>(null);
		}
	}
}