using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    public class PerformanceEnable : HPMarshaller
    {
        public bool enable;

        public PerformanceEnable(bool enable)
        {
            this.enable = enable;
        }

        public override byte[] marshall()
        {
            pushBool(enable);
            return base.marshall();
        }
    }
}
