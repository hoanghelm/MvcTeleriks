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
							Nric = row["empnric"]?.ToString() ?? string.Empty,
							Age = row["empage"]?.ToString() ?? string.Empty,
							Race = row["emprace"]?.ToString() ?? string.Empty,
							Nationality = row["empnationality"]?.ToString() ?? string.Empty,
							Gender = row["empgender"]?.ToString() ?? string.Empty,
							EmploymentType = row["empemploymenttype"]?.ToString() ?? string.Empty,
							DateOfEmployment = row["empdateofemployment"]?.ToString() ?? string.Empty
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

		public async Task<UserListResult> SearchUsers(string sector, string lob, string userId, string userName, string userRole)
		{
			try
			{
				var dataSet = await _userCredentialsDataAccess.SearchUsers(sector, lob, userId, userName, userRole);
				var result = new UserListResult();

				if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						result.Users.Add(new UserListItem
						{
							UserId = row["userid"]?.ToString() ?? string.Empty,
							UserName = row["username"]?.ToString() ?? string.Empty,
							UserRole = row["userrole"]?.ToString() ?? string.Empty,
							UserRoleName = row["userrolename"]?.ToString() ?? string.Empty,
							Email = row["email"]?.ToString() ?? string.Empty,
							AccountStatus = row["accountstatus"]?.ToString() ?? string.Empty,
							AccountStatusName = row["accountstatusname"]?.ToString() ?? string.Empty,
							LastLoginDate = row["lastlogindate"] != DBNull.Value ? Convert.ToDateTime(row["lastlogindate"]) : null,
							InactiveDate = row["inactivedate"] != DBNull.Value ? Convert.ToDateTime(row["inactivedate"]) : null,
							SectorName = row["sectorname"]?.ToString() ?? string.Empty,
							LOBName = row["lobname"]?.ToString() ?? string.Empty
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
				var userCredentials = await _userCredentialsDataAccess.GetUserCredentialsWithAccess(userId);
				
				if (userCredentials == null) return null;

				var userDetails = new UserDetailsModel
				{
					UserId = userCredentials.UserId,
					UserName = userCredentials.UserName,
					Email = userCredentials.Email,
					UserRole = userCredentials.UserRole,
					AccountStatus = userCredentials.AccountStatus,
					Creator = userCredentials.Creator,
					Modifier = userCredentials.Modifiedby,
					CreationDate = userCredentials.CreationDate,
					ModificationDate = userCredentials.LastModifyDate
				};

				if (!string.IsNullOrEmpty(userCredentials.InactiveDate))
				{
					if (DateTime.TryParse(userCredentials.InactiveDate, out var inactiveDate))
					{
						userDetails.InactiveDate = inactiveDate;
						userDetails.InactiveDateString = inactiveDate.ToString("dd-MMM-yyyy");
					}
				}

				// Get user access details
				if (userCredentials.UserAccess != null && userCredentials.UserAccess.Rows.Count > 0)
				{
					foreach (DataRow row in userCredentials.UserAccess.Rows)
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
					InactiveDate = request.InactiveDate,
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
				return string.Empty; // Success
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
				return string.Empty; // Success
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}