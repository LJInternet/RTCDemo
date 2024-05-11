using LJ.RTC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    internal class VideoFrameRateControl : HPMarshaller
    {
        public int frameRate;

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            frameRate = popInt();
        }
    }
}
