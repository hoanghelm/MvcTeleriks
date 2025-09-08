using System;
using System.Data;
using System.Threading.Tasks;
using WIRS.DataAccess.Entities;

namespace WIRS.DataAccess.Interfaces
{
    public interface IStatisticsReportDataAccess
    {
        Task<DataSet> GetStatisticsReport(StatisticsReport statisticsBE);
    }
}