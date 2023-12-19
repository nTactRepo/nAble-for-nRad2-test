using nAble;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace nTact
{
    public class DatabaseWrapper
    {
        #region Properties

        public string ConnectionString { get; private set; } = "";

        public string DataSource
        {
            get => _dataSource;

            set
            {
                _dataSource = value;
                ConnectionString = MakeConnectionString(_dataSource, _initialCatalog);
            }
        }

        public string InitialCatalog
        {
            get => _initialCatalog;

            set
            {
                _initialCatalog = value;
                ConnectionString = MakeConnectionString(_dataSource, _initialCatalog);
            }
        }

        #endregion

        #region Data Members

        private string _dataSource = "";
        private string _initialCatalog = "";

        #endregion

        #region Functions

        #region Constructors

        public DatabaseWrapper(string dataSource, string initialCatalog)
        {
            DataSource = dataSource;
            InitialCatalog = initialCatalog;
            ConnectionString = MakeConnectionString(_dataSource, _initialCatalog);
        }

        #endregion

        #region Public Functions

        public bool TestConnection()
        {
            bool connected = false;

            using (var sql = new SqlConnection(ConnectionString))
            {
                try
                {
                    sql.Open();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Database Connect Failed!{Environment.NewLine}{ex.Message}", "ERROR");
                }

                if (sql.State == System.Data.ConnectionState.Open)
                {
                    connected = true;
                }
            }

            return connected;
        }

        public static string MakeConnectionString(string dataSource, string initialCatalog)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                IntegratedSecurity = false,
                InitialCatalog = initialCatalog,
                UserID = "nAble",
                Password = "nAble",
                ConnectTimeout = 3
            };

            return csb?.ToString() ?? "";
        }

        #endregion

        #endregion
    }
}
