#pragma once

#include <stdint.h>
#include "il2cpp-config.h"
struct Il2CppObject;
struct Il2CppReflectionType;

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    class LIBIL2CPP_CODEGEN_API Activator
    {
    public:
        static Il2CppObject* CreateInstanceInternal(Il2CppReflectionType *type);
    };
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
