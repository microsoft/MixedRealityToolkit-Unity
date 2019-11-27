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
namespace System
{
namespace System
{
namespace Diagnostics
{
    typedef int32_t PerformanceCounterCategoryType;

    class LIBIL2CPP_CODEGEN_API PerformanceCounterCategory
    {
    public:
        static Il2CppString* CategoryHelpInternal(Il2CppString* category, Il2CppString* machine);
        static bool CounterCategoryExists(Il2CppString* counter, Il2CppString* category, Il2CppString* machine);
        static bool Create(Il2CppString* categoryName, Il2CppString* categoryHelp, PerformanceCounterCategoryType categoryType, Il2CppArray* items);
        static Il2CppArray* GetCategoryNames(Il2CppString* machine);
        static Il2CppArray* GetCounterNames(Il2CppString* category, Il2CppString* machine);
        static Il2CppArray* GetInstanceNames(Il2CppString* category, Il2CppString* machine);
        static int32_t InstanceExistsInternal(Il2CppString* instance, Il2CppString* category, Il2CppString* machine);
        static bool CategoryDelete(Il2CppString* name);
    };
} /* namespace Diagnostics */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
