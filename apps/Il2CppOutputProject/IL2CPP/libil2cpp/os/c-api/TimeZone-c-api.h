#pragma once

#include <stdint.h>

#if defined(__cplusplus)
extern "C"
{
#endif

int32_t UnityPalGetTimeZoneData(int32_t year, int64_t data[4], const char* names[2]);

#if defined(__cplusplus)
}
#endif
