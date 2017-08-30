using EMT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.DAL
{
    public interface ICounterRepository
    {
        IEnumerable<CounterValue> GetValues(int counterId, DateTime timeFrom, DateTime timeTo);
        IEnumerable<CounterValue> GetValues(int counterId, DateTime after);
        IEnumerable<Counter> GetCounters();
        IEnumerable<int> GetCounterIdsAfter(DateTime after);
        void Insert(IEnumerable<CounterValue> values);
    }
}