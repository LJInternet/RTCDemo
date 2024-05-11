
# unity版本接入:

# 日志库需要包含的plugin有：basestone ljlog (IOS 不需要 mars-boots.lib)

```c#
    FXLog fxlog = new FXLog();
     /**
	 *
	 * @param level 日志等级
	 * @param mode 同步或者异步模式
	 * @param logDir 日志文件夹路径
	 * @param tag 日志文件的前缀
	 * @param cacheDays 日志缓存天数
	 * @param log2File 是否写文件
	 */
    fxlog.Init((int)FLogLevel.LEVEL_DEBUG, (int)LogMode.ASYNC, logDir, tag, cacheDays, true);
    // 设置每个日志文件的大小
    fxlog.SetMaxLogFileSize(10 * 1024 * 1024);
    // 设置是否打开控制台输出，android 是logcat 
    fxlog.SetConsoleOpen(true);
    // 设置实例到Log中，方便静态方法打印日志
    FLog.Init(fxlog);
    FLog.Info("logpath:" + logDir);

    ///////////////////////
    //退出应用时候，调用销毁
    FLog.Uninit();
    
    /////////////////////////////////////////
     /// <summary>
    /// 
    /// </summary>
    /// <param name="level">LogLevel</param>
    /// <param name="mode">LogMode</param>
    /// <param name="logDir">log path</param>
    /// <param name="tag">log tag</param>
    /// <param name="cacheDays">文件保存多少天</param>
    public void Init(int level, int mode, string logDir, string tag, int cacheDays, bool log2File)

    /// <summary>
    /// 销毁，在应用退出的时候调用
    /// </summary>
    public void Destroy()


    /// <summary>
    /// 刷新日志缓存到文件
    /// </summary>
    public void Flush()


    /// <summary>
    /// 
    ///Unity的控制台是否需要显示日志
    /// </summary>
    /// <param name="enable"></param>
    public void SetUnityConsoleOpen(bool enable)


    /// <summary>
    /// 设置日志打印模式，支持同步或者异步
    /// </summary>
    /// <param name="mode">同步 0 异步 1</param>
    public void SetMode(int mode)


    /// <summary>
    /// 设置是否在控制台打印日志，Android 是logcat
    /// </summary>
    /// <param name="enable"></param>
    public void SetConsoleOpen(bool enable)

    /// <summary>
    /// 设置每个文件的大小，单位是B， 例如：10 * 1024 * 1024 1M
    /// </summary>
    /// <param name="fileSize"></param>
    public void SetMaxLogFileSize(long fileSize)

    /// <summary>
    /// 设置日志最大的保存时间，内部会定期清理，单位是秒 例如：24 * 60 * 60 既一天
    /// </summary>
    /// <param name="time"></param>
    public void SetMaxLogAliveTime(long time)

```