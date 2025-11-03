using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WIRS.DataAccess.Implementations;
using WIRS.DataAccess.Interfaces;
using WIRS.DataAccess.Entities;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Services.Implementations
{
	public class UserService : IUserService
	{
		private readonly IUserDataAccess _userDataAccess;
		private readonly IUserCredentialsDataAccess _userCredentialsDataAccess;
		private readonly IDataMapperService _dataMapper;

		public UserService(IUserDataAccess userDataAccess, IUserCredentialsDataAccess userCredentialsDataAccess, IDataMapperService dataMapper)
		{
			_userDataAccess = userDataAccess;
			_userCredentialsDataAccess = userCredentialsDataAccess;
			_dataMapper = dataMapper;
		}

		public async Task<UserModel?> GetUserById(string userId)
		{
			try
			{
				var userBE = await _userDataAccess.GetUserByUserID(userId);

				if (userBE?.UserId == null) return null;

				return new UserModel
				{
					UserId = userBE.UserId,
					UserName = userBE.UserName,
					UserRole = userBE.UserRole,
					AccountStatus = userBE.AccountStatus,
					UnsuccessfulLogin = userBE.UnsuccessfulLogin ?? 0,
					SbaName = userBE.sbaname ?? string.Empty
				};
			}
			catch
			{
				return null;
			}
		}

		public async Task<UserModel?> GetUserByEIP(string eipId)
		{
			try
			{
				var userBE = await _userDataAccess.GetUserByUserEIP(eipId);

				if (userBE?.UserId == null) return null;

				return new UserModel
				{
					UserId = userBE.UserId,
					UserName = userBE.UserName,
					UserRole = userBE.UserRole,
					AccountStatus = userBE.AccountStatus,
					UnsuccessfulLogin = userBE.UnsuccessfulLogin ?? 0,
					SbaName = userBE.sbaname ?? string.Empty
				};
			}
			catch
			{
				return null;
			}
		}

		public async Task<List<PartACopyToItem>> GetPartACopyToList(string sba_code, string sbu_code, string department_code, string location_code)
		{
			try
			{
				var result = new List<PartACopyToItem>();
				var dataSet = await _userDataAccess.get_all_copyto_list_by_sbu(sba_code, sbu_code, department_code, location_code);

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Add(new PartACopyToItem
						{
							Id = row["empid"]?.ToString() ?? string.Empty,
							Name = row["empname"]?.ToString() ?? string.Empty
						});
					}
				}

				return result;
			}
			catch
			{
				return new List<PartACopyToItem>();
			}
		}

		public async Task<List<UserItem>> GetWSHOs(string sba_code, string sbu_code, string department_code, string location_code)
		{
			try
			{
				var result = new List<UserItem>();
				var dataSet = await _userDataAccess.get_wsho_by_sbu(sba_code, sbu_code, department_code, location_code);

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Add(new UserItem
						{
							Id = row["empid"]?.ToString() ?? string.Empty,
							Name = row["empname"]?.ToString() ?? string.Empty
						});
					}
				}

				return result;
			}
			catch
			{
				return new List<UserItem>();
			}
		}

		public async Task<List<UserItem>> GetHODs(string sba_code, string sbu_code, string department_code, string location_code)
		{
			try
			{
				var result = new List<UserItem>();
				var dataSet = await _userDataAccess.get_hod_by_sbu(sba_code, sbu_code, department_code, location_code);

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Add(new UserItem
						{
							Id = row["empid"]?.ToString() ?? string.Empty,
							Name = row["empname"]?.ToString() ?? string.Empty
						});
					}
				}

				return result;
			}
			catch
			{
				return new List<UserItem>();
			}
		}

		public async Task<List<UserItem>> GetAHODs(string sba_code, string sbu_code, string department_code, string location_code)
		{
			try
			{
				var result = new List<UserItem>();
				var dataSet = await _userDataAccess.get_ahod_by_sbu(sba_code, sbu_code, department_code, location_code);

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Add(new UserItem
						{
							Id = row["empid"]?.ToString() ?? string.Empty,
							Name = row["empname"]?.ToString() ?? string.Empty
						});
					}
				}

				return result;
			}
			catch
			{
				return new List<UserItem>();
			}
		}

		public async Task<List<UserItem>> GetCWSHOs(string sba_code, string sbu_code, string department_code, string location_code)
		{
			try
			{
				var result = new List<UserItem>();
				var dataSet = await _userDataAccess.get_c_wsho_by_sbu(sba_code, sbu_code, department_code, location_code);

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Add(new UserItem
						{
							Id = row["empid"]?.ToString() ?? string.Empty,
							Name = row["empname"]?.ToString() ?? string.Empty
						});
					}
				}

				return result;
			}
			catch
			{
				return new List<UserItem>();
			}
		}

		public async Task<List<UserItem>> GetHSBUs(string sba_code, string sbu_code, string department_code, string location_code)
		{
			try
			{
				var result = new List<UserItem>();
				var dataSet = await _userDataAccess.get_h_hod_by_sbu(sba_code, sbu_code, department_code, location_code);

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Add(new UserItem
						{
							Id = row["empid"]?.ToString() ?? string.Empty,
							Name = row["empname"]?.ToString() ?? string.Empty
						});
					}
				}

				return result;
			}
			catch
			{
				return new List<UserItem>();
			}
		}

		public async Task<List<UserItem>> GetAWSHOs(string sba_code, string sbu_code, string department_code, string location_code)
		{
			try
			{
				var result = new List<UserItem>();
				var dataSet = await _userDataAccess.get_awsho_by_sbu(sba_code, sbu_code, department_code, location_code);

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Add(new UserItem
						{
							Id = row["empid"]?.ToString() ?? string.Empty,
							Name = row["empname"]?.ToString() ?? string.Empty
						});
					}
				}

				return result;
			}
			catch
			{
				return new List<UserItem>();
			}
		}

		public async Task<bool> CheckUserExists(string userId)
		{
			try
			{
				return await _userDataAccess.CheckUserExists(userId);
			}
			catch
			{
				return false;
			}
		}

		public async Task<UserCreationModel?> ValidateUserExists(string userId)
		{
			try
			{
				var dataSet = await _userDataAccess.GetEmployeeInfoByEmployeeNo(userId);

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					var row = dataSet.Tables[0].Rows[0];
					return new UserCreationModel
					{
						UserId = row["empid"]?.ToString() ?? string.Empty,
						UserName = row["empname"]?.ToString() ?? string.Empty,
						Email = row["empemailaddress"]?.ToString() ?? string.Empty
					};
				}

				return null;
			}
			catch
			{
				return null;
			}
		}

		public async Task<string> CreateUser(UserCreationRequest request, string creatorId)
		{
			try
			{
				var userCredentials = new UserCredentials
				{
					UserId = request.UserId,
					UserName = request.UserName,
					Email = request.Email,
					UserRole = request.UserRole,
					Creator = creatorId,
					CreationDate = DateTime.Now
				};

				var userAccessTable = new DataTable();
				userAccessTable.TableName = "wirs_user_access";
				userAccessTable.Columns.Add(new DataColumn("user_role_code", typeof(string)));
				userAccessTable.Columns.Add(new DataColumn("sba_code", typeof(string)));
				userAccessTable.Columns.Add(new DataColumn("sbu_code", typeof(string)));
				userAccessTable.Columns.Add(new DataColumn("department_code", typeof(string)));
				userAccessTable.Columns.Add(new DataColumn("location_code", typeof(string)));

				foreach (var access in request.UserAccess)
				{
					var row = userAccessTable.NewRow();
					row["user_role_code"] = access.UserRoleCode ?? request.UserRole;
					row["sba_code"] = access.SectorCode;
					row["sbu_code"] = string.IsNullOrEmpty(access.LobCode) ? DBNull.Value : access.LobCode;
					row["department_code"] = string.IsNullOrEmpty(access.DepartmentCode) ? DBNull.Value : access.DepartmentCode;
					row["location_code"] = string.IsNullOrEmpty(access.LocationCode) ? DBNull.Value : access.LocationCode;
					userAccessTable.Rows.Add(row);
				}

				userCredentials.UserAccess = userAccessTable;

				var result = await _userCredentialsDataAccess.SaveUsers(userCredentials);
				return result;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public async Task<EmployeeSearchResult> SearchEmployees(string empId, string empName, int pageNo = 1, int pageSize = 10)
		{
			try
			{
				var dataSet = await _userDataAccess.SearchEmployee(
					empId, empName, string.Empty, string.Empty, string.Empty, string.Empty, pageNo, pageSize);

				var result = new EmployeeSearchResult
				{
					PageNo = pageNo,
					PageSize = pageSize
				};

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Employees.Add(new EmployeeInfo
						{
							EmployeeId = row["empid"]?.ToString() ?? string.Empty,
							EmployeeName = row["empname"]?.ToString() ?? string.Empty,
							CostCentreNo = row["empcostcentreno"]?.ToString() ?? string.Empty,
							CostCentreName = row["empcostcentrename"]?.ToString() ?? string.Empty,
							Designation = row["empdesignation"]?.ToString() ?? string.Empty,
							Email = row["empemailaddress"]?.ToString() ?? string.Empty,
							ContactNo = row["empcontactno"]?.ToString() ?? string.Empty,
							Age = row["empage"]?.ToString() ?? string.Empty,
							Race = row["emprace"]?.ToString() ?? string.Empty,
							Nationality = row["empnationality"]?.ToString() ?? string.Empty,
							Gender = row["empgender"]?.ToString() ?? string.Empty,
							EmploymentType = row["empemploymenttype"]?.ToString() ?? string.Empty,
							DateOfEmployment = row["empjoineddate"]?.ToString() ?? string.Empty,
						});
					}

					result.TotalCount = result.Employees.Count;
				}

				return result;
			}
			catch
			{
				return new EmployeeSearchResult { PageNo = pageNo, PageSize = pageSize };
			}
		}

		public async Task<UserListResult> SearchUsers(string currentUserId, string sector, string lob, string userId, string userName, string userRole)
		{
			try
			{
				var dataSet = await _userCredentialsDataAccess.SearchUsers(currentUserId, sector, lob, userId, userName, userRole);
				var result = new UserListResult();

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Users.Add(new UserListItem
						{
							UserId = row["user_id"]?.ToString() ?? string.Empty,
							UserName = row["user_name"]?.ToString() ?? string.Empty,
							UserRoleName = row["user_role_name"]?.ToString() ?? string.Empty,
							Email = row["email_address"]?.ToString() ?? string.Empty,
							AccountStatusName = row["statusdesc"]?.ToString() ?? string.Empty
						});
					}

					result.TotalCount = result.Users.Count;
				}

				return result;
			}
			catch
			{
				return new UserListResult();
			}
		}

		public async Task<UserDetailsModel?> GetUserDetails(string userId)
		{
			try
			{
				UserCredentials user = new UserCredentials() { UserId = userId };

				var errorCode = await _userCredentialsDataAccess.GetUser(user);

				if (!string.IsNullOrEmpty(errorCode))
				{
					return null;
				}

				var userDetails = new UserDetailsModel
				{
					UserId = user.UserId,
					UserName = user.UserName,
					Email = user.Email,
					UserRole = user.UserRole,
					AccountStatus = user.AccountStatus,
					Creator = user.Creator,
					Modifier = user.Modifiedby,
					CreationDate = user.CreationDate,
					ModificationDate = user.LastModifyDate
				};

				if (!string.IsNullOrEmpty(user.InactiveDate))
				{
					if (DateTime.TryParse(user.InactiveDate, out var inactiveDate))
					{
						userDetails.InactiveDate = inactiveDate;
						userDetails.InactiveDateString = inactiveDate.ToString("dd-MMM-yyyy");
					}
				}

				if (user.UserAccess != null && user.UserAccess.Rows.Count > 0)
				{
					foreach (DataRow row in user.UserAccess.Rows)
					{
						userDetails.UserAccess.Add(new UserAccessDetails
						{
							UserRoleCode = row["ua_user_role_code"]?.ToString() ?? string.Empty,
							UserRoleName = row["user_role_name"]?.ToString() ?? string.Empty,
							SectorCode = row["sba_code"]?.ToString() ?? string.Empty,
							SectorValue = row["sba_value"]?.ToString() ?? string.Empty,
							LOBCode = row["sbu_code"]?.ToString() ?? string.Empty,
							LOBValue = row["sbu_value"]?.ToString() ?? string.Empty,
							DepartmentCode = row["department_code"]?.ToString() ?? string.Empty,
							DepartmentValue = row["department_value"]?.ToString() ?? string.Empty,
							LocationCode = row["location_code"]?.ToString() ?? string.Empty,
							LocationValue = row["location_value"]?.ToString() ?? string.Empty
						});
					}
				}

				return userDetails;
			}
			catch
			{
				return null;
			}
		}

		public async Task<string> UpdateUser(UserUpdateRequest request, string modifierUserId)
		{
			try
			{
				var userCredentials = new UserCredentials
				{
					UserId = request.UserId,
					Email = request.Email,
					UserRole = request.UserRole,
					AccountStatus = request.AccountStatus,
					InactiveDate = !string.IsNullOrEmpty(request.InactiveDate) ? request.InactiveDate : string.Empty,
					Modifiedby = modifierUserId,
					LastModifyDate = DateTime.Now
				};

				var userAccessTable = new DataTable();
				userAccessTable.TableName = "wirs_user_access";
				userAccessTable.Columns.Add(new DataColumn("user_role_code", typeof(string)));
				userAccessTable.Columns.Add(new DataColumn("sba_code", typeof(string)));
				userAccessTable.Columns.Add(new DataColumn("sbu_code", typeof(string)));
				userAccessTable.Columns.Add(new DataColumn("department_code", typeof(string)));
				userAccessTable.Columns.Add(new DataColumn("location_code", typeof(string)));

				foreach (var access in request.UserAccess)
				{
					var row = userAccessTable.NewRow();
					row["user_role_code"] = access.UserRoleCode ?? request.UserRole;
					row["sba_code"] = access.SectorCode;
					row["sbu_code"] = string.IsNullOrEmpty(access.LobCode) ? DBNull.Value : access.LobCode;
					row["department_code"] = string.IsNullOrEmpty(access.DepartmentCode) ? DBNull.Value : access.DepartmentCode;
					row["location_code"] = string.IsNullOrEmpty(access.LocationCode) ? DBNull.Value : access.LocationCode;
					userAccessTable.Rows.Add(row);
				}

				userCredentials.UserAccess = userAccessTable;

				var result = await _userCredentialsDataAccess.UpdateUsers(userCredentials);
				return result;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public async Task<string> InactiveUser(string userId, string modifierUserId)
		{
			try
			{
				var userCredentials = new UserCredentials
				{
					UserId = userId,
					Modifiedby = modifierUserId,
					LastModifyDate = DateTime.Now
				};

				await _userCredentialsDataAccess.InactiveUsers(userCredentials);
				return string.Empty;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public async Task<string> ResetPassword(string userId, string modifierUserId)
		{
			try
			{
				var userCredentials = new UserCredentials
				{
					UserId = userId,
					Modifiedby = modifierUserId,
					LastModifyDate = DateTime.Now
				};

				await _userCredentialsDataAccess.ResetPassword(userCredentials);
				return string.Empty;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}