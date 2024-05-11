//
// Created by Administrator on 2023/4/12.
//

#ifndef LJSDK_WIN2C_XLOG_H
#define LJSDK_WIN2C_XLOG_H
#include <compiler_util.h>
#ifdef __cplusplus
extern "C" {
#endif
    /**
     * example
     * std::string filePath = "C:/Users/Administrator/AppData/Local/Temp/DefaultCompany/�ھ�RTC����/log";
        char* logDirstr = "C:/Users/Administrator/AppData/Local/Temp/DefaultCompany/�ھ�RTC����/log";
        LJ::Log::log("xlog", LJ::LogLevel::LOG_INFO, "logDirstr %s", filePath.c_str());
        FLogInit(0, 0, logDirstr, prix, 0, strlen(logDirstr), strlen(prix), true);

        for (int i = 0; i < 100000; i++) {
            FLogWritLog(1, logStr, strlen(logStr), prix, strlen(prix), 0, 0);
            SLEEP(5);
        }
        FLogDestroy();
     */

    /**
    *
    * @param level 日志等级
     * typedef enum {
            kLevelAll = 0,
            kLevelVerbose = 0,
            kLevelDebug,    // Detailed information on the flow through the system.
            kLevelInfo,     // Interesting runtime events (startup/shutdown), should be conservative and keep to a minimum.
            kLevelWarn,     // Other runtime situations that are undesirable or unexpected, but not necessarily "wrong".
            kLevelError,    // Other runtime errors or unexpected conditions.
            kLevelFatal,    // Severe errors that cause premature termination.
            kLevelNone,     // Special level used to disable all log messages.
        } TLogLevel;
    *
    * @param mode 同步或者异步模式  0  kAppednerAsync 1 kAppednerSync,
    * @param logDir 日志文件夹路径
    * @param nameprefix 日志文件的前缀
    * @param cacheDays 日志缓存天数
    * @param dirLen 日志路径字符长度
    * @param tagLen 日志前缀字符长度
    * @param log2File 是否写文件
    */
	XLOG_EXTERN void FLogInit(int level, int mode, char* logDir, char* nameprefix, int cacheDays, int dirLen, int tagLen, bool enableLog2File);
    /**
     * 销毁，在应用退出的时候调用
     */
	XLOG_EXTERN void FLogDestroy();
    /**
     * 异步模式下，清空内存buffer到文件中
     * @param isSync
     */
	XLOG_EXTERN void FLogFlush(bool isSync);
	XLOG_EXTERN void FLogWritLog(int level, char* logStr, int logLen, char* tag, int tagLen, long tid, int pid);
	XLOG_EXTERN int GetLogLevel();
	XLOG_EXTERN void SetLogMode(int mode);
    /**
     * 是否显示到控制台
     * @param enable
     */
	XLOG_EXTERN void SetConsoleLogOpen(bool enable);
    /**
     * 设置每个文件的大小，单位是B， 例如：10 * 1024 * 1024 1M
     * @param fileSize
     */
	XLOG_EXTERN void SetMaxFileSize(long fileSize);
    /**
     * 设置日志最大的保存时间，内部会定期清理，单位是秒 例如：24 * 60 * 60 既一天
     * @param time
     */
	XLOG_EXTERN void SetMaxAliveTime(long time);
#ifdef __cplusplus
}
#endif
#endif //LJSDK_WIN2C_XLOG_H
