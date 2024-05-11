using System;
using System.Collections.Generic;


namespace LJ.RTM
{
    public class RTMEngineEx
    {
        private Dictionary<string, RTMChannel> channelDic = new Dictionary<string, RTMChannel>();

        private string _token = "";
        private UInt64 _appid = 0;
        private bool _isDebug = true;
        public RTMEngineEx(string token, UInt64 appid, bool isDebug) {
            if (appid <= 0) {
                throw new System.Exception("appId <= 0, please check the appid");
            }
            if (!isDebug && token.Length == 0) {
                throw new System.Exception("release mode please add login token!!");
            }
            _token = token;
            _appid = appid;
            _isDebug = isDebug;
        }

        public RTMChannel CreateRTMChannel(DataWorkMode mode, UInt64 uid, string channelId, IRTMEngineEventHandler handler) {
            string key = uid + channelId;
            if (channelDic.ContainsKey(key)) {
                return channelDic[key];
            }
            RTMChannel channel = new RTMChannel(RUDPRole.RUDP_CONTROLLER, mode, uid, _token, _appid, _isDebug, channelId, handler);
            channelDic.Add(key, channel);
            return channel;
        }

        public RTMChannel GetRTMChannel(UInt64 uid, string channelId) {
            string key = uid + channelId;
            if (channelDic.ContainsKey(key))
            {
                return channelDic[key];
            }
            return null;
        }

        public void Release() {
            foreach (KeyValuePair<string, RTMChannel> item in channelDic)
            {
                RTMChannel channel = item.Value;
                if (channel != null)
                {
                    channel.Dispose();
                }
            }
            channelDic.Clear();
            channelDic = null;
        }
    }
}
