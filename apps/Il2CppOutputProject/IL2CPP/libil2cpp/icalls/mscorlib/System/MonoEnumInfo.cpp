#include "il2cpp-config.h"
#include <string>
#include "il2cpp-object-internals.h"
#include "il2cpp-class-internals.h"
#include "icalls/mscorlib/System/MonoEnumInfo.h"
#include "vm/Array.h"
#include "vm/Class.h"
#include "vm/Enum.h"
#include "vm/Field.h"
#include "vm/GenericClass.h"
#include "vm/String.h"
#include "vm/Reflection.h"
#include "vm/Type.h"
#include "utils/MemoryRead.h"

#if !NET_4_0

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    void MonoEnumInfo::get_enum_info(Il2CppReflectionType* type, Il2CppEnumInfo* info)
    {
        IL2CPP_OBJECT_SETREF(info, utype, vm::Reflection::GetTypeObject(vm::Type::GetUnderlyingType(type->type)));
        vm::Enum::GetEnumValuesAndNames(vm::Class::FromIl2CppType(type->type), &info->values, &info->names);
    }
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */

#endif
