using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{

    public class NetQuality : HPMarshaller
    {

    public int mLocalQuality;
    public int mRemoteQuality;

    public override void unmarshall(byte[] buf)
    {
        base.unmarshall(buf);
        mLocalQuality = popByte();
        mRemoteQuality = popByte();
    }
}
}
