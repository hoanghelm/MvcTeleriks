using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.Shared.Helpers;

namespace WIRS.Shared.Extensions
{
    public static class PostgresqlExtensions
    {
        public static DataSet ExecuteDataSet(this NpgsqlCommand cmd, NpgsqlConnection con)
        {
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                NpgsqlTransaction tran = con.BeginTransaction();
                cmd.Connection = con;
                cmd.Transaction = tran;

                List<string> cursorNames = new List<string>();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cursorNames.Add(reader.GetString(0));
                    }
                }

                foreach (string cursorName in cursorNames)
                {
                    using (NpgsqlCommand fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\";", con))
                    {
                        using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(fetchCmd))
                        {
                            DataTable dt = new DataTable(cursorName);
                            da.Fill(dt);
                            ds.Tables.Add(dt);
                        }
                    }
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                con.Close();
                con.Dispose();
            }
            return ds;
        }

        public static void AddParameter(this NpgsqlCommand cmd, string name, NpgsqlTypes.NpgsqlDbType type, object value)
        {
            var param = new NpgsqlParameter(name, type);
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}