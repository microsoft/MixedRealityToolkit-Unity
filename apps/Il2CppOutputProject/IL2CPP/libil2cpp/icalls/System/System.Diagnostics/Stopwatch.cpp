#include "il2cpp-config.h"
#include "icalls/System/System.Diagnostics/Stopwatch.h"
#include "os/Time.h"

namespace il2cpp
{
namespace icalls
{
namespace System
{
namespace System
{
namespace Diagnostics
{
    int64_t Stopwatch::GetTimestamp()
    {
        return il2cpp::os::Time::GetTicks100NanosecondsMonotonic();
    }
} /* namespace Diagnostics */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */

#if IL2CPP_DOTS

// When the debugger is enabled, we use the big libil2cpp runtime code
// with the dots profile. We need to build this icall for big libil2cpp,
// so direcrtly include the .cpp file to avboid code duplication.
    #include "../libil2cppdots/icalls/mscorlib/System.Diagnostics/Stopwatch.cpp"

#endif
