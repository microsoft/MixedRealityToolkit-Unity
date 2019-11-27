#pragma once

#include <stdint.h>
#include "il2cpp-config.h"

struct Il2CppArray;
struct Il2CppString;

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    class LIBIL2CPP_CODEGEN_API Convert
    {
    public:
        static Il2CppArray* InternalFromBase64CharArray(Il2CppArray* arr, int32_t offset, int32_t length);
        static Il2CppArray* InternalFromBase64String(Il2CppString* str, bool allowWhitespaceOnly);
        static Il2CppArray* Base64ToByteArray(Il2CppChar* start, int length, bool allowWhitespaceOnly);
    };
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
