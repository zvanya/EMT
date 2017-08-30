using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EMT.Web.Api.Models
{
    [DataContract]
    public class CounterValueModel
    {
        [DataMember(Name ="t", Order = 1)]
        public long Time { get; set; }
        [DataMember(Name = "v", Order = 2)]
        public double Value { get; set; }
    }
}