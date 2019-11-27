#pragma once

#include <stdint.h>
#include <vector>
#include <string>
#include "il2cpp-config.h"
struct Il2CppAssemblyName;
struct Il2CppReflectionAssemblyName;
struct Il2CppMonoAssemblyName;

namespace il2cpp
{
namespace vm
{
    class LIBIL2CPP_CODEGEN_API AssemblyName
    {
// exported
    public:
        static std::string AssemblyNameToString(const Il2CppAssemblyName& aname);
        static bool ParseName(Il2CppReflectionAssemblyName* aname, std::string assemblyName);
#if NET_4_0
        static void FillNativeAssemblyName(const Il2CppAssemblyName& aname, Il2CppMonoAssemblyName* nativeName);
#endif
    private:
    };
} /* namespace vm */
} /* namespace il2cpp */
