using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace EMT.Web.Api.Receive.Models
{
    [DataContract]
    public class CounterValueModel
    {
        [DataMember(Name = "c", Order = 1)]
        public int CounterId { get; set; }

        [DataMember(Name = "t", Order = 2)]
        public long Time { get; set; }

        [DataMember(Name = "v", Order = 3)]
        public double Value { get; set; }
    }
}