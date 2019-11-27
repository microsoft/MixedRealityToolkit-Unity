#pragma once

#include "il2cpp-config.h"

namespace il2cpp
{
namespace utils
{
    class LIBIL2CPP_CODEGEN_API RegisterRuntimeInitializeAndCleanup
    {
    public:
        typedef void (*CallbackFunction) ();
        RegisterRuntimeInitializeAndCleanup(CallbackFunction Initialize, CallbackFunction Cleanup, int order = 0);

        static void ExecuteInitializations();
        static void ExecuteCleanup();
    };
} /* namespace vm */
} /* namespace utils */
