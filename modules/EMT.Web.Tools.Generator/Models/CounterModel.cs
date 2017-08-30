using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMT.Web.Tools.Generator.Models
{
    public class CounterModel
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public int? LineId { get; set; }
        public string Name { get; set; }
        public int? Color { get; set; }
        public string ISO { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
    }
}