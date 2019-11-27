#if ENABLE_UNIT_TESTS

#include "il2cpp-config.h"
#include "UnitTest++.h"
#include "os/c-api/TimeZoneInfo-c-api.h"
#include "utils/Memory.h"

SUITE(TimeZoneInfo)
{
#if IL2CPP_PLATFORM_SUPPORTS_TIMEZONEINFO
    TEST(TimeZoneInfo_UnityPalGetLocalTimeZoneData)
    {
        void* tzdata;
        char* id;
        int size = 0;
        bool success = UnityPalGetLocalTimeZoneData(&tzdata, &id, &size);
        CHECK(success);
        CHECK_NOT_NULL(tzdata);
        CHECK_NOT_NULL(id);
        CHECK(size > 0);
        IL2CPP_FREE(id);
        IL2CPP_FREE(tzdata);
    }

    TEST(TimeZoneInfo_UnityPalGetLocalTimeZoneData_NameHasSaneLength)
    {
        void* tzdata;
        char* id;
        int size = 0;
        bool success = UnityPalGetLocalTimeZoneData(&tzdata, &id, &size);
        CHECK(strlen(id) >= 3 && strlen(id) <= 100);
        CHECK_NOT_NULL(id);

        IL2CPP_FREE(id);
        IL2CPP_FREE(tzdata);
    }
#endif
}

#endif // ENABLE_UNIT_TESTS
