#pragma once

#if IL2CPP_TARGET_WINDOWS

#include <intrin.h>
#include "c-api/Atomic-c-api.h"

namespace il2cpp
{
namespace os
{
    inline void Atomic::FullMemoryBarrier()
    {
#if defined(_M_X64)
        ::__faststorefence();
#elif defined(_M_IX86)
        //Since we're no longer pulling in windows.h, use inline assembly for the memory barrier
        //https://msdn.microsoft.com/en-us/library/windows/desktop/ms684208%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        long Barrier;
        __asm {
            xchg Barrier, eax
        }
#elif defined(_M_ARM)
        __dmb(_ARM_BARRIER_SY);
#elif defined(_M_ARM64)
        __dmb(_ARM64_BARRIER_SY);
#else
#error Not implemented;
#endif
    }
}
}

#endif
