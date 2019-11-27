#include "os/c-api/il2cpp-config-platforms.h"

#if !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "os/Locale.h"
#include "Allocator.h"

#include <string>

extern "C"
{
    void UnityPalLocaleInitialize()
    {
        il2cpp::os::Locale::Initialize();
    }

    void UnityPalLocaleUnInitialize()
    {
        il2cpp::os::Locale::UnInitialize();
    }

    char* UnityPalGetLocale()
    {
        return Allocator::CopyToAllocatedStringBuffer(il2cpp::os::Locale::GetLocale());
    }
}

#endif
