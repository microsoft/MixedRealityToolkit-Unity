#pragma once

namespace il2cpp
{
namespace os
{
    class MarshalAlloc
    {
    public:
        static void* Allocate(size_t size);
        static void* ReAlloc(void* ptr, size_t size);
        static void Free(void* ptr);
    };
} /* namespace os */
} /* namespace il2cpp*/
