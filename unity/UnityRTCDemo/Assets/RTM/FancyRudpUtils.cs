using AOT;
using System.Runtime.InteropServices;

namespace LJ.RTM
{

    public enum RudpStatus {
        STATUS_CONNECTED = 1, STATUS_DISCONNECTED = 2, STATUS_LOST = 3
    }

    public class FancyRudpUtils
    {

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string RudpLibName = "rudp";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private const string RudpLibName = "rudp";
#elif UNITY_IOS
		private const string RudpLibName = "__Internal";
#else
        private const string RudpLibName = "rudp";
#endif

        [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void status_cb_inner(int status);

        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "FancyJingMsgRegisterConnStatusCallback", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RegisterStatusCallbackNative([MarshalAs(UnmanagedType.FunctionPtr)] status_cb_inner connDelegate);

        public delegate void RudpStatusDelegate(RudpStatus status);

        private static RudpStatusDelegate statusDelegate;

        [MonoPInvokeCallback(typeof(status_cb_inner))]
        private static void fireStatusChange(int status) {
            if (statusDelegate != null) {
                statusDelegate((RudpStatus)status);
            }
        }

        public static int RegisterStatusCallback(RudpStatusDelegate cb) {
            statusDelegate = cb;
            return RegisterStatusCallbackNative(fireStatusChange);
        }
    }
}
