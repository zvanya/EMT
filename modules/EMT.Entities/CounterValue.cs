using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Entities
{
    public class CounterValue
    {
        public int id_counter { get; set; }
        public DateTime dt { get; set; }
        public double value { get; set; }
        public string annotation { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string fillColor { get; set; }
    }
}