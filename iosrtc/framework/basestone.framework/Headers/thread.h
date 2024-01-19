#pragma once

#include <string>
#include "common_defs.h"
#include "scoped_lock.h"
#include "functor.h"
#include "mutex.h"
#include "semaphore1.h"
#include "util.h"

#ifdef WIN32
#include "windows.h"
#include "./private/windows/thread_impl.h"
#else
#include "./private/common/thread_impl_posix.h"
#endif

namespace LJ
{

class Runnable;


/**
 * Thread class for creating a thread.
 */
class BASESTONE_EXTERN Thread : public ThreadImpl
{
public:
    enum ThreadPriority
    {
        High,
        Normal,
        Low
    };

    /**
     * Get current thread ID.
     * @return current thread ID.
     */
    static long getCurrentThreadID();

    /**
     * Start a thread with a member function without parameter. The member
     * function will run in the new thread.
     * @param obj object of function pointer.
     * @param func function pointer.
     * @param name thread name.
     * @return true if successfully.
     */
    template<typename TClass, typename Func>
    bool start(TClass *obj, Func func, const std::string &name, ThreadPriority priority = Thread::Normal)
    {
        return start(new Functor0<TClass, Func>(obj, func), name, priority);
    }

    /**
     * Start a thread with a member function with one parameter. The member
     * function will run in the new thread.
     * @param obj object of function pointer.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param name thread name.
     * @return true if successfully.
     */
    template<typename TClass, typename Func, typename Param1>
    bool start(TClass *obj, Func func, Param1 p1, const std::string &name, ThreadPriority priority = Thread::Normal)
    {
        return start(new Functor1<TClass, Func, Param1>(obj, func, p1), name, priority);
    }

    /**
     * Start a thread with a member function with two parameters. The member
     * function will run in the new thread.
     * @param obj object of function pointer.
     * @param func function pointer.
     * @param p1 the first parameter.
     * @param p2 the second parameter.
     * @param name thread name.
     * @return true if successfully.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2>
    bool start(TClass *obj, Func func, Param1 p1, Param2 p2, const std::string &name, ThreadPriority priority = Thread::Normal)
    {
        return start(new Functor2<TClass, Func, Param1, Param2>(obj, func, p1, p2), name, priority);
    }

    /**
     * Start a thread with a member function with three parameters. The member
     * function will run in the new thread.
     * @param obj object of function pointer.
     * @param func function pointer.
     * @param p1 the first parameter.
     * @param p2 the second parameter.
     * @param p3 the third parameter.
     * @param name thread name.
     * @return true if successfully.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3>
    bool start(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, const std::string &name, ThreadPriority priority = Thread::Normal)
    {
        return start(new Functor3<TClass, Func, Param1, Param2, Param3>(obj, func, p1, p2, p3), name, priority);
    }

    /**
     * Start a thread with a member function with four parameters. The member
     * function will run in the new thread.
     * @param obj object of function pointer.
     * @param func function pointer.
     * @param p1 the first parameter.
     * @param p2 the second parameter.
     * @param p3 the third parameter.
     * @param p4 the fouth parameter.
     * @param name thread name.
     * @return true if successfully.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4>
    bool start(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, const std::string &name, ThreadPriority priority = Thread::Normal)
    {
        return start(new Functor4<TClass, Func, Param1, Param2, Param3, Param4>(obj, func, p1, p2, p3, p4), name, priority);
    }

    /**
     * Start a thread with a member function with five parameters. The member
     * function will run in the new thread.
     * @param obj object of function pointer.
     * @param func function pointer.
     * @param p1 the first parameter.
     * @param p2 the second parameter.
     * @param p3 the third parameter.
     * @param p4 the fouth parameter.
     * @param p5 the fifth parameter.
     * @param name thread name.
     * @return true if successfully.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5>
    bool start(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, const std::string &name, ThreadPriority priority = Thread::Normal)
    {
        return start(new Functor5<TClass, Func, Param1, Param2, Param3, Param4, Param5>(obj, func, p1, p2, p3, p4, p5), name, priority);
    }

    bool start(thread_run func, void* obj, const std::string &name, ThreadPriority priority = Thread::Normal)
    {
        return start(new CFuncWrapper(func, obj), name, priority);
    }

    //    template<typename Func>
    //    bool start(Func func, const std::string &name)
    //    {
    //        return start(new StaticFunctor0<Func>(func), name);
    //    }
    //
    //    template<typename Func, typename Param1>
    //    bool start(Func func, Param1 p1, const std::string &name)
    //    {
    //        return start(new StaticFunctor1<Func, Param1>(func, p1), name);
    //    }
    //
    //    template<typename Func, typename Param1, typename Param2>
    //    bool start(Func func, Param1 p1, Param2 p2, const std::string &name)
    //    {
    //        return start(new StaticFunctor2<Func, Param1, Param2>(func, p1, p2), name);
    //    }
    //
    //    template<typename Func, typename Param1, typename Param2, typename Param3>
    //    bool start(Func func, Param1 p1, Param2 p2, Param3 p3, const std::string &name)
    //    {
    //        return start(new StaticFunctor3<Func, Param1, Param2, Param3>(func, p1, p2, p3), name);
    //    }
    //
    //    template<typename Func, typename Param1, typename Param2, typename Param3, typename Param4>
    //    bool start(Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, const std::string &name)
    //    {
    //        return start(new StaticFunctor4<Func, Param1, Param2, Param3, Param4>(func, p1, p2, p3, p4), name);
    //    }
    //
    //    template<typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5>
    //    bool start(Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, const std::string &name)
    //    {
    //        return start(new StaticFunctor5<Func, Param1, Param2, Param3, Param4, Param5>(func, p1, p2, p3, p4, p5), name);
    //    }

    /**
     * Set stopping flag to thread. The thread may not be stopped immediately.
     */
    void stop();

    /**
     * Wait for thread stopped synchronized.
     */
    void waitForStop();

    /**
     * Get stopping flag.
     * @return true if stopping flag is set.
     */
    bool isStopping() ;
    /**
      * Get stopped flag.
      * @return true if stopping flag is set or start flag is false.
      */
    bool isStopped() ;

    /**
     * Get thread ID.
     * @return thread ID.
     */
    long getThreadID()
    {
        ScopedLock autoLock(_lock);
        return _threadID;
    }

    /**
     * Get thread name.
     * @return thread name.
     */
    std::string getName() ;

    /**
     * Identify whether thread is started.
     * @return true if started.
     */
    bool isStart()
    {
        ScopedLock autoLock(_lock);
        return _start;
    }

    /**
     * Default constructor.
     */
    Thread();

    /**
     * Destructor.
     */
    virtual ~Thread();

    /**
     * Invalid thread ID.
     */
    static const long INVALID_THREAD_ID;

private:
    /**
     * For implementation on Windows,
     * we have to use this  Win32 thunk to call the c-style entry point : threadProc
     */
#ifdef WIN32
    static DWORD WINAPI threadProcWin32thunk(_In_  LPVOID lpEventData );
#endif
    static void *threadProc(void *args);

    bool start(Runnable *runnable, const std::string &name, ThreadPriority priority = Normal);
    void clear();
    void setThreadName(const std::string &name);
    void setPriority(ThreadPriority priority);

    // Thread object itself must be thread-safe
    Mutex _lock;
    bool _start;
    bool _stopping;
    Runnable *_runnable;
    std::string _name;
    long _threadID;
    Semaphore _startSem;
    ThreadPriority _priorityLevel;
};

}
