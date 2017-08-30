using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace EMT.Web.Grafana.Api.Models
{
    public class AnnotationReq
    {
        public string name { get; set; }
        public string datasource { get; set; }
        public bool enable { get; set; }
        public string iconColor { get; set; }
        public string query { get; set; }
        public bool enabled { get; set; }
    }

    public class RequestAnnotationModel
    {
        public Range range { get; set; }
        public AnnotationReq annotation { get; set; }
        public RangeRaw rangeRaw { get; set; }
    }

    public class AnnotationResp
    {
        public string name { get; set; }
        public bool enabled { get; set; }
        public string datasource { get; set; }
    }

    public class ResponceAnnotationModel
    {
        public AnnotationResp annotation { get; set; }
        public string title { get; set; }
        public long time { get; set; }
        public string text { get; set; }
        public string tags { get; set; }
    }
}