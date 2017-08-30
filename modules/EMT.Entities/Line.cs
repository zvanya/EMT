using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Entities
{
    public class Line
    {
        public int id { get; set; }
        public int id_werk { get; set; }
        public string name { get; set; }
    }

    public class LineWriteModelRead
    {
        public string connectionStringName { get; set; } //"Org-Test-1"
        public string typeInfo { get; set; } //"brand", "lineState", "lineMode"
        public long dtFrom { get; set; } //Unix time
        public int idLine { get; set; }
        public int idState { get; set; } //idBrandState, idLineState, idLineMode
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
