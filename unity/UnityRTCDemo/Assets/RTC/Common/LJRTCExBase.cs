using System.Runtime.InteropServices;

namespace LJ.RTC.Common
{

    public enum RTCDataWorkMode
    {
        SEND_AND_RECV = 0,
        SEND_ONLY = 1,
        RECV_ONLY = 2,
        LOCK_STEP_SEND_RECV = 3,
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioFrameEx
    {
        public int sampleRate;
        public int channelCount;
        public int bytePerSample;
        public int uid;
        public int frame_num;
        public string channelId;
        public byte[] pcm;
    };

    ///
    /// <summary>
    /// Contains connection information.
    /// </summary>
    ///
    public class LJRtcConnection
    {

        public LJRtcConnection(string channelId, long localUid)
        {
            this.channelId = channelId;
            this.localUid = localUid;
            key = channelId + localUid;
        }

        public string GetKey() {
            return key;
        }

        ///
        /// <summary>
        /// The channel name.
        /// </summary>
        ///
        public string channelId;

        ///
        /// <summary>
        /// The ID of the local user.
        /// </summary>
        ///
        public long localUid;

        public string key;
    };

    ///
    /// <summary>
    /// The options for leaving a channel.
    /// </summary>
    ///
    public class LJLeaveChannelOptions
    {
        public LJLeaveChannelOptions()
        {
            stopAudioMixing = true;
            stopAllEffect = true;
            stopMicrophoneRecording = true;
        }

        ///
        /// <summary>
        /// Whether to stop playing and mixing the music file when a user leaves the channel. true: (Default) Stop playing and mixing the music file.false: Do not stop playing and mixing the music file.
        /// </summary>
        ///
        public bool stopAudioMixing;

        ///
        /// <summary>
        /// Whether to stop playing all audio effects when a user leaves the channel. true: (Default) Stop playing all audio effects.false: Do not stop playing any audio effect.
        /// </summary>
        ///
        public bool stopAllEffect;

        ///
        /// <summary>
        /// Whether to stop microphone recording when a user leaves the channel. true: (Default) Stop microphone recording.false: Do not stop microphone recording.
        /// </summary>
        ///
        public bool stopMicrophoneRecording;
    };
}
