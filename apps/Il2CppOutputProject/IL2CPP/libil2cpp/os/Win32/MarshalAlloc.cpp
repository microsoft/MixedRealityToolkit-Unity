#include "il2cpp-config.h"

#if IL2CPP_TARGET_WINDOWS

#include "os/MarshalAlloc.h"

#include "WindowsHeaders.h"
#include "Objbase.h"

namespace il2cpp
{
namespace os
{
    void* MarshalAlloc::Allocate(size_t size)
    {
        return ::CoTaskMemAlloc(size);
    }

    void* MarshalAlloc::ReAlloc(void* ptr, size_t size)
    {
        return ::CoTaskMemRealloc(ptr, size);
    }

    void MarshalAlloc::Free(void* ptr)
    {
        ::CoTaskMemFree(ptr);
    }
} /* namespace os */
} /* namespace il2cpp*/

#endif
