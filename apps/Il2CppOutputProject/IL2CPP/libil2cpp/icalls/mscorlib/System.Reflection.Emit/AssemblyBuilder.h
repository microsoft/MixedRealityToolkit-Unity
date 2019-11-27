#pragma once

#include "il2cpp-config.h"
struct Il2CppString;
struct mscorlib_System_Reflection_Emit_AssemblyBuilder;
struct mscorlib_System_Reflection_Module;

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
namespace Emit
{
    class LIBIL2CPP_CODEGEN_API AssemblyBuilder
    {
    public:
        static void basic_init(mscorlib_System_Reflection_Emit_AssemblyBuilder*);
        static mscorlib_System_Reflection_Module* InternalAddModule(mscorlib_System_Reflection_Emit_AssemblyBuilder * thisPtr, Il2CppString* fileName);
        static void UpdateNativeCustomAttributes40(mscorlib_System_Reflection_Emit_AssemblyBuilder * thisPtr);
    };
} /* namespace Emit */
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
