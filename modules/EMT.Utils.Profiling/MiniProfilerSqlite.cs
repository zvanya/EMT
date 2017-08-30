using StackExchange.Profiling;
using StackExchange.Profiling.Storage;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using StackExchange.Profiling.Helpers.Dapper;

namespace EMT.Utils.Profiling
{
    public static class MiniProfilerSqlite
    {
        #region Private Classes

        private class SqliteMiniProfilerStorage : SqlServerStorage
        {
            /// <summary>
            /// Initialises a new instance of the <see cref="SqliteMiniProfilerStorage"/> class.
            /// </summary>
            /// <param name="connectionString">The connection string.</param>
            public SqliteMiniProfilerStorage(string connectionString)
                : base(connectionString)
            {
            }

            /// <summary>
            /// Get the Connection.
            /// </summary>
            /// <returns>The Abstracted Connection</returns>
            protected override System.Data.Common.DbConnection GetConnection()
            {
                return new System.Data.SQLite.SQLiteConnection(ConnectionString);
            }

            /// <summary>
            /// Used for testing purposes - destroys and recreates the SQLITE file with needed tables.
            /// </summary>
            /// <param name="extraTablesToCreate">The Extra Tables To Create.</param>
            public void RecreateDatabase(bool deleteExisting, params string[] extraTablesToCreate)
            {
                SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder(ConnectionString);
                string databasePath;
                if (ConnectionString.Contains("|DataDirectory|"))
                {
                    var dataDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
                    if (Directory.Exists(dataDirectory) == false)
                    {
                        Directory.CreateDirectory(dataDirectory);
                    }
                    var databaseFile = builder.DataSource.Replace("|DataDirectory|", string.Empty);
                    databasePath = Path.Combine(dataDirectory, databaseFile);
                }
                else
                {
                    databasePath = builder.DataSource;
                }

                if (File.Exists(databasePath))
                {
                    if (deleteExisting == true)
                    {
                        File.Delete(databasePath);
                    }
                    else
                    {
                        return;
                    }
                }

                using (var cnn = new System.Data.SQLite.SQLiteConnection(this.ConnectionString))
                {
                    cnn.Open();

                    // we need some tiny mods to allow sqlite support 
                    foreach (var sql in TableCreationScripts.Union(extraTablesToCreate))
                    {
                        cnn.Execute(sql);
                    }
                }
            }

            /// <summary>
            /// The list of results.
            /// </summary>
            /// <param name="maxResults">The max results.</param>
            /// <param name="start">The start</param>
            /// <param name="finish">The finish</param>
            /// <param name="orderBy">The order by.</param>
            /// <returns>The result set</returns>
            public override IEnumerable<Guid> List(
                int maxResults,
                DateTime? start = null,
                DateTime? finish = null,
                ListResultsOrder orderBy = ListResultsOrder.Descending)
            {
                var builder = new SqlBuilder();
                var t = builder.AddTemplate("select Id from MiniProfilers /**where**/ /**orderby**/ LIMIT(" + maxResults + ")");

                if (start != null)
                {
                    builder.Where("Started > @start", new { start });
                }
                if (finish != null)
                {
                    builder.Where("Started < @finish", new { finish });
                }

                builder.OrderBy(orderBy == ListResultsOrder.Descending ? "Started desc" : "Started asc");

                using (var conn = GetOpenConnection())
                {
                    return conn.Query<Guid>(t.RawSql, t.Parameters).ToList();
                }
            }

            private static readonly List<string> TableCreationScripts = new List<string>{@"
                CREATE TABLE MiniProfilers
                  (
                     RowId                                integer not null primary key,
                     Id                                   uniqueidentifier not null, 
                     RootTimingId                         uniqueidentifier null,
                     Started                              datetime not null,
                     DurationMilliseconds                 decimal(9, 3) not null,
                     User                                 nvarchar(100) null,
                     HasUserViewed                        bit not null,
                     MachineName                          nvarchar(100) null,
                     CustomLinksJson                      text null,
                     ClientTimingsRedirectCount           int null
                  );",
                     @"create table MiniProfilerTimings
                  (
                     RowId                               integer not null primary key,
                     Id                                  uniqueidentifier not null,
                     MiniProfilerId                      uniqueidentifier not null,
                     ParentTimingId                      uniqueidentifier null,
                     Name                                nvarchar(200) not null,
                     DurationMilliseconds                decimal(9, 3) not null,
                     StartMilliseconds                   decimal(9, 3) not null,
                     IsRoot                              bit not null,
                     Depth                               smallint not null,
                     CustomTimingsJson                   text null
                  );",
                     @" create table MiniProfilerClientTimings
                  (
                     RowId                               integer not null primary key,
                     Id                                  uniqueidentifier not null,
                     MiniProfilerId                      uniqueidentifier not null,
                     Name                                nvarchar(200) not null,
                     Start                               decimal(9, 3) not null,
                     Duration                            decimal(9, 3) not null
                  );"};
        }


        #endregion

        public static void Initialize(string connectionString, bool deleteExisting = false)
        {
            var storage = new SqliteMiniProfilerStorage(connectionString);
            storage.RecreateDatabase(deleteExisting);
            MiniProfiler.Settings.Storage = storage;
        }
    }
}
