#pragma once

#include <stdint.h>
#include "il2cpp-config.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    class LIBIL2CPP_CODEGEN_API Char
    {
    public:
        static void GetDataTablePointers(
            const unsigned char** category_data,
            const unsigned char** numeric_data,
            const double** numeric_data_values,
            const Il2CppChar** to_lower_data_low,
            const Il2CppChar** to_lower_data_high,
            const Il2CppChar** to_upper_data_low,
            const Il2CppChar** to_upper_data_high);
    };
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
