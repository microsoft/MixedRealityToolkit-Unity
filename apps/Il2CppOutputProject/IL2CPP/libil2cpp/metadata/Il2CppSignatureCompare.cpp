#include "il2cpp-config.h"
#include "il2cpp-class-internals.h"
#include "Il2CppTypeCompare.h"
#include "Il2CppSignatureCompare.h"
#include "utils/KeyWrapper.h"

namespace il2cpp
{
namespace metadata
{
    bool Il2CppSignatureCompare::operator()(const il2cpp::utils::dynamic_array<const Il2CppType*>& s1, const il2cpp::utils::dynamic_array<const Il2CppType*>& s2) const
    {
        return Equals(s1, s2);
    }

    bool Il2CppSignatureCompare::Equals(const il2cpp::utils::dynamic_array<const Il2CppType*>& s1, const il2cpp::utils::dynamic_array<const Il2CppType*>& s2)
    {
        if (s1.size() != s2.size())
            return false;

        il2cpp::utils::dynamic_array<const Il2CppType*>::const_iterator s1Iter, s1End = s1.end(), s2Iter;

        for (s1Iter = s1.begin(), s2Iter = s2.begin(); s1Iter != s1End; ++s1Iter, ++s2Iter)
        {
            if (!Il2CppTypeEqualityComparer::AreEqual(*s1Iter, *s2Iter))
                return false;
        }

        return true;
    }
} /* namespace vm */
} /* namespace il2cpp */
