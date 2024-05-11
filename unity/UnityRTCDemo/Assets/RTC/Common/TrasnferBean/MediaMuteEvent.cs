using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    internal class MediaMuteEvent : HPMarshaller
    {
        public static int EVENT_VIDEO = 1;
        public static int EVENT_AUDIO = 2;
        public static int EVENT_NONE = 0;

        public int mediaType = EVENT_NONE;
        public bool mute = false;

        public MediaMuteEvent(int mediaType, bool mute)
        {
            this.mediaType = mediaType;
            this.mute = mute;
        }


        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            pushInt(mediaType);
            pushBool(mute);

        }
    }
}
