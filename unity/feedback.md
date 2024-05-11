# unity版本接入:

## 反馈库需要包含的plugin有：ssl crypto libcurl basestone feedback zlibwapi libeay32

 ```csharp

    /**
     *
     * @param token 用户登录成功的token，用于请求上传信息时，做校验
     * @param host 反馈请求的域名SDK内部设置生产环境为"app.fancyjing.com” 测试环境为"testapp.fancyjing.com"
     * @param port 端口，默认-1，没有则填-1
     * @param isDebug 是否是测试，用于旋转host
     */
     public void Init(String token, String host, int port, boolean isDebug);
     
     /**
     * 销毁反馈实例
     */
     public void Destroy();
    
     /**
     * 反馈日志
     * @param title 反馈标题
     * @param content 反馈内容
     * @param filePath 日志文件夹路径
     * @param liveId liveId
     */
     public void SendFeedBack(String title, String content, String filePath, String liveId);
     
     /**
     * 设置公共字段
     * system 系统 adr ios or windows
     * appver 应用版本号
     * userId 用户Id
     */
    public void SetCommonAttrs(Map<String,Object> map);
    
    //////////////////////////
    //初始化
    FeedbackConfig config = new FeedbackConfig();
    config.SetDebug(true);
    FeedbackMgr.Init(config);
    Dictionary<string, System.Object> commonAttrs = new Dictionary<string, System.Object>();
    commonAttrs.Add("system", "win_unity");
    commonAttrs.Add("appver", "0.0.1");
    commonAttrs.Add("userId", "0");
    FeedbackMgr.SetCommonAttrs(commonAttrs);
    
    //调用反馈接口，即可完成文件打包和文件上传：
    FeedbackMgr.getInstance().SendFeedBack("test", "ssss", FLog.getLogPath(), "");

    //退出应用时，调用销毁接口
    FeedbackMgr.getInstance().Destroy();

    //若需要监听上传结果,则增加监听回调
    FeedbackMgr.SubscribeFeedbackResult(OnFeedbackResult callback);

    /**
        *
        * @param result 上传结果： 0 成功， 非0 失败
        * @param msg 描述
        */
    public delegate void OnFeedbackResult(int result, string msg);
````