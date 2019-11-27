#include "il2cpp-config.h"
#if !IL2CPP_DOTS || IL2CPP_DOTS_DEBUGGER
#include <il2cpp-object-internals.h>
#include "Il2CppHStringReference.h"
#include "StringView.h"
#include "vm/WindowsRuntime.h"

namespace il2cpp
{
namespace utils
{
    Il2CppHStringReference::Il2CppHStringReference(const StringView<Il2CppNativeChar>& str)
    {
        il2cpp::vm::WindowsRuntime::CreateHStringReference(str, &m_Header, &m_String);
    }
}
}

#endif
