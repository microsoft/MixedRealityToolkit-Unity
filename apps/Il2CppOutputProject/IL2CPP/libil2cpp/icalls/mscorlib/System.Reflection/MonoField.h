#pragma once

#include <stdint.h>
#include "il2cpp-config.h"
#include "il2cpp-object-internals.h"

struct mscorlib_System_Reflection_FieldInfo;
struct mscorlib_System_Reflection_Emit_UnmanagedMarshal;

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
    class LIBIL2CPP_CODEGEN_API MonoField
    {
    public:
        static int32_t GetFieldOffset(Il2CppReflectionField * thisPtr);
        static Il2CppReflectionType * GetParentType(Il2CppReflectionField * field, bool declaring);
        static Il2CppObject* GetRawConstantValue(Il2CppReflectionField* field);
        static Il2CppObject * GetValueInternal(Il2CppReflectionField * thisPtr, Il2CppObject * obj);
        static void SetValueInternal(Il2CppReflectionField * fi, Il2CppObject * obj, Il2CppObject * value);

#if NET_4_0
        static int32_t get_core_clr_security_level(Il2CppObject* _this);
        static Il2CppObject* ResolveType(Il2CppObject* _this);
#endif
    };
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
