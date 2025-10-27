using System;
using System.Data;
using WIRS.DataAccess.Entities;

using Npgsql;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Implementations
{
    public class UserCredentialsDataAccess : IUserCredentialsDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public UserCredentialsDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<(DataSet accessList, string errorCode)> GetUsersAccessSBU(string userId, string userRole, string sba_code)
        {
            string errorCode = string.Empty;
            DataSet accesslist = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_get_sbu_by_user";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_user_id", userId);
                cmd.Parameters.AddWithValue("@p_userole", userRole);
                cmd.Parameters.AddWithValue("@p_sba_code", sba_code);

                //NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                //err_Code.Direction = ParameterDirection.Output;
                //err_Code.Size = 15;

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(accesslist);
                //errorCode = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
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

            return (accesslist, errorCode);
        }

        public async Task<string> SaveUsers(UserCredentials usercredentials)
        {
            string errorCode = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                DataSet ds = new DataSet();
                DataTable dt = usercredentials.UserAccess;
                ds.Tables.Add(dt);
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_insert_user";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentials.UserId == null || usercredentials.UserId == string.Empty ? DBNull.Value : (object)usercredentials.UserId);
                cmd.AddParameter("@p_username", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentials.UserName == null || usercredentials.UserName == string.Empty ? DBNull.Value : (object)usercredentials.UserName);
                cmd.AddParameter("@p_userrolecode", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentials.UserRole == null || usercredentials.UserRole == string.Empty ? DBNull.Value : (object)usercredentials.UserRole);
                cmd.AddParameter("@p_userpassword", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentials.Password == null || usercredentials.Password == string.Empty ? DBNull.Value : (object)usercredentials.Password);
                cmd.AddParameter("@p_emailaddress", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentials.Email == null || usercredentials.Email == string.Empty ? DBNull.Value : (object)usercredentials.Email);
                cmd.AddParameter("@p_useraccessxml", NpgsqlTypes.NpgsqlDbType.Xml, ds.GetXml());
                cmd.AddParameter("@p_createdby", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentials.Creator);

                object result = cmd.ExecuteScalar();
                errorCode = result == DBNull.Value || result == null ? string.Empty : (string)result;
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

        public async Task<DataSet> SeachUsers(UserCredentials usercredentails, string LoginID)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_search_Users";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_loginid", LoginID);
                cmd.Parameters.AddWithValue("@p_sba_code", usercredentails.sbaname);
                cmd.Parameters.AddWithValue("@p_sbu_code", usercredentails.sbuname);
                cmd.Parameters.AddWithValue("@p_userid", usercredentails.UserId);
                cmd.Parameters.AddWithValue("@p_username", usercredentails.UserName);
                cmd.Parameters.AddWithValue("@p_userole", usercredentails.UserRole);
                
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

        public async Task<string> GetUser(UserCredentials usercredentails)
        {
            string errorCode = "ERR-999";

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.CommandText = "spc_get_user";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_userid", usercredentails.UserId);

                DataSet ds =  cmd.ExecuteDataSet(_dBHelper.GetConnection());

                if (ds.Tables.Count == 2 && ds.Tables[0].Rows.Count != 0)
                {
                    DataTable userInfo = ds.Tables[0];
                    DataTable userAccess = ds.Tables[1];

                    usercredentails.UserAccess = userAccess;
                    usercredentails.UserName = ds.Tables[0].Rows[0]["user_name"].ToString();
                    usercredentails.UserRole = ds.Tables[0].Rows[0]["user_role_code"].ToString();
                    usercredentails.Email = ds.Tables[0].Rows[0]["email_address"].ToString();
                    usercredentails.AccountStatus = ds.Tables[0].Rows[0]["acct_status"].ToString();
                    usercredentails.InactiveDate = ds.Tables[0].Rows[0]["inactive_date"].ToString();
                    errorCode = string.Empty;
                }
                else 
                {
                    errorCode = "ERR-094";
                }
            }

            return errorCode;
        }

        public async Task InactiveUsers(UserCredentials usercredentails)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_inactive_User";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.UserId);
                cmd.AddParameter("@p_modifiedby", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.Modifiedby);
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
        }

        public async Task<DataSet> GetAllUserRole()
        {
            DataSet roleList = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_userrole";
                cmd.CommandType = CommandType.StoredProcedure;
                 NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(roleList);
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

            return roleList;
        }

        public async Task<string> UpdateUsers(UserCredentials usercredentails)
        {

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            string errorCode = string.Empty;

            DataSet ds = new DataSet();
            DataTable dt = usercredentails.UserAccess;
            ds.Tables.Add(dt);

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_save_user";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.UserId);
                cmd.AddParameter("@p_email", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.Email);
                cmd.AddParameter("@p_userrolecode", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.UserRole);
                cmd.AddParameter("@p_acct_status", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.AccountStatus);
                cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.InactiveDate);
                cmd.AddParameter("@p_useraccessxml", NpgsqlTypes.NpgsqlDbType.Xml, ds.GetXml());
                cmd.AddParameter("@p_modifiedby", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.Modifiedby);

                object result = cmd.ExecuteScalar();
                errorCode = result == DBNull.Value || result == null ? string.Empty : (string)result;
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

        public async Task ResetPassword(UserCredentials usercredentails)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "CALL spc_reset_password(@p_userid, @p_userpassword, @p_pwdhistory, @p_modifiedby)";
                cmd.CommandType = CommandType.Text;

                cmd.AddParameter("@p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.UserId);
                cmd.AddParameter("@p_userpassword", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.Password);
                cmd.AddParameter("@p_pwdhistory", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.PasswordHistory);
                cmd.AddParameter("@p_modifiedby", NpgsqlTypes.NpgsqlDbType.Varchar, usercredentails.Modifiedby);
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
        }

        public async Task ValidateUserExists(UserCredentials usercredentails)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_validateUserExists";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userid", usercredentails.UserId).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.Add("@username", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@emailaddress", NpgsqlTypes.NpgsqlDbType.Varchar, 80).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                usercredentails.UserId = cmd.Parameters["@userid"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@userid"].Value;
                usercredentails.UserName = cmd.Parameters["@username"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@username"].Value;
                usercredentails.Email = cmd.Parameters["@emailaddress"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@emailaddress"].Value;

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

        }
        public async Task<string> ValidatUserName(UserCredentials usercredentails)
        {
            string error_Code = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_validatUserName";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_userid", usercredentails.UserId);
                cmd.Parameters.AddWithValue("@p_username", usercredentails.UserName);
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

        public async Task<string> ValidateUserResign(UserCredentials usercredentails)
        {
            string error_Code = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_validateuserresign";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_userid",usercredentails.UserId.ToString());
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

        public async Task<string> GetUserRoleName(string userRoleCode)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string userRoleName = string.Empty;
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "fn_get_userrolename";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_user_role_code", userRoleCode);
                NpgsqlParameter userrole_name = cmd.Parameters.Add("@p_user_role_name", NpgsqlTypes.NpgsqlDbType.Varchar);
                userrole_name.Direction = ParameterDirection.Output;
                userrole_name.Size = 40;
                cmd.ExecuteNonQuery();
                userRoleName = cmd.Parameters["@p_user_role_name"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@p_user_role_name"].Value;
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

            return userRoleName;
        }

        public async Task<DataSet> SearchUsers(string currentUserId, string sector, string lob, string userId, string userName, string userRole)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_search_users";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_loginid", NpgsqlTypes.NpgsqlDbType.Varchar, currentUserId);
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sector);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, lob);
                cmd.AddParameter("@p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, userId);
                cmd.AddParameter("@p_username", NpgsqlTypes.NpgsqlDbType.Varchar, userName);
                cmd.AddParameter("@p_userole", NpgsqlTypes.NpgsqlDbType.Varchar, userRole);

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