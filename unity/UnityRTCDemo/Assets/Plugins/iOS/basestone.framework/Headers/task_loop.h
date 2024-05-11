#pragma once

#include <stdint.h>
#include <deque>
#include <string>
#include <list>
#include "thread.h"
#include "mutex.h"
#include "semaphore1.h"
#include "shared_ptr.h"

namespace LJ
{

class Runnable;

/**
 * TaskLoop provides a mechanism of running a runner into another thread.
 * TaskLoop object runs in its own thread, and all runners post into this
 * object will be saved into a queue and run in TaskLoop thread in order.
 * After ran, the runner object will be delete by TaskLoop.
 */
class BASESTONE_EXTERN TaskLoop
{
public:
    /**
     * Constructor.
     * @param name thread name.
     */
    TaskLoop(const std::string &name);

    /**
     * Destructor.
     */
    virtual ~TaskLoop();

    /**
     * Start TaskLoop thread.
     */
    void start(Thread::ThreadPriority priority = Thread::Normal);

    /**
     * Stop TaskLoop thread synchronized.
     */
    void stop();

    /*
     * Post a member function into run loop. The member function will run in
     * run loop thread by order of post time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func>
    long post(TClass *obj, Func func, bool isHighPriority)
    {
        return postItem(new Functor0<TClass, Func>(obj, func), isHighPriority, false);
    }

    /**
     * Post a member function into run loop. The member function will run in
     * run loop thread by order of post time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1>
    long post(TClass *obj, Func func, Param1 p1, bool isHighPriority)
    {
        return postItem(new Functor1<TClass, Func, Param1>(obj, func, p1), isHighPriority, false);
    }

    /**
     * Post a member function into run loop. The member function will run in
     * run loop thread by order of post time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2>
    long post(TClass *obj, Func func, Param1 p1, Param2 p2, bool isHighPriority)
    {
        return postItem(new Functor2<TClass, Func, Param1, Param2>(obj, func, p1, p2), isHighPriority, false);
    }

    /**
     * Post a member function into run loop. The member function will run in
     * run loop thread by order of post time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3>
    long post(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, bool isHighPriority)
    {
        return postItem(new Functor3<TClass, Func, Param1, Param2, Param3>(obj, func, p1, p2, p3), isHighPriority, false);
    }

    /**
     * Post a member function into run loop. The member function will run in
     * run loop thread by order of post time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4>
    long post(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, bool isHighPriority)
    {
        return postItem(new Functor4<TClass, Func, Param1, Param2, Param3, Param4>(obj, func, p1, p2, p3, p4), isHighPriority, false);
    }

    /**
     * Post a member function into run loop. The member function will run in
     * run loop thread by order of post time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param p5 fifth parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5>
    long post(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, bool isHighPriority)
    {
        return postItem(new Functor5<TClass, Func, Param1, Param2, Param3, Param4, Param5>(obj, func, p1, p2, p3, p4, p5), isHighPriority, false);
    }

	template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5, typename Param6>
	long post(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, Param6 p6, bool isHighPriority)
	{
		return postItem(new Functor6<TClass, Func, Param1, Param2, Param3, Param4, Param5, Param6>(obj, func, p1, p2, p3, p4, p5, p6), isHighPriority, false);
	}

    /**
     * Post a member function into run loop. The member function will run in
     * run loop thread by order of post time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param p5 fifth parameter.
     * @param p6 fifth parameter.
     * @param p7 fifth parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5, typename Param6, typename Param7>
    long post(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, Param6 p6, Param7 p7, bool isHighPriority)
    {
        return postItem(new Functor7<TClass, Func, Param1, Param2, Param3, Param4, Param5, Param6, Param7>(obj, func, p1, p2, p3, p4, p5, p6, p7), isHighPriority, false);
    }

	template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5, typename Param6, typename Param7, typename Param8>
	long post(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, Param6 p6, Param7 p7, Param8 p8, bool isHighPriority)
	{
		return postItem(new Functor8<TClass, Func, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8>(obj, func, p1, p2, p3, p4, p5, p6, p7, p8), isHighPriority, false);
	}

    /*
     * Send a member function into run loop. The member function will run in
     * run loop thread by order of send time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func>
    long send(TClass *obj, Func func, bool isHighPriority)
    {
        return postItem(new Functor0<TClass, Func>(obj, func), isHighPriority, true);
    }

    /**
     * Send a member function into run loop. The member function will run in
     * run loop thread by order of send time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1>
    long send(TClass *obj, Func func, Param1 p1, bool isHighPriority)
    {
        return postItem(new Functor1<TClass, Func, Param1>(obj, func, p1), isHighPriority, true);
    }

    /**
     * Send a member function into run loop. The member function will run in
     * run loop thread by order of send time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2>
    long send(TClass *obj, Func func, Param1 p1, Param2 p2, bool isHighPriority)
    {
        return postItem(new Functor2<TClass, Func, Param1, Param2>(obj, func, p1, p2), isHighPriority, true);
    }

    /**
     * Send a member function into run loop. The member function will run in
     * run loop thread by order of send time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3>
    long send(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, bool isHighPriority)
    {
        return postItem(new Functor3<TClass, Func, Param1, Param2, Param3>(obj, func, p1, p2, p3), isHighPriority, true);
    }

    /**
     * Send a member function into run loop. The member function will run in
     * run loop thread by order of send time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4>
    long send(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, bool isHighPriority)
    {
        return postItem(new Functor4<TClass, Func, Param1, Param2, Param3, Param4>(obj, func, p1, p2, p3, p4), isHighPriority, true);
    }

    /**
     * Send a member function into run loop. The member function will run in
     * run loop thread by order of send time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param p5 fifth parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5>
    long send(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, bool isHighPriority)
    {
        return postItem(new Functor5<TClass, Func, Param1, Param2, Param3, Param4, Param5>(obj, func, p1, p2, p3, p4, p5), isHighPriority, true);
    }

    /**
     * Send a member function into run loop. The member function will run in
     * run loop thread by order of send time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param p5 fifth parameter.
     * @param p6 sixth parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5, typename Param6>
    long send(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, Param6 p6, bool isHighPriority)
    {
        return postItem(new Functor6<TClass, Func, Param1, Param2, Param3, Param4, Param5, Param6>(obj, func, p1, p2, p3, p4, p5, p6), isHighPriority, true);
    }

    /**
     * Send a member function into run loop. The member function will run in
     * run loop thread by order of send time.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param p5 fifth parameter.
     * @param p6 sixth parameter.
     * @param p7 sixth parameter.
     * @param isHighPriority whether is high priority function. High priority
     * function will be ran earlier than normal priority ones. All high priority
     * functions will be ran in order.
     * @param syncWait whether to wait synchronuously until the function is
     * completed.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5, typename Param6, typename Param7>
    long send(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, Param6 p6, Param7 p7, bool isHighPriority)
    {
        return postItem(new Functor7<TClass, Func, Param1, Param2, Param3, Param4, Param5, Param6, Param7>(obj, func, p1, p2, p3, p4, p5, p6, p7), isHighPriority, true);
    }

    /**
    * Send a member function into run loop. The member function will run in
    * run loop thread by order of send time.
    * @param obj object of member function.
    * @param func function pointer.
    * @param p1 first parameter.
    * @param p2 second parameter.
    * @param p3 third parameter.
    * @param p4 fourth parameter.
    * @param p5 fifth parameter.
    * @param p6 sixth parameter.
    * @param p7 seventh parameter.
    # @param p8 eighth parameter.
    * @param isHighPriority whether is high priority function. High priority
    * function will be ran earlier than normal priority ones. All high priority
    * functions will be ran in order.
    * @param syncWait whether to wait synchronuously until the function is
    * completed.
    */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5, typename Param6, typename Param7, typename Param8>
    long send(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, Param6 p6, Param7 p7, Param8 p8, bool isHighPriority)
    {
        return postItem(new Functor8<TClass, Func, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8>(obj, func, p1, p2, p3, p4, p5, p6, p7, p8), isHighPriority, true);
    }


