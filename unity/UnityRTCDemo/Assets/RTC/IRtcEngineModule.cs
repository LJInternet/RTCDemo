
using LJ.RTC.Common;
using LJ.RTC.Video;

namespace LJ.RTC
{

    public abstract class BaseRtcEngineModule : ILifecylce
    {
        protected IRtcEngineApi mRtcEngineApi;
        public BaseRtcEngineModule(IRtcEngineApi rtcEngineApi)
        {
            mRtcEngineApi = rtcEngineApi;
        }

        public abstract void OnCreate();

        public virtual void OnDestroy() {
            if (mRtcEngineApi != null)
            {
                mRtcEngineApi.OnComponentDestroy(this);
                mRtcEngineApi = null;

            }  
        }

        public virtual void onTansConnectStateChange(bool connected)
        {

        }
    }
}
