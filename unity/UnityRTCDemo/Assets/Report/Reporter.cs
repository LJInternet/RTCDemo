using AOT;
using LJ.Log;
using LJ.RTC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static LJ.Report.ReportCenter;

namespace LJ.Report
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public class ReportCenterConfig
    {
        public int collectDuration; // 上报时间间隔
        public do_upload_func cb; // 使用业务长链接初始化时，必须实现该方法做上报，该方法马上被废弃了
        public upload_event_func eventCb; // 使用sdk内部长链接，一些事件回调
        public bool isTestEvn; // 是否测试环境， true 测试环境 false 正式环境
        public int appId; // 分配的appid
    }

    /// <summary>
    /// 使用sdk内部长链接，一些事件定义
    /// </summary>
    public enum ReportResult
    {
        SUCCESS = 0,
        TOKEN_INVALID = -1, // token 无效需要重新请求token或者检查token释放正确
        APPID_INVALID = -2, // appId 无效，需要设置一个有效的appId
        TOKEN_TIMEOUT = -3, // Token 过期，需要重新请求token
    };
    public class ReportCenter
    {

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string baseLibName = "apm";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private const string baseLibName = "apm";
#elif UNITY_IPHONE
		private const string baseLibName = "__Internal";
#else
        private const string baseLibName = "apm";
#endif

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void do_upload_func_inner(IntPtr buf, int len);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void upload_evn_func_inner(int result, IntPtr buf, int len);

        /// <summary>
        /// 使用业务长链接初始化时，必须实现该方法做上报，该方法马上被废弃了
        /// </summary>
        /// <param name="msg"></param>
        public delegate void do_upload_func(string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result">@see ReportResult</param>
        /// <param name="msg"></param>
        public delegate void upload_event_func(int result, string msg);

        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_init", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void InitNative(int cd, [MarshalAs(UnmanagedType.FunctionPtr)] do_upload_func_inner cb);

        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_set_common_attr", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetCommonAttrsNative(string msg, int len);

        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_register_slot", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr RegisterSlotNative(string key, int len, int level);

        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_register_event_slot", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr RegisterEventSlotNative(string key, int len, int level);

        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_do_report", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReportNative(IntPtr slot, string msg, int len);
        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_enable_performance", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void NativeEnablePerformance(bool enable);

        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_release", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseNative();

        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_init_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void InitExNative(bool isTestEvn, int cd, int appId, upload_evn_func_inner callback);

        [DllImport(baseLibName, CharSet = CharSet.Ansi, EntryPoint = "reporter_set_userinfo", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetUserInfoNative(string token, long uid);

        private static do_upload_func f;

        private static upload_event_func eventCb;

        private static System.Collections.Concurrent.ConcurrentDictionary<string, IntPtr> dict = new System.Collections.Concurrent.ConcurrentDictionary<string, IntPtr>();

        private static Performance mPerformance;

        /// <summary>
        /// 使用业务的长链接，初始化统计sdk
        /// </summary>
        /// <param name="config"></param>
        [Obsolete("Method1 will deprecated, please use InitEx instead.", false)]
        public static void Init(ReportCenterConfig config)
        {
            f = config.cb;
            InitNative(config.collectDuration, UploadFunc);
        }

        /// <summary>
        /// 使用Sdk内部长链接初始化SDK，初始化后必须调用SetUserInfo，设置token以及uid
        /// </summary>
        /// <param name="config"></param>
        public static void InitEx(ReportCenterConfig config)
        {
            f = config.cb;
            eventCb = config.eventCb;
            InitExNative(config.isTestEvn,config.collectDuration, config.appId, UploadEventFunc);
        }

        /// <summary>
        /// 使用SDK内部长链接必须调用该方法设置token以及uid
        /// </summary>
        /// <param name="token">服务器分配的token</param>
        /// <param name="uid">用户uid</param>
        public static void SetUserInfo(string token, long uid) {
            SetUserInfoNative(token, uid);
        }

        public static void Release() {
            ReleaseNative();
            f = null;
            eventCb = null;
        }

        public static void SetCommonAttrs(Dictionary<string,System.Object> attrs)
        {
            string msg = JsonConvert.SerializeObject(attrs);
            FLog.Info("SetCommonAttrs:" + msg);
            SetCommonAttrsNative(msg, msg.Length);
        }

        public static void Report(string key, Dictionary<string, System.Object> info) {

            IntPtr solt = dict.GetOrAdd(key, k => RegisterSlotNative(k, k.Length, 0));

            string msg = JsonConvert.SerializeObject(info);
            ReportNative(solt, msg, msg.Length);
        }

        public static void Report(string key, string jsonStr)
        {
            if (jsonStr == null) {
                return;
            }
            IntPtr solt = dict.GetOrAdd(key, k => RegisterSlotNative(k, k.Length, 0));

            ReportNative(solt, jsonStr, jsonStr.Length);
        }

        public static void ReportEvent(string key, Dictionary<string, System.Object> info)
        {
            IntPtr solt = dict.GetOrAdd(key, k => RegisterEventSlotNative(k, k.Length, 0));

            string msg = JsonConvert.SerializeObject(info);
            ReportNative(solt, msg, msg.Length);
        }

        public static void ReportEvent(string key, string jsonStr)
        {
            if (jsonStr == null)
            {
                return;
            }
            IntPtr solt = dict.GetOrAdd(key, k => RegisterEventSlotNative(k, k.Length, 0));
            ReportNative(solt, jsonStr, jsonStr.Length);
        }

        [MonoPInvokeCallback(typeof(do_upload_func_inner))]
        public static void UploadFunc(IntPtr buf, int len)
        {
            if (f == null) {
                return;
            }

            byte[] data = new byte[len];
            Marshal.Copy(buf, data, 0, len);

            string msg = System.Text.Encoding.UTF8.GetString(data);

            //String format = "{\"seq\":\"%d\",\"cmd\":\"report\",\"data\":%s}";
            //String json = String.format(format, System.currentTimeMillis(), info);
            f(msg);
        }

        [MonoPInvokeCallback(typeof(upload_evn_func_inner))]
        public static void UploadEventFunc(int result, IntPtr buf, int len)
        {
            if (eventCb == null)
            {
                return;
            }

            byte[] data = new byte[len];
            Marshal.Copy(buf, data, 0, len);

            string msg = System.Text.Encoding.UTF8.GetString(data);

            //String format = "{\"seq\":\"%d\",\"cmd\":\"report\",\"data\":%s}";
            //String json = String.format(format, System.currentTimeMillis(), info);
            eventCb(result, msg);
        }

        public static int EnablePerformance(bool enable)
        {
            NativeEnablePerformance(enable);
            if (enable)
            {
                if (mPerformance == null)
                {
                    mPerformance = Performance.CreatePerformance();
                    if (mPerformance != null) {
                        mPerformance.Start();
                    }

                }
            }
            else
            {
                if (mPerformance != null)
                {
                    mPerformance.Stop();
                }
            }
            return 0;
        }
    }
}