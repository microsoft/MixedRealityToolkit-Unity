#pragma once

#include "il2cpp-class-internals.h"

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
    struct CategoryDesc
    {
        const char *name;
        const char *help;
        unsigned char id;
        signed int type : 2;
        unsigned int instance_type : 6;
        short first_counter;
    };

#define PERFCTR_CAT(id, name, help, type, inst, first_counter) CATEGORY_ ## id,
#define PERFCTR_COUNTER(id, name, help, type, field)
    enum
    {
#include "perfcounters-def.h"
        NUM_CATEGORIES
    };

#undef PERFCTR_CAT
#undef PERFCTR_COUNTER
#define PERFCTR_CAT(id, name, help, type, inst, first_counter) CATEGORY_START_ ## id = -1,
#define PERFCTR_COUNTER(id, name, help, type, field) COUNTER_ ## id,
/* each counter is assigned an id starting from 0 inside the category */
    enum
    {
#include "perfcounters-def.h"
        END_COUNTERS
    };

#undef PERFCTR_CAT
#undef PERFCTR_COUNTER
#define PERFCTR_CAT(id, name, help, type, inst, first_counter)
#define PERFCTR_COUNTER(id, name, help, type, field) CCOUNTER_ ## id,
/* this is used just to count the number of counters */
    enum
    {
#include "perfcounters-def.h"
        NUM_COUNTERS
    };

    struct CounterDesc
    {
        const char *name;
        const char *help;
        short id;
        unsigned short offset; // offset inside Il2CppPerfCounters
        int type;
    };

    enum
    {
        FTYPE_CATEGORY = 'C',
        FTYPE_DELETED = 'D',
        FTYPE_PREDEF_INSTANCE = 'P', // an instance of a predef counter
        FTYPE_INSTANCE = 'I',
        FTYPE_DIRTY = 'd',
        FTYPE_END = 0
    };

    struct SharedHeader
    {
        unsigned char ftype;
        unsigned char extra;
        unsigned short size;
    };

    struct SharedCategory
    {
        SharedHeader header;
        unsigned short num_counters;
        unsigned short counters_data_size;
        int num_instances;
        /* variable length data follows */
        char name[1];
        // string name
        // string help
        // SharedCounter counters_info [num_counters]
    };

    struct SharedInstance
    {
        SharedHeader header;
        size_t category_offset;
        /* variable length data follows */
        char instance_name[1];
        // string name
    };

    struct SharedCounter
    {
        unsigned char type;
        uint8_t seq_num;
        /* variable length data follows */
        char name[1];
        // string name
        // string help
    };

    struct CatSearch
    {
        Il2CppString *name;
        SharedCategory *cat;
    };

/* map of PerformanceCounterType.cs */
    enum
    {
        NumberOfItemsHEX32 = 0x00000000,
        NumberOfItemsHEX64 = 0x00000100,
        NumberOfItems32 = 0x00010000,
        NumberOfItems64 = 0x00010100,
        CounterDelta32 = 0x00400400,
        CounterDelta64 = 0x00400500,
        SampleCounter = 0x00410400,
        CountPerTimeInterval32 = 0x00450400,
        CountPerTimeInterval64 = 0x00450500,
        RateOfCountsPerSecond32 = 0x10410400,
        RateOfCountsPerSecond64 = 0x10410500,
        RawFraction = 0x20020400,
        CounterTimer = 0x20410500,
        Timer100Ns = 0x20510500,
        SampleFraction = 0x20C20400,
        CounterTimerInverse = 0x21410500,
        Timer100NsInverse = 0x21510500,
        CounterMultiTimer = 0x22410500,
        CounterMultiTimer100Ns = 0x22510500,
        CounterMultiTimerInverse = 0x23410500,
        CounterMultiTimer100NsInverse = 0x23510500,
        AverageTimer32 = 0x30020400,
        ElapsedTime = 0x30240500,
        AverageCount64 = 0x40020500,
        SampleBase = 0x40030401,
        AverageBase = 0x40030402,
        RawBase = 0x40030403,
        CounterMultiBase = 0x42030500
    };

