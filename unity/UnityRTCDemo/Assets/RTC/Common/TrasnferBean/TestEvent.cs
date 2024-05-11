using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    internal class TestSaveVideoFileEvent : HPMarshaller
    {
        public bool enable;

        public TestSaveVideoFileEvent(bool enable)
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
