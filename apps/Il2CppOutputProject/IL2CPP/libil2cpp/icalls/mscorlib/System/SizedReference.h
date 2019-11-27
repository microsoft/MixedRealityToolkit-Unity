#pragma once

#if NET_4_0

#include "il2cpp-object-internals.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    class LIBIL2CPP_CODEGEN_API SizedReference
    {
    public:
        static int64_t GetApproximateSizeOfSizedRef(intptr_t h);
        static intptr_t CreateSizedRef(Il2CppObject* o);
        static Il2CppObject* GetTargetOfSizedRef(intptr_t h);
        static void FreeSizedRef(intptr_t h);
    };
} // namespace System
} // namespace mscorlib
} // namespace icalls
} // namespace il2cpp

#endif
