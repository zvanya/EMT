using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Web.Tools.Generator.Models
{
    public class CounterValueModel
    {
        public int CounterId { get; set; }
        public DateTime Time { get; set; }
        public double Value { get; set; }
    }
}