/* maps a small integer type to the counter types above */
    static const int simple_type_to_type[] =
    {
        NumberOfItemsHEX32, NumberOfItemsHEX64,
        NumberOfItems32, NumberOfItems64,
        CounterDelta32, CounterDelta64,
        SampleCounter, CountPerTimeInterval32,
        CountPerTimeInterval64, RateOfCountsPerSecond32,
        RateOfCountsPerSecond64, RawFraction,
        CounterTimer, Timer100Ns,
        SampleFraction, CounterTimerInverse,
        Timer100NsInverse, CounterMultiTimer,
        CounterMultiTimer100Ns, CounterMultiTimerInverse,
        CounterMultiTimer100NsInverse, AverageTimer32,
        ElapsedTime, AverageCount64,
        SampleBase, AverageBase,
        RawBase, CounterMultiBase
    };

    enum
    {
        SingleInstance,
        MultiInstance,
        CatTypeUnknown = -1
    };

    enum
    {
        ProcessInstance,
        ThreadInstance,
        CPUInstance,
        MonoInstance,
        NetworkInterfaceInstance,
        CustomInstance
    };

/* map of CounterSample.cs */
    struct Il2CppCounterSample
    {
        int64_t rawValue;
        int64_t baseValue;
        int64_t counterFrequency;
        int64_t systemFrequency;
        int64_t timeStamp;
        int64_t timeStamp100nSec;
        int64_t counterTimeStamp;
        int counterType;
    };

#undef PERFCTR_CAT
#undef PERFCTR_COUNTER
#define PERFCTR_CAT(id, name, help, type, inst, first_counter) {name, help, CATEGORY_ ## id, type, inst ## Instance, CCOUNTER_ ## first_counter},
#define PERFCTR_COUNTER(id, name, help, type, field)
    static const CategoryDesc
        predef_categories[] =
    {
        /* sample runtime counter */
#include "perfcounters-def.h"
        { NULL, NULL, NUM_CATEGORIES, -1, 0, NUM_COUNTERS }
    };

#undef PERFCTR_CAT
#undef PERFCTR_COUNTER
#define PERFCTR_CAT(id, name, help, type, inst, first_counter)
#define PERFCTR_COUNTER(id, name, help, type, field) {name, help, COUNTER_ ## id, offsetof (Il2CppPerfCounters, field), type},
    static const CounterDesc
        predef_counters[] =
    {
#include "perfcounters-def.h"
        { NULL, NULL, -1, 0, 0 }
    };
/*
* We have several different classes of counters:
* *) system counters
* *) runtime counters
* *) remote counters
* *) user-defined counters
* *) windows counters (the implementation on windows will use this)
*
* To easily handle the differences we create a vtable for each class that contains the
* function pointers with the actual implementation to access the counters.
*/
    typedef struct _ImplVtable ImplVtable;

    typedef bool(*SampleFunc) (ImplVtable *vtable, bool only_value, Il2CppCounterSample* sample);
    typedef uint64_t(*UpdateFunc) (ImplVtable *vtable, bool do_incr, int64_t value);
    typedef void(*CleanupFunc) (ImplVtable *vtable);

    struct _ImplVtable
    {
        void *arg;
        SampleFunc sample;
        UpdateFunc update;
        CleanupFunc cleanup;
    };

    struct CustomVTable
    {
        ImplVtable vtable;
        SharedInstance *instance_desc;
        SharedCounter *counter_desc;
    };

    const CategoryDesc* find_category(Il2CppString *category);
    const CounterDesc* get_counter_in_category(const CategoryDesc *desc, Il2CppString *counter);
    ImplVtable* create_vtable(void *arg, SampleFunc sample, UpdateFunc update);
} /* namespace Diagnostics */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
