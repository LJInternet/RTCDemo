
using LJ.RTC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LJ.RTC
{
    internal class NativeDeviceManager : IVideoDeviceManager
    {
        private List<string> mDeviceList = new List<string>();

        private string mCurrentDevice;

        public override void Init() {
            
            mDeviceList.Clear();
            string deviceNames = "";
            string[] devs = RtcEngineNavite.GetCameraList();
            foreach (string device in devs)
            {
                mDeviceList.Add(device);
                deviceNames += device;
                deviceNames += ";";
            }
            JLog.Debug("deviceNames:" + deviceNames);
            mCurrentDevice = mDeviceList.First();
        }

        public override int GetDevice(ref string deviceIdUTF8) {
            if (string.Equals(mCurrentDevice, "")) {
                deviceIdUTF8 = "";
                return -1;
            }
            deviceIdUTF8 = mCurrentDevice;
            return 0;
        }

        public override int SetDevice(string deviceIdUTF8) {
            
            foreach (string device in mDeviceList)
            {
                if (string.Equals(device, deviceIdUTF8))
                {
                    mCurrentDevice = device;
                    return 0;
                }
            }

            return -1;
        }

        public override DeviceInfo[] EnumerateVideoDevices()
        {
            List<DeviceInfo> infos = new List<DeviceInfo>();
            int i = 0;
            foreach (string device in mDeviceList)
            {
                DeviceInfo info = new DeviceInfo();
                info.deviceId = Convert.ToString(i++);
                info.deviceName = device;
                infos.Add(info);
            }
            return infos.ToArray();
        }

        public override string[] GetCameraDeviceNames()
        {
            return RtcEngineNavite.GetCameraList();
        }
    }
}
