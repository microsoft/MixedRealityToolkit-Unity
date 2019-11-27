#pragma once

#include "il2cpp-config.h"
struct Il2CppArray;
struct mscorlib_System_Reflection_MonoGenericClass;

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
namespace Reflection
{
    class LIBIL2CPP_CODEGEN_API MonoGenericClass
    {
    public:
        static void initialize(mscorlib_System_Reflection_MonoGenericClass * thisPtr, Il2CppArray* methods, Il2CppArray* ctors, Il2CppArray* fields, Il2CppArray* properties, Il2CppArray* events);
#if NET_4_0
        static void initialize40(Il2CppObject* _this, Il2CppArray* fields);
        static void register_with_runtime(Il2CppObject* type);
#endif
    };
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
