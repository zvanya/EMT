using CsvHelper;
using CsvHelper.Configuration;
using EMT.Common.Services.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Common.Services
{
    public class CsvService : ICsvService
    {

        #region ICsvService        

        public string ToCSV(IEnumerable data, bool headerRow = false)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteCSV(data, writer, headerRow);
                return writer.ToString();
            }
        }

        public IEnumerable<T> FromCSV<T>(string csv, bool headerRow = false)
        {
            using (StringReader reader = new StringReader(csv))
            {
                return ReadCSV<T>(reader, headerRow);
            }
        }

        #endregion

        #region Helpers

        private void WriteCSV(IEnumerable data, TextWriter output, bool headerRow = false)
        {
            CsvConfiguration config = new CsvConfiguration()
            {
                HasHeaderRecord = headerRow,
                CultureInfo = CultureInfo.InvariantCulture
            };
            var csv = new CsvWriter(output, config);
            csv.WriteRecords(data);
        }

        private IEnumerable<T> ReadCSV<T>(TextReader input, bool headerRow = false)
        {
            CsvConfiguration config = new CsvConfiguration()
            {
                HasHeaderRecord = headerRow,
                CultureInfo = CultureInfo.InvariantCulture
            };
            var reader = new CsvReader(input, config);
            return reader.GetRecords<T>().ToArray();
        }

        #endregion

    }
}
