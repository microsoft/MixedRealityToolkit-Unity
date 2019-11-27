#include "os/c-api/il2cpp-config-platforms.h"

#if !IL2CPP_PLATFORM_SUPPORTS_TIMEZONEINFO

#include "os/TimeZoneInfo.h"
#include <stddef.h>

namespace il2cpp
{
namespace os
{
    bool TimeZoneInfo::UsePalForTimeZoneInfo()
    {
        return false;
    }

    void* TimeZoneInfo::GetTimeZoneIDs()
    {
        return NULL;
    }

    bool TimeZoneInfo::GetLocalTimeZoneData(void** nativeRawData, char** nativeID, int* size)
    {
        *nativeRawData = NULL;
        *nativeID = NULL;
        *size = 0;
        return false;
    }

    bool TimeZoneInfo::GetTimeZoneDataForID(char* id, void** nativeRawData, int* size)
    {
        *nativeRawData = NULL;
        *size = 0;
        return false;
    }
}
}

#endif
