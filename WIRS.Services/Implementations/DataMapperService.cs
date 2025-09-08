using AutoMapper;
using System.Data;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Services.Implementations
{
    public interface IDataMapperService
    {
        List<IncidentModel> MapDataSetToIncidents(DataSet dataset, int tableIndex = 0);
        UserModel MapDataRowToUser(DataRow dataRow);
        List<T> MapDataTableToList<T>(DataTable dataTable) where T : class, new();
    }

    public class DataMapperService : IDataMapperService
    {
        private readonly IMapper _mapper;

        public DataMapperService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<IncidentModel> MapDataSetToIncidents(DataSet dataset, int tableIndex = 0)
        {
            var incidents = new List<IncidentModel>();

            if (dataset?.Tables?.Count <= tableIndex) return incidents;

            var dataTable = dataset.Tables[tableIndex];
            foreach (DataRow row in dataTable.Rows)
            {
                incidents.Add(_mapper.Map<IncidentModel>(row));
            }

            return incidents;
        }

        public UserModel MapDataRowToUser(DataRow dataRow)
        {
            return _mapper.Map<UserModel>(dataRow);
        }

        public List<T> MapDataTableToList<T>(DataTable dataTable) where T : class, new()
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties();

            foreach (DataRow row in dataTable.Rows)
            {
                var obj = new T();
                foreach (var prop in properties)
                {
                    var columnName = prop.Name.ToLower();
                    var matchingColumn = dataTable.Columns.Cast<DataColumn>()
                        .FirstOrDefault(col => col.ColumnName.ToLower() == columnName);
                    
                    if (matchingColumn != null)
                    {
                        var value = row[matchingColumn.ColumnName];
                        if (value != DBNull.Value && value != null)
                        {
                            try
                            {
                                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                                    prop.SetValue(obj, Convert.ChangeType(value, underlyingType));
                                }
                                else
                                {
                                    prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                list.Add(obj);
            }

            return list;
        }
    }
}