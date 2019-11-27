#include "os/c-api/il2cpp-config-platforms.h"

#if !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "os/Time.h"

#include <stdint.h>

extern "C"
{
    uint32_t UnityPalGetTicksMillisecondsMonotonic()
    {
        return il2cpp::os::Time::GetTicksMillisecondsMonotonic();
    }

    int64_t UnityPalGetTicks100NanosecondsMonotonic()
    {
        return il2cpp::os::Time::GetTicks100NanosecondsMonotonic();
    }

    int64_t UnityPalGetTicks100NanosecondsDateTime()
    {
        return il2cpp::os::Time::GetTicks100NanosecondsDateTime();
    }

    int64_t UnityPalGetSystemTimeAsFileTime()
    {
        return il2cpp::os::Time::GetSystemTimeAsFileTime();
    }
}

#endif
