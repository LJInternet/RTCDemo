#pragma once

#if defined(USE_CPP11_STL)
// #define SEMPHORE_USE_CPP11_STL
#endif

#include "common_defs.h"

#if !defined(SEMPHORE_USE_CPP11_STL)
#ifdef WIN32
#include "./private/windows/semaphore_impl.h"
#else
#include "./private/common/semaphore_impl_posix.h"
#endif
#else
#include <mutex>
#include <condition_variable>
#include "util.h"
#endif

namespace LJ
{

#if !defined(SEMPHORE_USE_CPP11_STL)
    class Semaphore : public SemaphoreImpl
#else
        class Semaphore : public NoCopyable
#endif
    {
    public:
        /**
         * Wait for notified permanently.
         * @return RETURN_OK if notified. RETURN_ERROR if error occurs.
         */
#if !defined(SEMPHORE_USE_CPP11_STL)
        int wait();
#else
        int wait()
    {
        std::unique_lock<std::mutex> lock(_mutex);
        while (_count == 0) {
            _cond.wait(lock);
        }
        if (_count > 0) {
            --_count;
        }
        return RETURN_OK;
    }
#endif

        /**
         * Wait for notified during a time.
         * @param msec waiting time in milliseconds.
         * @return RETURN_OK if notified. RETURN_TIMEOUT if time out. RETURN_ERROR
         * if error occurs.
         */
#if !defined(SEMPHORE_USE_CPP11_STL)
        int wait(unsigned int msec);
#else
        int wait(unsigned int msec)
    {
        auto now = std::chrono::high_resolution_clock::now();
        auto until = now + std::chrono::milliseconds(msec);
        std::unique_lock<std::mutex> lock(_mutex);
        while (_count == 0) {
            auto status = _cond.wait_until(lock, until);
            if (status == std::cv_status::timeout) {
                return RETURN_TIMEOUT;
            }
        }
        if (_count > 0) {
            --_count;
        }
        return RETURN_OK;
    }
#endif

        /**
         * Test if notified and return immediately.
         * @return true if notified. false if not yet notified.
         */
        bool try_wait();

        /**
         * Notify object. This function could be called before wait, and the notify
         * count will increase.
         */
#if !defined(SEMPHORE_USE_CPP11_STL)
        void notify();
#else
        void notify()
    {
        std::unique_lock<std::mutex> lock(_mutex);
        ++_count;
        _cond.notify_one();
    }
#endif

        /**
         * Default constructor.
         */
#if !defined(SEMPHORE_USE_CPP11_STL)
        Semaphore();
#else
        Semaphore() : _count(0) {}
#endif

        /**
         * Destructor.
         */
#if !defined(SEMPHORE_USE_CPP11_STL)
        virtual ~Semaphore();
#else
        virtual ~Semaphore() {}
#endif

#if defined(SEMPHORE_USE_CPP11_STL)
        private:
    std::mutex _mutex;
    std::condition_variable _cond;
    unsigned int _count;
#endif
    };

}
