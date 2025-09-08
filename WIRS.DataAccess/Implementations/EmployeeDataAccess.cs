using System;
using System.Data;

using Npgsql;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class EmployeeDataAccess : IEmployeeDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public EmployeeDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<DataSet> GetEmployeeByName(Employee employee)
        {
            DataSet empDS = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_employeebyname";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_empname", employee.EmpName == null || employee.EmpName == string.Empty ? DBNull.Value : (object)employee.EmpName);
                cmd.Parameters.AddWithValue("@p_prefname", employee.EmpPrefName == null || employee.EmpPrefName == string.Empty ? DBNull.Value : (object)employee.EmpPrefName);
                //cmd.Parameters.AddWithValue("@empName", employee.EmpName);
                //cmd.Parameters.AddWithValue("@prefName", employee.EmpPrefName);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(empDS);
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

            return empDS;
        }
        public async Task<string> ValidateEmployee(Employee employee)
        {
            string error_Code = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_validateEmployee";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@empNo", employee.EmpID);
                cmd.Parameters.AddWithValue("@empName", employee.EmpName);
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

        public async Task<(string emailAddress, string userRoleName, string errorCode)> GetEmployeeEmailAddress(Employee employee)
        {
            string error_Code = string.Empty;
            string emailAddress = string.Empty;
            string userRoleName = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_employeeemail";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_empno", employee.EmpID == null || employee.EmpID == string.Empty ? DBNull.Value : (object)employee.EmpID);

                //cmd.Parameters.AddWithValue("@empNo", employee.EmpID).Direction = ParameterDirection.Input;
                //cmd.Parameters.AddWithValue("@emailAddress", emailAddress).Direction = ParameterDirection.Input;
                //cmd.Parameters.Add("@emailAddress", SqlDbType.NVarChar, 80).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("@errCode", SqlDbType.NVarChar, 15).Direction = ParameterDirection.Output;

                NpgsqlParameter email = cmd.Parameters.Add("@emailAddress", NpgsqlTypes.NpgsqlDbType.Varchar);
                email.Direction = ParameterDirection.Output;
                email.Size = 80;

                NpgsqlParameter userRole = cmd.Parameters.Add("@roleName", NpgsqlTypes.NpgsqlDbType.Varchar);
                userRole.Direction = ParameterDirection.Output;
                userRole.Size = 40;

                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 15;

                cmd.ExecuteNonQuery();
                emailAddress = cmd.Parameters["@emailAddress"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@emailAddress"].Value;
                userRoleName = cmd.Parameters["@roleName"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@roleName"].Value;
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

            return (emailAddress, userRoleName, error_Code);
        }

        public async Task<string> ValidateNextUser(string empNo)
        {
            string error_Code = string.Empty;
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_validateNextUser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@empNo", empNo);
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