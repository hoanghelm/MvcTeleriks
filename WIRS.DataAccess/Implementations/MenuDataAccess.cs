using Npgsql;
using System;
using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class MenuDataAccess : IMenuDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public MenuDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }


        public async Task<IDataReader> GetTopMenuByRole(string role)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_gettopmenubyrole";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_user_role_code", role);
                return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public async Task<IDataReader> GetSubMenuByRoleAndMenu(string role, decimal menuId)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_getsubmenubyroleandmenu";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_user_role_code", role);
                cmd.Parameters.AddWithValue("@p_menu_opt_no", Convert.ToString(menuId));
                return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public async Task<DataSet> GetMenuInfoByRole(string role)
        {
            DataSet ds = new DataSet();


            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_getMenuInfoByRole";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user_role_code", role);
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

        public async Task<DataSet> GetMenuInfo()
        {
            DataSet ds = new DataSet();


            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_getMenuInfo";
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
            return ds;
        }

        public async Task<DataSet> GetMainpage(string UserID)
        {
            DataSet ds = new DataSet();


            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_getMainPageByUserID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user_id", UserID == "" ? DBNull.Value : (object)UserID);
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