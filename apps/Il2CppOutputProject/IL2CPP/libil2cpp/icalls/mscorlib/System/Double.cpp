#include "il2cpp-config.h"
#include <stdlib.h>
#include "icalls/mscorlib/System/Double.h"
#include "os/Locale.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    bool Double::ParseImpl(char *ptr, double *result)
    {
        char *endptr = NULL;
        *result = 0.0;

        if (*ptr)
        {
#if IL2CPP_SUPPORT_LOCALE_INDEPENDENT_PARSING
            *result = il2cpp::os::Locale::DoubleParseLocaleIndependentImpl(ptr, &endptr);
#else
            *result = strtod(ptr, &endptr);
#endif
        }

        if (!*ptr || (endptr && *endptr))
            return false;

        return true;
    }
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
