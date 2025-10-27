using Npgsql;
using System.Data;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
    public class MockMenuDataAccess : IMenuDataAccess
    {
        public Task<DataSet> GetMainpage(string UserID)
        {
            var dataSet = new DataSet();
            var table = new DataTable("MainPage");
            table.Columns.Add("user_id", typeof(string));
            table.Columns.Add("welcome_message", typeof(string));
            table.Rows.Add(UserID, "Welcome to WIRS - Workplace Incident Reporting System");
            dataSet.Tables.Add(table);
            return Task.FromResult(dataSet);
        }

        public Task<DataSet> GetMenuInfo()
        {
            var dataSet = new DataSet();
            var table = new DataTable("Menus");
            table.Columns.Add("menu_id", typeof(int));
            table.Columns.Add("menu_name", typeof(string));
            table.Columns.Add("menu_url", typeof(string));
            table.Columns.Add("order_no", typeof(int));
            table.Columns.Add("has_children", typeof(bool));

            table.Rows.Add(1, "Home", "/Home", 1, false);
            table.Rows.Add(2, "Incident", "#", 2, true);
            table.Rows.Add(4, "Maintenance", "#", 4, true);
            table.Rows.Add(5, "Help", "#", 5, true);
            table.Rows.Add(8, "Logout", "/Login/Logout", 8, false);

            dataSet.Tables.Add(table);
            return Task.FromResult(dataSet);
        }

        public Task<DataSet> GetMenuInfoByRole(string role)
        {
            var dataSet = new DataSet();
            var table = new DataTable("Menus");
            table.Columns.Add("menu_id", typeof(int));
            table.Columns.Add("menu_name", typeof(string));
            table.Columns.Add("menu_url", typeof(string));
            table.Columns.Add("order_no", typeof(int));
            table.Columns.Add("has_children", typeof(bool));
            table.Columns.Add("parent_id", typeof(int));

            // Mock data - main menus
            table.Rows.Add(1, "Home", "/Home", 1, false, 0);
            table.Rows.Add(2, "Incident", "#", 2, true, 0);
            table.Rows.Add(4, "Maintenance", "#", 4, true, 0);
            table.Rows.Add(5, "Help", "#", 5, true, 0);
            table.Rows.Add(8, "Logout", "/Login/Logout", 8, false, 0);

            // Mock data - sub menus
            table.Rows.Add(21, "Create Incident Report", "/Incident/Create", 1, false, 2);
            table.Rows.Add(23, "View Incident Report", "/Incident/View", 3, false, 2);
            table.Rows.Add(41, "Create User", "/User/Create", 1, false, 4);
            table.Rows.Add(42, "Update User", "/User/Update", 2, false, 4);
            table.Rows.Add(43, "Maintain Copy To", "/Maintenance/CopyTo", 3, false, 4);
            table.Rows.Add(45, "Maintain LOB", "/Maintenance/LOB", 5, false, 4);
            table.Rows.Add(46, "Maintain Locations", "/Maintenance/Locations", 6, false, 4);
            table.Rows.Add(47, "Maintain Department", "/Maintenance/Department", 7, false, 4);
            table.Rows.Add(51, "User Guide", "/Help/UserGuide", 1, false, 5);

            dataSet.Tables.Add(table);
            return Task.FromResult(dataSet);
        }

        // Working mock implementation for GetTopMenuByRole
        public Task<IDataReader> GetTopMenuByRole(string role)
        {
            var table = new DataTable("TopMenus");

            // Column structure matching your stored procedure (using int for ID to match MenuModel.MenuId)
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Text", typeof(string));
            table.Columns.Add("Link", typeof(string));
            table.Columns.Add("IsEnable", typeof(bool));

            // Top-level menu items based on role
            table.Rows.Add(1, "Home", "/Home", true);
            table.Rows.Add(2, "Incident", "#", true);
            table.Rows.Add(4, "Maintenance", "#", true);
            table.Rows.Add(5, "Help", "#", true);
            table.Rows.Add(8, "Logout", "/Login/Logout", true);

            IDataReader reader = table.CreateDataReader();
            return Task.FromResult(reader);
        }

        // Working mock implementation for GetSubMenuByRoleAndMenu
        public Task<IDataReader> GetSubMenuByRoleAndMenu(string role, decimal menuId)
        {
            var table = new DataTable("SubMenus");

            // Column structure matching your stored procedure (using int for ID to match MenuModel.MenuId)
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Text", typeof(string));
            table.Columns.Add("Link", typeof(string));
            table.Columns.Add("IsEnable", typeof(bool));

            // Add sub-menus based on parent menu ID
            switch ((int)menuId)
            {
                case 2: // Incident sub-menus
                    table.Rows.Add(21, "Create Incident Report", "/Incident/Create", true);
                    table.Rows.Add(23, "View Incident Report", "/Incident/View", true);
                    break;

                case 4: // Maintenance sub-menus
                    table.Rows.Add(41, "Create User", "/User/Create", true);
                    table.Rows.Add(42, "Update User", "/User/Update", true);
                    table.Rows.Add(43, "Maintain Copy To", "/Maintenance/CopyTo", true);
                    table.Rows.Add(45, "Maintain LOB", "/Maintenance/LOB", true);
                    table.Rows.Add(46, "Maintain Locations", "/Maintenance/Locations", true);
                    table.Rows.Add(47, "Maintain Department", "/Maintenance/Department", true);
                    break;

                case 5: // Help sub-menus
                    table.Rows.Add(51, "User Guide", "/Help/UserGuide", true);
                    break;

                default:
                    // No sub-menus for this parent - return empty table
                    break;
            }

            IDataReader reader = table.CreateDataReader();
            return Task.FromResult(reader);
        }
    }
}