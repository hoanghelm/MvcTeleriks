using System;
using System.Data;

using Npgsql;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class CommonFunDataAccess : ICommonFunDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public CommonFunDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<DataSet> get_config_data(string config_type)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_app_config_by_type";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_config_type", config_type);
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
        public async Task<DataSet> Get_sbu_by_uid(string sba_code, string sbu_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_sbu_by_uid";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == string.Empty ? (object)DBNull.Value : sbu_code);
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

        public async Task<DataSet> Setup_all_sbus(string sba_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_all_sbus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
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

        public async Task<DataSet> Setup_active_sbus(string sba_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_activesbus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
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
        public async Task<string> Insert_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by)
        {
            string error_Code = string.Empty;
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_insert_sbu";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code == string.Empty ? (object)DBNull.Value : sba_code);
                    cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code == string.Empty ? (object)DBNull.Value : sbu_code);
                    cmd.AddParameter("@p_sbu_name", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_name == string.Empty ? (object)DBNull.Value : sbu_name);
                    if (string.IsNullOrEmpty(inactive_date))
                    {
                        cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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
                            cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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

                return error_Code;
            }
        }

        public async Task<string> Update_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by)
        {
            string error_Code = string.Empty;
            string strConnection = _dBHelper.GetConnectionString();

            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_update_sbu";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                    cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                    cmd.AddParameter("@p_sbu_name", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_name == string.Empty ? (object)DBNull.Value : sbu_name);
                    if (string.IsNullOrEmpty(inactive_date))
                    {
                        cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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
                            cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
                        }
                    }
                    cmd.AddParameter("@p_modified_by", NpgsqlTypes.NpgsqlDbType.Varchar, modified_by);

                    object result = cmd.ExecuteScalar();
                    error_Code = result == DBNull.Value || result == null ? string.Empty : (string)result;
                    return error_Code;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<DataSet> GetLookUpType(string lookup_type, string parent_id)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_lookups";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_lookup_type", NpgsqlTypes.NpgsqlDbType.Varchar, lookup_type);
                cmd.AddParameter("@p_parent_id", NpgsqlTypes.NpgsqlDbType.Varchar, parent_id == string.Empty ? (object)DBNull.Value : parent_id);
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

        public async Task<DataSet> GetAllLookUpType(string lookup_type, string parent_id)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_all_lookups";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_lookup_type", NpgsqlTypes.NpgsqlDbType.Varchar, lookup_type);
                cmd.AddParameter("@p_parent_id", NpgsqlTypes.NpgsqlDbType.Varchar, parent_id == string.Empty ? (object)DBNull.Value : parent_id);
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

        public async Task<DataSet> Get_location_by_uid(string sba_code, string sbu_code, string department_code, string location_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_get_location_uid";
                cmd.CommandType = CommandType.StoredProcedure;                
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code);
                cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code);
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
        public async Task<DataSet> get_all_locations(string sba_code, string sbu_code, string department_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_all_locations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code);
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

        public async Task<DataSet> get_active_locations(string sba_code, string sbu_code, string department_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_active_locations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code);
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
        public async Task<DataSet> search_departments(string code_type, string sba_code, string sbu_code, string department_name)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_search_departments";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_code_type", NpgsqlTypes.NpgsqlDbType.Varchar, code_type);
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.AddParameter("@p_department_name", NpgsqlTypes.NpgsqlDbType.Varchar, department_name == null || department_name == string.Empty ? DBNull.Value : (object)department_name);
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

        public async Task<DataSet> get_all_departments(string code_type, string sba_code, string sbu_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_all_departments";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_code_type", NpgsqlTypes.NpgsqlDbType.Varchar, code_type);
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
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
        public async Task<DataSet> get_active_departments(string code_type, string sba_code, string sbu_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_active_departments";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_code_type", NpgsqlTypes.NpgsqlDbType.Varchar, code_type);
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
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
        public async Task<DataSet> get_department_by_uid(string code_type, string sba_code, string sbu_code, string department_code)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_department_uid";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@code_type", NpgsqlTypes.NpgsqlDbType.Varchar, code_type);
                cmd.AddParameter("@sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.AddParameter("@department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code);
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

        public async Task<string> update_department(string code_type, string sba_code, string sbu_code, string department_code, string department_name, string inactive_date, string modified_by)
        {
            string error_Code = string.Empty;
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_update_department";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_code_type", NpgsqlTypes.NpgsqlDbType.Varchar, code_type);
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                    cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                    cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == string.Empty ? (object)DBNull.Value : department_code);
                    cmd.AddParameter("@p_department_name", NpgsqlTypes.NpgsqlDbType.Varchar, department_name == string.Empty ? (object)DBNull.Value : department_name);

                    if (string.IsNullOrEmpty(inactive_date))
                    {
                        cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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
                            cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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

                return error_Code;
            }
        }
        public async Task<string> insert_department(string code_type, string sba_code, string sbu_code, string department_code, string department_name, string inactive_date, string modified_by)
        {
            string error_Code = string.Empty;
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_insert_department";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_code_type", NpgsqlTypes.NpgsqlDbType.Varchar, code_type);
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                    cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                    cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code == string.Empty ? (object)DBNull.Value : department_code);
                    cmd.AddParameter("@p_department_name", NpgsqlTypes.NpgsqlDbType.Varchar, department_name == string.Empty ? (object)DBNull.Value : department_name);

                    if (string.IsNullOrEmpty(inactive_date))
                    {
                        cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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
                            cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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
            }

            return error_Code;
        }
        public async Task<DataSet> search_locations(string sba_code, string sbu_code, string department_code, string location_name)
        {
            DataSet ds = new DataSet();

            //string strConnection = ConfigurationManager.AppSettings["connstr"].ToString();
            string strConnection = _dBHelper.GetConnectionString();

            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_all_locations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code);
                //cmd.Parameters.AddWithValue("@location_name", location_name == null || location_name == string.Empty ? DBNull.Value : (object)location_name);
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
        public async Task<string> Update_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by)
        {
            string error_Code = string.Empty;
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_update_location";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@p_code_type", NpgsqlTypes.NpgsqlDbType.Varchar, string.Empty);
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                    cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                    cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code);
                    cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == string.Empty ? (object)DBNull.Value : location_code);
                    cmd.AddParameter("@p_location_name", NpgsqlTypes.NpgsqlDbType.Varchar, location_name == string.Empty ? (object)DBNull.Value : location_name);

                    if (string.IsNullOrEmpty(inactive_date))
                    {
                        cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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
                            cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
                        }
                    }

                    cmd.AddParameter("@p_modified_by", NpgsqlTypes.NpgsqlDbType.Varchar, modified_by);

                    object result = cmd.ExecuteScalar();
                    error_Code = result == DBNull.Value || result == null ? string.Empty : (string)result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return error_Code;
        }

        public async Task<string> Insert_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by)
        {
            string error_Code = string.Empty;
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "dbo.spc_insert_location";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@p_code_type", NpgsqlTypes.NpgsqlDbType.Varchar, string.Empty);
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);
                    cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, sbu_code);
                    cmd.AddParameter("@p_department_code", NpgsqlTypes.NpgsqlDbType.Varchar, department_code);
                    cmd.AddParameter("@p_location_code", NpgsqlTypes.NpgsqlDbType.Varchar, location_code == string.Empty ? (object)DBNull.Value : location_code);
                    cmd.AddParameter("@p_location_name", NpgsqlTypes.NpgsqlDbType.Varchar, location_name == string.Empty ? (object)DBNull.Value : location_name);

                    if (string.IsNullOrEmpty(inactive_date))
                    {
                        cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
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
                            cmd.AddParameter("@p_inactive_date", NpgsqlTypes.NpgsqlDbType.Date, DBNull.Value);
                        }
                    }

                    cmd.AddParameter("@p_modified_by", NpgsqlTypes.NpgsqlDbType.Varchar, modified_by);

                    object result = cmd.ExecuteScalar();
                    error_Code = result == DBNull.Value || result == null ? string.Empty : (string)result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return error_Code;
        }

        public async Task<DataSet> generate_sbu_code(string sba_code)
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_generate_sbu_code";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);

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
            }
            return ds;
        }

        public async Task<DataSet> generate_lookup_code(string lookup_type)
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_generate_lookups_code";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@lookup_type", NpgsqlTypes.NpgsqlDbType.Varchar, lookup_type);

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
            }
            return ds;
        }

        public async Task<DataSet> generate_department_code(string lookup_type)
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_generate_department_code";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_lookup_type", NpgsqlTypes.NpgsqlDbType.Varchar, lookup_type);

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
            }
            return ds;
        }

        public async Task<DataSet> generate_location_code(string lookup_type)
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_generate_location_code";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@lookup_type", NpgsqlTypes.NpgsqlDbType.Varchar, lookup_type);

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
            }
            return ds;
        }

        public async Task<DataSet> GetLookUpType(string lookuptype)
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_lookuptype";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_lookuptype", NpgsqlTypes.NpgsqlDbType.Varchar, lookuptype);

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
            }
            return ds;
        }

        public async Task<string> GetLookUpTypebyValue(string lookuptype, string lookup_value)
        {
            string _return = string.Empty;
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_lookuptypebyvalue";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_lookuptype", NpgsqlTypes.NpgsqlDbType.Varchar, lookuptype);
                    cmd.AddParameter("@p_lookup_value", NpgsqlTypes.NpgsqlDbType.Varchar, lookup_value);

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        _return = ds.Tables[0].Rows[0]["lookup_code"].ToString();
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
            }
            return _return;
        }

        public async Task<string> GetLookUpTypebyCode(string lookuptype, string lookup_code)
        {
            string _return = string.Empty;
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_lookuptypebycode";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_lookuptype", NpgsqlTypes.NpgsqlDbType.Varchar, lookuptype);
                    cmd.AddParameter("@p_lookup_code", NpgsqlTypes.NpgsqlDbType.Varchar, lookup_code);

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        _return = ds.Tables[0].Rows[0]["lookup_value"].ToString();
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
            }
            return _return;
        }

        public async Task<DataSet> GetAllSbus()
        {
            DataSet sbuList = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_all_sbus";
                    cmd.CommandType = CommandType.StoredProcedure;

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    da.Fill(sbuList);
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
            return sbuList;
        }

        public async Task<DataSet> GetSbusWithAll()
        {
            DataSet sbuList = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_SbuswithAll";
                    cmd.CommandType = CommandType.StoredProcedure;

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    da.Fill(sbuList);
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
            return sbuList;
        }

        public async Task<string> GetIncidentTypeListByID(string incidentID)
        {
            string incidentTypeList = string.Empty;
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "dbo.spc_get_incidenttypelistbyid";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("p_incidentid", NpgsqlTypes.NpgsqlDbType.Varchar, incidentID);

                    object result = cmd.ExecuteScalar();
                    incidentTypeList = result == DBNull.Value ? string.Empty : (string)result;
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
            return incidentTypeList;
        }

        public async Task<DataSet> GetAccessSbubyUser(string userId, string userRole, string sba_code, string errorCode)
        {
            DataSet sbuList = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_sbu_by_user";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_user_id", NpgsqlTypes.NpgsqlDbType.Varchar, userId);
                    cmd.AddParameter("@p_userole", NpgsqlTypes.NpgsqlDbType.Varchar, userRole);
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, sba_code);

                    // For output parameter, you'll need to use Parameters.Add properly
                    NpgsqlParameter err_Code = new NpgsqlParameter("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                    err_Code.Direction = ParameterDirection.Output;
                    err_Code.Size = 15;
                    cmd.Parameters.Add(err_Code);

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    da.Fill(sbuList);
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
            }
            return sbuList;
        }

        public async Task<(DataSet result, string errorCode)> GetAccessSbabyUser(string userId, string userRole)
        {
            DataSet sbuList = new DataSet();
            string errorCode = string.Empty;
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_sba_by_user";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_user_id", NpgsqlTypes.NpgsqlDbType.Varchar, userId);
                    cmd.AddParameter("@p_userole", NpgsqlTypes.NpgsqlDbType.Varchar, userRole);

                    // If you need output parameter, uncomment these lines
                    /*
                    NpgsqlParameter err_Code = new NpgsqlParameter("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                    err_Code.Direction = ParameterDirection.Output;
                    err_Code.Size = 15;
                    cmd.Parameters.Add(err_Code);
                    */

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    da.Fill(sbuList);

                    // If using output parameter
                    // errorCode = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
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
            return (sbuList, errorCode);
        }

        public async Task<DataSet> GetCCEmailDistribution()
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_CCEmailDistribtutionList";
                    cmd.CommandType = CommandType.StoredProcedure;

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
            }
            return ds;
        }

        public async Task<DataSet> GetYears()
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_Years";
                    cmd.CommandType = CommandType.StoredProcedure;

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
            }
            return ds;
        }

        public async Task<DataSet> GetCCEmailDistributionByEmailGroup(string emailgroup)
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_EmailGroup";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@emailGroupType", NpgsqlTypes.NpgsqlDbType.Varchar, emailgroup);

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
            }
            return ds;
        }

        public async Task<DataSet> get_UserInfo_by_userID(string userID)
        {
            DataSet ds = new DataSet();
            string strConnection = _dBHelper.GetConnectionString();
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "spc_get_userinfobyuserid";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, userID);

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
            }
            return ds;
        }
    }
}