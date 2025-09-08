using System;
using System.Data;
using System.Threading.Tasks;

namespace WIRS.DataAccess.Interfaces
{
    public interface ICommonFunDataAccess
    {
        Task<DataSet> get_config_data(string config_type);

        Task<DataSet> Get_sbu_by_uid(string sba_code, string sbu_code);

        Task<DataSet> Setup_all_sbus(string sba_code);

        Task<DataSet> Setup_active_sbus(string sba_code);

        Task<string> Insert_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by);

        Task<string> Update_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by);

        Task<DataSet> GetLookUpType(string lookup_type, string parent_id);

        Task<DataSet> GetAllLookUpType(string lookup_type, string parent_id);

        Task<DataSet> Get_location_by_uid(string sba_code, string sbu_code, string department_code, string location_code);

        Task<DataSet> get_all_locations(string sba_code, string sbu_code, string department_code);

        Task<DataSet> get_active_locations(string sba_code, string sbu_code, string department_code);

        Task<DataSet> search_departments(string code_type, string sba_code, string sbu_code, string department_name);

        Task<DataSet> get_all_departments(string code_type, string sba_code, string sbu_code);

        Task<DataSet> get_active_departments(string code_type, string sba_code, string sbu_code);

        Task<DataSet> get_department_by_uid(string code_type, string sba_code, string sbu_code, string department_code);

        Task<string> update_department(string code_type, string sba_code, string sbu_code, string department_code, string department_name, string inactive_date, string modified_by);

        Task<string> insert_department(string code_type, string sba_code, string sbu_code, string department_code, string department_name, string inactive_date, string modified_by);

        Task<DataSet> search_locations(string sba_code, string sbu_code, string department_code, string location_name);

        Task<string> Update_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by);

        Task<string> Insert_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by);

        Task<DataSet> generate_sbu_code(string sba_code);

        Task<DataSet> generate_lookup_code(string lookup_type);

        Task<DataSet> generate_department_code(string lookup_type);

        Task<DataSet> generate_location_code(string lookup_type);

        Task<DataSet> GetLookUpType(string lookuptype);

        Task<string> GetLookUpTypebyValue(string lookuptype, string lookup_value);

        Task<string> GetLookUpTypebyCode(string lookuptype, string lookup_code);

        Task<DataSet> GetAllSbus();

        Task<DataSet> GetSbusWithAll();

        Task<string> GetIncidentTypeListByID(string incidentID);

        Task<DataSet> GetAccessSbubyUser(string userId, string userRole, string sba_code, string errorCode);

        Task<(DataSet result, string errorCode)> GetAccessSbabyUser(string userId, string userRole);

        Task<DataSet> GetCCEmailDistribution();

        Task<DataSet> GetYears();

        Task<DataSet> GetCCEmailDistributionByEmailGroup(string emailgroup);

        Task<DataSet> get_UserInfo_by_userID(string userID);
    }
}