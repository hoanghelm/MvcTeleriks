using System;
using System.Data;
using System.Threading.Tasks;
using WIRS.DataAccess.Entities;

namespace WIRS.DataAccess.Interfaces
{
    public interface IUserDataAccess
    {
        Task<DataSet> GetInfoByUserID(string userID, string userrole);
        Task<bool> ValidateUsers(string userid, string pw);
        Task<User> GetUserByUserID(string userId);
        Task<int> LockUser(string userId, string LoginID);
        Task<User> GetUserByUserEIP(string userId);
        Task<bool> NeedChangePassword(object userId);
        Task<bool> CheckPasswordHasUsed(string userId, string encodedPassword);
        Task UserChangePassword(string userId, string encodedPassword, string PasswordHistory);
        Task<bool> CheckUserExists(string userid);
        Task<DataSet> SearchEmployee(string empid, string empname, string empcostcentreno,
            string empcostcentrename, string empsbuname, string incident_id, int? pageNo, int? pageSize);
        Task<DataSet> SearchUsersInfo(string empid, string empname, string empcostcentreno,
            string empcostcentrename, string empsbuname, string loginid);
        Task<DataSet> GetEmployeeInfoByEmployeeNo(string empid);
        Task<DataSet> get_hod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_ahod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_h_hod_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_hrlist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_wsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_awsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_c_wsho_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_active_cclist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_partA_cclist_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_all_copyto_list_by_sbu(string sba_code, string sbu_code, string department_code, string location_code);
        Task<DataSet> get_all_copyto_list_by_uid(string sba_code, string sbu_code, string department_code,
            string location_code, string user_id);
        Task<string> insert_copyto_list(string sba_code, string sbu_code, string department_code,
            string location_code, string user_id, string inactive_date, string modified_by);
        Task<string> update_copyto_list(string sba_code, string sbu_code, string department_code,
            string location_code, string user_id, string inactive_date, string modified_by);
        Task LoginADUser(string userId);
    }
}