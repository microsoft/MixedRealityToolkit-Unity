#pragma once

#include "NonCopyable.h"
#include "../os/Atomic.h"
#include "../os/Mutex.h"

namespace il2cpp
{
namespace utils
{
    typedef void (*CallOnceFunc) (void* arg);

    struct OnceFlag : NonCopyable
    {
        OnceFlag() : m_Flag(NULL)
        {
        }

        friend void CallOnce(OnceFlag& flag, CallOnceFunc func, void* arg);

        bool IsSet()
        {
            return il2cpp::os::Atomic::ReadPointer(&m_Flag) ? true : false;
        }

    private:
        void* m_Flag;
        il2cpp::os::FastMutex m_Mutex;
    };

    inline void CallOnce(OnceFlag& flag, CallOnceFunc func, void* arg)
    {
        if (!il2cpp::os::Atomic::ReadPointer(&flag.m_Flag))
        {
            os::FastAutoLock lock(&flag.m_Mutex);
            if (!il2cpp::os::Atomic::ReadPointer(&flag.m_Flag))
            {
                func(arg);
                il2cpp::os::Atomic::ExchangePointer(&flag.m_Flag, (void*)1);
            }
        }
    }
}
}
