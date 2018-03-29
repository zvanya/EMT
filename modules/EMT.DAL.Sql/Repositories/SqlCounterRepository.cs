using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMT.Entities;
using System.Data.SqlClient;
using EMT.Common.Extentions;
using EMT.Common.Services.Base;

namespace EMT.DAL.Sql.Repositories
{
    public class SqlCounterRepository : ICounterRepository
    {
        #region Queries

        //private const string SelectValuesQuery = "SELECT [dt],[value] FROM [dbo].[Counters_value] WHERE [id_counter] = @counter_id AND [dt] >= @time_from AND [dt] <= @time_to ORDER BY [dt]";
        //private const string SelectValuesAfterQuery = "SELECT [dt],[value] FROM [dbo].[Counters_value] WHERE [id_counter] = @counter_id AND [dt] > @time_after ORDER BY [dt]";
        private const string GetCounterValuesProcedure = "[dbo].[spGetCounterValues]";
        private const string SelectCountersQuery = "SELECT [id],[id_tag],[id_lines],[name],[color],[ISO],[minY],[maxY] FROM [dbo].[Counters] WHERE [id_lines] IS NOT NULL";
        private const string InsertCounterValueQuery = "INSERT INTO [dbo].[Counters_value] ([id_counter],[dt],[value]) VALUES (@CounterId, @Time, @Value)";
        private const string SelectCounterIdsAfterQuery = "SELECT DISTINCT [id_counter] FROM [dbo].[Counters_value] WHERE [dt] > @After";

        #endregion

        #region Fields

        private readonly IDatabaseSettings _databaseSettings;
        private readonly IProfilerService _profilerService;

        #endregion

        #region Constructors

        public SqlCounterRepository(IDatabaseSettings databaseSettings, IProfilerService profilerService)
        {
            _databaseSettings = databaseSettings;
            _profilerService = profilerService;
        }

        #endregion

        #region ICounterRepository
        
        public IEnumerable<Counter> GetCounters()
        {
            using (_profilerService.Step("SqlCounterRepository.GetCounters()"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectCountersQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Counter> counters = new List<Counter>();
                            while (reader.Read())
                            {
                                Counter counter = new Counter();
                                counter.id = reader.GetValue<int>("id");
                                //counter.Tag = reader.GetNullableValue<string>("id_tag");
                                counter.lineId = reader.GetNullableValue<int?>("id_lines");
                                counter.name = reader.GetNullableValue<string>("name");
                                counter.color = reader.GetNullableValue<string>("color");
                                counter.iso = reader.GetNullableValue<string>("ISO");
                                counter.min = reader.GetNullableValue<double?>("minY");
                                counter.max = reader.GetNullableValue<double?>("maxY");
                                counters.Add(counter);
                            }
                            return counters;
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

            using (_profilerService.Step($"SqlCounterRepository.GetValues(counterId:{counterId},timeFrom:{timeFrom},timeTo:{timeTo})"))
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

        public IEnumerable<CounterValue> GetValues(int counterId, DateTime after)
        {
            using (_profilerService.Step($"SqlCounterRepository.GetValues(counterId:{counterId},after:{after})"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(GetCounterValuesProcedure, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CounterId", counterId);
                        command.Parameters.AddWithValue("@TimeFrom", after);
                        command.Parameters.AddWithValue("@TimeTo", DBNull.Value);
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

        public IEnumerable<int> GetCounterIdsAfter(DateTime after)
        {
            using (_profilerService.Step($"SqlCounterRepository.GetCounterIdsAfter(after:{after})"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(SelectCounterIdsAfterQuery, connection))
                    {
                        command.Parameters.AddWithValue("@After", after);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<int> counterIds = new List<int>();
                            while (reader.Read())
                            {
                                int counterId = reader.GetValue<int>("id_counter");
                                counterIds.Add(counterId);
                            }
                            return counterIds;
                        }
                    }
                }
            }
        }

        public void Insert(IEnumerable<CounterValue> items)
        {
            using (_profilerService.Step($"SqlCounterRepository.Insert(itemsCount:{items.Count()})"))
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

        #endregion

        #region Helpers

        private CounterValue ReadCounterValue(SqlDataReader reader)
        {
            CounterValue counterValue = new CounterValue();
            counterValue.dt = reader.GetValue<DateTime>("dt");
            counterValue.value = reader.GetValue<Double>("value");
            return counterValue;
        }

        #endregion
    }

}