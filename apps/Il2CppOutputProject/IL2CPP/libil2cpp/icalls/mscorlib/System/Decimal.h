#pragma once

#include <stdint.h>
#include "il2cpp-config.h"

struct Il2CppString;
struct mscorlib_System_Decimal;

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    class LIBIL2CPP_CODEGEN_API Decimal
    {
    public:
#if !NET_4_0
        static int32_t decimal2Int64(il2cpp_decimal_repr* val, int64_t* result);
        static int32_t decimal2UInt64(il2cpp_decimal_repr* val, uint64_t* result);
        static int32_t decimal2string(il2cpp_decimal_repr* val, int32_t digits, int32_t decimals, Il2CppArray* bufDigits, int32_t bufSize, int32_t* decPos, int32_t* sign);
        static int decimalCompare(il2cpp_decimal_repr *pA, il2cpp_decimal_repr *pB);
        static int32_t decimalDiv(il2cpp_decimal_repr* pC, il2cpp_decimal_repr* pA, il2cpp_decimal_repr* pB);
        static void decimalFloorAndTrunc(il2cpp_decimal_repr * pA, int32_t floorFlag);
        static int32_t decimalIncr(il2cpp_decimal_repr * d1, il2cpp_decimal_repr * d2);
        static int32_t decimalIntDiv(il2cpp_decimal_repr* pc, il2cpp_decimal_repr* pa, il2cpp_decimal_repr* pb);
        static int32_t decimalMult(il2cpp_decimal_repr* pA, il2cpp_decimal_repr* pB);
        static int decimalSetExponent(il2cpp_decimal_repr*, int);
        static int string2decimal(il2cpp_decimal_repr *pA, Il2CppString *str, unsigned int decrDecimal, int sign);
        static double decimal2double(il2cpp_decimal_repr* val);
#else
        static double ToDouble(Il2CppDecimal d);
        static int32_t FCallCompare(Il2CppDecimal* left, Il2CppDecimal* right);
        static int32_t FCallToInt32(Il2CppDecimal d);
        static int32_t GetHashCode(Il2CppDecimal* _this);
        static float ToSingle(Il2CppDecimal d);
        static void ConstructorDouble(Il2CppDecimal* _this, double value);
        static void ConstructorFloat(Il2CppDecimal* _this, float value);
        static void FCallAddSub(Il2CppDecimal* left, Il2CppDecimal* right, uint8_t sign);
        static void FCallDivide(Il2CppDecimal* left, Il2CppDecimal* right);
        static void FCallFloor(Il2CppDecimal* d);
        static void FCallMultiply(Il2CppDecimal* d1, Il2CppDecimal* d2);
        static void FCallRound(Il2CppDecimal* d, int32_t decimals);
        static void FCallTruncate(Il2CppDecimal* d);
#endif
    };
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
