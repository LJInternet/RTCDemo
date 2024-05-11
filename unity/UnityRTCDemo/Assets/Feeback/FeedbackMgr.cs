using LJ.Log;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LJ.Feedback
{
    public class FeedbackConfig {
        public string _token = "";
        public string _host = "";
        public int _port = -1;
        public bool _isDebug = false;

        public FeedbackConfig SetToken(string token) {
            _token = token;
            return this;
        }

        public FeedbackConfig SetHost(string host) {
            _host = host;
            return this;
        }

        public FeedbackConfig SetPort(int port) {
            _port = port;
            return this;
        }

        public FeedbackConfig SetDebug(bool isDebug) {
            _isDebug = isDebug;
            return this;
        }
    }

    public delegate void OnFeedbackResult(int result, string msg);

    public class FeedbackMgr
    {
        private static Dictionary<string, System.Object> commonAttrs = new Dictionary<string, System.Object>();

        public static void Init(FeedbackConfig config) {
            FLog.Info("FeedbackNative.Init");
            FeedbackNative.Init(config._token, config._host, config._port, config._isDebug);
        }

        public static void Destroy() {
            FLog.Info("feedback Destroy");
            FeedbackNative.Destroy();
        }

        public static void SetCommonAttrs(Dictionary<string, System.Object> attrs) {      
            if (attrs == null)
            {
                return;
            }
            commonAttrs = attrs;
            string msg = JsonConvert.SerializeObject(attrs);
            FLog.Info("SetCommonAttrs:" + msg);
            FeedbackNative.SetCommonAttrs(msg, msg.Length);          
        }

        public static void AddCommonAttrs(Dictionary<string, System.Object> attrs)
        {       
            if (attrs == null) {
                return;
            }
            if (commonAttrs == null) {
                commonAttrs = new Dictionary<string, System.Object>();
            }
            foreach (KeyValuePair<string, System.Object> kv in attrs)
            {
                commonAttrs.Add(kv.Key, kv.Value);
            }
            string msg = JsonConvert.SerializeObject(attrs);
            FLog.Info("SetCommonAttrs:" + msg);
            FeedbackNative.SetCommonAttrs(msg, msg.Length);         
        }

        public static void SendFeedBack(string title, string context, string filePath, string liveId) {
            //filePath.Replace("/", "\\");
            //FLog.Info("SendFeedBack: filePath " + filePath);
            FeedbackNative.SendFeedBack(title, context, filePath, liveId, filePath.Length);           
        }

        public static void SubscribeFeedbackResult(OnFeedbackResult onFeedback) {        
            FLog.Info("feedback SubscribeFeedbackResult");
            FeedbackNative.SubscribeResult(onFeedback);           
        }
    }
}
