using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    internal class JNIWebSocketMessage : HPMarshaller
    {

        public String cmd;
        public String msg;

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            cmd = popString16();
            msg = popString16();
        }
    }


    public class SendMsgObj
    {


        public string seq = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() + "";

        private string cmd = "UniRelaySdKMsg";
 
        private DataObj data;

        public SendMsgObj(DataObj data)
        {
            this.data = data;
        }

        public class DataObj
        {

            private string cmd;
            private string sdkMsg;

            public string getCmd()
            {
                return cmd;
            }

            public void setCmd(string cmd)
            {
                this.cmd = cmd;
            }

            public string getSdkMsg()
            {
                return sdkMsg;
            }

            public void setSdkMsg(string sdkMsg)
            {
                this.sdkMsg = sdkMsg;
            }
        }
    }
}
