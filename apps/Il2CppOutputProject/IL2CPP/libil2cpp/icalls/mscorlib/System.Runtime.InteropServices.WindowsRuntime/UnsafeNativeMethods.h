#pragma once

#if NET_4_0

#include "il2cpp-object-internals.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
namespace Runtime
{
namespace InteropServices
{
namespace WindowsRuntime
{
    class LIBIL2CPP_CODEGEN_API UnsafeNativeMethods
    {
    public:
        static bool RoOriginateLanguageException(int32_t error, Il2CppString* message, intptr_t languageException);
        static Il2CppChar* WindowsGetStringRawBuffer(intptr_t hstring, uint32_t* length);
        static int32_t WindowsCreateString(Il2CppString* sourceString, int32_t length, intptr_t* hstring);
        static int32_t WindowsDeleteString(intptr_t hstring);
        static Il2CppObject* GetRestrictedErrorInfo();
        static void RoReportUnhandledError(Il2CppObject* error);
    };
} // namespace WindowsRuntime
} // namespace InteropServices
} // namespace Runtime
} // namespace System
} // namespace mscorlib
} // namespace icalls
} // namespace il2cpp

#endif
