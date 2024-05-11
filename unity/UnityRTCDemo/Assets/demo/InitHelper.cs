using Fancy;
using LJ.Feedback;
using LJ.Log;
using LJ.Report;
using LJ.RTC;
using LJ.RTC.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LJ.demo
{
    class ReportImpl : IReprot
    {
        public void DoReport(string key, string info)
        {
            ReportCenter.Report(key, info);
        }
    }

    public class InitHelper
    {
        public static int _sessionId = 954523111;
        public static String _token = "token";
        public static int _mutiChannelId = 954523222;
        public static int ENCODE_WIDTH = 640;
        public static int ENCODE_HEIGHT = 480;
        public static Int64 userId = Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        public static ORIENTATION_MODE OriMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_PORTRAIT;

        public static int GetEncodeWidth() {
            if (OriMode == ORIENTATION_MODE.ORIENTATION_MODE_FIXED_PORTRAIT)
            {
                return Math.Min(ENCODE_WIDTH, ENCODE_HEIGHT);
            }
            else
            {
                return Math.Max(ENCODE_WIDTH, ENCODE_HEIGHT);
            }
        }

        public static int GetEncodeHeight()
        {
            if (OriMode == ORIENTATION_MODE.ORIENTATION_MODE_FIXED_LANDSCAPE)
            {
                return Math.Min(ENCODE_WIDTH, ENCODE_HEIGHT);
            }
            else
            {
                return Math.Max(ENCODE_WIDTH, ENCODE_HEIGHT);
            }
        }

        public static void InitLog(string logDir, string tag, int cacheDays) {
            FXLog fxlog = new FXLog();
            fxlog.Init((int)FLogLevel.LEVEL_DEBUG, (int)LogMode.ASYNC, logDir, tag, cacheDays, true);
            fxlog.SetMaxLogFileSize(10 * 1024 * 1024);
            fxlog.SetConsoleOpen(true);
            FLog.Init(fxlog);
        }

        public static void InitFeedBack() {
            UnityEngine.Debug.Log("InitFeedBack");
            FeedbackConfig config = new FeedbackConfig();
            config.SetDebug(true);
            FeedbackMgr.Init(config);
            Dictionary<string, System.Object> commonAttrs = new Dictionary<string, System.Object>();
            commonAttrs.Add("system", "win_unity");
            commonAttrs.Add("appver", "0.0.1");
            commonAttrs.Add("userId", "0");
            FeedbackMgr.SetCommonAttrs(commonAttrs);
            FeedbackMgr.SubscribeFeedbackResult((int result, string msg)=>{
                FLog.Info("feedback msg result =" + result + " :msg :" + msg);
            });
        }

        public static IRtcEngine StartRtcEngine()
        {
            return StartRtcEngine(true);
        }

        public static IRtcEngine StartRtcEngine(bool joinChannel)
        {
            RtcEngineConfig config = new RtcEngineConfig();
            config.mReport = new ReportImpl();
            config.mAppId = 111;
            config.isTestEv = true;
            config.mJLog = new RtcLogImpl();
            config.mReport = new ReportImpl();
            IRtcEngine mRtcEngine = IRtcEngine.CreateRtcEngine(config);

            mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            //encoderConfiguration.mirrorMode = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED;
            mRtcEngine.SetVideoEncoderConfiguration(CreateVideoEncodeConfiguration());
            mRtcEngine.EnableVideo(true);
            if (joinChannel) {
                mRtcEngine.JoinChannel(GetChannelConfig());
            }
            //string msg = "{\"p2pSignalServerIp\":\"61.155.136.209\", \"p2pSignalServerPort\":9988, \"liveid\": " + sessionId + "}";
            //mRtcEngine.onRecvMessage("UniRelayStartP2P", msg);
            return mRtcEngine;
        }

        public static VideoEncoderConfiguration CreateVideoEncodeConfiguration() {
            VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration();
            encoderConfiguration.dimensions = new VideoDimensions(ENCODE_WIDTH, ENCODE_HEIGHT);
            encoderConfiguration.frameRate = 15;
            encoderConfiguration.orientationMode = OriMode;
            encoderConfiguration.fillMode = FILL_MODE.COR_CENTER;
            encoderConfiguration.keyFrameInterval = -1;
            encoderConfiguration.bitrateMode = 1;
            return encoderConfiguration;
        }

        public static IRtcEngineEx StartRtcEngineEx(bool joinChannel)
        {
            RtcEngineConfig config = new RtcEngineConfig();
            config.mAppId = 0x70FFFFFF;
            config.isTestEv = true;
            config.mJLog = new RtcLogImpl();
            IRtcEngineEx mRtcEngine = IRtcEngine.CreateRtcEngineEx(config);

            mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            //encoderConfiguration.mirrorMode = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED;
            mRtcEngine.SetVideoEncoderConfiguration(CreateVideoEncodeConfiguration());
            mRtcEngine.EnableVideo(true);
            if (joinChannel)
            {
                mRtcEngine.JoinChannel(GetChannelConfig());
            }
            //string msg = "{\"p2pSignalServerIp\":\"61.155.136.209\", \"p2pSignalServerPort\":9988, \"liveid\": " + sessionId + "}";
            //mRtcEngine.onRecvMessage("UniRelayStartP2P", msg);
            return mRtcEngine;
        }

        public static ChannelConfig GetChannelConfig()
        {
            LJRtcEngine.SetDebugEnv(true);

            int sessionId = _sessionId;
            ChannelConfig channelConfig = new ChannelConfig();
            channelConfig.appID = 1;
            channelConfig.channelID = sessionId + "";
            channelConfig.userID = userId;
            channelConfig.token = InitHelper._token;
            //UdpInitConfig initConfig = new UdpInitConfig();
            //initConfig.remoteSessionId = sessionId;

            //UdpInitConfig initConfig2 = new UdpInitConfig();
            //initConfig2.remoteSessionId = sessionId;
            //initConfig2.relayId = sessionId;
            //initConfig2.netType = 2;
            //initConfig2.remoteIP = "114.236.138.71";
            //initConfig2.remotePort = 30001;

            //channelConfig.configs.Add(initConfig);
            //channelConfig.configs.Add(initConfig2);
            return channelConfig;
        }

        public static LjRudpUtils StartRtm()
        {
            LjRudpUtils rtm = new LjRudpUtils();
            ChannelConfig channel = GetRtmConfig();
            rtm.JoinChannelRudp(123456, _sessionId+ "", channel.configs, "123456");
            return rtm;
        }

        public static ChannelConfig GetRtmConfig()
        {

            ChannelConfig channelConfig = new ChannelConfig();
            UdpInitConfig initConfig = new UdpInitConfig();
            initConfig.remoteSessionId = _sessionId;

            UdpInitConfig initConfig2 = new UdpInitConfig();
            initConfig2.remoteSessionId = _sessionId;
            initConfig2.relayId = _sessionId;
            initConfig2.netType = 2;
            initConfig2.remoteIP = "114.236.138.71";
            initConfig2.remotePort = 30001;

            channelConfig.configs.Add(initConfig);
            channelConfig.configs.Add(initConfig2);
            return channelConfig;
        }

        private static long previousTimeMillis = TimeHelper.GetMillSecond();
        private static long counter = 0L;

        public static long nextID()
        {
            long currentTimeMillis = TimeHelper.GetMillSecond();
            counter = (currentTimeMillis == previousTimeMillis) ? (counter + 1L) & 1048575L : 0L;
            previousTimeMillis = currentTimeMillis;
            long timeComponent = (currentTimeMillis & 8796093022207L) << 20;
            return timeComponent | counter;
        }

        public static void InitReport(ReportCenter.upload_event_func cb) {
            LJ.Report.ReportCenterConfig cfg = new LJ.Report.ReportCenterConfig();
            cfg.eventCb = cb;
            cfg.collectDuration = 10000;
            cfg.isTestEvn = true;
            cfg.appId = cfg.isTestEvn ? 1003 : 1002;
            LJ.Report.ReportCenter.InitEx(cfg);
            LJ.Report.ReportCenter.SetUserInfo(InitHelper._token, 123);
            Dictionary<string, System.Object> info = new Dictionary<string, System.Object>();
            info.Add("appid", cfg.appId);
            info.Add("ua", "unityRTCdemo&0.0.1&test");
            info.Add("userId", 123456);
#if UNITY_ANDROID
            info.Add("platform", "android");
#elif UNITY_IOS
           info.Add("platform", "apple");
#else
            info.Add("platform", "windows");
#endif
            info.Add("monitorVer", "0.0.1");
            info.Add("rtcMode", 1);
            info.Add("liveid", nextID());
            LJ.Report.ReportCenter.SetCommonAttrs(info);
            LJ.Report.ReportCenter.EnablePerformance(true);
        }
    }
}
