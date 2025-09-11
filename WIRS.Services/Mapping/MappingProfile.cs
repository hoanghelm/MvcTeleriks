using AutoMapper;
using System.Data;
using WIRS.DataAccess.Entities;
using WIRS.Services.Models;

namespace WIRS.Services.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<User, UserModel>()
				.ForMember(dest => dest.SbaName, opt => opt.MapFrom(src => src.sbaname))
				.ForMember(dest => dest.UnsuccessfulLogin, opt => opt.MapFrom(src => src.UnsuccessfulLogin ?? 0));

			CreateMap<DataRow, IncidentModel>()
				.ConstructUsing(row => new IncidentModel
				{
					IncidentId = GetSafeString(row, "incident_id"),
					IncidentDateTime = GetSafeDateTime(row, "incident_datetime"),
					SbuName = GetSafeString(row, "sbu_name"),
					DepartmentName = GetSafeString(row, "department_name"),
					IncidentDesc = GetSafeString(row, "incidentdesc"),
					CreatorName = GetSafeString(row, "creator_name"),
					SubmittedOn = GetSafeDateTime(row, "submitted_on"),
					StatusDesc = GetSafeString(row, "statusdesc"),
					Status = GetSafeString(row, "status"),
					CreatedBy = GetSafeString(row, "created_by")
				});

			CreateMap<DataRow, UserModel>()
				.ConstructUsing(row => new UserModel
				{
					UserId = GetSafeString(row, "user_id"),
					UserName = GetSafeString(row, "user_name"),
					UserRole = GetSafeString(row, "user_role"),
					AccountStatus = GetSafeString(row, "account_status"),
					UnsuccessfulLogin = GetSafeInt(row, "unsuccessful_login"),
					SbaName = GetSafeString(row, "sba_name")
				});
		}

		private static string GetSafeString(DataRow row, string columnName)
		{
			try
			{
				return row.Table.Columns.Contains(columnName) ? (row[columnName]?.ToString() ?? string.Empty) : string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}

		private static DateTime GetSafeDateTime(DataRow row, string columnName)
		{
			try
			{
				if (!row.Table.Columns.Contains(columnName)) return DateTime.MinValue;
				if (DateTime.TryParse(row[columnName]?.ToString(), out var result))
					return result;
				return DateTime.MinValue;
			}
			catch
			{
				return DateTime.MinValue;
			}
		}

		private static int GetSafeInt(DataRow row, string columnName)
		{
			try
			{
				if (!row.Table.Columns.Contains(columnName)) return 0;
				if (int.TryParse(row[columnName]?.ToString(), out var result))
					return result;
				return 0;
			}
			catch
			{
				return 0;
			}
		}
	}
}