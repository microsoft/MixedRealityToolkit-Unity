#pragma once

// Assumes clang or gcc as compiler.
#if IL2CPP_TARGET_POSIX && !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "os/c-api/Atomic-c-api.h"

namespace il2cpp
{
namespace os
{
    inline void Atomic::FullMemoryBarrier()
    {
        __sync_synchronize();
    }
}
}

#endif
