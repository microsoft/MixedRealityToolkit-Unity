#pragma once

#include "utils/dynamic_array.h"

struct Il2CppType;

namespace il2cpp
{
namespace metadata
{
    struct Il2CppSignatureCompare
    {
        bool operator()(const il2cpp::utils::dynamic_array<const Il2CppType*>& s1, const il2cpp::utils::dynamic_array<const Il2CppType*>& s2) const;
        static bool Equals(const il2cpp::utils::dynamic_array<const Il2CppType*>& s1, const il2cpp::utils::dynamic_array<const Il2CppType*>& s2);
    };
} /* namespace metadata */
} /* namespace il2cpp */
