#pragma once

#include <map>
#include <list>
#include <deque>
#include "thread.h"
#include "shared_ptr.h"
#include "task_loop.h"

namespace LJ {

    /**
     * 定时任务调度中心
     */
    class BASESTONE_EXTERN ScheduledTaskCenter {
        friend class TaskLoop;

    public:
        /**
         * 停止指定任务
         * @param timerID
         */
        void stopScheduledTasks(long timerID);

        long addTimer(Runnable *runnable, unsigned int msecInterval, bool repeat);

        static ScheduledTaskCenter *getInstance();

    private:

        template<typename TClass, typename Func>
        long startTimer(TClass *obj, Func func, unsigned int msecInterval, bool repeat) {
            return addTimer(new Functor0<TClass, Func>(obj, func), msecInterval, repeat);
        }

        template<typename TClass, typename Func, typename Param1>
        long startTimer(TClass *obj, Func func, Param1 p1, unsigned int msecInterval, bool repeat) {
            return addTimer(new Functor1<TClass, Func, Param1>(obj, func, p1), msecInterval, repeat);
        }

        template<typename TClass, typename Func, typename Param1, typename Param2>
        long startTimer(TClass *obj, Func func, Param1 p1, Param2 p2, unsigned int msecInterval, bool repeat) {
            return addTimer(new Functor2<TClass, Func, Param1, Param2>(obj, func, p1, p2), msecInterval, repeat);
        }

        template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3>
        long startTimer(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, unsigned int msecInterval, bool repeat) {
            return addTimer(new Functor3<TClass, Func, Param1, Param2, Param3>(obj, func, p1, p2, p3), msecInterval, repeat);
        }

        template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4>
        long startTimer(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, unsigned int msecInterval, bool repeat) {
            return addTimer(new Functor4<TClass, Func, Param1, Param2, Param3, Param4>(obj, func, p1, p2, p3, p4), msecInterval, repeat);
        }

        template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5>
        long startTimer(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, unsigned int msecInterval, bool repeat) {
            return addTimer(new Functor5<TClass, Func, Param1, Param2, Param3, Param4, Param5>(obj, func, p1, p2, p3, p4, p5), msecInterval, repeat);
        }

        /**
         * 将制定looper上的定时任务注册到调度中心
         * @param loop
         * @param tFun
         * @param msecInterval
         * @param repeat
         * @return
         */
        long startScheduledTasks(TaskLoop *loop, Runnable *runnable, unsigned int msecInterval, bool repeat);

        /**
         * 重置指定任务在下一个周期运行
         * @param timerID
         */
        void resetScheduledTasks(long timerID);

        /**
         * 取消某个指定对象上的定时任务
         * @param object
         */
        void stopScheduledTasksForObject(void *object);

        /**
         * 取消某个looper运行的所有定时任务
         * @param object
         */
        void stopScheduledTasksForLooper(void *object);

        ScheduledTaskCenter();

        ~ScheduledTaskCenter();

    private:

        long buildTimerId(bool repeat);
        class TimerFunction : public Runnable {
        public:
            TimerFunction(TaskLoop *obj, Runnable *runnable, long timerId) :
                    _loop(obj),
                    _runnable(runnable),
                    _timerId(timerId) {
            }

            virtual void run() {
                Runnable *_run = _runnable.ptr()->clone();
                //LJ_LOG(LOG_INFO, "STC_FUNC timerFunction %d", _timerId);
                _loop->postItem(_run, false, false, _timerId);
            }

            virtual void *getObject() {
                return _runnable->getObject();
            }

            TaskLoop *getTaskLoop() {
                return _loop;
            }

            const char *getClassInfo() {
                return __PRETTY_FUNCTION__;
            }

        protected:
            TaskLoop *_loop;//这里别乱析构，生命周期不归这里管
            SharedPtr<Runnable> _runnable;
            long _timerId;
        };

        //将简单任务包装成TimerFunction的样子
        class TimerFunctionProxy : public TimerFunction {
        public:
            TimerFunctionProxy(Runnable *runnable, long timerId) : TimerFunction(NULL, runnable, timerId) {}

            virtual void run() {
                _runnable->run();
            }

            virtual void *getObject() {
                return _runnable->getObject();
            }
        };

        void taskLoopProc();

        unsigned int getWaitTime(uint64_t current) const;

        class Timer {
        public:
            Timer(long timerId, TimerFunction *runnable, unsigned int interval, bool repeat);

            void updateNextTime(uint64_t current);

            void resetNextTime();

            bool operator<(const Timer &timer) const;

            bool operator==(long timerID) const;

            long timerId;
            SharedPtr<TimerFunction> tFun;
            unsigned int interval;
            uint64_t nextTime;
            bool repeat;
        };

        Semaphore _itemsSem;
        Thread _thread;
        Mutex _lock;
        std::list<Timer> _sortedTimers;
        Mutex _runningLock;
        std::list<Timer> _runningTimers;
        static ScheduledTaskCenter s_scheduledTaskCenter;
        long _timerSeqLimit;
        long _repeatTimerIdSeq;
        long _timerIdSeq;
    };
}