using System;
using System.Data;

using Npgsql;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class EmailDistributionDataAccess : IEmailDistributionDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public EmailDistributionDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<string> SaveEmailUsers(EmailDistribution emailBE)
        {
            string errorCode = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_insert_emaildistribution";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_name", emailBE.EmpName);
                cmd.Parameters.AddWithValue("@p_empid", emailBE.EmpNo);
                cmd.Parameters.AddWithValue("@p_sba_code", emailBE.sba_code);
                cmd.Parameters.AddWithValue("@p_sbu_code", emailBE.sbu_code);
                cmd.Parameters.AddWithValue("@p_group", emailBE.Group);
                cmd.Parameters.AddWithValue("@p_designation", emailBE.Designation);
                cmd.Parameters.AddWithValue("@p_email", emailBE.Email);
                cmd.Parameters.AddWithValue("@p_createdby", emailBE.Creator);

                NpgsqlParameter err_Code = cmd.Parameters.Add("@v_errcode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 15;

                cmd.ExecuteNonQuery();
                errorCode = cmd.Parameters["@v_errcode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;

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

            return errorCode;
        }

        public async Task<DataSet> SeachEmailUsers(EmailDistribution emailBE)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_search_emaildistribution";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_empid", emailBE.EmpNo);
                cmd.Parameters.AddWithValue("@p_name", emailBE.EmpName);
                cmd.Parameters.AddWithValue("@p_designation", emailBE.Designation);
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

        public async Task<string> GetEmailUsers(EmailDistribution emailBE)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            string errorCode = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_emaildistributionuser";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@empid", emailBE.EmpNo).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.Add("@name", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@group", NpgsqlTypes.NpgsqlDbType.Char, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@designation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@emailaddress", NpgsqlTypes.NpgsqlDbType.Varchar, 80).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@inactivedate", NpgsqlTypes.NpgsqlDbType.Varchar, 22).Direction = ParameterDirection.Output;

                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 15;

                 NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    dt.Merge(ds.Tables[0]);
                }
                emailBE.EmpNo = cmd.Parameters["@empid"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@empid"].Value;
                emailBE.EmpName = cmd.Parameters["@name"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@name"].Value;
                emailBE.sba_code = cmd.Parameters["@sba_code"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sba_code"].Value;
                emailBE.sbu_code = cmd.Parameters["@sbu_code"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbu_code"].Value;
                emailBE.Group = cmd.Parameters["@group"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@group"].Value;
                emailBE.Designation = cmd.Parameters["@designation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@designation"].Value;
                emailBE.Email = cmd.Parameters["@emailaddress"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@emailaddress"].Value;
                emailBE.InactiveDate = cmd.Parameters["@inactivedate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@inactivedate"].Value;
                errorCode = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
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

            return errorCode;
        }

        public async Task<string> UpdateEmailUsers(EmailDistribution emailBE)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string errorCode = string.Empty;
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_save_emaildistributionuser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_empid", emailBE.EmpNo);
                cmd.Parameters.AddWithValue("@p_name", emailBE.EmpName);
                cmd.Parameters.AddWithValue("@p_sba_code", emailBE.sba_code);
                cmd.Parameters.AddWithValue("@p_sbu_code", emailBE.sbu_code);
                cmd.Parameters.AddWithValue("@p_group", emailBE.Group);
                cmd.Parameters.AddWithValue("@p_designation", emailBE.Designation);
                cmd.Parameters.AddWithValue("@p_email", emailBE.Email);
                cmd.Parameters.AddWithValue("@p_inactivedate", emailBE.InactiveDate);
                cmd.Parameters.AddWithValue("@p_modifiedby", emailBE.Modifiedby);

                NpgsqlParameter err_Code = cmd.Parameters.Add("@v_errcode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 15;
                cmd.ExecuteNonQuery();
                errorCode = cmd.Parameters["@v_errcode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;


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

            return errorCode;
        }
    }
}