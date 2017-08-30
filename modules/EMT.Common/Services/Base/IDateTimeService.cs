using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Common.Services.Base
{
    public interface IDateTimeService
    {
        DateTime UnixTimeStart { get; }
        DateTime UnixTimeToDateTime(long unixTimestamp);
        long DateTimeToUnixTime(DateTime dateTime);
        DateTime Now();
        long UnixTimeNow();
    }
}
