using Microsoft.AspNetCore.Http;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;
using WIRS.Services.Auth;
using WIRS.DataAccess.Interfaces;
using System.Text.Json;
using System.Data;

namespace WIRS.Services.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;
        private readonly IMenuDataAccess _menuDataAccess;
        private const string MENU_SESSION_KEY = "UserMenu";

        public MenuService(IHttpContextAccessor httpContextAccessor, IAuthService authService, IMenuDataAccess menuDataAccess)
        {
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
            _menuDataAccess = menuDataAccess;
        }

        public async Task<List<MenuModel>> GetUserMenuAsync(string userRole)
        {
            try
            {
                var dataSet = await _menuDataAccess.GetMenuInfoByRole(userRole);
                var menuItems = new List<MenuModel>();
                
                if (dataSet.Tables.Count > 0)
                {
                    var table = dataSet.Tables[0];
                    var parentMenus = table.AsEnumerable()
                        .Where(row => row.Field<int>("parent_id") == 0)
                        .OrderBy(row => row.Field<int>("order_no"));

                    foreach (var parentRow in parentMenus)
                    {
                        var menuItem = new MenuModel
                        {
                            MenuId = parentRow.Field<int>("menu_id"),
                            MenuName = parentRow.Field<string>("menu_name"),
                            MenuUrl = parentRow.Field<string>("menu_url"),
                            Order = parentRow.Field<int>("order_no"),
                            HasChildren = parentRow.Field<bool>("has_children"),
                            Children = new List<MenuModel>()
                        };

                        // Get children for this parent
                        if (menuItem.HasChildren)
                        {
                            var childRows = table.AsEnumerable()
                                .Where(row => row.Field<int>("parent_id") == menuItem.MenuId)
                                .OrderBy(row => row.Field<int>("order_no"));

                            foreach (var childRow in childRows)
                            {
                                var childItem = new MenuModel
                                {
                                    MenuId = childRow.Field<int>("menu_id"),
                                    MenuName = childRow.Field<string>("menu_name"),
                                    MenuUrl = childRow.Field<string>("menu_url"),
                                    Order = childRow.Field<int>("order_no"),
                                    HasChildren = false,
                                    Children = new List<MenuModel>()
                                };
                                menuItem.Children.Add(childItem);
                            }
                        }

                        menuItems.Add(menuItem);
                    }
                }

                return menuItems;
            }
            catch (Exception ex)
            {
                // Return default menu on error
                return new List<MenuModel>
                {
                    new MenuModel { MenuId = 1, MenuName = "Home", MenuUrl = "/Home", Order = 1, HasChildren = false }
                };
            }
        }

        public async Task<List<MenuModel>> GetUserMenuFromSessionAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                Console.WriteLine("No session available");
                return new List<MenuModel>();
            }

            // Check if menu is cached in session
            var cachedMenu = session.GetString(MENU_SESSION_KEY);
            if (!string.IsNullOrEmpty(cachedMenu))
            {
                try
                {
                    Console.WriteLine("Returning cached menu");
                    return JsonSerializer.Deserialize<List<MenuModel>>(cachedMenu) ?? new List<MenuModel>();
                }
                catch
                {
                    // If deserialization fails, clear cache and regenerate
                    session.Remove(MENU_SESSION_KEY);
                }
            }

            // Get current user and generate menu
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                Console.WriteLine("No current user found - using default admin role for menu generation");
                // Default to admin role (9) for menu generation when no user session exists
                var defaultMenuItems = await GetUserMenuAsync("9");
                Console.WriteLine($"Generated {defaultMenuItems.Count} default admin menu items");
                return defaultMenuItems;
            }

            var userRoleString = ((int)currentUser.UserRole).ToString();
            Console.WriteLine($"Loading menu for user role: {userRoleString} ({currentUser.UserRole})");
            
            var menuItems = await GetUserMenuAsync(userRoleString);
            Console.WriteLine($"Generated {menuItems.Count} menu items");
            
            // Cache menu in session
            try
            {
                var serializedMenu = JsonSerializer.Serialize(menuItems);
                session.SetString(MENU_SESSION_KEY, serializedMenu);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to cache menu: {ex.Message}");
            }

            return menuItems;
        }

        public void ClearMenuCache()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(MENU_SESSION_KEY);
        }
    }
}