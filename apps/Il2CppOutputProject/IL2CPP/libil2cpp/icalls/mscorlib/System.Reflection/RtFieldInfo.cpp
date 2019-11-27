#if NET_4_0
#include "il2cpp-config.h"
#include "RtFieldInfo.h"
#include "icalls/mscorlib/System.Reflection/MonoField.h"

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
    Il2CppObject* RtFieldInfo::UnsafeGetValue(Il2CppReflectionField* _this, Il2CppObject* obj)
    {
        // In mono's icall-def.h file, this maps to the same icall as MonoField.GetValueInternal
        // so our implementation will do the same
        return MonoField::GetValueInternal(_this, obj);
    }
} // namespace Reflection
} // namespace System
} // namespace mscorlib
} // namespace icalls
} // namespace il2cpp
#endif
