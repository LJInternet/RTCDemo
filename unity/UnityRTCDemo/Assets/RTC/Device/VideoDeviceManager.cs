
using LJ.RTC.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LJ.RTC
{
    internal class VideoDeviceManager : IVideoDeviceManager
    {
        private List<WebCamDevice> mDeviceList = new List<WebCamDevice>();

        private WebCamDevice mCurrentDevice;
        public override void Init() {
            //MainThreadHelper.QueueOnMainThread((object obj) => {
                mDeviceList.Clear();
            string deviceNames = "";
                foreach (WebCamDevice device in WebCamTexture.devices)
                {
                    mDeviceList.Add(device);
                    deviceNames += device.name;
                    deviceNames += ";";
                }
            JLog.Debug("deviceNames:" + deviceNames);
                mCurrentDevice = WebCamTexture.devices.First();
           // }, null);
        }

        public override int GetDevice(ref string deviceIdUTF8) {
            if (string.Equals(mCurrentDevice.name, "")) {
                deviceIdUTF8 = "";
                return -1;
            }
            deviceIdUTF8 = mCurrentDevice.name;
            return 0;
        }

        public override int SetDevice(string deviceIdUTF8) {
            WebCamDevice webCamDevice = mDeviceList.First();
            foreach (WebCamDevice device in mDeviceList)
            {
                if (string.Equals(device.name, deviceIdUTF8))
                {
                    webCamDevice = device;
                    break;
                }
            }

            if (webCamDevice.name != deviceIdUTF8)
            {
                return -1;
            }
            mCurrentDevice = webCamDevice;
            return 0;
        }

        public override DeviceInfo[] EnumerateVideoDevices()
        {
            if (mDeviceList.Count == 0)
            {
                Init();
            }
            List<DeviceInfo> infos = new List<DeviceInfo>();
            foreach (WebCamDevice device in mDeviceList)
            {
                DeviceInfo info = new DeviceInfo();
                info.deviceId = device.name;
                info.deviceName = device.name;
                infos.Add(info);
            }
            return infos.ToArray();
        }

        public override string[] GetCameraDeviceNames() {
            if (mDeviceList.Count == 0) { 
                Init();
            }
            List<string> list = new List<string>();
            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                list.Add(device.name);
            }
            return list.ToArray();
        }
    }
}
