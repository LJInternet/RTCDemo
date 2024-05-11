using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    public class TimeHelper
    {
        public static long GetMillSecond()
        { 
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        public static long GetCurrenTime() {
           return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }
}
