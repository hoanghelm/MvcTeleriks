//using ADS1_AWS_SecretManager;
using System;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Collections.Generic;
using WIRS.Shared.Configurations;

namespace WIRS.Shared.Helpers
{
    public interface IDBHelper
    {
        NpgsqlConnection GetConnection();
        string GetConnectionString();
    }

    public class DBHelper : IDBHelper
    {
        private readonly string _connectionString;
        private readonly AppSettings _appSettings;
        private readonly object _lockObject = new object();
        private bool _poolInitialized = false;

        public DBHelper(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _connectionString = GetConfiguratedConnectionString();
        }

        private void InitializeConnectionPool()
        {
            if (!_poolInitialized)
            {
                lock (_lockObject)
                {
                    if (!_poolInitialized)
                    {
                        try
                        {
                            int minPoolSize = 5;
                            List<NpgsqlConnection> initialConnections = new List<NpgsqlConnection>();

                            for (int i = 0; i < minPoolSize; i++)
                            {
                                var conn = new NpgsqlConnection(_connectionString);
                                conn.Open();
                                initialConnections.Add(conn);
                            }

                            foreach (var conn in initialConnections)
                            {
                                if (conn.State == System.Data.ConnectionState.Open)
                                {
                                    conn.Close();
                                    conn.Dispose();
                                }
                            }

                            _poolInitialized = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error initializing connection pool: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
        }

        private string GetConfiguratedConnectionString()
        {
            string connectionString = GetConnectionString();

            Dictionary<string, string> requiredSettings = new Dictionary<string, string>()
            {
                { "Pooling", "true" },
                { "MinPoolSize", "5" },
                { "MaxPoolSize", "100" },
                { "Connection Idle Lifetime", "300" },
                { "Connection Pruning Interval", "10" }
            };

            foreach (var setting in requiredSettings)
            {
                if (connectionString.IndexOf(setting.Key + "=", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    connectionString += $";{setting.Key}={setting.Value}";
                }
            }

            return connectionString;
        }

        public NpgsqlConnection GetConnection()
        {
            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting connection from pool: {ex.Message}");
                throw;
            }
        }

        public string GetConnectionString()
        {
            string connstr = _appSettings.ConnectionString;

            //try
            //{
            //    string useAWS_SM = ConfigurationManager.AppSettings["UseAWS_SM"];
            //    string strAWS_SearchKey = ConfigurationManager.AppSettings["AWS_SearchKey"];

            //    if (!string.IsNullOrEmpty(useAWS_SM))
            //    {
            //        if (useAWS_SM.Equals("Y"))
            //        {
            //            SM awsSecretManager = new SM();
            //            string secretString = awsSecretManager.GetSecretString(strAWS_SearchKey);
            //            DBInfo dbSecret = awsSecretManager.GetSecretValue(secretString);
            //            NpgsqlConnectionStringBuilder npgsqlBuilder = new NpgsqlConnectionStringBuilder
            //            {
            //                Host = dbSecret.Server,
            //                Port = Convert.ToInt16(dbSecret.Port),
            //                Database = dbSecret.DBName,
            //                Username = dbSecret.UserName,
            //                Password = dbSecret.Password,
            //                Timeout = 30,
            //                Pooling = true,
            //                SslMode = Npgsql.SslMode.Prefer,
            //                TrustServerCertificate = true
            //            };

            //            //SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder
            //            //{
            //            //    DataSource = dbSecret.Server + "," + dbSecret.Port,
            //            //    InitialCatalog = dbSecret.DBName,
            //            //    Password = dbSecret.Password,
            //            //    UserID = dbSecret.UserName,
            //            //    ConnectTimeout = 30,
            //            //    MultipleActiveResultSets = true,
            //            //    Encrypt= true,
            //            //    TrustServerCertificate = true
            //            //};

            //            //connstr = sqlBuilder.ToString();
            //            connstr = npgsqlBuilder.ToString();
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally { }

            return connstr;
        }
    }
}