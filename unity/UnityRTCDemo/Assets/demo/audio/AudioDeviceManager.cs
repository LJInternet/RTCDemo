
using UnityEngine;
using LJ.RTC.Common;
using LJ.RTC;
using LJ.RTC.Video;
using LJ.RTC.Utils;
using LJ.demo;

public class AudioDeviceManager : MonoBehaviour
{

    internal IRtcEngine mRtcEngine;
    IAudioDeviceManager audioDevice;

    // Start is called before the first frame update
    void Start()
    {
        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        InitHelper.StartRtcEngine(false);
        audioDevice = mRtcEngine.GetAudioDeviceManager();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(60, 100, 100, 40), "Enumerate Devices"))
        {
            DeviceInfo[] playDeviceInfo = audioDevice.EnumeratePlaybackDevices();
            DeviceInfo[] recordDeviceInfo = audioDevice.EnumerateRecordingDevices();
            string text = "";
            foreach (DeviceInfo info in playDeviceInfo) {
                text += "playDevice: id:" + info.deviceId + " name:" + info.deviceName +";";
            }
            foreach (DeviceInfo info in recordDeviceInfo)
            {
                text += "recordDevice: id:" + info.deviceId + " name:" + info.deviceName + "";
            }
            Debug.Log("Enumerate Devices " + text);
            string lookBackDevice = "";
            audioDevice.GetLoopbackDevice(ref lookBackDevice);
            string playkDevice = "";
            audioDevice.GetPlaybackDevice(ref playkDevice);
            string recordBackDevice = "";
            audioDevice.GetRecordingDevice(ref recordBackDevice);

            string defaultPlaykDevice = "";
            audioDevice.GetPlaybackDevice(ref defaultPlaykDevice);
            string defaultRecordBackDevice = "";
            audioDevice.GetRecordingDevice(ref defaultRecordBackDevice);
            Debug.Log("devices: lookBackDevice :" + lookBackDevice + " playkDevice:" + playkDevice + " recordBackDevice:" + recordBackDevice
                + " defaultPlaykDevice:" + defaultPlaykDevice + " defaultRecordBackDevice:" + defaultRecordBackDevice);
        }
    }

    private void StartRtcEngine()
    {
        InitHelper.StartRtcEngine(false);
    }
}
