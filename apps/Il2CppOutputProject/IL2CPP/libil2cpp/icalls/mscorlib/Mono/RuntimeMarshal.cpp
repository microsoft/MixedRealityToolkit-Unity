#if NET_4_0

#include "il2cpp-config.h"
#include "RuntimeMarshal.h"
#include "mono-structs.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace Mono
{
    void RuntimeMarshal::FreeAssemblyName(Il2CppMonoAssemblyName* name, bool freeStruct)
    {
        IL2CPP_FREE(const_cast<char*>(name->name));
        IL2CPP_FREE(const_cast<char*>(name->culture));
        IL2CPP_FREE(const_cast<char*>(name->hash_value));
        IL2CPP_FREE(const_cast<uint8_t*>(name->public_key));
        if (freeStruct)
            IL2CPP_FREE(name);
    }
} // namespace Mono
} // namespace mscorlib
} // namespace icalls
} // namespace il2cpp

#endif
