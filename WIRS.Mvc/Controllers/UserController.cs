using Microsoft.AspNetCore.Mvc;
using WIRS.Services.Auth;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Mvc.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMasterDataService _masterDataService;

        public UserController(IAuthService authService, IUserService userService, IMasterDataService masterDataService) : base(authService)
        {
            _userService = userService;
            _masterDataService = masterDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Load master data for dropdowns
                var userRoles = await _masterDataService.GetUserRoles();
                var sectors = await _masterDataService.GetSectors();
                var locations = await _masterDataService.GetLocations();

                ViewBag.UserRoles = userRoles;
                ViewBag.Sectors = sectors;
                ViewBag.Locations = locations;
            }
            catch (Exception)
            {
                // If master data fails to load, continue with empty collections
                ViewBag.UserRoles = new List<LookupItem>();
                ViewBag.Sectors = new List<LookupItem>();
                ViewBag.Locations = new List<LookupItem>();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateUserExists([FromBody] string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || userId.Length != 8 || !userId.All(char.IsDigit))
                {
                    return Json(new { success = false, message = "User ID must be exactly 8 digits" });
                }

                var userInfo = await _userService.ValidateUserExists(userId);
                if (userInfo != null)
                {
                    return Json(new { 
                        success = true, 
                        user = new { 
                            userId = userInfo.UserId,
                            userName = userInfo.UserName,
                            email = userInfo.Email
                        }
                    });
                }

                return Json(new { success = false, message = "User not found in employee database" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error validating user", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchEmployees([FromBody] EmployeeSearchRequest request)
        {
            try
            {
                var result = await _userService.SearchEmployees(
                    request.EmployeeId ?? string.Empty,
                    request.EmployeeName ?? string.Empty,
                    request.PageNo ?? 1,
                    request.PageSize ?? 10
                );

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error searching employees", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid data provided" });
                }

                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var result = await _userService.CreateUser(request, currentUser.UserId);

                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { success = true, message = "User created successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error creating user", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var userInfo = new
                {
                    userId = currentUser.UserId,
                    userName = currentUser.UserName,
                    userRole = currentUser.UserRole.ToString(),
                    sbaName = currentUser.SbaName,
                    displayName = !string.IsNullOrEmpty(currentUser.UserName) ? currentUser.UserName : "User",
                    loginTime = currentUser.LoginTime,
                    lastActivity = currentUser.LastActivity
                };

                return Json(new { success = true, user = userInfo });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to get user information", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                await UpdateLastActivityAsync();

                var profile = new
                {
                    userId = currentUser.UserId,
                    userName = currentUser.UserName,
                    userRole = currentUser.UserRole.ToString(),
                    sbaName = currentUser.SbaName,
                    permissions = currentUser.Permissions,
                    loginTime = currentUser.LoginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    lastActivity = currentUser.LastActivity.ToString("yyyy-MM-dd HH:mm:ss"),
                    sessionDuration = (DateTime.Now - currentUser.LoginTime).ToString(@"hh\:mm\:ss")
                };

                return Json(new { success = true, profile });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to get profile information", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateActivity()
        {
            try
            {
                await UpdateLastActivityAsync();
                return Json(new { success = true, message = "Activity updated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update activity", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Load master data for dropdowns
                var userRoles = await _masterDataService.GetUserRoles();
                var sectors = await _masterDataService.GetSectors();

                ViewBag.UserRoles = userRoles;
                ViewBag.Sectors = sectors;
            }
            catch (Exception)
            {
                // If master data fails to load, continue with empty collections
                ViewBag.UserRoles = new List<LookupItem>();
                ViewBag.Sectors = new List<LookupItem>();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchUsers([FromBody] UserSearchRequest request)
        {
            try
            {
                var result = await _userService.SearchUsers(
                    request.Sector ?? string.Empty,
                    request.LOB ?? string.Empty,
                    request.UserId ?? string.Empty,
                    request.UserName ?? string.Empty,
                    request.UserRole ?? string.Empty
                );

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error searching users", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            try
            {
                var userDetails = await _userService.GetUserDetails(id);
                if (userDetails == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index");
                }

                // Load master data for dropdowns
                var userRoles = await _masterDataService.GetUserRoles();
                var sectors = await _masterDataService.GetSectors();
                var locations = await _masterDataService.GetLocations();

                ViewBag.UserRoles = userRoles;
                ViewBag.Sectors = sectors;
                ViewBag.Locations = locations;

                return View(userDetails);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading user details: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid data provided" });
                }

                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var result = await _userService.UpdateUser(request, currentUser.UserId);

                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { success = true, message = "User updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating user", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetUserPassword([FromBody] string userId)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var result = await _userService.ResetPassword(userId, currentUser.UserId);

                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { success = true, message = "Password reset successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error resetting password", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InactiveUser([FromBody] string userId)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var result = await _userService.InactiveUser(userId, currentUser.UserId);

                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { success = true, message = "User inactivated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error inactivating user", error = ex.Message });
            }
        }
    }

    public class UserSearchRequest
    {
        public string? Sector { get; set; }
        public string? LOB { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserRole { get; set; }
    }
}