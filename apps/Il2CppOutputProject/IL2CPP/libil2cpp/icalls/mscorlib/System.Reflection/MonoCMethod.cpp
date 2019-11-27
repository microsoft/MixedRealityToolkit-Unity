#include "il2cpp-config.h"

#include "icalls/mscorlib/System.Reflection/MonoCMethod.h"
#include "icalls/mscorlib/System.Reflection/MonoMethod.h"
#include "vm/Exception.h"

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
    Il2CppObject* MonoCMethod::InternalInvoke(Il2CppReflectionMethod* self, Il2CppObject* obj, Il2CppArray* parameters, Il2CppException** exc)
    {
        return MonoMethod::InternalInvoke(self, obj, parameters, exc);
    }

#if NET_4_0
    int32_t MonoCMethod::get_core_clr_security_level(Il2CppObject* _this)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(MonoCMethod::get_core_clr_security_level);
        IL2CPP_UNREACHABLE;
        return 0;
    }

#endif
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
