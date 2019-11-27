#pragma once

#include "il2cpp-config.h"
#include "il2cpp-pinvoke-support.h"
#include "utils/StringView.h"
#include <string>

namespace il2cpp
{
namespace os
{
    class LibraryLoader
    {
    public:
        static Il2CppMethodPointer GetHardcodedPInvokeDependencyFunctionPointer(const il2cpp::utils::StringView<Il2CppNativeChar>& nativeDynamicLibrary, const il2cpp::utils::StringView<char>& entryPoint);
        static void* LoadDynamicLibrary(const utils::StringView<Il2CppNativeChar>& nativeDynamicLibrary);
        static void* LoadDynamicLibrary(const utils::StringView<Il2CppNativeChar>& nativeDynamicLibrary, int flags);
        static Il2CppMethodPointer GetFunctionPointer(void* dynamicLibrary, const PInvokeArguments& pinvokeArgs);
        static Il2CppMethodPointer GetFunctionPointer(void* dynamicLibrary, const char* functionName);
        static void CleanupLoadedLibraries();
        static bool CloseLoadedLibrary(void*& dynamicLibrary);
    };
} /* namespace os */
} /* namespace il2cpp*/
