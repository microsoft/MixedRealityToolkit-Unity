#include "il2cpp-config.h"

#if IL2CPP_TARGET_ANDROID

#include "os/Initialize.h"
#include "utils/Logging.h"

#include <android/log.h>

static void AndroidLogCallback(const char* message)
{
    __android_log_print(ANDROID_LOG_INFO, "IL2CPP", "%s", message);
}

void il2cpp::os::Initialize()
{
    if (!utils::Logging::IsLogCallbackSet())
        utils::Logging::SetLogCallback(AndroidLogCallback);
}

void il2cpp::os::Uninitialize()
{
}

#endif
