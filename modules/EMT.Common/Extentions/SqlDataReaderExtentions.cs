using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Common.Extentions
{
    public static class SqlDataReaderExtentions
    {
        public static T GetNullableValue<T>(this SqlDataReader reader, string columnName)
        {
            int columnIndex = reader.GetOrdinal(columnName);
            return (reader.IsDBNull(columnIndex) == false)
                ? (T)reader.GetValue(columnIndex)
                : default(T);
        }
        public static T GetValue<T>(this SqlDataReader reader, string columnName)
        {
            int columnIndex = reader.GetOrdinal(columnName);

            var obj = reader.GetValue(columnIndex);

            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }

            //return (T)reader.GetValue(columnIndex);            
        }
    }
}