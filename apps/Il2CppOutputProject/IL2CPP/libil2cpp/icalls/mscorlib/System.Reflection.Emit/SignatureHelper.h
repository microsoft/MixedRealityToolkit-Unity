#pragma once

#include "il2cpp-config.h"
struct Il2CppArray;
struct Il2CppReflectionSigHelper;

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
namespace Reflection
{
namespace Emit
{
    class LIBIL2CPP_CODEGEN_API SignatureHelper
    {
    public:
        static Il2CppArray* get_signature_field(Il2CppReflectionSigHelper*);
        static Il2CppArray* get_signature_local(Il2CppReflectionSigHelper*);
    };
} /* namespace Emit */
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
