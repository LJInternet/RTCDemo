## RTM使用说明

### 一、初始化数据结构
``````

    public enum DataWorkMode
    {
        SEND_AND_RECV = 0, // 既发送又接收
        SEND_ONLY = 1, // 只是发送
        RECV_ONLY = 2, // 只是接收
    };

    public enum RUDPMode
    {
        RUDP_REALTIME_ULTRA, //RTM
        RUDP_REALTIME_NORMAL // RTC
    };

    public enum RUDPRole
    {
        RUDP_NORMAL, // 
        RUDP_CONTROLLER // 一般客户端设置为controller
    };
    public struct RUDPConfig
    {
        public string token;     // 正式环境不能为空，测试环境使用默认的token
        public int appId;        // 服务器分配的APPID
        public int mode;         // RUDPMode 0 RTM 1 RTC
        public int role;         // RUDPRole 0 normal 1 controller
        public bool isDebug;     // 测试环境还是正式环境
        public int dataWorkMode; // DataWorkMode
    };

    public class IRTMEngineEventHandler
    {
        /// <summary>
        /// 1V1 RTM 接收对端消息回调
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <param name="channelId"></param>
        public virtual void OnRecvMessage(byte[] message, long uid, string channelId)
        {

        }

        /// <summary>
        /// 多人RTM 接收对端消息回调
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <param name="channelId"></param>
        public virtual void OnRecvExtMessage(byte[] message, long uid, string channelId)
        {

        }

        /// <summary>
        /// 加入频道失败
        /// </summary>
        public virtual void OnJoinChannelFail()
        {
        }

        /// <summary>
        /// 加入频道成功
        /// </summary>
        public virtual void OnJoinChannelSuccess()
        {
           
        }

        /// <summary>
        /// 离开频道成功
        /// </summary>
        public virtual void OnLeaveChannelSuccess()
        {
            
        }

        /// <summary>
        /// 离开频道失败
        /// </summary>
        public virtual void OnLeaveChannelFail()
        {
            
        }
        /// <summary>
        ///STATUS_CONNECTED,
        ///STATUS_DISCONNECTED,
        ///STATUS_LOST,
        /// </summary>
        /// <param name="status">LinkStatus</param>
        public virtual void OnLinkStatusChange(int status)
        {

        }
    }
````````

### 二、RTM 1V1 使用说明
###### 1.初始化
 ``````

class RTMEngineEventHandler : IRTMEngineEventHandler
{

    // todo 实现回调逻辑
}

// 实现回调处理
RTMEngineEventHandler handler = new RTMEngineEventHandler();
//初始化RTM配置
RUDPConfig config = new RUDPConfig();
config.isDebug = true;
config.role = (int)RUDPRole.RUDP_CONTROLLER;
config.dataWorkMode = (int)DataWorkMode.SEND_AND_RECV;
config.appId = 1;
config.token = "token";
//初始化RTM实例
mRTMEngine = new RTMEngine(config, handler);
//加入RTM频道，目前与RTC的channelId一致就行
mRTMEngine.JoinChannel(userId, "channelId");
 ``````

 ###### 2.销毁
 ``````
if (mRTMEngine != null) {
    mRTMEngine.LeaveChannel();
    mRTMEngine.Destroy();
    mRTMEngine = null;
}
 ``````

 ### 三、多人RTM使用说明

 ###### 1.初始化
 ``````

class RTMEngineEventExtHandler : IRTMEngineEventHandler
{

    // todo 实现回调逻辑
}

// 实现回调处理
RTMEngineEventHandler handler = new RTMEngineEventExtHandler();

//初始化多人RTM实例
RTMEngineEx _RTMEngineEx = new RTMEngineEx("token", appid, isDebug);
// 创建RTM Channel实例
RTMChannel _RTMChannel = _RTMEngineEx.CreateRTMChannel(DataWorkMode.SEND_AND_RECV,
            uid, "channelId", handler);
//加入RTM频道
_RTMChannel.Join();
 ``````

 ###### 2.销毁
 ``````
if (_RTMChannel != null)
{
    _RTMChannel.Leave();
    _RTMChannel.Dispose();
    _RTMChannel = null;
}

if (_RTMEngineEx != null)
{
    _RTMEngineEx.Release();
    _RTMEngineEx = null;
}