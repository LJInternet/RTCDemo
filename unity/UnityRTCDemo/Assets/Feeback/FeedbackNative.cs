using AOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LJ.Feedback
{
    public class FeedbackNative
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string FeedbackLibName = "feedback";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private const string FeedbackLibName = "feedback";
#elif UNITY_IPHONE
		private const string FeedbackLibName = "__Internal";
#else
        private const string FeedbackLibName = "feedback";
#endif


        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void feedback_callback(int result, IntPtr buf, int size);

        [DllImport(FeedbackLibName, CharSet = CharSet.Ansi, EntryPoint = "feedback_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(string token, string host, int port, bool isDebug);

        [DllImport(FeedbackLibName, CharSet = CharSet.Ansi, EntryPoint = "feedback_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy();

        [DllImport(FeedbackLibName, CharSet = CharSet.Ansi, EntryPoint = "set_common_attrs", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCommonAttrs(string jsonStr, int len);

        [DllImport(FeedbackLibName, CharSet = CharSet.Ansi, EntryPoint = "send_feedback", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendFeedBack(string title, string context, string filePath, string liveId, int pathLen);
        [DllImport(FeedbackLibName, CharSet = CharSet.Ansi, EntryPoint = "subscribe_feedback_result", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SubscribeFeedbackResult([MarshalAs(UnmanagedType.FunctionPtr)] feedback_callback cb);

        private static OnFeedbackResult _callback;

        public static void SubscribeResult(OnFeedbackResult onFeedback)
        {
            _callback = onFeedback;
            SubscribeFeedbackResult(FeedbackCallbackFunc);
        }

        [MonoPInvokeCallback(typeof(feedback_callback))]
        public static void FeedbackCallbackFunc(int result, IntPtr buf, int size)
        {
            if (_callback != null) {
                if (buf == IntPtr.Zero)
                {
                    _callback(result, "");
                    return;
                }
                string str = Marshal.PtrToStringAnsi(buf, size);
                _callback(result, str);
            }
        }

        public static void DoDestroy() {
            Destroy();
            _callback = null;

        }
    }
}
