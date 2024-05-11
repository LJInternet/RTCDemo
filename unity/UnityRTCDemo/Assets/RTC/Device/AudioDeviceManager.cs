

using LJ.RTC.Audio;
using LJ.RTC.Common;
using System;

namespace LJ.RTC
{
    public class AudioDeviceManager : IAudioDeviceManager
    {
        private static object _lock = new object();

        private static AudioDeviceManager sInstantce;
        private IRtcEngineApi mIRtcEngineApi;

        public AudioDeviceManager(IRtcEngineApi engineApi)
        {
            this.mIRtcEngineApi = engineApi;
        }

        public static AudioDeviceManager Instance(IRtcEngineApi engineApi)
        {
            if (sInstantce == null)
            {
                lock (_lock)
                {
                    if (sInstantce == null)
                    {
                        sInstantce = new AudioDeviceManager(engineApi);
                    }
                }
            }
            return sInstantce;
        }

        public override DeviceInfo[] EnumeratePlaybackDevices()
        {
            return GetDevices((int)DEVICE_TYPE.PLAY_RENDER);
        }

        private DeviceInfo[] GetDevices(int type) {

            if (mIRtcEngineApi == null)
            {
                return new DeviceInfo[0];
            }
            EnumerateAudioDevicesEvent deviceEvent = new EnumerateAudioDevicesEvent();
            deviceEvent.type = type;
            byte[] msg = mIRtcEngineApi.GetMediaEvent((int)MediaInvokeEventType.AUDIO_ENUMERATE_DEVICES_EVENT, deviceEvent.HPmarshall());
            if (msg == null)
            {
                return new DeviceInfo[0];
            }
            AudioDevicesEvent devicesEvent = new AudioDevicesEvent();
            devicesEvent.unmarshall(msg);
            if (devicesEvent.devices == null)
            {
                return new DeviceInfo[0];
            }
            DeviceInfo[] infos = new DeviceInfo[devicesEvent.devices.Count];
            for (int i = 0; i < devicesEvent.devices.Count; i++)
            {
                DeviceInfo info = new DeviceInfo();
                info.deviceId = devicesEvent.devices[i].id + "";
                info.deviceName = devicesEvent.devices[i].name;
                infos[i] = info;
            }
            return infos;
        }

        public override DeviceInfo[] EnumerateRecordingDevices()
        {
            return GetDevices((int)DEVICE_TYPE.RECORD);
        }

        public override int FollowSystemLoopbackDevice(bool enable)
        {
            throw new System.NotImplementedException();
        }

        public override int FollowSystemPlaybackDevice(bool enable)
        {
            throw new System.NotImplementedException();
        }

        public override int FollowSystemRecordingDevice(bool enable)
        {
            throw new System.NotImplementedException();
        }

        public override int GetLoopbackDevice(ref string deviceId)
        {
            AudioDevice device = GetDevice((int)MediaInvokeEventType.AUDIO_GET_SUBMIX_DEVICE_EVENT);
            if (device == null) {
                deviceId = null;
                return -1;
            }
            deviceId = device.id + "";
            return 0;
        }

        private AudioDevice GetDevice(int type) {
            byte[] msg = mIRtcEngineApi.GetMediaEvent(type, new byte[0]);
            if (msg == null)
            {
                return null;
            }
            AudioDevice device = new AudioDevice();
            device.unmarshall(msg);
            if (device.id == null)
            {
                return null;
            }
            return device;
        }

        private int SetDeviceInfo(int type, DEVICE_TYPE deviceType, string deviceId, string deviceName)
        {
            AudioDevice device = new AudioDevice();
            device.id = Int32.Parse(deviceId);
            device.name = deviceName;
            SetDeviceInfoEvent setDeviceInfoEvent = new SetDeviceInfoEvent();
            setDeviceInfoEvent.type = (int)deviceType;
            setDeviceInfoEvent.audioDevice = device;
            return mIRtcEngineApi.SendMediaEvent(type, setDeviceInfoEvent.HPmarshall());
        }

        public override int GetPlaybackDefaultDevice(ref string deviceId, ref string deviceName)
        {
            AudioDevice device = GetDevice((int)MediaInvokeEventType.AUDIO_GET_DEFAULT_OUT_DEVICE_EVENT);
            if (device == null)
            {
                deviceId = null;
                return -1;
            }
            deviceId = device.id + ""; ;
            deviceName = device.name;
            return 0;
        }

        public override int GetPlaybackDevice(ref string deviceId)
        {
            AudioDevice device = GetDevice((int)MediaInvokeEventType.AUDIO_GET_OUT_DEVICE_EVENT);
            if (device == null)
            {
                deviceId = null;
                return -1;
            }
            deviceId = device.id + ""; ;
            return 0;
        }

        public override int GetPlaybackDeviceInfo(ref string deviceId, ref string deviceName)
        {
            AudioDevice device = GetDevice((int)MediaInvokeEventType.AUDIO_GET_OUT_DEVICE_EVENT);
            if (device == null)
            {
                deviceId = null;
                deviceName = null;
                return -1;
            }
            deviceId = device.id + ""; ;
            deviceName = device.name;
            return 0;
        }

        public override int GetPlaybackDeviceMute(ref bool mute)
        {
            return GetDeviceMuteState(DEVICE_TYPE.PLAY_RENDER, ref mute);
        }

        public override int GetPlaybackDeviceVolume(ref int volume)
        {
            return getDeviceVilume(DEVICE_TYPE.PLAY_RENDER, ref volume);
        }

