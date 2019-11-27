#pragma once

#include <stdint.h>
#include "il2cpp-object-internals.h"
#include "il2cpp-config.h"

struct Il2CppObject;
struct Il2CppDelegate;
struct Il2CppReflectionType;
struct Il2CppReflectionMethod;
struct Il2CppReflectionField;
struct Il2CppArray;
struct Il2CppException;
struct Il2CppReflectionModule;
struct Il2CppAssembly;
struct Il2CppAssemblyName;
struct Il2CppAppDomain;

namespace il2cpp
{
namespace icalls
{
namespace System
{
namespace System
{
namespace Configuration
{
    class LIBIL2CPP_CODEGEN_API DefaultConfig
    {
    public:
        static Il2CppString* get_machine_config_path();
        static Il2CppString* get_bundled_machine_config();
    };
} /* namespace Configuration */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
