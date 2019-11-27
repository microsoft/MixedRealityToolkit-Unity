#include "os/c-api/il2cpp-config-platforms.h"

#if !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "Allocator.h"
#include "os/Messages.h"
#include "os/c-api/Messages-c-api.h"

extern "C"
{
    char* UnityPalMessagesFromCode(UnityPalErrorCode code)
    {
        return Allocator::CopyToAllocatedStringBuffer(il2cpp::os::Messages::FromCode(code));
    }
}

#endif
