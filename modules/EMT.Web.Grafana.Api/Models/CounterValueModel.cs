using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace EMT.Web.Grafana.Api.Controllers
{
    [DataContract]
    public class CounterValueModel
    {
        public int id_counter { get; set; }
        [DataMember]
        public double Value { get; set; }
        [DataMember]
        public long Time { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string fillColor { get; set; }
    }

    [DataContract]
    public class CounterValueInsertModel
    {
        [DataMember(Name = "c", Order = 1)]
        public int CounterId { get; set; }

        [DataMember(Name = "t", Order = 2)]
        public long Time { get; set; }

        [DataMember(Name = "v", Order = 3)]
        public double Value { get; set; }
    }

    //public class LineStatusModel
    //{
    //    public string typeInfo { get; set; }
    //    public long dtFrom { get; set; }
    //    public int idLine { get; set; }
    //    public int idState { get; set; }
    //}

    public class CounterValueInsert
    {
        public string connectionStringName { get; set; }
        public List<CounterValueInsertModel> counterValue { get; set; }
    }
}