        public override int GetRecordingDefaultDevice(ref string deviceId, ref string deviceName)
        {
            AudioDevice device = GetDevice((int)MediaInvokeEventType.AUDIO_GET_DEFAULT_INPUT_DEVICE_EVENT);
            if (device == null)
            {
                deviceId = null;
                deviceName = null;
                return -1;
            }
            deviceId = device.id + "";
            deviceName = device.name;
            return 0;
        }

        public override int GetRecordingDevice(ref string deviceId)
        {
            AudioDevice device = GetDevice((int)MediaInvokeEventType.AUDIO_GET_INPUT_DEVICE_EVENT);
            if (device == null)
            {
                deviceId = null;
                return -1;
            }
            deviceId = device.id + ""; ;
            return 0;
        }

        public override int GetRecordingDeviceInfo(ref string deviceId, ref string deviceName)
        {
            return GetRecordingDefaultDevice(ref deviceId, ref deviceName);
        }

        public override int GetRecordingDeviceMute(ref bool mute)
        {
            return GetDeviceMuteState(DEVICE_TYPE.RECORD, ref mute);
        }

        public override int GetRecordingDeviceVolume(ref int volume)
        {
            return getDeviceVilume(DEVICE_TYPE.RECORD, ref volume);
        }

        public override int SetLoopbackDevice(string deviceId)
        {
            return SetDeviceInfo((int)MediaInvokeEventType.AUDIO_SET_DEVICE_EVENT, DEVICE_TYPE.SUBMIX_LOOPBACK, deviceId, null);
        }

        public override int SetPlaybackDevice(string deviceId)
        {
            return SetDeviceInfo((int)MediaInvokeEventType.AUDIO_SET_DEVICE_EVENT, DEVICE_TYPE.PLAY_RENDER, deviceId, null);
        }

        private int MuteDevice(DEVICE_TYPE deviceType, bool isMute)
        {
            MuteDeviceEvent muteEvent = new MuteDeviceEvent();
            muteEvent.type = (int)deviceType;
            muteEvent.isMute = isMute;
            return mIRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_MUTE_DEVICE_EVENT, muteEvent.HPmarshall());
        }

        private int GetDeviceMuteState(DEVICE_TYPE deviceType, ref bool isMute)
        {
            MuteDeviceEvent muteDevice = new MuteDeviceEvent();
            muteDevice.type = (int)deviceType;
            byte[] msg = mIRtcEngineApi.GetMediaEvent((int)MediaInvokeEventType.AUDIO_GET_DEVICE_MUTE_STATE_EVENT, muteDevice.HPmarshall()) ;
            if (msg == null)
            {
                return 0;
            }
            MuteDeviceEvent muteDeviceResult = new MuteDeviceEvent();

            muteDeviceResult.unmarshall(msg);
            isMute = muteDeviceResult.isMute;
            return 0;
        }

        private int SetDeviceVolume(DEVICE_TYPE type, int volume) {
            AudioAdjustEvent adjustEvent = new AudioAdjustEvent();
            adjustEvent.val = volume;
            adjustEvent.evtType = (int)type;
            return mIRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_DEVICE_SET_VOLUME, adjustEvent.HPmarshall());
        }

        private int getDeviceVilume(DEVICE_TYPE type, ref int volume) {

            AudioAdjustEvent adjustEvent = new AudioAdjustEvent();
            adjustEvent.evtType = (int)type;
            byte[] msg = mIRtcEngineApi.GetMediaEvent((int)MediaInvokeEventType.AUDIO_DEVICE_GET_VOLUME, adjustEvent.HPmarshall());
            if (msg == null)
            {
                volume = 0;
                return -1;
            }
            AudioAdjustEvent result = new AudioAdjustEvent();
            result.unmarshall(msg);
            volume = result.val;
            return 0;

        }
        public override int SetPlaybackDeviceMute(bool mute)
        {
            return MuteDevice(DEVICE_TYPE.PLAY_RENDER, mute);
        }

        public override int SetPlaybackDeviceVolume(int volume)
        {
            return SetDeviceVolume(DEVICE_TYPE.PLAY_RENDER, volume);
        }

        public override int SetRecordingDevice(string deviceId)
        {
            return SetDeviceInfo((int)MediaInvokeEventType.AUDIO_SET_DEVICE_EVENT, DEVICE_TYPE.RECORD, deviceId, null);
        }

        public override int SetRecordingDeviceMute(bool mute)
        {
            return MuteDevice(DEVICE_TYPE.RECORD, mute);
        }

        public override int SetRecordingDeviceVolume(int volume)
        {
            return SetDeviceVolume(DEVICE_TYPE.RECORD, volume);
        }

        public override int StartAudioDeviceLoopbackTest(int indicationInterval)
        {
            throw new System.NotImplementedException();
        }

        public override int StartPlaybackDeviceTest(string testAudioFilePath)
        {
            throw new System.NotImplementedException();
        }

        public override int StartRecordingDeviceTest(int indicationInterval)
        {
            throw new System.NotImplementedException();
        }

        public override int StopAudioDeviceLoopbackTest()
        {
            throw new System.NotImplementedException();
        }

        public override int StopPlaybackDeviceTest()
        {
            throw new System.NotImplementedException();
        }

        public override int StopRecordingDeviceTest()
        {
            throw new System.NotImplementedException();
        }

        internal override void Clear()
        {
            mIRtcEngineApi = null;
        }
    }
}
