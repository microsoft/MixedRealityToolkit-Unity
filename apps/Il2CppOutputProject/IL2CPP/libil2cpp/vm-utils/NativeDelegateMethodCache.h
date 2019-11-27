#pragma once

#include "il2cpp-api-types.h"
#include "il2cpp-vm-support.h"
#include "os/Mutex.h"
#include <map>

namespace il2cpp
{
namespace utils
{
    typedef std::map<Il2CppMethodPointer, const VmMethod*> NativeDelegateMap;

    class NativeDelegateMethodCache
    {
    public:
        static const VmMethod* GetNativeDelegate(Il2CppMethodPointer nativeFunctionPointer);
        static void AddNativeDelegate(Il2CppMethodPointer nativeFunctionPointer, const VmMethod* managedMethodInfo);
    private:
        static il2cpp::os::FastMutex m_CacheMutex;
        static NativeDelegateMap m_NativeDelegateMethods;
    };
} // namespace utils
} // namespace il2cpp
