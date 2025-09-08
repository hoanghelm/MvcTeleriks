using System;
using System.Data;

using Npgsql;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class ErrorMessageDataAccess : IErrorMessageDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public ErrorMessageDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<string> GetErrorMessage(string errorCode)
        {
            string errorMessage = string.Empty;

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_errormessage";
                cmd.CommandType = CommandType.StoredProcedure;

                NpgsqlParameter errCode = cmd.Parameters.AddWithValue("@errcode", errorCode);
                NpgsqlParameter errMsg = cmd.Parameters.Add("@errmsg", NpgsqlTypes.NpgsqlDbType.Varchar);

                errCode.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;
                errMsg.Direction = ParameterDirection.Output;
                errMsg.Size = 1000;
                cmd.ExecuteNonQuery();
                errorMessage = cmd.Parameters["@errmsg"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errmsg"].Value;
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

            return errorMessage;
        }
    }
}