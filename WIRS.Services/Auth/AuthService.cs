using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using WIRS.DataAccess.Interfaces;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;
using WIRS.Shared.Configuration;
using WIRS.Shared.Enums;
using WIRS.Shared.Extensions;
using WIRS.Shared.Models;

namespace WIRS.Services.Auth
{
	public class AuthService : IAuthService
	{
		private readonly IUserDataAccess _userDataAccess;
		private readonly ICommonFunDataAccess _commonFunDataAccess;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly AppSettings _appSettings;
		private const string USER_SESSION_KEY = "UserSession";

		public AuthService(
			IUserDataAccess userDataAccess,
			ICommonFunDataAccess commonFunDataAccess,
			IHttpContextAccessor httpContextAccessor,
			IOptions<AppSettings> appSettings)
		{
			_userDataAccess = userDataAccess;
			_commonFunDataAccess = commonFunDataAccess;
			_httpContextAccessor = httpContextAccessor;
			_appSettings = appSettings.Value;
		}

		public async Task<LoginResult> ValidateUserAsync(string userId, string password)
		{
			try
			{
				var isValid = await _userDataAccess.ValidateUsers(userId, password);

				if (!isValid)
				{
					var userBE = await _userDataAccess.GetUserByUserID(userId);
					if (userBE?.UserId != null)
					{
						if (userBE.UnsuccessfulLogin > _appSettings.MaxLoginError)
						{
							await _userDataAccess.LockUser(userId, userId);
							return new LoginResult { ErrorType = LoginErrorType.AccountLocked };
						}

						return userBE.AccountStatus switch
						{
							"X" => new LoginResult { ErrorType = LoginErrorType.AccountInactive },
							"L" => new LoginResult { ErrorType = LoginErrorType.AccountLocked },
							_ => new LoginResult { ErrorType = LoginErrorType.InvalidCredentials }
						};
					}
					return new LoginResult { ErrorType = LoginErrorType.NoAccess };
				}

				var validUser = await GetUserModelFromDataAccess(userId);
				if (validUser == null)
				{
					return new LoginResult { ErrorType = LoginErrorType.NoAccess };
				}

				if (validUser.AccountStatus == "L")
				{
					return new LoginResult { ErrorType = LoginErrorType.AccountLocked };
				}

				if (validUser.AccountStatus == "X")
				{
					return new LoginResult { ErrorType = LoginErrorType.AccountInactive };
				}

				var requiresPasswordChange = await _userDataAccess.NeedChangePassword(userId);

				return new LoginResult
				{
					IsSuccess = true,
					User = validUser,
					RequiresPasswordChange = requiresPasswordChange
				};
			}
			catch (Exception ex)
			{
				return new LoginResult
				{
					ErrorType = LoginErrorType.SystemError,
					ErrorMessage = ex.Message
				};
			}
		}