    /**
    * Send a member function into run loop. The member function will run in
    * run loop thread by order of send time.
    * @param obj object of member function.
    * @param func function pointer.
    * @param p1 first parameter.
    * @param p2 second parameter.
    * @param p3 third parameter.
    * @param p4 fourth parameter.
    * @param p5 fifth parameter.
    * @param p6 sixth parameter.
    * @param p7 seventh parameter.
    # @param p8 eighth parameter.
    # @param p9 eighth parameter.
    * @param isHighPriority whether is high priority function. High priority
    * function will be ran earlier than normal priority ones. All high priority
    * functions will be ran in order.
    * @param syncWait whether to wait synchronuously until the function is
    * completed.
    */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5, typename Param6, typename Param7, typename Param8, typename Param9>
    long send(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, Param6 p6, Param7 p7, Param8 p8, Param9 p9, bool isHighPriority)
    {
        return postItem(new Functor9<TClass, Func, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9>(obj, func, p1, p2, p3, p4, p5, p6, p7, p8, p9), isHighPriority, true);
    }
    /**
     * Start a timer for a member function into run loop. The member function
     * will run in run loop thread when timer is expired.
     * @param obj object of member function.
     * @param func function pointer.
     * @param msecInterval timer interval.
     * @param repeat whether timer is repeatable.
     * @return timer ID.
     */
    template<typename TClass, typename Func>
    long startTimer(TClass *obj, Func func, unsigned int msecInterval, bool repeat)
    {
        return addTimer(new Functor0<TClass, Func>(obj, func), msecInterval, repeat);
    }

