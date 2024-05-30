

<h2 id="1"> RTM使用说明</h2>

## RTM需要包含的plugin有：basestone libuv rudp等三个模块即可

#### 1V1RTM SDK 的接口代码主要封装在[RTMEngine.cs](UnityRTCDemo/Assets/RTM/RTMEngine.cs)中

#### 多人RTM SDK 的接口代码主要封装在[RTMEngineEx.cs](UnityRTCDemo/Assets/RTM/RTMEngineEx.cs)中

## RTM使用说明

### 一、初始化数据结构

 ```csharp
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
        /// 加入频道成功，加入频道，只是表示执行加入频道方法成功，并不表示连接连通可用，连接状态请参考OnLinkStatusChange回调
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
        /// channel的连接状态回调，这个才是链接是否可用的状态,
        /// 1V1 RTM时，当status == STATUS_CONNECTED时，表示与对端是连通的，可以互发消息
        /// 多人 RTM时，当status == STATUS_CONNECTED时，表示与服务端是连通的，可以互发消息
        ///STATUS_CONNECTED, 1
        ///STATUS_DISCONNECTED, 2
        ///STATUS_LOST, 3
        /// </summary>
        /// <param name="status">LinkStatus</param>
        public virtual void OnLinkStatusChange(int status)
        {

        }
        /// <summary>
        /// 远端用户加入频道
        /// </summary>
        /// <param name="userId"></param>
        public virtual void OnRemoteUserJoined(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserJoined:" + userId);
        }
        /// <summary>
        /// 远端用户退出频道
        /// </summary>
        /// <param name="userId"></param>
        public virtual void OnRemoteUserOffLine(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserOffLine:" + userId);
        }
    }
````````

<h2 id="2"> RTM 1V1 使用说明</h2>

### 二、RTM 1V1 使用说明

### 1V1RTM示例代码在[RTMEngineTest.cs](UnityRTCDemo/Assets/demo/rtm/RTMEngineTest.cs)中

### 1V1RTM示例场景在[RTMTest.unity](UnityRTCDemo/Assets/Scenes/RTMTest.unity)中

#### 1.初始化

 ```csharp
class RTMEngineEventHandler : IRTMEngineEventHandler
{
    // todo 实现回调逻辑
            /// <summary>
        /// 1V1 RTM 接收对端消息回调
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <param name="channelId"></param>
        public virtual void OnRecvMessage(byte[] message, long uid, string channelId)
        {
            //UnityEngine.Debug.Log("OnRecvMessage:" + System.Text.Encoding.UTF8.GetString(message));
        }

        /// <summary>
        /// 多人RTM 接收对端消息回调
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <param name="channelId"></param>
        public virtual void OnRecvExtMessage(byte[] message, long uid, string channelId)
        {
            //UnityEngine.Debug.Log("OnRecvExtMessage:" + System.Text.Encoding.UTF8.GetString(message));
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
        ///STATUS_CONNECTED, 1
        ///STATUS_DISCONNECTED, 2
        ///STATUS_LOST, 3
        ///STATUS_CLOSE 4
        /// </summary>
        /// <param name="status">LinkStatus</param>
        public virtual void OnLinkStatusChange(int status)
        {

        }
        /// <summary>
        /// 远端用户加入频道
        /// </summary>
        /// <param name="userId"></param>
        public virtual void OnRemoteUserJoined(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserJoined:" + userId);
        }
        /// <summary>
        /// 远端用户退出频道
        /// </summary>
        /// <param name="userId"></param>
        public virtual void OnRemoteUserOffLine(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserOffLine:" + userId);
        }
}

// 实现回调处理
RTMEngineEventHandler handler = new RTMEngineEventHandler();
//初始化RTM配置
RUDPConfig config = new RUDPConfig();
config.isDebug = true;
config.role = (int)RUDPRole.RUDP_CONTROLLER; // 1V1RTM在使用时，两端的role必须不一样，一端是0 则另外一端是1，假如有一端是服务端
config.dataWorkMode = (int)DataWorkMode.SEND_AND_RECV;//0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
config.appId = 1;
config.token = "token";
//初始化RTM实例
mRTMEngine = new RTMEngine(config, handler);
//加入RTM频道，目前与RTC的channelId一致就行
mRTMEngine.JoinChannel(userId, "channelId");
 ``````

#### 2.销毁

 ```csharp
if (mRTMEngine != null) {
    mRTMEngine.LeaveChannel();
    mRTMEngine.Destroy();
    mRTMEngine = null;
}
 ``````

<h2 id="3"> 多人RTM使用说明</h2>

### 三、多人RTM使用说明

### 1V1RTM示例代码在[RTMEngineExTest.cs](UnityRTCDemo/Assets/demo/rtm/RTMEngineExTest.cs)中

### 1V1RTM示例场景在[RTMExTest.unity](UnityRTCDemo/Assets/Scenes/RTMExTest.unity)中

#### 1.初始化多人RTM

 ```csharp

class RTMEngineEventExtHandler : IRTMEngineEventHandler
{
    // todo 实现回调逻辑
            /// <summary>
        /// 1V1 RTM 接收对端消息回调
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <param name="channelId"></param>
        public virtual void OnRecvMessage(byte[] message, long uid, string channelId)
        {
            //UnityEngine.Debug.Log("OnRecvMessage:" + System.Text.Encoding.UTF8.GetString(message));
        }

        /// <summary>
        /// 多人RTM 接收对端消息回调
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <param name="channelId"></param>
        public virtual void OnRecvExtMessage(byte[] message, long uid, string channelId)
        {
            //UnityEngine.Debug.Log("OnRecvExtMessage:" + System.Text.Encoding.UTF8.GetString(message));
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
        ///STATUS_CONNECTED, 1
        ///STATUS_DISCONNECTED, 2
        ///STATUS_LOST, 3
        ///STATUS_CLOSE 4
        /// </summary>
        /// <param name="status">LinkStatus</param>
        public virtual void OnLinkStatusChange(int status)
        {

        }
        /// <summary>
        /// 远端用户加入频道
        /// </summary>
        /// <param name="userId"></param>
        public virtual void OnRemoteUserJoined(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserJoined:" + userId);
        }
        /// <summary>
        /// 远端用户退出频道
        /// </summary>
        /// <param name="userId"></param>
        public virtual void OnRemoteUserOffLine(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserOffLine:" + userId);
        }
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

#### 2.销毁多人RTM

 ```csharp
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
 ``````