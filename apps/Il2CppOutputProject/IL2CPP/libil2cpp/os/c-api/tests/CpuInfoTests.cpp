#if ENABLE_UNIT_TESTS
#include "os/c-api/il2cpp-config-platforms.h"
#if NET_4_0

#include "UnitTest++.h"

#include "../CpuInfo-c-api.h"
#include "../../CpuInfo.h"

#if IL2CPP_PLATFORM_SUPPORTS_CPU_INFO

SUITE(CpuInfo)
{
    TEST(ApiCreateReturnsANonNullObject)
    {
        CHECK_NOT_NULL(UnityPalCpuInfoCreate());
    }

    TEST(ApiCpuInfoUsageMatchesClassCpuInfo)
    {
        CHECK_EQUAL(il2cpp::os::CpuInfo::Usage(NULL), UnityPalCpuInfoUsage(NULL));
    }
}

#endif // IL2CPP_PLATFORM_SUPPORTS_CPU_INFO

#endif // NET_4_0
#endif // ENABLE_UNIT_TESTS
