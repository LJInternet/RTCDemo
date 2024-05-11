using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LJ.RTC.Common
{
    public interface ILog
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
    }

    public interface IReprot {
        void DoReport(string key, string info);
    }

}