    /**
     * Start a timer for a member function into run loop. The member function
     * will run in run loop thread when timer is expired.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param msecInterval timer interval.
     * @param repeat whether timer is repeatable.
     * @return timer ID.
     */
    template<typename TClass, typename Func, typename Param1>
    long startTimer(TClass *obj, Func func, Param1 p1, unsigned int msecInterval, bool repeat)
    {
        return addTimer(new Functor1<TClass, Func, Param1>(obj, func, p1), msecInterval, repeat);
    }

    /**
     * Start a timer for a member function into run loop. The member function
     * will run in run loop thread when timer is expired.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param msecInterval timer interval.
     * @param repeat whether timer is repeatable.
     * @return timer ID.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2>
    long startTimer(TClass *obj, Func func, Param1 p1, Param2 p2, unsigned int msecInterval, bool repeat)
    {
        return addTimer(new Functor2<TClass, Func, Param1, Param2>(obj, func, p1, p2), msecInterval, repeat);
    }

    /**
     * Start a timer for a member function into run loop. The member function
     * will run in run loop thread when timer is expired.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param msecInterval timer interval.
     * @param repeat whether timer is repeatable.
     * @return timer ID.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3>
    long startTimer(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, unsigned int msecInterval, bool repeat)
    {
        return addTimer(new Functor3<TClass, Func, Param1, Param2, Param3>(obj, func, p1, p2, p3), msecInterval, repeat);
    }

    /**
     * Start a timer for a member function into run loop. The member function
     * will run in run loop thread when timer is expired.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param msecInterval timer interval.
     * @param repeat whether timer is repeatable.
     * @return timer ID.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4>
    long startTimer(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, unsigned int msecInterval, bool repeat)
    {
        return addTimer(new Functor4<TClass, Func, Param1, Param2, Param3, Param4>(obj, func, p1, p2, p3, p4), msecInterval, repeat);
    }

    /**
     * Start a timer for a member function into run loop. The member function
     * will run in run loop thread when timer is expired.
     * @param obj object of member function.
     * @param func function pointer.
     * @param p1 first parameter.
     * @param p2 second parameter.
     * @param p3 third parameter.
     * @param p4 fourth parameter.
     * @param p5 fifth parameter.
     * @param msecInterval timer interval.
     * @param repeat whether timer is repeatable.
     * @return timer ID.
     */
    template<typename TClass, typename Func, typename Param1, typename Param2, typename Param3, typename Param4, typename Param5>
    long startTimer(TClass *obj, Func func, Param1 p1, Param2 p2, Param3 p3, Param4 p4, Param5 p5, unsigned int msecInterval, bool repeat)
    {
        return addTimer(new Functor5<TClass, Func, Param1, Param2, Param3, Param4, Param5>(obj, func, p1, p2, p3, p4, p5), msecInterval, repeat);
    }

    /**
     * Stop a timer.
     * @param timer ID.
     */
    void stopTimer(long timerID);

