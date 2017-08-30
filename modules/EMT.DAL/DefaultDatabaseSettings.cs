using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.DAL
{
    public class DefaultDatabaseSettings : IDatabaseSettings
    {
        private string _connectionString;

        public DefaultDatabaseSettings(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString { get { return _connectionString; } set { _connectionString = value; } }

        //public string ConnectionString => _connectionString;

    }
}
