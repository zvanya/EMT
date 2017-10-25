using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EMT.Entities
{
    public class Line
    {
        public int id { get; set; }
        public int id_werk { get; set; }
        public string name { get; set; }
    }

    [DataContract]
    public class LineWriteModelRead
    {
        [DataMember(Name = "cnn", Order = 1)]
        public string connectionStringName { get; set; } //"Org-Test-1"
        [DataMember(Name = "tp", Order = 2)]
        public string typeInfo { get; set; } //1:"brand", 2:"lineState", 3:"lineMode"
        [DataMember(Name = "dt", Order = 4)]
        public long dtFrom { get; set; } //"2017-02-17 08:55:48.000"
        [DataMember(Name = "idL", Order = 5)]
        public int idLine { get; set; }
        [DataMember(Name = "idS", Order = 6)]
        public int idState { get; set; } //idBrandState, idLineState, idLineMode //зн-е приходит из PLC
    }

    public class LineWriteModelInsert
    {
        public string connectionStringName { get; set; } //"Org-Test-1"
        public string typeInfo { get; set; } //"brand", "lineState", "lineMode"
        public DateTime dtFrom { get; set; } //"2017-02-17 08:55:48.000"
        public int idLine { get; set; }
        public int idState { get; set; } //idBrandState, idLineState, idLineMode
    }
}
