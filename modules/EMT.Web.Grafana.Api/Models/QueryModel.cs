using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace EMT.Web.Grafana.Api.Models
{
    public class SearchQueryObjectModel
    {
        public string target { get; set; }
        public bool lineinfo { get; set; }
        public User user { get; set; }
    }

    public class User
    {
        public string email { get; set; }
        public string login { get; set; }
        public string orgName { get; set; }
        public string orgRole { get; set; }
    }

    public class Raw
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Range
    {
        public string from { get; set; }
        public string to { get; set; }
        public Raw raw { get; set; }
    }

    public class RangeRaw
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Target
    {
        //public string target { get; set; }
        public string target { get; set; }
        public string refId { get; set; }
        public string type { get; set; }
    }

    public class Interval
    {
        public string text { get; set; }
        public string value { get; set; }
    }

    public class IntervalMs
    {
        public int text { get; set; }
        public int value { get; set; }
    }

    public class ScopedVars
    {
        public Interval __interval { get; set; }
        public IntervalMs __interval_ms { get; set; }
    }

    public class QueryObjectModel
    {
        public int panelId { get; set; }
        public Range range { get; set; }
        public RangeRaw rangeRaw { get; set; }
        public string interval { get; set; }
        public int intervalMs { get; set; }
        public List<Target> targets { get; set; }
        public string format { get; set; }
        public int maxDataPoints { get; set; }
        public ScopedVars scopedVars { get; set; }
        public User user { get; set; }
    }

    [DataContract]
    public class DataPointsModel
    {
        [DataMember]
        public string target { get; set; }

        [DataMember]
        public List<List<double>> datapoints { get; set; }
    }

    public class DataPointTXTModel
    {
        [DataMember]
        public string target { get; set; }

        [DataMember]
        public List<List<string>> datapoints { get; set; }
    }

    public class Datapoint
    {
        public int pointNumber { get; set; }
        public long time { get; set; }
        public string pointName { get; set; }
        public string commentText { get; set; }
        public string fillColor { get; set; }
    }
    public class LineStatusModel
    {
        public int panelId { get; set; }
        public User user { get; set; }
        public string target { get; set; }
        public Datapoint datapoint { get; set; }

    }

    public class UpdateLineStatusAnswerModel
    {
        public string target { get; set; }
        public string statusWrite { get; set; }
    }

    //[DataContract]
    //public class LineWriteModel
    //{
    //    [DataMember(Name = "cnn", Order = 1)]
    //    public string connectionStringName { get; set; } //"Org-Test-1"
    //    [DataMember(Name = "tp", Order = 2)]
    //    public string typeInfo { get; set; } //"brand", "lineState", "lineMode"
    //    [DataMember(Name = "dt", Order = 3)]
    //    public long dtFrom { get; set; } //Unix time. "2017-02-17 08:55:48.000"
    //    [DataMember(Name = "idL", Order = 4)]
    //    public int idLine { get; set; }
    //    [DataMember(Name = "idS", Order = 5)]
    //    public int idState { get; set; } //idBrandState, idLineState, idLineMode
    //}
}
