#pragma once

#include <stdint.h>
#include "il2cpp-config.h"
#include "il2cpp-object-internals.h"

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
namespace mscorlib
{
namespace Mono
{
namespace Globalization
{
namespace Unicode
{
    class LIBIL2CPP_CODEGEN_API Normalization
    {
    public:
        static void load_normalization_resource(intptr_t* argProps, intptr_t* argMappedChars, intptr_t* argCharMapIndex, intptr_t* argHelperIndex, intptr_t* argMapIdxToComposite, intptr_t* argCombiningClass);
    };
} /* namespace Unicode */
} /* namespace Globalization */
} /* namespace Mono */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
