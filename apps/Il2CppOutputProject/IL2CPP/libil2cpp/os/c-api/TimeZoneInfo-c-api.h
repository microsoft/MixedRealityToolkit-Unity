#pragma once

#include <stdint.h>
#include "il2cpp-config-platforms.h"
#include "il2cpp-config-api-platforms.h"

#if defined(__cplusplus)
extern "C"
{
#endif

IL2CPP_EXPORT int UseUnityPalForTimeZoneInformation();
IL2CPP_EXPORT void* UnityPalTimeZoneInfoGetTimeZoneIDs();
IL2CPP_EXPORT int UnityPalGetLocalTimeZoneData(void** nativeRawData, char** nativeID, int* size);
IL2CPP_EXPORT int UnityPalGetTimeZoneDataForID(char* id, void** nativeRawData, int* size);

#if defined(__cplusplus)
}
#endif
