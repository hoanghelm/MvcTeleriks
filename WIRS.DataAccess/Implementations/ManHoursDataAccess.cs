using System;
using System.Data;

using Npgsql;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class ManHoursDataAccess : IManHoursDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public ManHoursDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        //public DataSet GetManHoursByYear(int Year, int Month, String emptype, String sbu,int lastentrydate)
        public async Task<DataSet> GetManHoursByYear(int YearFrom, int MonthFrom, int YearTo, int MonthTo, string sbu, int lastentrydate, string userid, string role)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_ManHoursByYear";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@yearFrom", YearFrom);
                cmd.Parameters.AddWithValue("@monthFrom", MonthFrom);
                //cmd.Parameters.AddWithValue("@monthFrom", MonthFrom == 0 ? (object)DBNull.Value : MonthFrom);
                cmd.Parameters.AddWithValue("@yearTo", YearTo);
                cmd.Parameters.AddWithValue("@monthTo", MonthTo);
                //cmd.Parameters.AddWithValue("@emptype", emptype == string.Empty ? (object)DBNull.Value : emptype);
                cmd.Parameters.AddWithValue("@sbu", sbu == string.Empty ? (object)DBNull.Value : sbu);
                cmd.Parameters.AddWithValue("@lastentry", lastentrydate);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@userrole", role);

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

        public async Task<string> SaveManHours(DataTable manhrsdt)
        {
            string error_Code = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_save_ManHours";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@manhrs", manhrsdt);

                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 15;

                cmd.ExecuteNonQuery();
                error_Code = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;

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

            return error_Code;
        }
    }
}