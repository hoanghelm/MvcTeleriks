using System;
using System.Data;
using WIRS.DataAccess.Entities;

using Npgsql;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Implementations
{
    public class StatisticsReportDataAccess : IStatisticsReportDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public StatisticsReportDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<DataSet> GetStatisticsReport(StatisticsReport statisticsBE)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_Statistics_Report";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@yearFrom", statisticsBE.YearFrom);
                cmd.Parameters.AddWithValue("@monthFrom", statisticsBE.MonthFrom);
                cmd.Parameters.AddWithValue("@yearTo", statisticsBE.YearTo);
                cmd.Parameters.AddWithValue("@monthTo", statisticsBE.MonthTo);
                cmd.Parameters.AddWithValue("@nodays", statisticsBE.MCdays);
                cmd.Parameters.AddWithValue("@userid", statisticsBE.UserId);
                cmd.Parameters.AddWithValue("@userrole", statisticsBE.UserRole);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };

                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
            return ds;
        }
    }
}