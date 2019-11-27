#include "il2cpp-config.h"
#include "il2cpp-class-internals.h"
#include "Il2CppTypeHash.h"
#include "Il2CppSignatureHash.h"
#include "utils/HashUtils.h"

using il2cpp::utils::HashUtils;

namespace il2cpp
{
namespace metadata
{
    size_t Il2CppSignatureHash::operator()(const il2cpp::utils::dynamic_array<const Il2CppType*>& signature) const
    {
        return Hash(signature);
    }

    size_t Il2CppSignatureHash::Hash(const il2cpp::utils::dynamic_array<const Il2CppType*>& signature)
    {
        il2cpp::utils::dynamic_array<const Il2CppType*>::const_iterator iter, end = signature.end();
        size_t retVal = 0;

        for (iter = signature.begin(); iter != end; ++iter)
            retVal = HashUtils::Combine(retVal, Il2CppTypeHash::Hash(*iter));

        return retVal;
    }
} /* namespace metadata */
} /* namespace il2cpp */
