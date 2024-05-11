using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    internal class WebSocketMessage : HPMarshaller
    {

        private string cmd;

        private string msg;

        public WebSocketMessage(String cmdStr, String jsonStr)
        {
            this.cmd = cmdStr;
            this.msg = jsonStr;
        }


        public override byte[] marshall()
        {
            pushString16(cmd);
            pushString16(msg);

            return base.marshall();
        }
    }
}
