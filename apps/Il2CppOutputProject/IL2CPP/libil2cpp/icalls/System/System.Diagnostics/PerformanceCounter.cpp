#include "il2cpp-config.h"

#include "icalls/System/System.Diagnostics/PerformanceCounter.h"
#include "PerformanceCounterUtils.h"
#include "os/Time.h"
#include "utils/Memory.h"
#include "utils/StringUtils.h"
#include "vm/Exception.h"
#include "vm-utils/VmStringUtils.h"
#include "il2cpp-runtime-stats.h"

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
    static void fill_sample(Il2CppCounterSample *sample)
    {
        sample->timeStamp = os::Time::GetTicksMillisecondsMonotonic();
        sample->timeStamp100nSec = sample->timeStamp;
        sample->counterTimeStamp = sample->timeStamp;
        sample->counterFrequency = 10000000;
        sample->systemFrequency = 10000000;
        // the real basevalue needs to be get from a different counter...
        sample->baseValue = 0;
    }

    static bool Il2CppMemoryCounter(ImplVtable *vtable, bool only_value, Il2CppCounterSample *sample)
    {
        int id = (int)(intptr_t)vtable->arg;
        if (!only_value)
        {
            fill_sample(sample);
            sample->baseValue = 1;
        }
        sample->counterType = predef_counters[predef_categories[CATEGORY_MONO_MEM].first_counter + id].type;
        switch (id)
        {
            case COUNTER_MEM_NUM_OBJECTS:
                sample->rawValue = il2cpp_runtime_stats.new_object_count;
                return true;
        }
        return false;
    }

    static void* Il2CppMemoryCounterImpl(Il2CppString* counter, Il2CppString* instance, int* type, bool* custom)
    {
        *custom = false;
        const CounterDesc *cdesc = get_counter_in_category(&predef_categories[CATEGORY_MONO_MEM], counter);
        if (cdesc)
        {
            *type = cdesc->type;
            return create_vtable((void*)(intptr_t)cdesc->id, Il2CppMemoryCounter, NULL);
        }
        return NULL;
    }

    intptr_t PerformanceCounter::GetImpl(Il2CppString* category, Il2CppString* counter, Il2CppString* instance, Il2CppString* machine, int* type, bool* custom)
    {
        intptr_t returnValue = {0};
        const CategoryDesc *cdesc;
        if (!utils::VmStringUtils::CaseInsensitiveEquals(machine, "."))
            return returnValue;

        cdesc = find_category(category);
        if (!cdesc)
        {
            return returnValue;
        }
        else
        {
            switch (cdesc->id)
            {
                case CATEGORY_MONO_MEM:
                    returnValue = reinterpret_cast<intptr_t>(Il2CppMemoryCounterImpl(counter, instance, type, custom));
            }
        }

        return returnValue;
    }

    bool PerformanceCounter::GetSample(intptr_t impl, bool only_value, Il2CppCounterSample* sample)
    {
        ImplVtable *vtable = (ImplVtable*)impl;
        if (vtable && vtable->sample)
            return vtable->sample(vtable, only_value, sample);
        return false;
    }

    int64_t PerformanceCounter::UpdateValue(intptr_t impl, bool do_incr, int64_t value)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(PerformanceCounter::UpdateValue);

        return 0;
    }

    void PerformanceCounter::FreeData(intptr_t impl)
    {
        ImplVtable* vtable = (ImplVtable*)impl;
        if (vtable && vtable->cleanup)
            vtable->cleanup(vtable);
        IL2CPP_FREE(vtable);
    }
} /* namespace Diagnostics */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
