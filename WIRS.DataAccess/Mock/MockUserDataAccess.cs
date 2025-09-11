using System;
using System.Data;
using System.Threading.Tasks;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockUserDataAccess : IUserDataAccess
	{
		public async Task<DataSet> GetInfoByUserID(string userID, string userrole)
		{
			var dataSet = new DataSet();

			var incidentsTable = new DataTable("Table");
			incidentsTable.Columns.Add("incident_id", typeof(string));
			incidentsTable.Columns.Add("incident_datetime", typeof(string));
			incidentsTable.Columns.Add("sba_code", typeof(string));
			incidentsTable.Columns.Add("sba_name", typeof(string));
			incidentsTable.Columns.Add("sbu_code", typeof(string));
			incidentsTable.Columns.Add("sbu_name", typeof(string));
			incidentsTable.Columns.Add("department", typeof(string));
			incidentsTable.Columns.Add("department_name", typeof(string));
			incidentsTable.Columns.Add("location", typeof(string));
			incidentsTable.Columns.Add("incidentdesc", typeof(string));
			incidentsTable.Columns.Add("creator_name", typeof(string));
			incidentsTable.Columns.Add("submitted_on", typeof(string));
			incidentsTable.Columns.Add("statusdesc", typeof(string));
			incidentsTable.Columns.Add("status", typeof(string));

			incidentsTable.Rows.Add("STEA-20180001", "03-Jan-2018 09:30 AM", "0001", "Aerospace", "0003", "ST Engineering Defence Aviation Services", "0001", "CAS", "0001", "Injuries", "DANG VAN HOANG JEREMY", "07-Aug-2018 10:06 AM", "Closed", "08");
			incidentsTable.Rows.Add("STEA-20190018", "11-Jun-2019 01:25 PM", "0001", "Aerospace", "0007", "ST Engineering Aerospace Supplies Pte. L", "0001", "Aviation Division (AD)", "0005", "Injuries", "DANG VAN HOANG JEREMY", "12-Jun-2019 02:49 PM", "Closed", "08");
			incidentsTable.Rows.Add("STEL-20210009", "11-May-2021 10:45 AM", "0004", "Land Systems", "0034", "KDM- Kinetics Design Manufacturing", "0203", "GMO", "0005", "Injuries", "DANG VAN HOANG JEREMY", "28-May-2021 03:37 PM", "Closed", "08");
			incidentsTable.Rows.Add("STEL-20250006", "11-Mar-2025 07:30 PM", "0004", "Land Systems", "0026", "ST Engineering Synthesis Pte. Ltd.", "", "", "", "Injuries", "DANG VAN HOANG JEREMY", "11-Mar-2025 03:30 PM", "Open", "01");
			incidentsTable.Rows.Add("STEL-20250007", "11-Mar-2025 07:30 PM", "0004", "Land Systems", "0026", "ST Engineering Synthesis Pte. Ltd.", "", "", "", "Injuries ,Injuries", "DANG VAN HOANG JEREMY", "11-Mar-2025 04:04 PM", "Pending", "02");

			dataSet.Tables.Add(incidentsTable);

			var pendingTable = new DataTable("PendingTable");
			pendingTable.Columns.Add("incident_id", typeof(string));
			pendingTable.Columns.Add("incident_datetime", typeof(string));
			pendingTable.Columns.Add("sba_code", typeof(string));
			pendingTable.Columns.Add("sba_name", typeof(string));
			pendingTable.Columns.Add("sbu_code", typeof(string));
			pendingTable.Columns.Add("sbu_name", typeof(string));
			pendingTable.Columns.Add("department", typeof(string));
			pendingTable.Columns.Add("department_name", typeof(string));
			pendingTable.Columns.Add("location", typeof(string));
			pendingTable.Columns.Add("incidentdesc", typeof(string));
			pendingTable.Columns.Add("creator_name", typeof(string));
			pendingTable.Columns.Add("submitted_on", typeof(string));
			pendingTable.Columns.Add("statusdesc", typeof(string));
			pendingTable.Columns.Add("status", typeof(string));

			pendingTable.Rows.Add("STEL-20250006", "11-Mar-2025 07:30 PM", "0004", "Land Systems", "0026", "ST Engineering Synthesis Pte. Ltd.", "", "", "", "Injuries", "DANG VAN HOANG JEREMY", "11-Mar-2025 03:30 PM", "Open", "01");
			pendingTable.Rows.Add("STEL-20250007", "11-Mar-2025 07:30 PM", "0004", "Land Systems", "0026", "ST Engineering Synthesis Pte. Ltd.", "", "", "", "Injuries ,Injuries", "DANG VAN HOANG JEREMY", "11-Mar-2025 04:04 PM", "Pending", "02");
			pendingTable.Rows.Add("STEL-20250008", "11-Mar-2025 07:30 PM", "0004", "Land Systems", "0026", "ST Engineering Synthesis Pte. Ltd.", "", "", "", "Injuries ,Injuries", "DANG VAN HOANG JEREMY", "11-Mar-2025 04:05 PM", "Under Review", "03");

			dataSet.Tables.Add(pendingTable);

			return await Task.FromResult(dataSet);
		}

		public async Task<bool> ValidateUsers(string userid, string pw)
		{
			// Mock admin user
			return await Task.FromResult(userid == "admin" && pw == "admin");
		}

		public async Task<User> GetUserByUserID(string userId)
		{
			if (userId == "admin")
			{
				return await Task.FromResult(new User
				{
					UserId = "admin",
					UserName = "Administrator",
					UserRole = "9", // Admin role
					AccountStatus = "A",
					UnsuccessfulLogin = 0,
					sbaname = "System Administration"
				});
			}

			return await Task.FromResult(new User
			{
				UserId = userId,
				UserName = $"User {userId}",
				UserRole = "1", // Regular user
				AccountStatus = "A",
				UnsuccessfulLogin = 0,
				sbaname = "General Department"
			});
		}

		public async Task<int> LockUser(string userId, string LoginID)
		{
			return await Task.FromResult(1);
		}

		public async Task<User> GetUserByUserEIP(string userId)
		{
			return await GetUserByUserID(userId);
		}

		public async Task<bool> NeedChangePassword(object userId)
		{
			return await Task.FromResult(false);
		}

		public async Task<bool> CheckPasswordHasUsed(string userId, string encodedPassword)
		{
			return await Task.FromResult(false);
		}

		public async Task UserChangePassword(string userId, string encodedPassword, string PasswordHistory)
		{
			await Task.CompletedTask;
		}

		public async Task<bool> CheckUserExists(string userid)
		{
			return await Task.FromResult(true);
		}

		public async Task<DataSet> SearchEmployee(string empid, string empname, string empcostcentreno,
			string empcostcentrename, string empsbuname, string incident_id, int? pageNo, int? pageSize)
		{
			var dataSet = new DataSet();
			var table = new DataTable();
			table.Columns.Add("employee_id", typeof(string));
			table.Columns.Add("employee_name", typeof(string));
			table.Columns.Add("cost_center_no", typeof(string));
			table.Columns.Add("cost_center_name", typeof(string));
			table.Columns.Add("sbu_name", typeof(string));

			table.Rows.Add("E001", "John Doe", "CC001", "IT Cost Center", "IT Department");
			table.Rows.Add("E002", "Jane Smith", "CC002", "HR Cost Center", "HR Department");

			dataSet.Tables.Add(table);
			return await Task.FromResult(dataSet);
		}

		public async Task<DataSet> SearchUsersInfo(string empid, string empname, string empcostcentreno,
			string empcostcentrename, string empsbuname, string loginid)
		{
			return await SearchEmployee(empid, empname, empcostcentreno, empcostcentrename, empsbuname, null, null, null);
		}

		public async Task<DataSet> GetEmployeeInfoByEmployeeNo(string empid)
		{
			var dataSet = new DataSet();
			var table = new DataTable();
			table.Columns.Add("employee_id", typeof(string));
			table.Columns.Add("employee_name", typeof(string));
			table.Columns.Add("department", typeof(string));

			table.Rows.Add(empid, $"Employee {empid}", "Mock Department");
			dataSet.Tables.Add(table);

			return await Task.FromResult(dataSet);
		}

		#region Mock methods for HOD, AHOD, etc.
		public async Task<DataSet> get_hod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("HOD");
		}

		public async Task<DataSet> get_ahod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("AHOD");
		}

		public async Task<DataSet> get_h_hod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("H-HOD");
		}

		public async Task<DataSet> get_hrlist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("HR");
		}

		public async Task<DataSet> get_wsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("WSHO");
		}

		public async Task<DataSet> get_awsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("AWSHO");
		}

		public async Task<DataSet> get_c_wsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("C-WSHO");
		}

		public async Task<DataSet> get_active_cclist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("CC");
		}

		public async Task<DataSet> get_partA_cclist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("PartA-CC");
		}

		public async Task<DataSet> get_all_copyto_list_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
		{
			return await CreateMockEmployeeDataSet("CopyTo");
		}

		public async Task<DataSet> get_all_copyto_list_by_uid(string sba_code, string sbu_code, string department_code,
			string location_code, string user_id)
		{
			return await CreateMockEmployeeDataSet("CopyTo");
		}

		public async Task<string> insert_copyto_list(string sba_code, string sbu_code, string department_code,
			string location_code, string user_id, string inactive_date, string modified_by)
		{
			return await Task.FromResult("Success");
		}

		public async Task<string> update_copyto_list(string sba_code, string sbu_code, string department_code,
			string location_code, string user_id, string inactive_date, string modified_by)
		{
			return await Task.FromResult("Success");
		}

		public async Task LoginADUser(string userId)
		{
			await Task.CompletedTask;
		}

		private async Task<DataSet> CreateMockEmployeeDataSet(string roleType)
		{
			var dataSet = new DataSet();
			var table = new DataTable();
			table.Columns.Add("employee_id", typeof(string));
			table.Columns.Add("employee_name", typeof(string));
			table.Columns.Add("role", typeof(string));

			table.Rows.Add("E001", $"Mock {roleType} User", roleType);
			dataSet.Tables.Add(table);

			return await Task.FromResult(dataSet);
		}
		#endregion
	}
}