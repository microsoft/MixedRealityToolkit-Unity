#include "il2cpp-config.h"

#if !IL2CPP_DOTS_WITHOUT_DEBUGGER

#include "NativeDelegateMethodCache.h"

namespace il2cpp
{
namespace utils
{
    il2cpp::os::FastMutex NativeDelegateMethodCache::m_CacheMutex;
    NativeDelegateMap NativeDelegateMethodCache::m_NativeDelegateMethods;

    const VmMethod* NativeDelegateMethodCache::GetNativeDelegate(Il2CppMethodPointer nativeFunctionPointer)
    {
        os::FastAutoLock lock(&m_CacheMutex);

        NativeDelegateMap::iterator i = m_NativeDelegateMethods.find(nativeFunctionPointer);
        if (i == m_NativeDelegateMethods.end())
            return NULL;

        return i->second;
    }

    void NativeDelegateMethodCache::AddNativeDelegate(Il2CppMethodPointer nativeFunctionPointer, const VmMethod* managedMethodInfo)
    {
        os::FastAutoLock lock(&m_CacheMutex);
        m_NativeDelegateMethods.insert(std::make_pair(nativeFunctionPointer, managedMethodInfo));
    }
} // namespace utils
} // namespace il2cpp

#endif
