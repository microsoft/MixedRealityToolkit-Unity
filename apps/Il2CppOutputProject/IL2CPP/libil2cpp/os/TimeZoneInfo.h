#pragma once

namespace il2cpp
{
namespace os
{
    class TimeZoneInfo
    {
    public:
        static bool UsePalForTimeZoneInfo();
        static void* GetTimeZoneIDs();
        static bool GetLocalTimeZoneData(void** nativeRawData, char** nativeID, int* size);
        static bool GetTimeZoneDataForID(char* id, void** nativeRawData, int* size);
    };
}
}
