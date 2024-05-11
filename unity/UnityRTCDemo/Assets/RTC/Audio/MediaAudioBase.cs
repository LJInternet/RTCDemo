using LJ.RTC.Common;
using System.Collections.Generic;
using System.IO;

namespace LJ.RTC.Audio
{
    class RTCAudioConfiguration {
        public int audioProfile = (int)AudioProfile.AUDIO_PROFILE_DEFAULT;
        public int audioScenario;

        public AudioConfig createAudioConfig() {
            AudioConfig audioConfig = new AudioConfig();
            audioConfig.bitrateInbps = 64000;
            audioConfig.framePerBuffer = 480; //
            audioConfig.sampleRate = 48000;
            audioConfig.channels = 1;
            audioConfig.audioType = (int)AudioType.OPUS;
            audioConfig.isHardEncode = true;
            audioConfig.needAdts = false;
            if (audioProfile == (int)AudioProfile.AUDIO_PROFILE_DEFAULT)
            {
                audioConfig.sampleRate = 48000;
                audioConfig.channels = 1;
            }
            else if (audioProfile == (int)AudioProfile.AUDIO_PROFILE_MUSIC_STANDARD_STEREO)
            {
                audioConfig.sampleRate = 48000;
                audioConfig.channels = 2;
                audioConfig.bitrateInbps = 80000;
            }
            else if (audioProfile == (int)AudioProfile.AUDIO_PROFILE_CALLBACK_DATA_NORENDER)
            {
                audioConfig.sampleRate = 48000;
                audioConfig.channels = 2;
                audioConfig.bitrateInbps = 80000;
                audioConfig.callbackDecodeData = true;
                audioConfig.renderAudioData = false;
                audioConfig.directDecode = true;
            }
            else if (audioProfile == (int)AudioProfile.AUDIO_PROFILE_NORENDER_NO_CALLBACK)
            {
                audioConfig.sampleRate = 48000;
                audioConfig.channels = 2;
                audioConfig.bitrateInbps = 80000;
                audioConfig.callbackDecodeData = false;
                audioConfig.renderAudioData = false;
                audioConfig.directDecode = false;
            }
            return audioConfig;
        }

    }

    public enum CaptureType {
        AudioRecorder = 3,
        AAudio = 2,
        openSLES = 1,
    }

    class AudioConfig
    {
        public int sampleRate = 48000;

        public int channels = 1;

        public int framePerBuffer = 480;

        public int bitrateInbps = 64000;

        public int audioType = (int)AudioType.AAC;
        public int micVolume = 100;
        public int sourceType = 1;

        public int aacProfile = 2;

        public bool needAdts = false;
        public bool isHardEncode = false;
        public bool callbackDecodeData = false;
        public bool renderAudioData = true;
        public bool directDecode = false;

        public int captureType = (int)CaptureType.AudioRecorder;

    }

    public class AudioEnableEvent : HPMarshaller
    {
        public int evtType = 0;
        public bool enabled = false;

        public override byte[] marshall()
        {
            pushInt(evtType);
            pushBool(enabled);

            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            evtType = popInt();
            enabled = popBool();
        }
    }

    public class AudioAdjustEvent : HPMarshaller
    {
        public int evtType;
        public int val;

        public override byte[] marshall()
        {
            pushInt(evtType);
            pushInt(val);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            evtType = popInt();
            val = popInt();
        }
    }

    public class AudioVolumeIndicationEvent : HPMarshaller
    {
        public int interval = 0;
        public int smooth = 0;
        public bool reportVad = false;

        public override byte[] marshall()
        {
            pushInt(interval);
            pushInt(smooth);
            pushBool(reportVad);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            interval = popInt();
            smooth = popInt();
            reportVad = popBool();
        }
    }

    public class AudioProfileEvent : HPMarshaller
    {
        public int profile = 0;
        public int scenario = 0;

        public override byte[] marshall()
        {
            pushInt(profile);
            pushInt(scenario);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            profile = popInt();
            scenario = popInt();
        }
    }

    public class AudioPlayerEvent : HPMarshaller
    {
       public bool callbackDecodeData = false;
       public bool renderAudioData = false;
       public bool directDecode = false;

        public override byte[] marshall()
        {
            pushBool(callbackDecodeData);
            pushBool(renderAudioData);
            pushBool(directDecode);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            callbackDecodeData = popBool();
            renderAudioData = popBool();
            directDecode = popBool();
        }
    }

    public class EnumerateAudioDevicesEvent : HPMarshaller
    {
        public int type = 0; // 0 Record devices 1 play devices

        public override byte[] marshall()
        {
            pushInt(type);
            return base.marshall();
        }
    }
    public class AudioDevice : HPMarshaller
    {
        public string name;
        public int id;

        public override byte[] marshall()
        {
            pushString16(name);
            pushInt(id);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            name = popString16();
            id = popInt();
        }
        public override void unmarshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.unmarshall(stream, writer, reader);
            name = popString16();
            id = popInt();
        }
    }

    public class AudioDevicesEvent : HPMarshaller
    {
        public List<AudioDevice> devices = new List<AudioDevice>();

        public override byte[] marshall()
        {
            pushCollection<AudioDevice>(devices, ELenType.E_NONE);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            devices = popCollection<AudioDevice>(ELenType.E_NONE);
        }
    }

    public class SetDeviceInfoEvent : HPMarshaller
    {
        public AudioDevice audioDevice = null;
        public int type;

        public override byte[] marshall()
        {
            pushInt(type);
            pushMarshallable(audioDevice);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            type = popInt();
            audioDevice = popMarshallable<AudioDevice>();
        }
    }

    public class MuteDeviceEvent : HPMarshaller
    {
        public int type;
        public bool isMute;

        public override byte[] marshall()
        {
            pushInt(type);
            pushBool(isMute);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            type = popInt();
            isMute = popBool();
        }
    }
    public class AudioMixingEvent : HPMarshaller
    {
        public string filePath;
        public bool loopback;
        public int cycle;
        public int startPos;
        public int acton; // 0 start 1 stop 2 pause 3 resume 4 set position

        public override byte[] marshall()
        {
            pushString16(filePath);
            pushBool(loopback);
            pushInt(cycle);
            pushInt(startPos);
            pushInt(acton);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            filePath = popString16();
            loopback = popBool();
            cycle = popInt();
            startPos = popInt();
            acton = popInt();
        }
    }

    public class AudioFrame
    {
        public int sampleRate;
        public int channelCount;
        public int pts;
        public byte[] buffer;
    }

}
