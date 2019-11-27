#include "il2cpp-config.h"

#include "icalls/mscorlib/System.Reflection/MemberInfo.h"
#include "vm/Exception.h"
#include "il2cpp-object-internals.h"
#include "vm/Reflection.h"
#include "vm/Field.h"
#include "vm/Property.h"
#include "vm/Method.h"
#include "vm/Event.h"
#include "vm/Type.h"

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
    int32_t MemberInfo::get_MetadataToken(Il2CppObject* /* System.Reflection.MemberInfo */ self)
    {
        if (vm::Reflection::IsField(self))
        {
            Il2CppReflectionField* field = (Il2CppReflectionField*)self;
            return vm::Field::GetToken(field->field);
        }
        else if (vm::Reflection::IsAnyMethod(self))
        {
            Il2CppReflectionMethod* method = (Il2CppReflectionMethod*)self;
            return vm::Method::GetToken(method->method);
        }
        else if (vm::Reflection::IsProperty(self))
        {
            Il2CppReflectionProperty* prop = (Il2CppReflectionProperty*)self;
            return vm::Property::GetToken(prop->property);
        }
        else if (vm::Reflection::IsEvent(self))
        {
            Il2CppReflectionMonoEvent* eventInfo = (Il2CppReflectionMonoEvent*)self;
            return vm::Event::GetToken(eventInfo->eventInfo);
        }
        else if (vm::Reflection::IsType(self))
        {
            Il2CppReflectionType* type = (Il2CppReflectionType*)self;
            return vm::Type::GetToken(type->type);
        }
        else
        {
            NOT_SUPPORTED_IL2CPP(MemberInfo::get_MetadataToken, "This icall is not supported by il2cpp.");
        }

        return 0;
    }
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
