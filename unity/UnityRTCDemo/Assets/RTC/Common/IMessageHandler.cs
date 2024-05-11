using LJ.RTC.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC
{
    public interface IMessageHandler
    {
        /**
         * 发送SDK消息到信令服务器
         * @param jsonStr
         */

        void onSendMessage(String jsonStr);
    }
}
