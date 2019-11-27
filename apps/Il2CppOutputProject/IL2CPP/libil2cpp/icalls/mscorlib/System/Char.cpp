#include "il2cpp-config.h"
#include "icalls/mscorlib/System/Char.h"
#include "char-conversions.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    void Char::GetDataTablePointers(
        const unsigned char** category_data,
        const unsigned char** numeric_data,
        const double** numeric_data_values,
        const Il2CppChar** to_lower_data_low,
        const Il2CppChar** to_lower_data_high,
        const Il2CppChar** to_upper_data_low,
        const Il2CppChar** to_upper_data_high)
    {
        *category_data = CategoryData_v4;
        *numeric_data = NumericData;
        *numeric_data_values = NumericDataValues;
        *to_lower_data_low = ToLowerDataLow;
        *to_lower_data_high = ToLowerDataHigh;
        *to_upper_data_low = ToUpperDataLow;
        *to_upper_data_high = ToUpperDataHigh;
    }
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
