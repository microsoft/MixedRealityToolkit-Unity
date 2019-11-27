#pragma once

#include "il2cpp-config.h"
struct Il2CppReflectionProperty;
struct Il2CppPropertyInfo;
struct Il2CppArray;

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
    typedef enum
    {
        PInfo_Attributes = 1,
        PInfo_GetMethod  = 1 << 1,
        PInfo_SetMethod  = 1 << 2,
        PInfo_ReflectedType = 1 << 3,
        PInfo_DeclaringType = 1 << 4,
        PInfo_Name = 1 << 5
    } PInfo;

    class LIBIL2CPP_CODEGEN_API MonoPropertyInfo
    {
    public:
        static Il2CppArray* GetTypeModifiers(void* /* System.Reflection.MonoProperty */ prop, bool optional);
        static void get_property_info(Il2CppReflectionProperty *property, Il2CppPropertyInfo *info, PInfo req_info);

#if NET_4_0
        static Il2CppObject* get_default_value(Il2CppReflectionProperty* prop);
#endif
    };
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
