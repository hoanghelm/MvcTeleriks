using System.Data;

namespace WIRS.DataAccess.Mock
{
    public static class MockDataAccessFactory
    {
        public static DataSet CreateEmptyDataSet()
        {
            return new DataSet();
        }

        public static DataSet CreateSingleValueDataSet(string columnName, object value)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            table.Columns.Add(columnName, value.GetType());
            table.Rows.Add(value);
            dataSet.Tables.Add(table);
            return dataSet;
        }

        public static DataSet CreateSuccessDataSet()
        {
            return CreateSingleValueDataSet("result", "Success");
        }
    }
}