    /**
     * Reset a timer.
     * @param timer ID.
     */
    void resetTimer(long timerID);

    /**
     * Remove tasks for a sepcific object from taskLoop pening task list
     * @param object the specific object wants pending functors to be removed
     */
    void cancelTasksForObject(void *object);

    /**
     * Cancel tasks for a specific task ID list
     * @param taskList the task ID list to be canceled
     * @return the removed task list.
     */
    std::list<long> cancelTasks(std::list<long> taskList);

    /**
     * get the total task number in the taskLoop queue.
     * @return total task number in taskLoop queue
     */
    int getTaskNum();

    /**
     * get the total timer number in the taskLoop queue.
     * @return total timer number in taskLoop queue
     */
    int getTimerNum();

    /**
     * get the run loop status.
     * @return the running status
     */
    bool isRunning();

    /**
     * dump all task info in the queue
     */
    void dumpTaskInfo();

    /**
     * Set flag indicating whether print log in destructor.
     * Mainly for Windows platform since log callback is provided by C# function.
     */
    void setPrintLog(bool printLog);

    /**
     * Set the auto warning param. if task queue size or timer queue size exceed limit, log will print out
     */
    void setQueueMonitorParam(uint32_t taskWarningSize, uint32_t timerWarningSize);

    std::string getName();
    long postItem(Runnable *runnable, bool isHighPriority, bool waitUntilDone, long timerId = 0);
    void printTimeOutTask(int time, std::string task);

    long getThreadID();
private:
    class TaskLoopItem
    {
    public:
        TaskLoopItem() : timerId(0), taskID(0), waitUntilDone(false) {}

        TaskLoopItem(Runnable *_runnable, long tId) 
        : runnable(_runnable),
        timerId(tId), 
        taskID(0), 
        semaphore(NULL),
        waitUntilDone(false) {}

        TaskLoopItem(const TaskLoopItem& obj)
        : runnable(obj.runnable),
        timerId(obj.timerId),
        taskID(obj.taskID),
        semaphore(obj.semaphore),
        waitUntilDone(obj.waitUntilDone) {}

        TaskLoopItem(TaskLoopItem&& obj)
        : runnable(std::move(obj.runnable)),
        timerId(obj.timerId),
        taskID(obj.taskID),
        semaphore(std::move(obj.semaphore)),
        waitUntilDone(obj.waitUntilDone)
        {
            obj.timerId = 0;
            obj.taskID = 0;
            obj.waitUntilDone = 0;
        }

        TaskLoopItem& operator=(const TaskLoopItem& obj)
        {
            runnable = obj.runnable;
            timerId = obj.timerId;
            taskID = obj.taskID;
            semaphore = obj.semaphore;
            waitUntilDone = obj.waitUntilDone;
            return *this;
        }

        TaskLoopItem& operator=(TaskLoopItem&& obj)
        {
            runnable = std::move(obj.runnable);
            timerId = obj.timerId;
            obj.timerId = 0;
            taskID = obj.taskID;
            obj.taskID = 0;
            semaphore = std::move(obj.semaphore);
            waitUntilDone = obj.waitUntilDone;
            obj.waitUntilDone = false;
            return *this;
        }

        const char *getRunnableInfo()
        {
            return runnable->getClassInfo();
        }

        SharedPtr<Runnable> runnable;
        long timerId;
        long taskID;
        SharedPtr<Semaphore> semaphore;
		bool waitUntilDone;
    };

    void taskLoopProc();
    long addTimer(Runnable *runnable, unsigned int msecInterval, bool repeat);
    void cancelTimer(long timerId);
    Mutex _lock;
    Mutex _runningLock;
    std::string _name;
    Thread _thread;
    Semaphore _itemsSem;
    std::list<TaskLoopItem> _items;
    std::list<TaskLoopItem> _highPriorityItems;
    std::list<TaskLoopItem> _sendItems;
    bool _hasRunningItem;
    TaskLoopItem _runningItem;
    uint32_t _nextTaskID;

    bool _printLog;
    uint32_t _taskWarningSize;
    uint32_t _timerWarningSize;

private:
	void delSendItems(Runnable *runnable);
};

}
