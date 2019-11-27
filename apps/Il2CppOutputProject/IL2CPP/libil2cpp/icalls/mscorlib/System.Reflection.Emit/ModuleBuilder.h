#pragma once

#include <stdint.h>
#include "il2cpp-config.h"
#include "il2cpp-object-internals.h"

struct Il2CppObject;
struct Il2CppArray;
struct Il2CppString;
struct Il2CppReflectionMethod;
struct Il2CppReflectionModuleBuilder;
struct Il2CppReflectionTypeBuilder;
struct Il2CppReflectionType;

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
    class LIBIL2CPP_CODEGEN_API ModuleBuilder
    {
    public:
        static void RegisterToken(Il2CppReflectionModuleBuilder*, Il2CppObject*, int);
        static void WriteToFile(Il2CppReflectionModuleBuilder*, intptr_t);
        static void basic_init(Il2CppReflectionModuleBuilder*);
        static void build_metadata(Il2CppReflectionModuleBuilder*);
        static Il2CppReflectionType * create_modified_type(Il2CppReflectionTypeBuilder*, Il2CppString*);
        static int32_t getToken(Il2CppReflectionModuleBuilder*, Il2CppObject*);
        static int32_t getUSIndex(Il2CppReflectionModuleBuilder*, Il2CppString*);
        static void set_wrappers_type(Il2CppReflectionModuleBuilder*, Il2CppReflectionType*);
        static int32_t getMethodToken(Il2CppReflectionModuleBuilder*, Il2CppReflectionMethod*, Il2CppArray*);
#if NET_4_0
        static int32_t getMethodToken40(Il2CppObject* mb, Il2CppObject* method, Il2CppArray* opt_param_types);
        static int32_t getToken40(Il2CppObject* mb, Il2CppObject* obj, bool create_open_instance);
        static Il2CppObject* GetRegisteredToken(Il2CppObject* _this, int32_t token);
#endif
    };
} /* namespace Emit */
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
