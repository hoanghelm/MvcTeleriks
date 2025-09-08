using System.Data;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
    public class MockCommonFunDataAccess : ICommonFunDataAccess
    {
        public async Task<DataSet> get_config_data(string config_type)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("config_key", typeof(string));
            table.Columns.Add("config_value", typeof(string));
            
            table.Rows.Add(config_type, config_type == "SSOKEY" ? "mock_sso_key_123" : "mock_value");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> Get_sbu_by_uid(string sba_code, string sbu_code)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("sba_code", typeof(string));
            table.Columns.Add("sbu_code", typeof(string));
            table.Columns.Add("sbu_name", typeof(string));
            
            table.Rows.Add(sba_code, sbu_code, $"SBU {sbu_code}");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> Setup_all_sbus(string sba_code)
        {
            return await CreateMockSbuDataSet(sba_code);
        }

        public async Task<DataSet> Setup_active_sbus(string sba_code)
        {
            return await CreateMockSbuDataSet(sba_code);
        }

        public async Task<string> Insert_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by)
        {
            return await Task.FromResult("Success");
        }

        public async Task<string> Update_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by)
        {
            return await Task.FromResult("Success");
        }

        public async Task<DataSet> GetLookUpType(string lookup_type, string parent_id)
        {
            return await CreateMockLookupDataSet(lookup_type);
        }

        public async Task<DataSet> GetAllLookUpType(string lookup_type, string parent_id)
        {
            return await CreateMockLookupDataSet(lookup_type);
        }

        public async Task<DataSet> Get_location_by_uid(string sba_code, string sbu_code, string department_code, string location_code)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("location_code", typeof(string));
            table.Columns.Add("location_name", typeof(string));
            
            table.Rows.Add(location_code, $"Location {location_code}");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> get_all_locations(string sba_code, string sbu_code, string department_code)
        {
            return await CreateMockLocationDataSet();
        }

        public async Task<DataSet> get_active_locations(string sba_code, string sbu_code, string department_code)
        {
            return await CreateMockLocationDataSet();
        }

        public async Task<DataSet> search_departments(string code_type, string sba_code, string sbu_code, string department_name)
        {
            return await CreateMockDepartmentDataSet();
        }

        public async Task<DataSet> get_all_departments(string code_type, string sba_code, string sbu_code)
        {
            return await CreateMockDepartmentDataSet();
        }

        public async Task<DataSet> get_active_departments(string code_type, string sba_code, string sbu_code)
        {
            return await CreateMockDepartmentDataSet();
        }

        public async Task<DataSet> get_department_by_uid(string code_type, string sba_code, string sbu_code, string department_code)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("department_code", typeof(string));
            table.Columns.Add("department_name", typeof(string));
            
            table.Rows.Add(department_code, $"Department {department_code}");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        public async Task<string> update_department(string code_type, string sba_code, string sbu_code, string department_code, string department_name, string inactive_date, string modified_by)
        {
            return await Task.FromResult("Success");
        }

        public async Task<string> insert_department(string code_type, string sba_code, string sbu_code, string department_code, string department_name, string inactive_date, string modified_by)
        {
            return await Task.FromResult("Success");
        }

        public async Task<DataSet> search_locations(string sba_code, string sbu_code, string department_code, string location_name)
        {
            return await CreateMockLocationDataSet();
        }

        public async Task<string> Update_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by)
        {
            return await Task.FromResult("Success");
        }

        public async Task<string> Insert_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by)
        {
            return await Task.FromResult("Success");
        }

        public async Task<DataSet> generate_sbu_code(string sba_code)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("new_code", typeof(string));
            
            table.Rows.Add("SBU001");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> generate_lookup_code(string lookup_type)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("new_code", typeof(string));
            
            table.Rows.Add($"{lookup_type}001");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> generate_department_code(string lookup_type)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("new_code", typeof(string));
            
            table.Rows.Add("DEPT001");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> generate_location_code(string lookup_type)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("new_code", typeof(string));
            
            table.Rows.Add("LOC001");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> GetLookUpType(string lookuptype)
        {
            return await CreateMockLookupDataSet(lookuptype);
        }

        public async Task<string> GetLookUpTypebyValue(string lookuptype, string lookup_value)
        {
            return await Task.FromResult($"Code for {lookup_value}");
        }

        public async Task<string> GetLookUpTypebyCode(string lookuptype, string lookup_code)
        {
            return await Task.FromResult($"Value for {lookup_code}");
        }

        public async Task<DataSet> GetAllSbus()
        {
            return await CreateMockSbuDataSet("ALL");
        }

        public async Task<DataSet> GetSbusWithAll()
        {
            return await CreateMockSbuDataSet("ALL");
        }

        public async Task<string> GetIncidentTypeListByID(string incidentID)
        {
            return await Task.FromResult("Safety,Environmental,Security");
        }

        public async Task<DataSet> GetAccessSbubyUser(string userId, string userRole, string sba_code, string errorCode)
        {
            return await CreateMockSbuDataSet(sba_code);
        }

        public async Task<(DataSet result, string errorCode)> GetAccessSbabyUser(string userId, string userRole)
        {
            var result = await CreateMockSbuDataSet("ALL");
            return (result, "SUCCESS");
        }

        public async Task<DataSet> GetCCEmailDistribution()
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("name", typeof(string));
            
            table.Rows.Add("admin@company.com", "Administrator");
            table.Rows.Add("safety@company.com", "Safety Team");
            
            dataSet.Tables.Add(table);
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> GetYears()
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("year", typeof(int));
            
            for (int year = DateTime.Now.Year - 5; year <= DateTime.Now.Year + 1; year++)
            {
                table.Rows.Add(year);
            }
            
            dataSet.Tables.Add(table);
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> GetCCEmailDistributionByEmailGroup(string emailgroup)
        {
            return await GetCCEmailDistribution();
        }

        public async Task<DataSet> get_UserInfo_by_userID(string userID)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(string));
            table.Columns.Add("user_name", typeof(string));
            table.Columns.Add("email", typeof(string));
            
            table.Rows.Add(userID, $"User {userID}", $"{userID}@company.com");
            dataSet.Tables.Add(table);
            
            return await Task.FromResult(dataSet);
        }

        private async Task<DataSet> CreateMockSbuDataSet(string sba_code)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("sba_code", typeof(string));
            table.Columns.Add("sbu_code", typeof(string));
            table.Columns.Add("sbu_name", typeof(string));
            
            table.Rows.Add(sba_code, "SBU001", "IT Department");
            table.Rows.Add(sba_code, "SBU002", "HR Department");
            table.Rows.Add(sba_code, "SBU003", "Finance Department");
            
            dataSet.Tables.Add(table);
            return await Task.FromResult(dataSet);
        }

        private async Task<DataSet> CreateMockLookupDataSet(string lookup_type)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("lookup_code", typeof(string));
            table.Columns.Add("lookup_value", typeof(string));
            table.Columns.Add("lookup_desc", typeof(string));
            
            table.Rows.Add($"{lookup_type}001", "Option 1", $"{lookup_type} Option 1");
            table.Rows.Add($"{lookup_type}002", "Option 2", $"{lookup_type} Option 2");
            
            dataSet.Tables.Add(table);
            return await Task.FromResult(dataSet);
        }

        private async Task<DataSet> CreateMockLocationDataSet()
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("location_code", typeof(string));
            table.Columns.Add("location_name", typeof(string));
            
            table.Rows.Add("LOC001", "Building A");
            table.Rows.Add("LOC002", "Building B");
            
            dataSet.Tables.Add(table);
            return await Task.FromResult(dataSet);
        }

        private async Task<DataSet> CreateMockDepartmentDataSet()
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add("department_code", typeof(string));
            table.Columns.Add("department_name", typeof(string));
            
            table.Rows.Add("DEPT001", "Software Development");
            table.Rows.Add("DEPT002", "Quality Assurance");
            
            dataSet.Tables.Add(table);
            return await Task.FromResult(dataSet);
        }
    }
}