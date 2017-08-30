using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMT.Entities;
using System.Data.SqlClient;
using EMT.Common.Extentions;
using EMT.Common.Services.Base;
using Dapper;
using System.Data;


namespace EMT.DAL.Sql.Repositories
{
    class SqlGrafanaCounterRepository : IGrafanaCounterRepository
    {
        #region Queries

        private const string GetCounterValuesProcedure = "[dbo].[spGetCounterValues]";
        private const string SelectCountersQuery = "SELECT [id],[id_tag],[id_lines],[name],[color],[ISO],[minY],[maxY] FROM [dbo].[Counters] WHERE [id_lines] IS NOT NULL";
        private const string InsertCounterValueQuery = "INSERT INTO [dbo].[Counters_value] ([id_counter],[dt],[value]) VALUES (@CounterId, @Time, @Value)";
        private const string SelectCounterIdsAfterQuery = "SELECT DISTINCT [id_counter] FROM [dbo].[Counters_value] WHERE [dt] > @After";

        private const string GetGrafanaCounterAnnotationsProcedure = "[dbo].[spGetCounterAnnotations]";
        private const string SelectGrafanaCountersQuery = "SELECT id, convert(NVARCHAR, id) + ' / ' + (SELECT (SELECT RTRIM(name) FROM Werks WHERE id = id_werks) + ' / ' + RTRIM(name) from Lines WHERE id = id_lines) + ' / ' + RTRIM(name) as 'dataName' FROM Counters where id_lines IS NOT NULL";
        private const string SelectGrafanaWerks = "SELECT id, LTRIM(RTRIM(name)) as name FROM Werks ORDER BY name";

        #endregion

        #region Fields

        //private readonly IDatabaseSettings _databaseSettings;
        private IDatabaseSettings _databaseSettings;
        private readonly IProfilerService _profilerService;

        const string lineState = "__productionProcess";
        const string lineMode = "__operationMode";
        const string brand = "__brandName";

        #endregion

        #region Properties
        public string ConnectionString { get; set; }
        #endregion

        #region Constructors

        public SqlGrafanaCounterRepository(IDatabaseSettings databaseSettings, IProfilerService profilerService)
        {
            _databaseSettings = databaseSettings;
            _profilerService = profilerService;
        }

        #endregion

        void SetConnectionString()
        {
            _databaseSettings.ConnectionString = ConnectionString;
        }

        #region IGrafanaCounterRepository

