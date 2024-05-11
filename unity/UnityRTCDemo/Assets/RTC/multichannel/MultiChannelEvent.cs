using LJ.RTC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC
{
    public class MultiChannelEvent : HPMarshaller
    {
        public string channelId;
        public long uid;

        public MultiChannelEvent(LJRtcConnection connection)
        {
            channelId = connection.channelId;
            uid = connection.localUid;
        }

        public override byte[] marshall()
        {
            pushString16(channelId);
            pushInt64(uid);
            return base.marshall();
        }
    }

    public class MultiChannelEnableEvent : MultiChannelEvent
    {
        public bool _enable;

        public MultiChannelEnableEvent(bool enable, LJRtcConnection connection) : base(connection)
        {
            _enable = enable;
        }

        public override byte[] marshall()
        {
            pushBool(_enable);
            return base.marshall();
        }
    }

    public class JoinChannelExConfig : MultiChannelEvent
    {
        string _token;
        ChannelMediaOptions _option;
        long _appId;
        bool _isDebug;

        public JoinChannelExConfig(string token, bool isDebug, long appId, LJRtcConnection connection,
            ChannelMediaOptions options) : base(connection)
        {
            _token = token;
            _option = options;
            _appId = appId;
            _isDebug = isDebug;
        }
        public override byte[] marshall()
        {
            pushMarshallable(_option);
            pushString16(_token);
            pushInt64(_appId);
            pushBool(_isDebug);
            return base.marshall();
        }
    }

    public class MultiChannelEventResult : HPMarshaller {
        public string msg;
        public int result;
        public long uid;
        public string channenlId;

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            result = popInt();
            uid = popInt64();
            msg = popString16();
            channenlId = popString16();
        }
    }
    public class SubscriberStreamEvent : MultiChannelEvent
    {
        public long _subscriberUid;

        public SubscriberStreamEvent(long subscriberUid, LJRtcConnection connection) : base(connection)
        {
            _subscriberUid = subscriberUid;
        }

        public override byte[] marshall()
        {
            pushInt64(_subscriberUid);
            return base.marshall();
        }
    }
}
