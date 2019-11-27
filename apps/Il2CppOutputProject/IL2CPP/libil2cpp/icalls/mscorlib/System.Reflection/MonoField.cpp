#include "il2cpp-config.h"
#include <stddef.h>
#include "gc/GarbageCollector.h"
#include "icalls/mscorlib/System.Reflection/MonoField.h"
#include "utils/StringUtils.h"
#include "vm/Class.h"
#include "vm/Field.h"
#include "vm/Object.h"
#include "vm/Reflection.h"
#include "vm/Runtime.h"
#include "vm/Type.h"
#include "vm/Exception.h"
#include "il2cpp-class-internals.h"
#include "il2cpp-tabledefs.h"
#include "vm-utils/BlobReader.h"

using il2cpp::utils::StringUtils;

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
    Il2CppReflectionType * MonoField::GetParentType(Il2CppReflectionField * field, bool declaring)
    {
        Il2CppClass *parent;

        parent = declaring ? field->field->parent : field->klass;

        return il2cpp::vm::Reflection::GetTypeObject(&parent->byval_arg);
    }

    int32_t MonoField::GetFieldOffset(Il2CppReflectionField * field)
    {
        return field->field->offset - sizeof(Il2CppObject);
    }

    Il2CppObject* MonoField::GetValueInternal(Il2CppReflectionField * field, Il2CppObject * obj)
    {
        return vm::Field::GetValueObject(field->field, obj);
    }

    void MonoField::SetValueInternal(Il2CppReflectionField* field, Il2CppObject* obj, Il2CppObject* value)
    {
        ::FieldInfo* fieldInfo = field->field;
        Il2CppClass* fieldType = vm::Class::FromIl2CppType(fieldInfo->type);
        vm::Class::Init(fieldType);

#ifndef NET_4_0 //This check is done in managed code in .NET 4.5+
        if (value != NULL && !vm::Class::IsAssignableFrom(fieldType, value->klass))
        {
            vm::Exception::Raise(vm::Exception::GetArgumentException("value",
                utils::StringUtils::Printf("Object of type '%s' cannot be converted to type '%s'.",
                    vm::Type::GetName(&value->klass->byval_arg, IL2CPP_TYPE_NAME_FORMAT_FULL_NAME).c_str(),
                    vm::Type::GetName(fieldInfo->type, IL2CPP_TYPE_NAME_FORMAT_FULL_NAME).c_str()
                ).c_str()));
        }
#endif

        uint8_t* fieldAddress;

        if (fieldInfo->type->attrs & FIELD_ATTRIBUTE_STATIC)
        {
            if (fieldInfo->offset == THREAD_STATIC_FIELD_OFFSET)
            {
                IL2CPP_NOT_IMPLEMENTED(Field::StaticSetValue);
            }

            vm::Runtime::ClassInit(fieldInfo->parent);
            fieldAddress = static_cast<uint8_t*>(fieldInfo->parent->static_fields) + fieldInfo->offset;
        }
        else
        {
            IL2CPP_ASSERT(obj);
            fieldAddress = reinterpret_cast<uint8_t*>(obj) + fieldInfo->offset;
        }

        if (fieldType->valuetype)
        {
            if (!vm::Class::IsNullable(fieldType))
            {
                uint32_t fieldSize = vm::Class::GetInstanceSize(fieldType) - sizeof(Il2CppObject);

                if (value != NULL)
                {
                    memcpy(fieldAddress, vm::Object::Unbox(value), fieldSize);
                }
                else
                {
                    // Setting value type to null is defined to zero it out
                    memset(fieldAddress, 0, fieldSize);
                }
                il2cpp::gc::GarbageCollector::SetWriteBarrier((void**)fieldAddress, fieldSize);
            }
            else
            {
                Il2CppClass* nullableArg = vm::Class::GetNullableArgument(fieldType);
                uint32_t valueSize = vm::Class::GetInstanceSize(nullableArg) - sizeof(Il2CppObject);

                if (value != NULL)
                {
                    memcpy(fieldAddress, vm::Object::Unbox(value), valueSize);
                    *(fieldAddress + valueSize) = true;
                }
                else
                {
                    *(fieldAddress + valueSize) = false;
                }
                il2cpp::gc::GarbageCollector::SetWriteBarrier((void**)fieldAddress, valueSize);
            }
        }
        else
        {
            memcpy(fieldAddress, &value, sizeof(Il2CppObject*));
            il2cpp::gc::GarbageCollector::SetWriteBarrier((void**)fieldAddress);
        }
    }

    Il2CppObject* MonoField::GetRawConstantValue(Il2CppReflectionField* field)
    {
        ::FieldInfo* fieldInfo = field->field;

        if (!(fieldInfo->type->attrs & FIELD_ATTRIBUTE_HAS_DEFAULT))
            vm::Exception::Raise(vm::Exception::GetInvalidOperationException(NULL));

        const Il2CppType* type = NULL;
        const char* data = vm::Class::GetFieldDefaultValue(fieldInfo, &type);

        switch (type->type)
        {
            case IL2CPP_TYPE_U1:
            case IL2CPP_TYPE_I1:
            case IL2CPP_TYPE_BOOLEAN:
            case IL2CPP_TYPE_U2:
            case IL2CPP_TYPE_I2:
            case IL2CPP_TYPE_CHAR:
            case IL2CPP_TYPE_U4:
            case IL2CPP_TYPE_I4:
            case IL2CPP_TYPE_R4:
            case IL2CPP_TYPE_U8:
            case IL2CPP_TYPE_I8:
            case IL2CPP_TYPE_R8:
            {
                Il2CppObject* obj = vm::Object::New(vm::Class::FromIl2CppType(type));
                utils::BlobReader::GetConstantValueFromBlob(type->type, data, vm::Object::Unbox(obj));
                return obj;
            }
            case IL2CPP_TYPE_STRING:
            case IL2CPP_TYPE_CLASS:
            case IL2CPP_TYPE_OBJECT:
            case IL2CPP_TYPE_GENERICINST:
            {
                Il2CppObject* obj = NULL;
                utils::BlobReader::GetConstantValueFromBlob(type->type, data, &obj);
                return obj;
            }
            default:
                vm::Exception::Raise(vm::Exception::GetInvalidOperationException(StringUtils::Printf("Attempting to get raw constant value for field of type %d", type).c_str()));
        }

        return NULL;
    }

#if NET_4_0
    int32_t MonoField::get_core_clr_security_level(Il2CppObject* _this)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(MonoField::get_core_clr_security_level);
        IL2CPP_UNREACHABLE;
        return 0;
    }

    Il2CppObject* MonoField::ResolveType(Il2CppObject* _this)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(MonoField::ResolveType);
        IL2CPP_UNREACHABLE;
        return NULL;
    }

#endif
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
