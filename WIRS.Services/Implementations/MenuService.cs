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
            List<MenuModel> menu = new List<MenuModel>();

            using (var topMenuReader = await _menuDataAccess.GetTopMenuByRole(userRole))
            {
                while (topMenuReader.Read())
                {
                    MenuModel topMenu = new MenuModel
                    {
                        MenuId = Convert.ToInt32(topMenuReader["ID"]),
                        MenuName = Convert.ToString(topMenuReader["Text"]) ?? string.Empty,
                        MenuUrl = Convert.ToString(topMenuReader["Link"]) ?? string.Empty,
                        HasChildren = false,
                        Children = new List<MenuModel>()
                    };

                    using (var subMenuReader = await _menuDataAccess.GetSubMenuByRoleAndMenu(userRole, (int)topMenu.MenuId))
                    {
                        while (subMenuReader.Read())
                        {
                            MenuModel subMenu = new MenuModel
                            {
                                MenuId = Convert.ToDouble(subMenuReader["ID"]),
                                MenuName = Convert.ToString(subMenuReader["Text"]) ?? string.Empty,
                                MenuUrl = Convert.ToString(subMenuReader["Link"]) ?? string.Empty,
                                HasChildren = false,
                                Children = new List<MenuModel>()
                            };
                            topMenu.Children.Add(subMenu);
                        }
                    }

                    topMenu.HasChildren = topMenu.Children.Count > 0;
                    menu.Add(topMenu);
                }
            }

            return menu;
        }

        public async Task<List<MenuModel>> GetUserMenuFromSessionAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return new List<MenuModel>();
            }

            var cachedMenu = session.GetString(MENU_SESSION_KEY);
            if (!string.IsNullOrEmpty(cachedMenu))
            {
                try
                {
                    return JsonSerializer.Deserialize<List<MenuModel>>(cachedMenu) ?? new List<MenuModel>();
                }
                catch
                {
                    session.Remove(MENU_SESSION_KEY);
                }
            }

            var currentUser = await _authService.GetCurrentUserAsync();
            var userRoleString = ((int)currentUser.UserRole).ToString();
            
            var menuItems = await GetUserMenuAsync(userRoleString);
            
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