#include "os/c-api/il2cpp-config-platforms.h"

#if !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "os/Error.h"
#include "os/c-api/Error-c-api.h"

extern "C"
{
    UnityPalErrorCode UnityPalGetLastError()
    {
        return il2cpp::os::Error::GetLastError();
    }

    void UnityPalSetLastError(UnityPalErrorCode code)
    {
        return il2cpp::os::Error::SetLastError(code);
    }

    int32_t UnityPalSuccess(UnityPalErrorCode code)
    {
        return (int32_t)(code == il2cpp::os::kErrorCodeSuccess);
    }
}

#endif
