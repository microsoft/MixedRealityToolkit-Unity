#include "il2cpp-config.h"

#if IL2CPP_TARGET_JAVASCRIPT || IL2CPP_TARGET_LINUX || IL2CPP_TARGET_LUMIN || IL2CPP_TARGET_ANDROID && !IL2CPP_DOTS_WITHOUT_DEBUGGER

#if IL2CPP_TARGET_JAVASCRIPT
#include <emscripten/emscripten.h>
#else
#include <dlfcn.h>
#endif

extern char __start_il2cpp;
extern char __stop_il2cpp;

namespace il2cpp
{
namespace os
{
namespace Image
{
    static char* s_ManagedSectionStart = NULL;
    static char* s_ManagedSectionEnd = NULL;

    void* GetImageBase()
    {
#if IL2CPP_TARGET_JAVASCRIPT
        emscripten_log(EM_LOG_NO_PATHS | EM_LOG_CONSOLE | EM_LOG_ERROR | EM_LOG_JS_STACK, "Warning: libil2cpp/os/Posix/Image.cpp: GetImageBase() called, but dynamic libraries are not available.");
        return NULL;
#else
        Dl_info info;
        void* const anySymbol = reinterpret_cast<void*>(&GetImageBase);
        if (dladdr(anySymbol, &info))
            return info.dli_fbase;
        else
            return NULL;
#endif
    }

    static IL2CPP_METHOD_ATTR void NoGeneratedCodeWorkaround()
    {
    }

    void InitializeManagedSection()
    {
        NoGeneratedCodeWorkaround();
        s_ManagedSectionStart = &__start_il2cpp;
        s_ManagedSectionEnd = &__stop_il2cpp;
    }

    void Initialize()
    {
#if IL2CPP_PLATFORM_SUPPORTS_CUSTOM_SECTIONS
        InitializeManagedSection();
#endif
    }

#if IL2CPP_PLATFORM_SUPPORTS_CUSTOM_SECTIONS
    bool IsInManagedSection(void* ip)
    {
        IL2CPP_ASSERT(s_ManagedSectionStart != NULL && s_ManagedSectionEnd != NULL);
        return s_ManagedSectionStart <= ip && ip <= s_ManagedSectionEnd;
    }

#endif
}
}
}

#endif
