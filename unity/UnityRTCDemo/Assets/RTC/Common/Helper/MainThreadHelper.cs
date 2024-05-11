using System;

namespace LJ.RTC.Common
{
    public class MainThreadHelper
    {
        public static void QueueOnMainThread(Action<object> taction, object tparam)
        {
            QueueOnMainThread(taction, tparam, 0f);
        }
        public static void QueueOnMainThread(Action<object> taction, object tparam, float time)
        {
            RtcEngineGameObject.QueueOnMainThread(taction, tparam, time);
        }
    }
}
