using LJ.Log;

namespace LJ.RTC.Common
{
    public class RtcLogImpl : ILog
    {
        public void Debug(string msg)
        {
            FLog.Debug(msg);
        }

        public void Error(string msg)
        {
            FLog.Debug(msg);
        }

        public void Info(string msg)
        {
            FLog.Debug(msg);
        }
    }
}
