#pragma once

#include <list>
#include "scoped_lock.h"
#include "common_defs.h"
#include "mutex.h"
#include "semaphore1.h"

namespace LJ
{

/**
 * BlockingQueue class is a thread-safe queue. BlockingQueue provides blocking
 * getter method to get objects from queue.
 */
template <class T>
class BlockingQueue
{
public:

    /**
     * Default constructor.
     */
    BlockingQueue() :
        _queue(),
        _queueSemaphore()
    {
    }

    /**
     * Destructor.
     */
    virtual ~BlockingQueue()
    {
    }

    /**
     * Return the size of queue.
     * @return size of queue.
     */
    unsigned int size()
    {
        ScopedLock autoLock(_lock);
        return _queue.size();
    }	
	
    /**
     * Push an item into queue. Item will be copied into queue by copy
     * constructor.
     * @param item item to be pushed into queue.
     */
    void push(const T &item)
    {
        ScopedLock autoLock(_lock);
        _queue.push_back(item);

        _queueSemaphore.notify();
    }

    void push(T&& item)
    {
        ScopedLock autoLock(_lock);
        _queue.push_back(std::move(item));

        _queueSemaphore.notify();
    }

    /**
     * Get the front item from queue. The item will also be poped out of queue
     * by this method.
     * @param out the front item will be copied into out object.
     * @return true if got item successfully.
     */
    bool get(T &out)
    {
        int ret = _queueSemaphore.wait();

        if (ret != RETURN_OK)
        {
            return false;
        }

        ScopedLock autoLock(_lock);

        if (_queue.empty())
        {
            return false;
        }

        out = std::move(_queue.front());
        _queue.pop_front();
        return true;
    }

    /**
     * Get the front item from queue within time. The item will also be poped
     * out of queue by this method.
     * @param msec waiting time for getting item in milliseconds.
     * @param out the front item will be copied into out object.
     * @return true if got item successfully.
     */
    bool get(unsigned int msec, T &out)
    {
        int ret = _queueSemaphore.wait(msec);

        if (ret != RETURN_OK)
        {
            return false;
        }

        ScopedLock autoLock(_lock);

        if (_queue.empty())
        {
            return false;
        }

        out = std::move(_queue.front());
        _queue.pop_front();
        return true;
    }


private:
    std::list<T> _queue;
    Mutex _lock;
    Semaphore _queueSemaphore;
};

}
