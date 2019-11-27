#include "os/c-api/il2cpp-config-platforms.h"

#if !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "os/Console.h"
#include "os/c-api/Console-c-api.h"

extern "C"
{
    int32_t UnityPalConsoleInternalKeyAvailable(int32_t ms_timeout)
    {
        return il2cpp::os::Console::InternalKeyAvailable(ms_timeout);
    }

    int32_t UnityPalConsoleSetBreak(int32_t wantBreak)
    {
        return il2cpp::os::Console::SetBreak(wantBreak);
    }

    int32_t UnityPalConsoleSetEcho(int32_t wantEcho)
    {
        return il2cpp::os::Console::SetEcho(wantEcho);
    }

    int32_t UnityPalConsoleTtySetup(const char* keypadXmit, const char* teardown, uint8_t* control_characters, int32_t** size)
    {
        return il2cpp::os::Console::TtySetup(keypadXmit, teardown, control_characters, size);
    }
}

#endif
