#pragma once

#include "il2cpp-config.h"
#include "os/Mutex.h"

namespace il2cpp
{
namespace vm
{
    extern il2cpp::os::FastMutex g_MetadataLock;
} // namespace vm
} // namespace il2cpp
