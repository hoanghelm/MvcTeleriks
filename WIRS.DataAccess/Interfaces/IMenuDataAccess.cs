using Npgsql;
using System.Data;
using System.Threading.Tasks;

namespace WIRS.DataAccess.Interfaces
{
    public interface IMenuDataAccess
    {
        Task<IDataReader> GetTopMenuByRole(string role);

        Task<IDataReader> GetSubMenuByRoleAndMenu(string role, decimal menuId);

        Task<DataSet> GetMenuInfoByRole(string role);

        Task<DataSet> GetMenuInfo();

        Task<DataSet> GetMainpage(string UserID);
    }
}