## 统计上报SDK使用说明

## 统计上报库需要包含的plugin有：basestone apm

### 示例代码在[ReportExTest.cs](UnityRTCDemo/Assets/demo/Report/ReportExTest.cs)中

### 示例场景在[ReportTest.unity](UnityRTCDemo/Assets/Scenes/ReportTest.unity)中

#### RTC SDK 的接口代码主要封装在[ReportCenter.cs](UnityRTCDemo/Assets/Report/Reporter.cs)中的IRtcEngineEx

#### 1.关键数据结构

```csharp
    namespace LJ.Report
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public class ReportCenterConfig
        {
            public int collectDuration; // 上报时间间隔
            public do_upload_func cb; // 使用业务长链接初始化时，必须实现该方法做上报，该方法马上被废弃了
            public upload_event_func eventCb; // 使用sdk内部长链接，一些事件回调
            public bool isTestEvn; // 是否测试环境， true 测试环境 false 正式环境
            public int appId; // 分配的appid
        }
    
        /// <summary>
        /// 使用sdk内部长链接，一些事件定义
        /// </summary>
        public enum ReportResult
        {
            SUCCESS = 0,
            TOKEN_INVALID = -1, // token 无效需要重新请求token
            APPID_INVALID = -2, // appId 无效，需要设置一个有限的appId
            TOKEN_TIMEOUT = -3, // Token 过期，需要重新请求token
        };


        /// <summary>
        /// 使用业务长链接初始化时，必须实现该方法做上报，该方法马上被废弃了
        /// </summary>
        /// <param name="msg"></param>
        public delegate void do_upload_func(string msg);

        /// <summary>
        /// 使用sdk内部长链接，一些事件回调
        /// </summary>
        /// <param name="result">result 的值是ReportResult，需要针对错误码做一些处理</param>
        /// <param name="msg"></param>
        public delegate void upload_event_func(int result, string msg);


        /// <summary>
        /// 使用业务的长链接，初始化统计sdk
        /// </summary>
        /// <param name="config"></param>
        public static void Init(ReportCenterConfig config)
        {
        }

        /// <summary>
        /// 使用Sdk内部长链接初始化SDK，初始化后必须调用SetUserInfo，设置token以及uid
        /// </summary>
        /// <param name="config"></param>
        public static void InitEx(ReportCenterConfig config)
        {
        }

        /// <summary>
        /// 使用SDK内部长链接必须调用该方法设置token以及uid
        /// 退出登录需要调用该方法重新设置uid，以保证上报数据的准确性
        /// </summary>
        /// <param name="token">服务器分配的token</param>
        /// <param name="uid">用户uid</param>
        public static void SetUserInfo(string token, long uid) {
        }
    }
```csharp```

#### 2.使用sdk内部长链接初始化 示例如下：

```csharp```

        public void Start()
        {
            LJ.Report.ReportCenterConfig cfg = new LJ.Report.ReportCenterConfig();
            cfg.eventCb = ReportEvent;
            cfg.collectDuration = 10000;
            cfg.isTestEvn = true;
            cfg.appId = cfg.isTestEvn ? 1003 : 1002;
            LJ.Report.ReportCenter.InitEx(cfg);
            LJ.Report.ReportCenter.SetUserInfo("", 123); // token 不能为空，否则校验不过
            Dictionary<string, System.Object> info = new Dictionary<string, System.Object>();
            info.Add("appid", cfg.appId);
            info.Add("liveid", 345678);
            LJ.Report.ReportCenter.SetCommonAttrs(info);
            LJ.Report.ReportCenter.EnablePerformance(true);
        }

        public void ReportEvent(int result, string msg) {
            // result 的值是ReportResult，需要针对错误码做一些处理
            FLog.Info("result:" + result + ", msg:" + msg);
        }
```csharp```

#### 3. sdk释放

```csharp```
    LJ.Report.ReportCenter.Release();
```csharp```