		public async Task<bool> ValidateSSO(string loginId, string digest)
		{
			try
			{
				var cleanDigest = digest.RemoveSpecialCharacters();
				var configData = await _commonFunDataAccess.get_config_data("SSOKEY");

				if (configData?.Tables?.Count > 0 && configData.Tables[0].Rows.Count > 0)
				{
					var key = configData.Tables[0].Rows[0]["config_value"].ToString();
					return SSOExtensions.sso_sunburstconnect(loginId, key, cleanDigest);
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		public async Task SignInUserAsync(UserModel user, HttpContext context)
		{
			var userRole = UserRoleExtensions.FromString(user.UserRole);
			var permissions = await GetUserPermissionsAsync(user.UserId, user.UserRole);

			var userSession = new UserSession
			{
				UserId = user.UserId,
				UserName = user.UserName,
				UserRole = userRole,
				SbaName = user.SbaName,
				SbuName = user.SbuName,
				Designation = user.Designation,
				Permissions = permissions
			};

			await SetCurrentUserAsync(userSession);

			var claims = new List<Claim>
			{
				new(ClaimTypes.Name, user.UserId),
				new(ClaimTypes.Role, user.UserRole),
				new("UserName", user.UserName),
				new("UserRole", user.UserRole)
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authProperties = new AuthenticationProperties
			{
				IsPersistent = false,
				ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
			};

			await context.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity),
				authProperties);
		}

		public async Task SignOutAsync(HttpContext context)
		{
			await ClearCurrentUserAsync();
			await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			context.Session.Clear();
		}

		public async Task<UserSession?> GetCurrentUserAsync()
		{
			var session = _httpContextAccessor.HttpContext?.Session;
			if (session == null) return null;

			var sessionData = session.GetString(USER_SESSION_KEY);
			if (string.IsNullOrEmpty(sessionData)) return null;

			return await Task.FromResult(JsonConvert.DeserializeObject<UserSession>(sessionData));
		}

		public async Task<bool> IsAuthenticatedAsync()
		{
			var currentUser = await GetCurrentUserAsync();
			return currentUser != null;
		}

		public async Task<UserModel> GetCurrentUserModelAsync()
		{
			var userSession = await GetCurrentUserAsync();
			if (userSession == null) return new UserModel();

			return new UserModel
			{
				UserId = userSession.UserId,
				UserName = userSession.UserName,
				UserRole = ((int)userSession.UserRole).ToString(),
				SbaName = userSession.SbaName
			};
		}

		public async Task<string> GetCurrentUserIdAsync()
		{
			var userSession = await GetCurrentUserAsync();
			return userSession?.UserId ?? string.Empty;
		}

		public async Task<string> GetCurrentUserRoleAsync()
		{
			var userSession = await GetCurrentUserAsync();
			return userSession != null ? ((int)userSession.UserRole).ToString() : string.Empty;
		}

		public async Task<List<string>> GetCurrentUserPermissionsAsync()
		{
			var userSession = await GetCurrentUserAsync();
			return userSession?.Permissions ?? new List<string>();
		}

		public async Task<bool> HasPermissionAsync(string permission)
		{
			var userSession = await GetCurrentUserAsync();
			return userSession?.HasPermission(permission) ?? false;
		}

		public async Task<bool> CanViewIncidentAsync(string incidentId)
		{
			var userSession = await GetCurrentUserAsync();
			return userSession?.CanViewIncident(incidentId) ?? false;
		}

		public async Task<bool> CanEditIncidentAsync(string incidentId)
		{
			var userSession = await GetCurrentUserAsync();
			return userSession?.CanEditIncident(incidentId) ?? false;
		}

		public async Task UpdateLastActivityAsync()
		{
			var currentUser = await GetCurrentUserAsync();
			if (currentUser == null) return;

			currentUser.LastActivity = DateTime.UtcNow;
			await SetCurrentUserAsync(currentUser);
		}

		private async Task SetCurrentUserAsync(UserSession userSession)
		{
			var session = _httpContextAccessor.HttpContext?.Session;
			if (session == null) return;

			userSession.LoginTime = userSession.LoginTime == default ? DateTime.UtcNow : userSession.LoginTime;
			userSession.LastActivity = DateTime.UtcNow;

			var sessionData = JsonConvert.SerializeObject(userSession);
			session.SetString(USER_SESSION_KEY, sessionData);

			await Task.CompletedTask;
		}

		private async Task ClearCurrentUserAsync()
		{
			var session = _httpContextAccessor.HttpContext?.Session;
			if (session == null) return;

			session.Remove(USER_SESSION_KEY);
			await Task.CompletedTask;
		}

		private async Task<List<string>> GetUserPermissionsAsync(string userId, string userRole)
		{
			var permissions = new List<string>();
			var role = UserRoleExtensions.FromString(userRole);

			switch (role)
			{
				case UserRole.Administrator:
					permissions.AddRange(new[]
					{
						"VIEW_ALL_INCIDENTS",
						"EDIT_ALL_INCIDENTS",
						"DELETE_INCIDENTS",
						"MANAGE_USERS",
						"SYSTEM_ADMIN"
					});
					break;

				case UserRole.HOD:
				case UserRole.DepartmentHead:
					permissions.AddRange(new[]
					{
						"VIEW_ALL_INCIDENTS",
						"EDIT_ALL_INCIDENTS",
						"APPROVE_INCIDENTS"
					});
					break;

				case UserRole.SeniorManagement:
				case UserRole.Management:
					permissions.AddRange(new[]
					{
						"VIEW_ALL_INCIDENTS",
						"EDIT_DEPARTMENT_INCIDENTS",
						"APPROVE_INCIDENTS"
					});
					break;

				case UserRole.WSHO:
				case UserRole.AssistantWSHO:
					permissions.AddRange(new[]
					{
						"VIEW_WSHO_INCIDENTS",
						"EDIT_WSHO_INCIDENTS",
						"SAFETY_REVIEW"
					});
					break;

				case UserRole.Supervisor:
					permissions.AddRange(new[]
					{
						"VIEW_TEAM_INCIDENTS",
						"EDIT_TEAM_INCIDENTS",
						"SUPERVISOR_REVIEW"
					});
					break;

				case UserRole.User:
				default:
					permissions.AddRange(new[]
					{
						"CREATE_INCIDENTS",
						"EDIT_OWN_INCIDENTS"
					});
					break;
			}

			permissions.AddRange(new[]
			{
				"VIEW_OWN_INCIDENTS",
				"VIEW_DASHBOARD"
			});

			return await Task.FromResult(permissions);
		}

		private async Task<UserModel?> GetUserModelFromDataAccess(string userId)
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
					SbaName = userBE.sbaname ?? string.Empty,
					SbuName = userBE.sbuname ?? string.Empty,
					Designation = userBE.Designation ?? string.Empty,
                };
			}
			catch
			{
				return null;
			}
		}
	}
}