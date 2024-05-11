using AOT;
using LJ.RTC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace LJ.Log
{
    internal class FLogNative
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string FLogLibName = "ljlog";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private const string FLogLibName = "ljlog";
#elif UNITY_IPHONE
		private const string FLogLibName = "__Internal";
#else
        private const string FLogLibName = "ljlog";
#endif
        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "FLogInit", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FLogInit(int level, int mode, byte[] logDir, byte[] tag, int cacheDays, int dirLen, int tagLen, bool log2File);

        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "FLogDestroy", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FLogDestroy();

        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "FLogFlush", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FLogFlush();

        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "FLogWritLog", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FLogWritLog(int level, byte[] logStr, int logLen, byte[] tag, int tagLen, long tid, int pid);

        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "GetLogLevel", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetLogLevel();

        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "SetLogMode", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetLogMode(int mode);

        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "SetConsoleLogOpen", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetConsoleLogOpen(bool enable);

        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "SetMaxFileSize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetMaxFileSize(long enable);

        [DllImport(FLogLibName, CharSet = CharSet.Ansi, EntryPoint = "SetMaxAliveTime", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetMaxAliveTime(long enable);

        private bool isInit = false;
        private object mLock = new();
        public FLogNative() {
        
        }
        public void Init(int level, int mode, string logDir, string nameprefix, int cacheDays, bool log2File) {
            lock (mLock) {
                if (isInit)
                {
                    return;
                }
            }
            isInit = true;
            byte[] tagBytes = System.Text.Encoding.UTF8.GetBytes(nameprefix);
            byte[] logDirBytes = System.Text.Encoding.UTF8.GetBytes(logDir);
            FLogInit(level, mode, logDirBytes, tagBytes, cacheDays, logDirBytes.Length, tagBytes.Length, log2File);
        }

        public void Destroy() {
            lock (mLock)
            {
                if (!isInit)
                {
                    return;
                }
            }
            FLogDestroy();
            isInit = false;
        }

        public void Flush()
        {
            FLogFlush();
        }

        public void WriteLog(FLogLevel level, string logStr, string tag)
        {
            //int tid = Thread.CurrentThread.ManagedThreadId;
            int pid = Process.GetCurrentProcess().Id;
            byte[] logBytes = System.Text.Encoding.UTF8.GetBytes(logStr);
            byte[] tagBytes = System.Text.Encoding.UTF8.GetBytes(tag);
            FLogWritLog((int)level, logBytes, logBytes.Length, tagBytes, tagBytes.Length, 0, pid);
        }

        public void WriteLog(FLogLevel level, string logStr)
        {
            WriteLog(level, logStr, "");
        }

        public int GetLevel()
        {
            return GetLogLevel();
        }

        public void SetMode(int mode)
        {
            SetLogMode(mode);
        }

        public void SetConsoleOpen(bool enable)
        {
            SetConsoleLogOpen(enable);
        }

        public void SetMaxLogFileSize(long fileSize)
        {
            SetMaxFileSize(fileSize);
        }

        public void SetMaxLogAliveTime(long time)
        {
            SetMaxAliveTime(time);
        }

    }
}
