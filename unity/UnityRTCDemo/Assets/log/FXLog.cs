using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LJ.Log
{
    public class FXLog : IFLog
    {
        private FLogNative mLogNative;
#if UNITY_EDITOR || UNITY_STANDALONE
        private bool mUnityConsoleEnable = true;
#else
        private bool mUnityConsoleEnable = false;
#endif
        private string mLogPath;
        private string mTag = "FLog";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level">LogLevel</param>
        /// <param name="mode">LogMode</param>
        /// <param name="logDir">log path</param>
        /// <param name="tag">log tag</param>
        /// <param name="cacheDays">文件保存多少天</param>
        public void Init(int level, int mode, string logDir, string tag, int cacheDays, bool log2File)
        {
            mLogNative = new FLogNative();
            mTag = tag;
            mLogNative.Init(level, mode, logDir, tag, cacheDays, log2File);
            mLogPath = logDir;
        }

        /// <summary>
        /// 销毁，在应用退出的时候调用
        /// </summary>
        public void Destroy()
        {
            if (mLogNative != null) {
                mLogNative.Flush();
                mLogNative.Destroy();
            }
        }

        /// <summary>
        /// 刷新日志缓存到文件
        /// </summary>
        public void Flush()
        {
            if (mLogNative != null)
            {
                mLogNative.Flush();
            }
        }

        /// <summary>
        /// 
        ///Unity的控制台是否需要显示日志
        /// </summary>
        /// <param name="enable"></param>
        public void SetUnityConsoleOpen(bool enable) {
            mUnityConsoleEnable = enable;
        }

        public int GetLevel()
        {
            if (mLogNative != null)
            {
               return mLogNative.GetLevel();
            }
            return 0;
        }

        /// <summary>
        /// 设置日志打印模式，支持同步或者异步
        /// </summary>
        /// <param name="mode">同步 0 异步 1</param>
        public void SetMode(int mode)
        {
            if (mLogNative != null)
            {
                mLogNative.SetMode(mode);
            }
        }

        /// <summary>
        /// 设置是否在控制台打印日志，Android 是logcat
        /// </summary>
        /// <param name="enable"></param>
        public void SetConsoleOpen(bool enable)
        {
            if (mLogNative != null)
            {
                mLogNative.SetConsoleOpen(enable);
            }
        }

        /// <summary>
        /// 设置每个文件的大小，单位是B， 例如：10 * 1024 * 1024 1M
        /// </summary>
        /// <param name="fileSize"></param>
        public void SetMaxLogFileSize(long fileSize)
        {
            if (mLogNative != null)
            {
                mLogNative.SetMaxLogFileSize(fileSize);
            }
        }

        /// <summary>
        /// 设置日志最大的保存时间，内部会定期清理，单位是秒 例如：24 * 60 * 60 既一天
        /// </summary>
        /// <param name="time"></param>
        public void SetMaxLogAliveTime(long time)
        {
            if (mLogNative != null)
            {
                mLogNative.SetMaxLogAliveTime(time);
            }
        }

        public void Debug(string msg)
        {
            Debug(mTag, msg);
        }
           

        public void Error(string msg)
        {
            Error(mTag, msg);
        }

        public void Info(string msg)
        {
            Info(mTag, msg);
        }

        public void Warring(string msg)
        {
            Warring(mTag, msg);
        }

        public void Fatal(string msg)
        {
            Fatal(mTag, msg);
        }

        public string GetLogPath()
        {
            return mLogPath;
        }

        public void Debug(string tag, string msg)
        {
            if (mUnityConsoleEnable)
            {
                UnityEngine.Debug.Log(tag + ":" + msg);
            }
            if (mLogNative != null)
            {
                mLogNative.WriteLog(FLogLevel.LEVEL_DEBUG, msg, tag);
            }
        }

        public void Info(string tag, string msg)
        {
            if (mUnityConsoleEnable)
            {
                UnityEngine.Debug.Log(tag + ":" + msg);
            }
            if (mLogNative != null)
            {
                mLogNative.WriteLog(FLogLevel.LEVEL_INFO, msg, tag);
            }
        }

        public void Error(string tag, string msg)
        {
            if (mUnityConsoleEnable)
            {
                UnityEngine.Debug.LogError(tag + ":" + msg);
            }
            if (mLogNative != null)
            {
                mLogNative.WriteLog(FLogLevel.LEVEL_ERROR, msg, msg);
            }
        }

        public void Warring(string tag, string msg)
        {
            if (mUnityConsoleEnable)
            {
                UnityEngine.Debug.LogWarning(tag + ":" + msg);
            }
            if (mLogNative != null)
            {
                mLogNative.WriteLog(FLogLevel.LEVEL_WARNING, msg, tag);
            }
        }

        public void Fatal(string tag, string msg)
        {
            if (mUnityConsoleEnable)
            {
                UnityEngine.Debug.LogAssertion(tag  + ":" + msg);
            }
            if (mLogNative != null)
            {
                mLogNative.WriteLog(FLogLevel.LEVEL_FATAL, msg, tag);
            }
        }
    }
}
