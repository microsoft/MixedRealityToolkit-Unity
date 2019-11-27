#include "os/c-api/il2cpp-config-platforms.h"

#if !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "os/c-api/TimeZoneInfo-c-api.h"
#include "os/TimeZoneInfo.h"

extern "C"
{
    int UseUnityPalForTimeZoneInformation()
    {
        return il2cpp::os::TimeZoneInfo::UsePalForTimeZoneInfo();
    }

    void* UnityPalTimeZoneInfoGetTimeZoneIDs()
    {
        return il2cpp::os::TimeZoneInfo::GetTimeZoneIDs();
    }

    int UnityPalGetLocalTimeZoneData(void** nativeRawData, char** nativeID, int* size)
    {
        return il2cpp::os::TimeZoneInfo::GetLocalTimeZoneData(nativeRawData, nativeID, size);
    }

    int UnityPalGetTimeZoneDataForID(char* id, void** nativeRawData, int* size)
    {
        return il2cpp::os::TimeZoneInfo::GetTimeZoneDataForID(id, nativeRawData, size);
    }
}

#endif
