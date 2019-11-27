#pragma once

#if NET_4_0

struct Il2CppString;

namespace il2cpp
{
namespace icalls
{
namespace System
{
namespace Mono
{
namespace Net
{
namespace Security
{
    class LIBIL2CPP_CODEGEN_API MonoTlsProviderFactory
    {
    public:
        static bool IsBtlsSupported();
        static Il2CppString* GetDefaultProviderForPlatform();
    };
} // namespace Security
} // namespace Net
} // namespace Mono
} // namespace System
} // namespace icalls
} // namespace il2cpp

#endif
