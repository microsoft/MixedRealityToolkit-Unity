#pragma once
#include "il2cpp-config.h"

#if !NET_4_0

struct Il2CppReflectionType;
struct Il2CppEnumInfo;

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    class LIBIL2CPP_CODEGEN_API MonoEnumInfo
    {
    public:
        static void get_enum_info(Il2CppReflectionType* type, Il2CppEnumInfo* info);
    };
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */

#endif
