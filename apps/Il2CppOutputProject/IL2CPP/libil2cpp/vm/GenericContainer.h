#pragma once

#include <stdint.h>
#include "il2cpp-config.h"
#include "il2cpp-metadata.h"

struct Il2CppClass;
struct Il2CppGenericContainer;
struct Il2CppGenericParameter;

namespace il2cpp
{
namespace vm
{
    class LIBIL2CPP_CODEGEN_API GenericContainer
    {
    public:
        // exported

    public:
        //internal
        static Il2CppClass* GetDeclaringType(const Il2CppGenericContainer* genericContainer);
        static const Il2CppGenericParameter* GetGenericParameter(const Il2CppGenericContainer* genericContainer, uint16_t index);
    };
} /* namespace vm */
} /* namespace il2cpp */
