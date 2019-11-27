#pragma once

#include <stdint.h>
#include "il2cpp-config.h"
#include "il2cpp-object-internals.h"

struct mscorlib_System_Runtime_InteropServices_DllImportAttribute;
struct mscorlib_System_Reflection_MethodInfo;

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
    class LIBIL2CPP_CODEGEN_API MonoMethod
    {
    public:
        static mscorlib_System_Runtime_InteropServices_DllImportAttribute * GetDllImportAttribute(intptr_t);
        static Il2CppArray * GetGenericArguments(Il2CppReflectionMethod *);
        static Il2CppReflectionMethod* GetGenericMethodDefinition_impl(Il2CppReflectionMethod* method);
        static Il2CppObject * InternalInvoke(Il2CppReflectionMethod * method, Il2CppObject * thisPtr, Il2CppArray * params, Il2CppException * * exc);
        static bool get_IsGenericMethod(Il2CppReflectionMethod *);
        static bool get_IsGenericMethodDefinition(Il2CppReflectionMethod *);
        static Il2CppReflectionMethod* get_base_definition(Il2CppReflectionMethod *);
        static Il2CppString *  get_name(Il2CppReflectionMethod * m);
        static Il2CppReflectionMethod* MakeGenericMethod_impl(Il2CppReflectionMethod *, Il2CppArray *);

#if NET_4_0
        static int32_t get_core_clr_security_level(Il2CppObject* _this);
        static Il2CppReflectionMethod* get_base_method(Il2CppReflectionMethod* method, bool definition);
        static void GetPInvoke(Il2CppReflectionMethod* _this, int32_t* flags, Il2CppString** entryPoint, Il2CppString** dllName);
#endif
    };
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
