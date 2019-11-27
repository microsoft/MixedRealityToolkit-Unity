#pragma once

#include "il2cpp-config.h"
#include "utils/StringView.h"

struct PInvokeArguments;

namespace il2cpp
{
namespace vm
{
    class LIBIL2CPP_CODEGEN_API LibraryLoader
    {
    public:
        static void* LoadDynamicLibrary(il2cpp::utils::StringView<Il2CppNativeChar> nativeDynamicLibrary);
        static void SetFindPluginCallback(Il2CppSetFindPlugInCallback method);
    };
} /* namespace vm */
} /* namespace il2cpp*/
