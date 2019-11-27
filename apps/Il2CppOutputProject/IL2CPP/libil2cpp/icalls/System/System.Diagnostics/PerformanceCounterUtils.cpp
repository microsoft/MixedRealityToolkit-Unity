#include "il2cpp-config.h"

#include "PerformanceCounterUtils.h"
#include "il2cpp-object-internals.h"
#include "utils/Memory.h"
#include "utils/StringUtils.h"
#include "vm/String.h"
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
    ImplVtable* create_vtable(void *arg, SampleFunc sample, UpdateFunc update)
    {
        ImplVtable* vtable = (ImplVtable*)IL2CPP_MALLOC_ZERO(sizeof(ImplVtable));
        vtable->arg = arg;
        vtable->sample = sample;
        vtable->update = update;
        return vtable;
    }

    const CategoryDesc* find_category(Il2CppString *category)
    {
        for (int i = 0; i < NUM_CATEGORIES; ++i)
        {
            if (utils::VmStringUtils::CaseInsensitiveEquals(category, predef_categories[i].name))
                return &predef_categories[i];
        }
        return NULL;
    }

    const CounterDesc* get_counter_in_category(const CategoryDesc *desc, Il2CppString *counter)
    {
        const CounterDesc *cdesc = &predef_counters[desc->first_counter];
        const CounterDesc *end = &predef_counters[desc[1].first_counter];
        for (; cdesc < end; ++cdesc)
        {
            if (utils::VmStringUtils::CaseInsensitiveEquals(counter, cdesc->name))
                return cdesc;
        }
        return NULL;
    }
} /* namespace Diagnostics */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
