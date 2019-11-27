#pragma once

#include <stdint.h>

#if defined(__cplusplus)
extern "C"
{
#endif

uint32_t UnityPalGetTicksMillisecondsMonotonic();
int64_t UnityPalGetTicks100NanosecondsMonotonic();
int64_t UnityPalGetTicks100NanosecondsDateTime();
int64_t UnityPalGetSystemTimeAsFileTime();

#if defined(__cplusplus)
}
#endif
