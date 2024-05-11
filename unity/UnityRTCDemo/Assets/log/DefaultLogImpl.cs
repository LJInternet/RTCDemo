using System;
using UnityEngine;

namespace LJ.Log
{
    public class DefaultLogImpl : IFLog
    {
        public void Debug(string msg)
        {
            UnityEngine.Debug.Log(msg); ;
        }

        public void Debug(string tag, string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Destroy()
        {
        }

        public void Error(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Error(string tag, string msg)
        {
            UnityEngine.Debug.Log(tag + ":" + msg);
        }

        public void Fatal(string tag, string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Fatal(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public string GetLogPath()
        {
            return "";
        }

        public void Info(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Info(string tag, string msg)
        {
            UnityEngine.Debug.Log(tag + ":" + msg);
        }

        public void Warring(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Warring(string tag, string msg)
        {
            UnityEngine.Debug.Log(tag + ":" + msg);
        }

        public void Flush() {

        }
    }
}
