using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Entities
{
    public class Counter
    {
        public int id { get; set; }
        //public string Tag { get; set; }
        public int? lineId { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string iso { get; set; }
        public double? min { get; set; }
        public double? max { get; set; }
        //public string DataName { get; set; } // Имена данных для Grafana id + " / " + ProductionLineName + " / " + CounterName
    }
}
