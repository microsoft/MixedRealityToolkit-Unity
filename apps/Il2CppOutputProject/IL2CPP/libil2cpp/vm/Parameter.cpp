#include "il2cpp-config.h"
#include "il2cpp-runtime-metadata.h"
#include "il2cpp-class-internals.h"
#include "il2cpp-object-internals.h"
#include "Parameter.h"
#include "vm-utils/BlobReader.h"
#include "vm/Class.h"
#include "vm/Object.h"
#include "vm/Method.h"

#include <assert.h>

namespace il2cpp
{
namespace vm
{
    Il2CppObject* Parameter::GetDefaultParameterValueObject(const MethodInfo* method, const ParameterInfo* parameter, bool* isExplicitySetNullDefaultValue)
    {
        const Il2CppType* typeOfDefaultValue;
        const char* data = Method::GetParameterDefaultValue(method, parameter, &typeOfDefaultValue, isExplicitySetNullDefaultValue);
        if (data == NULL)
            return NULL;

        Il2CppClass* parameterType = Class::FromIl2CppType(parameter->parameter_type);
        if (parameterType->valuetype)
        {
            Class::SetupFields(parameterType);
            IL2CPP_ASSERT(parameterType->size_inited);
            void* value = alloca(parameterType->instance_size - sizeof(Il2CppObject));
            utils::BlobReader::GetConstantValueFromBlob(typeOfDefaultValue->type, data, value);
            return Object::Box(parameterType, value);
        }

        Il2CppObject* value = NULL;
        utils::BlobReader::GetConstantValueFromBlob(typeOfDefaultValue->type, data, &value);
        return value;
    }
}
}
