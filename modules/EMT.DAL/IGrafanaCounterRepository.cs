using EMT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.DAL
{
    public interface IGrafanaCounterRepository
    {
        string ConnectionString { get; set; }
        IEnumerable<Werk> GetWerks();
        IEnumerable<Line> GetLines(string werkName);
        IEnumerable<Counter> GetCounters(string werkName);
        IEnumerable<Counter> GetCounters(string werkName, string lineName);
        Counter GetCounterId(string werkName, string lineName, string counterName);
        Line GetLineId(string werkName, string lineName);
        Werk GetWerkId(string werkName);
        IEnumerable<Counter> GetCounters();
        IEnumerable<CounterValue> GetValues(int counterId, DateTime timeFrom, DateTime timeTo);
        IEnumerable<CounterValue> GetValuesDapper(int counterId, DateTime timeFrom, DateTime timeTo);
        IEnumerable<CounterValue> GetLineStateValuesDapper(string werkName, string lineName, DateTime timeFrom, DateTime timeTo);
        IEnumerable<CounterValue> GetLineModeValuesDapper(string werkName, string lineName, DateTime timeFrom, DateTime timeTo);
        IEnumerable<CounterValue> GetBrandValuesDapper(string werkName, string lineName, DateTime timeFrom, DateTime timeTo);
        IEnumerable<CounterValue> GetCounterAnnotations(int counterId, DateTime timeFrom, DateTime timeTo);
        void Insert(IEnumerable<CounterValue> items);
        int UpdateLineStatus(CounterValue item, List<string> target);
        void WriteLineStatus(string typeInfo, DateTime dtFrom, int idLine, int idState);
        void WriteLinesStatus(IEnumerable<LineWriteModelInsert> items);
    }
}
