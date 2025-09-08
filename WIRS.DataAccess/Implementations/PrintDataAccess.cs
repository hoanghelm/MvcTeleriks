using System;
using System.Data;

using Npgsql;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class PrintDataAccess: IPrintDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public PrintDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<DataSet> PrintIncident(string incidentID)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_Print_Incident";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@incidentid", incidentID);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
                cmd.ExecuteNonQuery();
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

        public async Task<DataSet> PrintIncidentType(string incidentID)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_PrintIncidentType";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@incidentid", incidentID);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
                cmd.ExecuteNonQuery();
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