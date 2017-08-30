using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Common.Services.Base
{
    public interface ICsvService
    {        
        string ToCSV(IEnumerable data, bool headerRow = false);
        IEnumerable<T> FromCSV<T>(string csv, bool headerRow = false);
    }
}
