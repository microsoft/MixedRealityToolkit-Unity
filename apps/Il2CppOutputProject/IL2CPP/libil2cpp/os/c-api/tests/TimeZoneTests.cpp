#if !NET_4_0
#if ENABLE_UNIT_TESTS

#include "il2cpp-config.h"

#include "UnitTest++.h"

#include "../TimeZone-c-api.h"
#include "../../TimeZone.h"

SUITE(TimeZone)
{
    static const int32_t INVALID_YEAR = -45;
    static const int32_t VALID_YEAR = 2015;

    struct TimeZoneFixture
    {
        int64_t data_c_api[4];
        const char* names_c_api[2];

        int64_t data_class[4];
        std::string names_class[2];
    };

    TEST_FIXTURE(TimeZoneFixture, ResultMatchesClassResultValidYear)
    {
        CHECK_EQUAL((int32_t)il2cpp::os::TimeZone::GetTimeZoneData(VALID_YEAR, data_class, names_class), UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api));
    }

    TEST_FIXTURE(TimeZoneFixture, ResultMatchesClassResultInValidYear)
    {
        CHECK_EQUAL((int32_t)il2cpp::os::TimeZone::GetTimeZoneData(INVALID_YEAR, data_class, names_class), UnityPalGetTimeZoneData(INVALID_YEAR, data_c_api, names_c_api));
    }

    TEST_FIXTURE(TimeZoneFixture, StartOfDayLightSavingsValidYearMatchesClass)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);
        il2cpp::os::TimeZone::GetTimeZoneData(VALID_YEAR, data_class, names_class);

        CHECK_EQUAL(data_class[0], data_c_api[0]);
    }

    TEST_FIXTURE(TimeZoneFixture, EndOfDayLightSavingsValidYearMatchesClass)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);
        il2cpp::os::TimeZone::GetTimeZoneData(VALID_YEAR, data_class, names_class);

        CHECK_EQUAL(data_class[1], data_c_api[1]);
    }

    TEST_FIXTURE(TimeZoneFixture, UTCOffsetValidYear)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);

        CHECK(data_c_api[2] < 1L);
    }

    TEST_FIXTURE(TimeZoneFixture, UTCOffSetValidYearMatchesClass)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);
        il2cpp::os::TimeZone::GetTimeZoneData(VALID_YEAR, data_class, names_class);

        CHECK_EQUAL(data_class[2], data_c_api[2]);
    }

    TEST_FIXTURE(TimeZoneFixture, AdditionalOffSetValidYearMatchesClass)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);
        il2cpp::os::TimeZone::GetTimeZoneData(VALID_YEAR, data_class, names_class);

        CHECK_EQUAL(data_class[3], data_c_api[3]);
    }

#if !IL2CPP_TARGET_PS4
    TEST_FIXTURE(TimeZoneFixture, NameNotDayLightValidYear)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);

        CHECK(strlen(names_c_api[0]) > 1);
    }

    TEST_FIXTURE(TimeZoneFixture, NameNotDayLightValidYearMatchesClass)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);
        il2cpp::os::TimeZone::GetTimeZoneData(VALID_YEAR, data_class, names_class);

        CHECK_EQUAL(names_class[0].c_str(), names_c_api[0]);
    }

    TEST_FIXTURE(TimeZoneFixture, NameDayLightValidYear)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);

        CHECK(strlen(names_c_api[1]) > 1);
    }

    TEST_FIXTURE(TimeZoneFixture, NameDayLightValidYearMatchesClass)
    {
        UnityPalGetTimeZoneData(VALID_YEAR, data_c_api, names_c_api);
        il2cpp::os::TimeZone::GetTimeZoneData(VALID_YEAR, data_class, names_class);

        CHECK_EQUAL(names_class[1].c_str(), names_c_api[1]);
    }

#endif
}

#endif // ENABLE_UNIT_TESTS
#endif // !4_0
