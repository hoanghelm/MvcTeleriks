using WIRS.Services.Models;

namespace WIRS.Services.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuModel>> GetUserMenuAsync(string userRole);
        Task<List<MenuModel>> GetUserMenuFromSessionAsync();
        void ClearMenuCache();
    }
}