#include "il2cpp-config.h"
#include "il2cpp-class-internals.h"
#include "Il2CppGenericClassCompare.h"
#include "Il2CppGenericInstCompare.h"

namespace il2cpp
{
namespace metadata
{
    bool Il2CppGenericClassCompare::operator()(const Il2CppGenericClass* gc1, const Il2CppGenericClass* gc2) const
    {
        return Compare(gc1, gc2);
    }

    bool Il2CppGenericClassCompare::Compare(const Il2CppGenericClass* gc1, const Il2CppGenericClass* gc2)
    {
        if (gc1->typeDefinitionIndex != gc2->typeDefinitionIndex)
            return false;

        return Il2CppGenericInstCompare::Compare(gc1->context.class_inst, gc2->context.class_inst);
    }
} /* namespace vm */
} /* namespace il2cpp */
