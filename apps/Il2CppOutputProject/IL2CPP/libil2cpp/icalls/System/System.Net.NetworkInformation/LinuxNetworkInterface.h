#pragma once

#if !NET_4_0

#include "il2cpp-object-internals.h"

namespace il2cpp
{
namespace icalls
{
namespace System
{
namespace System
{
namespace Net
{
namespace NetworkInformation
{
    class LIBIL2CPP_CODEGEN_API LinuxNetworkInterface
    {
    public:
        static int32_t GetInterfaceAddresses(intptr_t* ifap);
        static void FreeInterfaceAddresses(intptr_t ifap);
        static void InitializeInterfaceAddresses();
    };
} // namespace NetworkInformation
} // namespace Net
} // namespace System
} // namespace System
} // namespace icalls
} // namespace il2cpp

#endif
