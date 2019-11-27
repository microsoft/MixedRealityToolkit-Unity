#include "il2cpp-config.h"
#include "icalls/mscorlib/System/DateTime.h"
#include "os/Time.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    int64_t DateTime::GetNow(void)
    {
        return os::Time::GetTicks100NanosecondsDateTime();
    }

    int64_t DateTime::GetTimeMonotonic()
    {
        return os::Time::GetTicks100NanosecondsMonotonic();
    }

#if NET_4_0
    int64_t DateTime::GetSystemTimeAsFileTime()
    {
        return os::Time::GetSystemTimeAsFileTime();
    }

#endif
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
