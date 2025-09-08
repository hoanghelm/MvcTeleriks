using Npgsql;
using System;
using System.Data;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Implementations
{
    public class UserDataAccess : IUserDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public UserDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<DataSet> GetInfoByUserID(string userID, string userrole)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.CommandText = "spc_get_incidents_by_userid";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_userid", userID);
                cmd.Parameters.AddWithValue("@p_userole", userrole);

                return cmd.ExecuteDataSet(_dBHelper.GetConnection());
            }
        }

        public async Task<bool> ValidateUsers(string userid, string pw)
        {
            object obj = false;
            string strError = string.Empty;
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_validateuser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", NpgsqlTypes.NpgsqlDbType.Varchar, userid);
                cmd.Parameters.AddWithValue("@pw", NpgsqlTypes.NpgsqlDbType.Varchar, pw);
                obj = cmd.ExecuteScalar();
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
            return obj != null;
        }

        public async Task<User> GetUserByUserID(string userId)
        {
            User userBE = new User();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_getUserByUserId";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_user_id", userId);

                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("user_id")))
                        {
                            userBE.UserId = Convert.ToString(reader["user_id"]);
                        }
                        else
                        {
                            userBE.UserId = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("user_name")))
                        {
                            userBE.UserName = Convert.ToString(reader["user_name"]);
                        }
                        else
                        {
                            userBE.UserName = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("empdesignation")))
                        {
                            userBE.Designation = Convert.ToString(reader["empdesignation"]);
                        }
                        else
                        {
                            userBE.Designation = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("empsbaname")))
                        {
                            userBE.sbaname = Convert.ToString(reader["empsbaname"]);
                        }
                        else
                        {
                            userBE.sbaname = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("empsbuname")))
                        {
                            userBE.sbuname = Convert.ToString(reader["empsbuname"]);
                        }
                        else
                        {
                            userBE.sbuname = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("empcostcentreno")))
                        {
                            userBE.costcentreno = Convert.ToString(reader["empcostcentreno"]);
                        }
                        else
                        {
                            userBE.costcentreno = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("email_address")))
                        {
                            userBE.Email = Convert.ToString(reader["email_address"]);
                        }
                        else
                        {
                            userBE.Email = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("user_role_code")))
                        {
                            userBE.UserRole = Convert.ToString(reader["user_role_code"]);
                        }
                        else
                        {
                            userBE.UserRole = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("user_password")))
                        {
                            userBE.Password = Convert.ToString(reader["user_password"]);
                        }
                        else
                        {
                            userBE.Password = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("pwd_expiry_date")))
                        {
                            userBE.PasswordExpiryDate = Convert.ToDateTime(reader["pwd_expiry_date"]);
                        }
                        else
                        {
                            userBE.PasswordExpiryDate = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("pwd_history")))
                        {
                            userBE.PasswordHistory = Convert.ToString(reader["pwd_history"]);
                        }
                        else
                        {
                            userBE.PasswordHistory = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("last_login_date")))
                        {
                            userBE.LastLoginDate = Convert.ToDateTime(reader["last_login_date"]);
                        }
                        else
                        {
                            userBE.LastLoginDate = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("unsuccessful_login")))
                        {
                            userBE.UnsuccessfulLogin = Convert.ToInt32(reader["unsuccessful_login"]);
                        }
                        else
                        {
                            userBE.UnsuccessfulLogin = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("acct_status")))
                        {
                            userBE.AccountStatus = Convert.ToString(reader["acct_status"]);
                        }
                        else
                        {
                            userBE.AccountStatus = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("acct_status_Desc")))
                        {
                            userBE.AccountStatusDescription = Convert.ToString(reader["acct_status_Desc"]);
                        }
                        else
                        {
                            userBE.AccountStatusDescription = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("inactive_date")))
                        {
                            userBE.InactiveDate = Convert.ToDateTime(reader["inactive_date"]);
                        }
                        else
                        {
                            userBE.InactiveDate = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("created_by")))
                        {
                            userBE.Creator = Convert.ToString(reader["created_by"]);
                        }
                        else
                        {
                            userBE.Creator = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("creation_date")))
                        {
                            userBE.CreationDate = Convert.ToDateTime(reader["creation_date"]);
                        }
                        else
                        {
                            userBE.CreationDate = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("modified_by")))
                        {
                            userBE.Modifiedby = Convert.ToString(reader["modified_by"]);
                        }
                        else
                        {
                            userBE.Modifiedby = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("modify_date")))
                        {
                            userBE.LastModifyDate = Convert.ToDateTime(reader["modify_date"]);
                        }
                        else
                        {
                            userBE.LastModifyDate = null;
                        }
                    }
                }
                else
                {
                    userBE = null;
                }
                reader.Close();
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
            return userBE;
        }

        public async Task<int> LockUser(string userId, string LoginID)
        {
            int Ret = 0;
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_lockUser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@acct_status", "L");
                cmd.Parameters.AddWithValue("@modified_by", LoginID);
                Ret = cmd.ExecuteNonQuery();
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
            return Ret;
        }

        public async Task<User> GetUserByUserEIP(string userId)
        {
            User userBE = new User();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_validateusereip";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_userid", userId);

                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("user_id")))
                        {
                            userBE.UserId = Convert.ToString(reader["user_id"]);
                        }
                        else
                        {
                            userBE.UserId = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("user_name")))
                        {
                            userBE.UserName = Convert.ToString(reader["user_name"]);
                        }
                        else
                        {
                            userBE.UserName = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("empdesignation")))
                        {
                            userBE.Designation = Convert.ToString(reader["empdesignation"]);
                        }
                        else
                        {
                            userBE.Designation = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("empsbaname")))
                        {
                            userBE.sbaname = Convert.ToString(reader["empsbaname"]);
                        }
                        else
                        {
                            userBE.sbaname = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("empsbuname")))
                        {
                            userBE.sbuname = Convert.ToString(reader["empsbuname"]);
                        }
                        else
                        {
                            userBE.sbuname = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("empcostcentreno")))
                        {
                            userBE.costcentreno = Convert.ToString(reader["empcostcentreno"]);
                        }
                        else
                        {
                            userBE.costcentreno = null;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("user_role_code")))
                        {
                            userBE.UserRole = Convert.ToString(reader["user_role_code"]);
                        }
                        else
                        {
                            userBE.UserRole = null;
                        }
                    }

                    reader.Close();
                }
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
            return userBE;
        }

        public async Task<bool> NeedChangePassword(object userId)
        {
            bool Ret = false;
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_check_change_password";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user_id_param", userId);
                Ret = Convert.ToBoolean(cmd.ExecuteScalar());
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
            return Ret;
        }

        public async Task<bool> CheckPasswordHasUsed(string userId, string encodedPassword)
        {
            bool Ret = false;
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_check_PasswordHasUsed";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@userpassword", encodedPassword);
                Ret = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                //CommonFunction.WriteExceptionLog(ex);
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
            return Ret;
        }

        public async Task UserChangePassword(string userId, string encodedPassword, string PasswordHistory)
        {
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "CALL spc_update_userpassword(@p_userid, @p_userpassword, @p_pwdhistory)";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@p_userid", userId);
                cmd.Parameters.AddWithValue("@p_userpassword", encodedPassword);
                cmd.Parameters.AddWithValue("@p_pwdhistory", PasswordHistory);
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

        public async Task<bool> CheckUserExists(string userid)
        {
            bool Ret = false;
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_checkUserExists";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                Ret = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                //CommonFunction.WriteExceptionLog(ex);
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
            return Ret;
        }

        public async Task<DataSet> SearchEmployee(string empid, string empname, string empcostcentreno, string empcostcentrename, string empsbuname, string incident_id, int? pageNo, int? pageSize)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_search_employees";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_empid", empid == null || empid == string.Empty ? DBNull.Value : (object)empid);
                cmd.Parameters.AddWithValue("@p_empname", empname == null || empname == string.Empty ? DBNull.Value : (object)empname);
                cmd.Parameters.AddWithValue("@p_empcostcentreno", empcostcentreno == null || empcostcentreno == string.Empty ? DBNull.Value : (object)empcostcentreno);
                cmd.Parameters.AddWithValue("@p_empcostcentrename", empcostcentrename == null || empcostcentrename == string.Empty ? DBNull.Value : (object)empcostcentrename);
                cmd.Parameters.AddWithValue("@p_empsbuname", empsbuname == null || empsbuname == string.Empty ? DBNull.Value : (object)empsbuname);
                cmd.Parameters.AddWithValue("@p_incident_id", incident_id == null || incident_id == string.Empty ? DBNull.Value : (object)incident_id);
                cmd.Parameters.AddWithValue("@p_pageno", pageNo == null || pageNo == null ? DBNull.Value : (object)pageNo);
                cmd.Parameters.AddWithValue("@p_pagesize", pageSize == null || pageSize == null ? DBNull.Value : (object)pageSize);
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

        public async Task<DataSet> SearchUsersInfo(string empid, string empname, string empcostcentreno, string empcostcentrename, string empsbuname, string loginid)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_search_users_info";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_empid", NpgsqlTypes.NpgsqlDbType.Varchar, empid == null || empid == string.Empty ? DBNull.Value : (object)empid);
                cmd.AddParameter("@p_empname", NpgsqlTypes.NpgsqlDbType.Varchar, empname == null || empname == string.Empty ? DBNull.Value : (object)empname);
                cmd.AddParameter("@p_empcostcentreno", NpgsqlTypes.NpgsqlDbType.Varchar, empcostcentreno == null || empcostcentreno == string.Empty ? DBNull.Value : (object)empcostcentreno);
                cmd.AddParameter("@p_empcostcentrename", NpgsqlTypes.NpgsqlDbType.Varchar, empcostcentrename == null || empcostcentrename == string.Empty ? DBNull.Value : (object)empcostcentrename);
                cmd.AddParameter("@p_empsbuname", NpgsqlTypes.NpgsqlDbType.Varchar, empsbuname == null || empsbuname == string.Empty ? DBNull.Value : (object)empsbuname);
                cmd.AddParameter("@p_user_id", NpgsqlTypes.NpgsqlDbType.Varchar, loginid == null || loginid == string.Empty ? DBNull.Value : (object)loginid);

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

        public async Task<DataSet> GetEmployeeInfoByEmployeeNo(string empid)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_get_employeeinfo_by_employeeno";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_empid", empid == null || empid == string.Empty ? DBNull.Value : (object)empid);
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

        public async Task<DataSet> get_hod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_hod_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_ahod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_ahod_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_h_hod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_h_hod_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_hrlist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_get_hr_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_wsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_wsho_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_awsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_awsho_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_c_wsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_c_wsho_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_active_cclist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_cclist_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_partA_cclist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_partA_copyto";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_all_copyto_list_by_sbu(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_all_copyto_list_by_sbu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == null || sba_code == string.Empty ? DBNull.Value : (object)sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == null || sbu_code == string.Empty ? DBNull.Value : (object)sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == null || department_code == string.Empty ? DBNull.Value : (object)department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == null || location_code == string.Empty ? DBNull.Value : (object)location_code);

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

        public async Task<DataSet> get_all_copyto_list_by_uid(string sba_code, string sbu_code, string department_code, string location_code, string user_id)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_all_copyto_list_by_uid";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code);
                cmd.AddParameter("@p_user_id", NpgsqlTypes.NpgsqlDbType.Varchar, user_id);

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

        public async Task<string> insert_copyto_list(string sba_code, string sbu_code, string department_code, string location_code, string user_id, string inactive_date, string modified_by)
        {
            string error_Code = string.Empty;

            //string _dBHelper.GetConnectionString() = ConfigurationManager.AppSettings["connstr"].ToString();

            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_insert_copyto_list";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == string.Empty ? (object)DBNull.Value : department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == string.Empty ? (object)DBNull.Value : location_code);
                cmd.AddParameter("@p_user_id", NpgsqlTypes.NpgsqlDbType.Varchar, user_id);
                if (string.IsNullOrEmpty(inactive_date))
                {
                    cmd.Parameters.AddWithValue("@p_inactive_date", DBNull.Value);
                }
                else
                {
                    DateTime parsedDate;
                    if (DateTime.TryParse(inactive_date, out parsedDate))
                    {
                        cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, parsedDate.Date);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@p_inactive_date", DBNull.Value);
                    }
                }
                cmd.AddParameter("@p_modified_by", NpgsqlTypes.NpgsqlDbType.Varchar, modified_by);

                object result = cmd.ExecuteScalar();
                error_Code = result == DBNull.Value || result == null ? string.Empty : (string)result;
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

        public async Task<string> update_copyto_list(string sba_code, string sbu_code, string department_code, string location_code, string user_id, string inactive_date, string modified_by)
        {
            string error_Code = string.Empty;

            //string _dBHelper.GetConnectionString() = ConfigurationManager.AppSettings["connstr"].ToString();

            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_update_copyto_list";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.Parameters.AddWithValue("@p_department_code", department_code == string.Empty ? (object)DBNull.Value : department_code);
                cmd.Parameters.AddWithValue("@p_location_code", location_code == string.Empty ? (object)DBNull.Value : location_code);
                cmd.AddParameter("@p_user_id", NpgsqlTypes.NpgsqlDbType.Varchar, user_id);
                if (string.IsNullOrEmpty(inactive_date))
                {
                    cmd.Parameters.AddWithValue("@p_inactive_date", DBNull.Value);
                }
                else
                {
                    DateTime parsedDate;
                    if (DateTime.TryParse(inactive_date, out parsedDate))
                    {
                        cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, parsedDate.Date);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@p_inactive_date", DBNull.Value);
                    }
                }
                cmd.AddParameter("@p_modified_by", NpgsqlTypes.NpgsqlDbType.Varchar, modified_by);

                object result = cmd.ExecuteScalar();
                error_Code = result == DBNull.Value || result == null ? string.Empty : (string)result;
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

        public async Task LoginADUser(string userId)
        {
            NpgsqlConnection con = new NpgsqlConnection(_dBHelper.GetConnectionString());
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                string sqlUpdate = "UPDATE wirs_users SET acct_status='A',user_password=NULL,pwd_expiry_date=NULL,pwd_history=NULL,unsuccessful_login=0,inactive_date=NULL,modify_date=NOW() WHERE user_id=@userId";
                // Add parameters

                NpgsqlParameter dbParameter = new NpgsqlParameter
                {
                    ParameterName = "@userId",
                    NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar,
                    Size = 10,
                    Value = userId
                };

                cmd.Parameters.Add(dbParameter);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlUpdate;
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
        }
    }
}