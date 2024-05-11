
using Newtonsoft.Json;
using System;


namespace LJ.RTC.Common
{

    public class TransferHelper
    {
        public static String getSendMsgObjStr(byte[] result)
        {
            JNIWebSocketMessage msg = new JNIWebSocketMessage();
            msg.unmarshall(result);

            SendMsgObj.DataObj data = new SendMsgObj.DataObj();
            data.setCmd(msg.cmd);
            data.setSdkMsg(msg.msg);

            SendMsgObj obj = new SendMsgObj(data);
            return JsonConvert.SerializeObject(obj); 
        }
    }
}
