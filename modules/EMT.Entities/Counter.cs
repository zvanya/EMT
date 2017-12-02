using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Entities
{
    public class Counter
    {
        public int Id { get; set; }
        //public string Tag { get; set; }
        public int? LineId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string ISO { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        //public string DataName { get; set; } // Имена данных для Grafana id + " / " + ProductionLineName + " / " + CounterName
    }
}
