#pragma once

#include "il2cpp-config.h"
struct Il2CppReflectionEvent;
struct Il2CppReflectionEventBuilder;
struct Il2CppReflectionType;
struct Il2CppReflectionTypeBuilder;

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
    class LIBIL2CPP_CODEGEN_API TypeBuilder
    {
    public:
        static void create_generic_class(Il2CppReflectionTypeBuilder*);
        static void create_internal_class(Il2CppReflectionTypeBuilder*);
        static Il2CppReflectionType* create_runtime_class(Il2CppReflectionTypeBuilder*, Il2CppReflectionTypeBuilder*);
        static bool get_IsGenericParameter(Il2CppReflectionTypeBuilder*);
        static Il2CppReflectionEvent* get_event_info(Il2CppReflectionTypeBuilder*, Il2CppReflectionEventBuilder*);
        static void setup_generic_class(Il2CppReflectionTypeBuilder*);
        static void setup_internal_class(Il2CppReflectionTypeBuilder*, Il2CppReflectionTypeBuilder*);
        static Il2CppReflectionType* create_runtime_class40(Il2CppReflectionTypeBuilder*);
    };
} /* namespace Emit */
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
