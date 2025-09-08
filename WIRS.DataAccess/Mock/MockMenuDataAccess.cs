using Npgsql;
using System.Data;
using WIRS.DataAccess.Interfaces;

namespace WIRS.DataAccess.Mock
{
	public class MockMenuDataAccess : IMenuDataAccess
	{
		private readonly Dictionary<string, List<MenuModel>> _menusByRole;

		public MockMenuDataAccess()
		{
			_menusByRole = new Dictionary<string, List<MenuModel>>
			{
				["1"] = new List<MenuModel> 
				{
					new MenuModel { MenuId = 1, MenuName = "Dashboard", MenuUrl = "/Home", Icon = "k-i-dashboard", Order = 1, HasChildren = false },
					new MenuModel { MenuId = 2, MenuName = "Incidents", MenuUrl = "#", Icon = "k-i-file-txt", Order = 2, HasChildren = true, 
						Children = new List<MenuModel>
						{
							new MenuModel { MenuId = 21, MenuName = "Create Incident", MenuUrl = "/Incident/Create", Icon = "k-i-plus", Order = 1 },
							new MenuModel { MenuId = 22, MenuName = "My Incidents", MenuUrl = "/Incident/My", Icon = "k-i-user", Order = 2 },
							new MenuModel { MenuId = 23, MenuName = "All Incidents", MenuUrl = "/Incident/All", Icon = "k-i-list-unordered", Order = 3 }
						}
					},
					new MenuModel { MenuId = 3, MenuName = "Reports", MenuUrl = "#", Icon = "k-i-chart-line", Order = 3, HasChildren = true,
						Children = new List<MenuModel>
						{
							new MenuModel { MenuId = 31, MenuName = "Incident Reports", MenuUrl = "/Reports/Incidents", Icon = "k-i-chart", Order = 1 },
							new MenuModel { MenuId = 32, MenuName = "Statistics", MenuUrl = "/Reports/Statistics", Icon = "k-i-chart-pie", Order = 2 }
						}
					},
					new MenuModel { MenuId = 4, MenuName = "Administration", MenuUrl = "#", Icon = "k-i-gear", Order = 4, HasChildren = true,
						Children = new List<MenuModel>
						{
							new MenuModel { MenuId = 41, MenuName = "User Management", MenuUrl = "/Admin/Users", Icon = "k-i-group", Order = 1 },
							new MenuModel { MenuId = 42, MenuName = "System Settings", MenuUrl = "/Admin/Settings", Icon = "k-i-cog", Order = 2 }
						}
					}
				},
				["2"] = new List<MenuModel>
				{
					new MenuModel { MenuId = 1, MenuName = "Dashboard", MenuUrl = "/Home", Icon = "k-i-dashboard", Order = 1, HasChildren = false },
					new MenuModel { MenuId = 2, MenuName = "Incidents", MenuUrl = "#", Icon = "k-i-file-txt", Order = 2, HasChildren = true,
						Children = new List<MenuModel>
						{
							new MenuModel { MenuId = 21, MenuName = "Create Incident", MenuUrl = "/Incident/Create", Icon = "k-i-plus", Order = 1 },
							new MenuModel { MenuId = 22, MenuName = "My Incidents", MenuUrl = "/Incident/My", Icon = "k-i-user", Order = 2 }
						}
					}
				},
				["3"] = new List<MenuModel>
				{
					new MenuModel { MenuId = 1, MenuName = "Dashboard", MenuUrl = "/Home", Icon = "k-i-dashboard", Order = 1, HasChildren = false },
					new MenuModel { MenuId = 2, MenuName = "Incidents", MenuUrl = "#", Icon = "k-i-file-txt", Order = 2, HasChildren = true,
						Children = new List<MenuModel>
						{
							new MenuModel { MenuId = 21, MenuName = "Create Incident", MenuUrl = "/Incident/Create", Icon = "k-i-plus", Order = 1 },
							new MenuModel { MenuId = 22, MenuName = "My Incidents", MenuUrl = "/Incident/My", Icon = "k-i-user", Order = 2 },
							new MenuModel { MenuId = 23, MenuName = "Team Incidents", MenuUrl = "/Incident/Team", Icon = "k-i-group", Order = 3 }
						}
					},
					new MenuModel { MenuId = 3, MenuName = "Reports", MenuUrl = "#", Icon = "k-i-chart-line", Order = 3, HasChildren = true,
						Children = new List<MenuModel>
						{
							new MenuModel { MenuId = 31, MenuName = "Team Reports", MenuUrl = "/Reports/Team", Icon = "k-i-chart", Order = 1 }
						}
					}
				}
			};
		}

		public Task<DataSet> GetMainpage(string UserID)
		{
			var dataSet = new DataSet();
			var table = new DataTable("MainPage");
			table.Columns.Add("page_url", typeof(string));
			table.Rows.Add("/Home");
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
			table.Columns.Add("icon", typeof(string));
			table.Columns.Add("order_no", typeof(int));
			table.Columns.Add("has_children", typeof(bool));

			foreach (var menuGroup in _menusByRole.Values)
			{
				foreach (var menu in menuGroup)
				{
					table.Rows.Add(menu.MenuId, menu.MenuName, menu.MenuUrl, menu.Icon, menu.Order, menu.HasChildren);
				}
			}

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
			table.Columns.Add("icon", typeof(string));
			table.Columns.Add("order_no", typeof(int));
			table.Columns.Add("has_children", typeof(bool));
			table.Columns.Add("parent_id", typeof(int));

			if (_menusByRole.ContainsKey(role))
			{
				foreach (var menu in _menusByRole[role])
				{
					table.Rows.Add(menu.MenuId, menu.MenuName, menu.MenuUrl, menu.Icon, menu.Order, menu.HasChildren, 0);
					
					if (menu.Children != null)
					{
						foreach (var child in menu.Children)
						{
							table.Rows.Add(child.MenuId, child.MenuName, child.MenuUrl, child.Icon, child.Order, false, menu.MenuId);
						}
					}
				}
			}

			dataSet.Tables.Add(table);
			return Task.FromResult(dataSet);
		}

		public Task<NpgsqlDataReader> GetSubMenuByRoleAndMenu(string role, decimal menuId)
		{
			throw new NotImplementedException("Use GetMenuInfoByRole instead");
		}

		public Task<NpgsqlDataReader> GetTopMenuByRole(string role)
		{
			throw new NotImplementedException("Use GetMenuInfoByRole instead");
		}
	}

	public class MenuModel
	{
		public int MenuId { get; set; }
		public string MenuName { get; set; }
		public string MenuUrl { get; set; }
		public string Icon { get; set; }
		public int Order { get; set; }
		public bool HasChildren { get; set; }
		public List<MenuModel> Children { get; set; }
	}
}