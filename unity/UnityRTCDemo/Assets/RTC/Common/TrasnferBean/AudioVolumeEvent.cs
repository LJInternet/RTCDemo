namespace LJ.RTC.Common
{
    public class AudioVolumeEvent : HPMarshaller
    {
        public int uid;
        public int volume;
        public string channelId;

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            uid = popInt();
            volume = popInt();
            channelId = popString16();
        }
    }
}
