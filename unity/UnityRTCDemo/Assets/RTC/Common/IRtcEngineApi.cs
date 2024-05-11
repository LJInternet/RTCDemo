using LJ.RTC.Video;
namespace LJ.RTC.Common
{
    public interface IRtcEngineApi
    {
        void OnCaptureVideoFrame(CaptureVideoFrame videoFrame);

        void OnCameraActionResult(int action, int result, string msg);
        void OnComponentDestroy(BaseRtcEngineModule baseRtcEngineModule);

        bool GetRUDPConnectState();

        int PushVideoCaptureFrame(CaptureVideoFrame videoFrame);

        int SendMediaEvent(int eventType, byte[] msg, UnityEngine.Object extraObject = null);

        byte[] GetMediaEvent(int eventType, byte[] msg);

        bool IsUseNativeCamera();

        void LocalRegisterDecodeVideo(OnDecodedVideoFrame videoFrame);

        void RegisterDecodeVideoEx(OnDecodeVideoInternel onDecodeVideo);

        void RegisterEventExListener(OnEventExCallbackInternel callbackEvent);
    }
}
