using Microsoft.AspNetCore.Mvc;
using WIRS.Mvc.Controllers;
using WIRS.Services.Auth;
using WIRS.Services.Interfaces;

namespace WIRS.Mvc.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuController(IAuthService authService, IMenuService menuService) 
            : base(authService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMenu()
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var menuItems = await _menuService.GetUserMenuFromSessionAsync();
                return Json(new { success = true, menuItems });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to load menu", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ClearMenuCache()
        {
            try
            {
                _menuService.ClearMenuCache();
                return Json(new { success = true, message = "Menu cache cleared" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to clear menu cache", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> RefreshMenu()
        {
            try
            {
                _menuService.ClearMenuCache();
                var menuItems = await _menuService.GetUserMenuFromSessionAsync();
                return Json(new { success = true, menuItems });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to refresh menu", error = ex.Message });
            }
        }
    }
}