#include "il2cpp-config.h"

#include "icalls/System/System.Diagnostics/PerformanceCounterCategory.h"
#include "PerformanceCounterUtils.h"
#include "utils/StringUtils.h"
#include "vm/Array.h"
#include "vm/String.h"
#include "vm/Exception.h"
#include "vm-utils/VmStringUtils.h"

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
    bool PerformanceCounterCategory::CategoryDelete(Il2CppString* name)
    {
        NOT_SUPPORTED_IL2CPP(PerformanceCounterCategory::Create, "The IL2CPP scripting backend does not support the removal of custom performance counter categories.");

        return false;
    }

    Il2CppString* PerformanceCounterCategory::CategoryHelpInternal(Il2CppString* category, Il2CppString* machine)
    {
        if (!utils::VmStringUtils::CaseInsensitiveEquals(machine, "."))
            return NULL;
        const CategoryDesc* cdesc = find_category(category);
        if (!cdesc)
            return NULL;
        return vm::String::New(cdesc->help);
    }

    bool PerformanceCounterCategory::CounterCategoryExists(Il2CppString* counter, Il2CppString* category, Il2CppString* machine)
    {
        if (!utils::VmStringUtils::CaseInsensitiveEquals(machine, "."))
            return false;
        const CategoryDesc* cdesc = find_category(category);
        if (!cdesc)
            return false;

        /* counter is allowed to be null */
        if (!counter)
            return true;
        if (get_counter_in_category(cdesc, counter))
            return true;
        return false;
    }

    bool PerformanceCounterCategory::Create(Il2CppString* categoryName, Il2CppString* categoryHelp, PerformanceCounterCategoryType categoryType, Il2CppArray* items)
    {
        NOT_SUPPORTED_IL2CPP(PerformanceCounterCategory::Create, "The IL2CPP scripting backend does not support the creation of custom performance counter categories.");

        return false;
    }

    int32_t PerformanceCounterCategory::InstanceExistsInternal(Il2CppString* instance, Il2CppString* category, Il2CppString* machine)
    {
        return 0;
    }

    Il2CppArray* PerformanceCounterCategory::GetCategoryNames(Il2CppString* machine)
    {
        if (!utils::VmStringUtils::CaseInsensitiveEquals(machine, "."))
            return vm::Array::New(il2cpp_defaults.string_class, 0);

        Il2CppArray* res = vm::Array::New(il2cpp_defaults.string_class, NUM_CATEGORIES);
        for (int i = 0; i < NUM_CATEGORIES; ++i)
        {
            const CategoryDesc *cdesc = &predef_categories[i];
            il2cpp_array_setref(res, i, vm::String::New(cdesc->name));
        }

        return res;
    }

    Il2CppArray* PerformanceCounterCategory::GetCounterNames(Il2CppString* category, Il2CppString* machine)
    {
        if (!utils::VmStringUtils::CaseInsensitiveEquals(machine, "."))
            return vm::Array::New(il2cpp_defaults.string_class, 0);

        const CategoryDesc* cdesc = find_category(category);
        if (cdesc)
        {
            Il2CppArray* res = vm::Array::New(il2cpp_defaults.string_class, cdesc[1].first_counter - cdesc->first_counter);
            for (int i = cdesc->first_counter; i < cdesc[1].first_counter; ++i)
            {
                const CounterDesc *desc = &predef_counters[i];
                il2cpp_array_setref(res, i - cdesc->first_counter, vm::String::New(desc->name));
            }
            return res;
        }

        return vm::Array::New(il2cpp_defaults.string_class, 0);
    }

    Il2CppArray* PerformanceCounterCategory::GetInstanceNames(Il2CppString* category, Il2CppString* machine)
    {
        return vm::Array::New(il2cpp_defaults.string_class, 0);
    }
} /* namespace Diagnostics */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
