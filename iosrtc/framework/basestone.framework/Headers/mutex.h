#pragma once

#if !defined(USE_CPP11_STL)
#ifdef WIN32
#include "./private/windows/mutex_impl.h"
#else
#include "./private/common/mutex_impl_posix.h"
#endif
#else
#include <mutex>
#include "util.h"
#endif
#include "common_defs.h"

namespace LJ
{

#if !defined(USE_CPP11_STL)

/**
 * Mutex object for lock and unlock.
 */
    class BASESTONE_EXTERN Mutex : public MutexImpl
    {
    public:
        /**
         * Lock object.
         * @return true if successfully.
         */
        bool lock();

        /**
     * Lock object.
     * @return true if successfully.
     */
        bool tryLock();
        /**
         * Unlock object.
         * @return true if successfully.
         */
        bool unlock();

        /**
         * Default constructor.
         */
        Mutex();

        /**
         * Destructor.
         */
        virtual ~Mutex();

    };

#else

    class Mutex : public std::recursive_mutex
{
};

#endif

}