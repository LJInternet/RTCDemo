
using LJ.RTC.Common;

namespace LJ.RTC
{
    ///
    /// <summary>
    /// Video device management methods.
    /// </summary>
    ///
    public class VideoDeviceManagerFactory {

        private static object _lock = new object();


#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
        private static bool useNative = true;
#else
        private static bool useNative = false;
#endif

        private static IVideoDeviceManager sInstantce;

        public static IVideoDeviceManager GetDeviceManager()
        {
            CreateDeviceManager(useNative);
            return sInstantce;
        }

        public static void CreateDeviceManager(bool isUseNative) {
        if (sInstantce == null)
            {
                lock (_lock)
                {
                    if (sInstantce == null)
                    {   
                        sInstantce = isUseNative ? new NativeDeviceManager() : new VideoDeviceManager();
                    }
                }
            }
        }

        public static void ResetDeviceManager(bool enableNative)
        {
            if (useNative == enableNative) {
                return;
            }
            if (sInstantce == null) {
                useNative = enableNative;
                return;
            }
            string deviceName = "";
            GetDeviceManager();
            sInstantce.GetDevice(ref deviceName);
            useNative = enableNative;
            sInstantce = null;
            sInstantce = GetDeviceManager();
            sInstantce.Init();
        }
    }


    public abstract class IVideoDeviceManager
    {
        ///
        /// <summary>
        /// Enumerates the video devices.
        /// </summary>
        ///
        /// <returns>
        /// Success: A DeviceInfo array including all video devices in the system.Failure: NULL.
        /// </returns>
        ///
        public abstract void Init();

        public abstract DeviceInfo[] EnumerateVideoDevices();

        ///
        /// <summary>
        /// Specifies the video capture device with the device ID.
        /// Plugging or unplugging a device does not change its device ID.
        /// </summary>
        ///
        /// <param name="deviceIdUTF8"> The device ID. You can get the device ID by calling EnumerateVideoDevices .</param>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int SetDevice(string deviceIdUTF8);

        ///
        /// <summary>
        /// Retrieves the current video capture device.
        /// </summary>
        ///
        /// <param name="deviceIdUTF8"> Output parameter. The device ID. </param>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int GetDevice(ref string deviceIdUTF8);

        public abstract string[] GetCameraDeviceNames();
    }
}
