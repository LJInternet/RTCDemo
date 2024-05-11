using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.Log
{
    public enum FLogLevel
    {
        LEVEL_ALL = 0,
        LEVEL_VERBOSE = 0,
        LEVEL_DEBUG = 1,
        LEVEL_INFO = 2,
        LEVEL_WARNING = 3,
        LEVEL_ERROR = 4,
        LEVEL_FATAL = 5,
        LEVEL_NONE = 6,
    }

    public enum LogMode
    {
        ASYNC = 0,
        SYNC = 1,
    }


    public interface IFLog
    {
        /// <summary>
        /// debug 日志
        /// </summary>
        /// <param name="msg"></param>
        void Debug(string msg);


        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="msg"></param>
        void Info(string msg);

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        void Error(string msg);

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        void Warring(string msg);

        void Debug(string tag, string msg);


        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="msg"></param>
        void Info(string tag, string msg);

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        void Error(string tag, string msg);

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        void Warring(string tag, string msg);

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        void Fatal(string tag, string msg);
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        void Fatal(string msg);

        string GetLogPath();

        void Flush();

        void Destroy();
    }

    public class FLog
    {
        private static IFLog mLogerImpl;
        public static void Init(IFLog logImpl)
        {
            mLogerImpl = logImpl;
        }

        public static void Debug(string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Debug(msg);
            }
        }

        public static void Debug(string tag, string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Debug(tag , msg);
            }
        }

        public static void Error(string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Error(msg);
            }
        }

        public static void Error(string tag, string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Error(tag, msg);
            }
        }

        public static void Info(string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Info(msg);
            }
        }

        public static void Info(string tag, string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Info(tag, msg);
            }
        }

        public static void Fatal(string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Fatal(msg);
            }
        }

        public static void Fatal(string tag, string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Fatal(tag, msg);
            }
        }

        public static void Warring(string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Warring(msg);
            }
        }

        public static void Warring(string tag, string msg)
        {
            if (mLogerImpl != null)
            {
                mLogerImpl.Warring(tag, msg);
            }
        }

        public static string GetLogPath() {
            if (mLogerImpl != null)
            {
                return mLogerImpl.GetLogPath();
            }
            return "";
        }

        public static void Uninit()
        {
            Info("Flog Uninit");
            if (mLogerImpl != null) {
                mLogerImpl.Destroy();
            }
        }

        public static void Flush() {
            Info("Flog Flush");
            if (mLogerImpl != null)
            {
                mLogerImpl.Flush();
            }
        }
    }
}