        public IEnumerable<Werk> GetWerks()
        {
            SetConnectionString();

            using (_profilerService.Step("SqlGrafanaCounterRepository.GetWerks()"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectGrafanaWerks, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Werk> werks = new List<Werk>();
                            while (reader.Read())
                            {
                                Werk werk = new Werk();
                                werk.id = reader.GetValue<int>("id");
                                werk.name = reader.GetValue<string>("name");
                                werks.Add(werk);
                            }
                            return werks;
                        }
                    }
                }
            }
        }

        public IEnumerable<Line> GetLines(string werkName)
        {
            SetConnectionString();

            string SelectGrafanaLines = $"SELECT id, id_werks, LTRIM(RTRIM(name)) as name FROM Lines WHERE id_werks = (SELECT id FROM Werks WHERE LTRIM(RTRIM(name)) = '{werkName}') AND name IS NOT NULL ORDER BY name";

            using (_profilerService.Step("SqlGrafanaCounterRepository.GetLines(string werkName)"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectGrafanaLines, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Line> lines = new List<Line>();
                            while (reader.Read())
                            {
                                Line line = new Line();
                                line.id = reader.GetValue<int>("id");
                                line.id_werk = reader.GetValue<int>("id_werks");
                                line.name = reader.GetValue<string>("name");
                                lines.Add(line);
                            }
                            return lines;
                        }
                    }
                }
            }
        }

        public IEnumerable<Counter> GetCounters(string werkName)
        {
            List<Counter> counters = new List<Counter>();

            return counters;
        }

        public IEnumerable<Counter> GetCounters(string werkName, string lineName)
        {
            SetConnectionString();

            string SelectGrafanaCounters = $"SELECT id, LTRIM(RTRIM(name)) as name FROM Counters WHERE id_lines = (SELECT id FROM Lines WHERE LTRIM(RTRIM(name)) = '{lineName}' AND id_werks = ((SELECT id FROM Werks WHERE LTRIM(RTRIM(name)) = '{werkName}')))";

            using (_profilerService.Step("SqlGrafanaCounterRepository.GetCounters(string werkName, string lineName)"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectGrafanaCounters, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Counter> counters = new List<Counter>();
                            while (reader.Read())
                            {
                                Counter counter = new Counter();
                                counter.Id = reader.GetValue<int>("id");
                                counter.Name = reader.GetValue<string>("name");
                                counters.Add(counter);
                            }
                            return counters;
                        }
                    }
                }
            }
        }

        public IEnumerable<Counter> GetCounters()
        {
            SetConnectionString();

            using (_profilerService.Step("SqlGrafanaCounterRepository.GetCounters()"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectGrafanaCountersQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Counter> counters = new List<Counter>();
                            while (reader.Read())
                            {
                                Counter counter = new Counter();
                                counter.Id = reader.GetValue<int>("id");
                                counter.DataName = reader.GetValue<string>("dataName");
                                counters.Add(counter);
                            }
                            return counters;
                        }
                    }
                }
            }
        }

        public Counter GetCounterId(string werkName, string lineName, string counterName)
        {
            SetConnectionString();

            string SelectGrafanaCounterId = $"SELECT id FROM Counters WHERE id_lines = (SELECT id FROM Lines WHERE LTRIM(RTRIM(name)) = '{lineName}' AND id_werks = ((SELECT id FROM Werks WHERE LTRIM(RTRIM(name)) = '{werkName}'))) AND LTRIM(RTRIM(name)) = '{counterName}'";

            using (_profilerService.Step("SqlGrafanaCounterRepository.GetCounterId(string werkName, string lineName, string counterName)"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectGrafanaCounterId, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Counter counter = new Counter();
                            while (reader.Read())
                            {
                                counter.Id = reader.GetValue<int>("id");
                            }
                            return counter;
                        }
                    }
                }
            }
        }

        public Line GetLineId(string werkName, string lineName)
        {
            SetConnectionString();

            string SelectGrafanaLineId = $"SELECT id FROM Lines WHERE id_werks = ((SELECT id FROM Werks WHERE LTRIM(RTRIM(name)) = '{werkName}')) AND LTRIM(RTRIM(name)) = '{lineName}'";

            using (_profilerService.Step("SqlGrafanaCounterRepository.GetLineId(string werkName, string lineName)"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectGrafanaLineId, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Line line = new Line();
                            while (reader.Read())
                            {
                                line.id = reader.GetValue<int>("id");
                            }
                            return line;
                        }
                    }
                }
            }
        }

        public Werk GetWerkId(string werkName)
        {
            SetConnectionString();

            string SelectGrafanaWerkId = $"SELECT id FROM Werks WHERE LTRIM(RTRIM(name)) = '{werkName}'";

            using (_profilerService.Step("SqlGrafanaCounterRepository.GetWerkId(string werkName)"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectGrafanaWerkId, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Werk werk = new Werk();
                            while (reader.Read())
                            {
                                werk.id = reader.GetValue<int>("id");
                            }
                            return werk;
                        }
                    }
                }
            }
        }

        public IEnumerable<CounterValue> GetValues(int counterId, DateTime timeFrom, DateTime timeTo)
        {
            #region Validation
            if (timeFrom > timeTo)
            {
                throw new ArgumentException("'timeFrom' should not be greater than 'timeTo'");
            }
            #endregion

            SetConnectionString();

            using (_profilerService.Step($"SqlGrafanaCounterRepository.GetValues(counterId:{counterId},timeFrom:{timeFrom},timeTo:{timeTo})"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(GetCounterValuesProcedure, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CounterId", counterId);
                        command.Parameters.AddWithValue("@TimeFrom", timeFrom);
                        command.Parameters.AddWithValue("@TimeTo", timeTo);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<CounterValue> counterValues = new List<CounterValue>();
                            while (reader.Read())
                            {
                                CounterValue counterValue = ReadCounterValue(reader);
                                counterValues.Add(counterValue);
                            }
                            return counterValues;
                        }
                    }
                }
            }
        }

        public IEnumerable<CounterValue> GetValuesDapper(int counterId, DateTime timeFrom, DateTime timeTo)
        {
            #region Validation
            if (timeFrom > timeTo)
            {
                throw new ArgumentException("'timeFrom' should not be greater than 'timeTo'");
            }
            #endregion

            SetConnectionString();

            using (_profilerService.Step($"SqlGrafanaCounterRepository.GetValuesDapper(counterId:{counterId},timeFrom:{timeFrom},timeTo:{timeTo})"))
            {
                IDbConnection db = new SqlConnection(_databaseSettings.ConnectionString);

                var p = new DynamicParameters();
                p.Add("@CounterId", counterId, dbType: DbType.Int32);
                p.Add("@TimeFrom", timeFrom, dbType: DbType.DateTime);
                p.Add("@TimeTo", timeTo, dbType: DbType.DateTime);

                List<CounterValue> counterValues = db.Query<CounterValue>(GetCounterValuesProcedure, p, commandType: CommandType.StoredProcedure).ToList<CounterValue>();

                return counterValues;
            }
        }

        public IEnumerable<CounterValue> GetLineStateValuesDapper(string werkName, string lineName, DateTime timeFrom, DateTime timeTo)
        {
            #region Validation
            if (timeFrom > timeTo)
            {
                throw new ArgumentException("'timeFrom' should not be greater than 'timeTo'");
            }
            #endregion

            SetConnectionString();

            //string SelectGrafanaLineState = $"SELECT idLineState as value, dtFrom as dt, (SELECT name FROM LineState WHERE LineState.id = LineStateStartTime.idLineState) as Name, Comment, (SELECT color FROM LineState WHERE LineState.id = idLineState) as fillColor " +
            //                                    $"FROM LineStateStartTime " +
            //                                    $"WHERE idLine = (SELECT id FROM Lines WHERE LTRIM(rtrim(name)) = LTRIM(RTRIM('{lineName}')) AND id_werks = (SELECT id FROM Werks WHERE LTRIM(rtrim(Werks.name)) = LTRIM(RTRIM('{werkName}')))) AND " +
            //                                    $"dtFrom BETWEEN '{string.Format("{0:yyyy-MM-dd HH:mm:ss}", timeFrom)}' AND '{string.Format("{0:yyyy-MM-dd HH:mm:ss}", timeTo)}' " +
            //                                    $"ORDER BY dtFrom";

            string sp_getLineStatus_Name = "spGetLineStateValues";

            using (_profilerService.Step($"SqlGrafanaCounterRepository.GetLineStateValuesDapper(werkName:{werkName},lineName:{lineName},timeFrom:{timeFrom},timeTo:{timeTo})"))
            {
                IDbConnection db = new SqlConnection(_databaseSettings.ConnectionString);

                var p = new DynamicParameters();
                p.Add("@lineName", lineName, dbType: DbType.String);
                p.Add("@werkName", werkName, dbType: DbType.String);
                p.Add("@dtFrom", timeFrom, dbType: DbType.DateTime);
                p.Add("@dtTo", timeTo, dbType: DbType.DateTime);

                List<CounterValue> values = db.Query<CounterValue>(sp_getLineStatus_Name, p, commandType: CommandType.StoredProcedure).ToList<CounterValue>();

                //List<CounterValue> values = db.Query<CounterValue>(SelectGrafanaLineState).ToList<CounterValue>();

                return values;
            }
        }

        public IEnumerable<CounterValue> GetLineModeValuesDapper(string werkName, string lineName, DateTime timeFrom, DateTime timeTo)
        {
            #region Validation
            if (timeFrom > timeTo)
            {
                throw new ArgumentException("'timeFrom' should not be greater than 'timeTo'");
            }
            #endregion

            SetConnectionString();

            //string SelectGrafanaLineState = $"SELECT idLineMode as value, dtFrom as dt, (SELECT name FROM LineMode WHERE LineMode.id = LineModeStartTime.idLineMode) as Name, Comment, (SELECT color FROM LineMode WHERE LineMode.id = idLineMode) as fillColor  " +
            //                                    $"FROM LineModeStartTime " +
            //                                    $"WHERE idLine = (SELECT id FROM Lines WHERE LTRIM(rtrim(name)) = LTRIM(RTRIM('{lineName}')) AND id_werks = (SELECT id FROM Werks WHERE LTRIM(rtrim(Werks.name)) = LTRIM(RTRIM('{werkName}')))) AND " +
            //                                    $"dtFrom BETWEEN '{string.Format("{0:yyyy-MM-dd HH:mm:ss}", timeFrom)}' AND '{string.Format("{0:yyyy-MM-dd HH:mm:ss}", timeTo)}' " +
            //                                    $"ORDER BY dtFrom";

            string sp_getLineStatus_Name = "spGetLineModeValues";

            using (_profilerService.Step($"SqlGrafanaCounterRepository.GetLineModeValuesDapper(werkName:{werkName},lineName:{lineName},timeFrom:{timeFrom},timeTo:{timeTo})"))
            {
                IDbConnection db = new SqlConnection(_databaseSettings.ConnectionString);

                var p = new DynamicParameters();
                p.Add("@lineName", lineName, dbType: DbType.String);
                p.Add("@werkName", werkName, dbType: DbType.String);
                p.Add("@dtFrom", timeFrom, dbType: DbType.DateTime);
                p.Add("@dtTo", timeTo, dbType: DbType.DateTime);

                List<CounterValue> values = db.Query<CounterValue>(sp_getLineStatus_Name, p, commandType: CommandType.StoredProcedure).ToList<CounterValue>();

                //List<CounterValue> values = db.Query<CounterValue>(SelectGrafanaLineState).ToList<CounterValue>();

                return values;
            }
        }

        public IEnumerable<CounterValue> GetBrandValuesDapper(string werkName, string lineName, DateTime timeFrom, DateTime timeTo)
        {
            #region Validation
            if (timeFrom > timeTo)
            {
                throw new ArgumentException("'timeFrom' should not be greater than 'timeTo'");
            }
            #endregion

            SetConnectionString();

            //string SelectGrafanaLineState = $"SELECT idBrand as value, dtFrom as dt, (SELECT name FROM Brands WHERE Brands.id = BrandStartTime.idBrand) as Name, Comment, (SELECT color FROM Brands WHERE Brands.id = idBrand) as fillColor  " +
            //                                    $"FROM BrandStartTime " +
            //                                    $"WHERE idLine = (SELECT id FROM Lines WHERE LTRIM(rtrim(name)) = LTRIM(RTRIM('{lineName}')) AND id_werks = (SELECT id FROM Werks WHERE LTRIM(rtrim(Werks.name)) = LTRIM(RTRIM('{werkName}')))) AND " +
            //                                    $"dtFrom BETWEEN '{string.Format("{0:yyyy-MM-dd HH:mm:ss}", timeFrom)}' AND '{string.Format("{0:yyyy-MM-dd HH:mm:ss}", timeTo)}' " +
            //                                    $"ORDER BY dtFrom";

            string sp_getLineStatus_Name = "spGetBrandsValues";

            using (_profilerService.Step($"SqlGrafanaCounterRepository.GetBrandValuesDapper(werkName:{werkName},lineName:{lineName},timeFrom:{timeFrom},timeTo:{timeTo})"))
            {
                IDbConnection db = new SqlConnection(_databaseSettings.ConnectionString);

                var p = new DynamicParameters();
                p.Add("@lineName", lineName, dbType: DbType.String);
                p.Add("@werkName", werkName, dbType: DbType.String);
                p.Add("@dtFrom", timeFrom, dbType: DbType.DateTime);
                p.Add("@dtTo", timeTo, dbType: DbType.DateTime);

                List<CounterValue> values = db.Query<CounterValue>(sp_getLineStatus_Name, p, commandType: CommandType.StoredProcedure).ToList<CounterValue>();

                //List<CounterValue> values = db.Query<CounterValue>(SelectGrafanaLineState).ToList<CounterValue>();

                return values;
            }
        }

        public int UpdateLineStatus(CounterValue item, List<string> target)
        {
            int result = -1;
            string tbl1 = string.Empty, tbl2 = string.Empty;
            string idColumnName = string.Empty;

            SetConnectionString();

            switch (target[2])
            {
                case lineState:
                    tbl1 = "LineState";
                    tbl2 = "LineStateStartTime";
                    idColumnName = "idLineState";
                    break;
                case lineMode:
                    tbl1 = "LineMode";
                    tbl2 = "LineModeStartTime";
                    idColumnName = "idLineMode";
                    break;
                case brand:
                    tbl1 = "Brands";
                    tbl2 = "BrandStartTime";
                    idColumnName = "idBrand";
                    break;
            }

            string UpdateLineStatusColorQuery = $"UPDATE {tbl1} SET color = '{item.fillColor}' WHERE id = {item.value}";
            string UpdateLineStatusCommentQuery = $"UPDATE {tbl2} SET Comment = '{item.Comment}' " +
                $"WHERE {idColumnName} = {item.value} AND " +
                $"dtFrom = '{string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.dt)}' AND " +
                $"idLine = (SELECT id FROM Lines WHERE LTRIM(rtrim(name)) = '{target[1]}' AND id_werks = (SELECT id FROM Werks WHERE LTRIM(rtrim(Werks.name)) = '{target[0]}')) ";

            string updateQuery = UpdateLineStatusColorQuery + " " + UpdateLineStatusCommentQuery;

            using (_profilerService.Step($"SqlGrafanaCounterRepository.UpdateLineStatus"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                command.Transaction = transaction;
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            result =  1;
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            result = -1;
                            //throw e;
                        }
                    }
                }
            }

            return result;
        }

        public void Insert(IEnumerable<CounterValue> items)
        {
            SetConnectionString();

            using (_profilerService.Step($"SqlGrafanaCounterRepository.Insert(itemsCount:{items.Count()})"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in items)
                            {
                                using (SqlCommand command = new SqlCommand(InsertCounterValueQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddWithValue("@CounterId", item.id_counter);
                                    command.Parameters.AddWithValue("@Time", item.dt);
                                    command.Parameters.AddWithValue("@Value", item.value);
                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            throw e;
                        }
                    }
                }
            }
        }

        public void WriteLinesStatus(IEnumerable<LineWriteModelInsert> items)
        {
            SetConnectionString();

            string tblName = string.Empty;
            string idStateColumnName = string.Empty;

            string insertQuery = string.Empty;

            using (_profilerService.Step($"SqlGrafanaCounterRepository.WriteLineStatus"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in items)
                            {
                                switch (Int32.Parse(item.typeInfo))
                                {
                                    case 1:
                                        item.typeInfo = "brand";
                                        break;
                                    case 2:
                                        item.typeInfo = "lineState";
                                        break;
                                    case 3:
                                        item.typeInfo = "lineMode";
                                        break;
                                }

                                switch (item.typeInfo)
                                {
                                    case "brand":
                                        tblName = "BrandStartTime";
                                        idStateColumnName = "idBrand";
                                        break;
                                    case "lineState":
                                        tblName = "LineStateStartTime";
                                        idStateColumnName = "idLineState";
                                        break;
                                    case "lineMode":
                                        tblName = "LineModeStartTime";
                                        idStateColumnName = "idLineMode";
                                        break;
                                }

                                insertQuery = $"INSERT INTO {tblName} (dtFrom, idLine, {idStateColumnName}) VALUES (@dtFrom,@idLine,@idState)";

                                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddWithValue("@dtFrom", string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.dtFrom));
                                    command.Parameters.AddWithValue("@idLine", item.idLine);
                                    command.Parameters.AddWithValue("@idState", item.idState);
                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            throw e;
                        }
                    }
                }
            }
        }

        public void WriteLineStatus(string typeInfo, DateTime dtFrom, int idLine, int idState)
        {
            SetConnectionString();

            string tblName = string.Empty;
            string idStateColumnName = string.Empty;

            switch (typeInfo)
            {
                case "brand":
                    tblName = "BrandStartTime";
                    idStateColumnName = "idBrand";
                    break;
                case "lineState":
                    tblName = "LineStateStartTime";
                    idStateColumnName = "idLineState";
                    break;
                case "lineMode":
                    tblName = "LineModeStartTime";
                    idStateColumnName = "idLineMode";
                    break;
            }

            string insertQuery = $"INSERT INTO {tblName} (dtFrom, idLine, {idStateColumnName}) VALUES (@dtFrom,@idLine,@idState)";

            using (_profilerService.Step($"SqlGrafanaCounterRepository.WriteLineStatus"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                command.Transaction = transaction;
                                command.Parameters.AddWithValue("@dtFrom", string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtFrom));
                                command.Parameters.AddWithValue("@idLine", idLine);
                                command.Parameters.AddWithValue("@idState", idState);
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            throw e;
                        }
                    }
                }
            }
        }

        public IEnumerable<CounterValue> GetCounterAnnotations(int counterId, DateTime timeFrom, DateTime timeTo)
        {
            #region Validation
            if (timeFrom > timeTo)
            {
                throw new ArgumentException("'timeFrom' should not be greater than 'timeTo'");
            }
            #endregion

            SetConnectionString();

            using (_profilerService.Step($"SqlGrafanaCounterRepository.GetCounterAnnotations(counterId:{counterId},timeFrom:{timeFrom},timeTo:{timeTo})"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(GetGrafanaCounterAnnotationsProcedure, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CounterId", counterId);
                        command.Parameters.AddWithValue("@TimeFrom", timeFrom);
                        command.Parameters.AddWithValue("@TimeTo", timeTo);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<CounterValue> counterValues = new List<CounterValue>();
                            while (reader.Read())
                            {
                                CounterValue counterValue = ReadCounterAnnotation(reader);
                                counterValues.Add(counterValue);
                            }
                            return counterValues;
                        }
                    }
                }
            }
        }
        #endregion

        #region Helpers

        private CounterValue ReadCounterValue(SqlDataReader reader)
        {
            CounterValue counterValue = new CounterValue();
            counterValue.dt = reader.GetValue<DateTime>("dt");
            counterValue.value = reader.GetValue<Double>("value");
            return counterValue;
        }
        private CounterValue ReadCounterAnnotation(SqlDataReader reader)
        {
            CounterValue counterValue = new CounterValue();
            counterValue.dt = reader.GetValue<DateTime>("dt");
            counterValue.annotation = reader.GetValue<string>("annotation");
            return counterValue;
        }

        #endregion

    }
}
