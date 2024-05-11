using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace LJ.RTC.Common
{
    public class RtcEngineGameObject : MonoBehaviour
    {
        void OnApplicationQuit()
        {
            IRtcEngine rtcEngine = LJRtcEngine.Get();
            if (rtcEngine != null)
            {
                rtcEngine.OnDestroy();
            }
            JLog.Info("OnApplicationQuit");
        }

        public struct NoDelayedQueueItem
        {
            public Action<object> action;
            public object param;
        }

        public class DelayedQueueItem
        {
            public float time;
            public Action<object> action;
            public object param;
        }

        private List<NoDelayedQueueItem> _actions = new List<NoDelayedQueueItem>();

        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        List<NoDelayedQueueItem> _currentActions = new List<NoDelayedQueueItem>();

        private static RtcEngineGameObject _current;

        private void Awake()
        {
            _current = this;
        }

        void OnDisable()
        {
            if (_current == this)
            {
                _current = null;
            }
#if UNITY_2018_1_OR_NEWER
            IRtcEngine rtcEngine = LJRtcEngine.Get();
            if (rtcEngine != null)
            {
                rtcEngine.OnDestroy();
            }
#endif
            JLog.Info("OnDisable");
        }


        public static void QueueOnMainThread(Action<object> taction, object tparam)
        {
            QueueOnMainThread(taction, tparam, 0f);
        }
        public static void QueueOnMainThread(Action<object> taction, object tparam, float time)
        {
            if (_current == null)
            {
                return;
            }
            if (time != 0)
            {
                lock (_current._delayed)
                {
                    _current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = taction, param = tparam });
                }
            }
            else
            {
                lock (_current._actions)
                {
                    _current._actions.Add(new NoDelayedQueueItem { action = taction, param = tparam });
                }
            }
        }

        void Update()
        {
            if (_actions.Count > 0)
            {
                lock (_actions)
                {
                    _currentActions.Clear();
                    _currentActions.AddRange(_actions);
                    _actions.Clear();
                }
                for (int i = 0; i < _currentActions.Count; i++)
                {
                    _currentActions[i].action(_currentActions[i].param);
                }
            }

            if (_delayed.Count > 0)
            {
                lock (_delayed)
                {
                    _currentDelayed.Clear();
                    _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                    for (int i = 0; i < _currentDelayed.Count; i++)
                    {
                        _delayed.Remove(_currentDelayed[i]);
                    }
                }

                for (int i = 0; i < _currentDelayed.Count; i++)
                {
                    _currentDelayed[i].action(_currentDelayed[i].param);
                }
            }
        }
    }
}
