#if !NET_4_0

#include "il2cpp-config.h"
#include "LinuxNetworkInterface.h"
#include "../external/xamarin-android/xamarin_getifaddrs.h"

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
    int32_t LinuxNetworkInterface::GetInterfaceAddresses(intptr_t* ifap)
    {
        return _monodroid_getifaddrs(reinterpret_cast<_monodroid_ifaddrs**>(ifap));
    }

    void LinuxNetworkInterface::FreeInterfaceAddresses(intptr_t ifap)
    {
        _monodroid_freeifaddrs(reinterpret_cast<_monodroid_ifaddrs*>(ifap));
    }

    void LinuxNetworkInterface::InitializeInterfaceAddresses()
    {
        _monodroid_getifaddrs_init();
    }
} // namespace NetworkInformation
} // namespace Net
} // namespace System
} // namespace System
} // namespace icalls
} // namespace il2cpp

#endif
