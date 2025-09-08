using System.Data;
using System.Threading.Tasks;

namespace WIRS.DataAccess.Interfaces
{
    public interface IManHoursDataAccess
    {
        Task<DataSet> GetManHoursByYear(int YearFrom, int MonthFrom, int YearTo, int MonthTo, string sbu, int lastentrydate, string userid, string role);

        Task<string> SaveManHours(DataTable manhrsdt);
    }
}