#pragma once

namespace il2cpp
{
namespace os
{
namespace Image
{
    void Initialize();
    void* GetImageBase();
#if IL2CPP_PLATFORM_SUPPORTS_CUSTOM_SECTIONS
    bool IsInManagedSection(void*ip);
#endif
}
}
}
