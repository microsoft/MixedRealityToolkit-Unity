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
namespace System
{
namespace Reflection
{
namespace Emit
{
    class LIBIL2CPP_CODEGEN_API CustomAttributeBuilder
    {
    public:
        static Il2CppArray* GetBlob(Il2CppAssembly* asmb, void* /* System.Reflection.ConstructorInfo */ con, Il2CppArray* constructorArgs, Il2CppArray* namedProperties, Il2CppArray* propertyValues, Il2CppArray* namedFields, Il2CppArray* fieldValues);
    };
} /* namespace Emit */
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
