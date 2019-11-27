#pragma once

#if IL2CPP_THREADS_PTHREAD && !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "PosixWaitObject.h"
#include "os/ErrorCodes.h"
#include "os/WaitStatus.h"

#include <stdint.h>
#include <semaphore.h>

namespace il2cpp
{
namespace os
{
    class SemaphoreImpl : public posix::PosixWaitObject
    {
    public:
        SemaphoreImpl(int32_t initialValue, int32_t maximumValue);

        bool Post(int32_t releaseCount, int32_t* previousCount);

    protected:
        uint32_t m_MaximumValue;
    };
}
}

#endif
