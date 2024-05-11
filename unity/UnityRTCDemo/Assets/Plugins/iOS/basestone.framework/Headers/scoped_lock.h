#pragma once

#if defined(USE_CPP11_STL)
#include "mutex.h"
#endif
#include "common_defs.h"

namespace LJ
{

#if !defined(USE_CPP11_STL)
    class Mutex;

/**
 * ScopedLock is the class to lock and unlock mutex object automatically.
 * Mutex object will be locked with ScopedLock object is constructed, and
 * will be unlocked with it is destructed.
 */
    class BASESTONE_EXTERN ScopedLock
    {
    public:
        /**
         * Constructor.
         * @param lock Mutex object that will be locked.
         */
        ScopedLock(Mutex &lock);

        /**
         * Destructor.
         * Mutex object will be unlocked in destructor.
         */
        virtual ~ScopedLock();

    private:
        Mutex &_lock;
    };

#else

    class ScopedLock : public std::unique_lock<std::recursive_mutex>
{
public:
    ScopedLock(Mutex& mtx)
        : std::unique_lock<std::recursive_mutex>(mtx)
    {
    }
};

#endif

}
