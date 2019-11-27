#include "il2cpp-config.h"
#include <algorithm>
#include "gc/GCHandle.h"
#include "metadata/ArrayMetadata.h"
#include "metadata/GenericMetadata.h"
#include "metadata/GenericMethod.h"
#include "metadata/FieldLayout.h"
#include "metadata/Il2CppTypeVector.h"
#include "os/Atomic.h"
#include "os/Mutex.h"
#include "utils/Memory.h"
#include "utils/StringUtils.h"
#include "vm/Assembly.h"
#include "vm/Class.h"
#include "vm/ClassInlines.h"
#include "vm/Exception.h"
#include "vm/Field.h"
#include "vm/GenericClass.h"
#include "vm/GenericContainer.h"
#include "vm/Image.h"
#include "vm/MetadataAlloc.h"
#include "vm/MetadataCache.h"
#include "vm/MetadataLock.h"
#include "vm/Method.h"
#include "vm/Property.h"
#include "vm/Runtime.h"
#include "vm/Reflection.h"
#include "vm/Thread.h"
#include "vm/Type.h"
#include "vm/Object.h"
#include "il2cpp-class-internals.h"
#include "il2cpp-object-internals.h"
#include "il2cpp-runtime-stats.h"
#include "il2cpp-tabledefs.h"
#include "gc/GarbageCollector.h"
#include "utils/Il2CppHashMap.h"
#include "utils/StringUtils.h"
#include "utils/HashUtils.h"
#include <string>
#include <memory.h>
#include <algorithm>
#include <limits>

namespace il2cpp
{
namespace vm
{
    const int Class::IgnoreNumberOfArguments = -1;

    static il2cpp::utils::dynamic_array<Il2CppClass*> s_staticFieldData;
    static int32_t s_FinalizerSlot = -1;
    static int32_t s_GetHashCodeSlot = -1;

    static void SetupGCDescriptor(Il2CppClass* klass);
    static void GetBitmapNoInit(Il2CppClass* klass, size_t* bitmap, size_t& maxSetBit, size_t parentOffset);
    static Il2CppClass* ResolveGenericInstanceType(Il2CppClass*, const il2cpp::vm::TypeNameParseInfo&, TypeSearchFlags searchFlags);
    static bool InitLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock);
    static void SetupVTable(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock);

    Il2CppClass* Class::FromIl2CppType(const Il2CppType* type)
    {
#define RETURN_DEFAULT_TYPE(fieldName) do { IL2CPP_ASSERT(il2cpp_defaults.fieldName); return il2cpp_defaults.fieldName; } while (false)

        switch (type->type)
        {
            case IL2CPP_TYPE_OBJECT:
                RETURN_DEFAULT_TYPE(object_class);
            case IL2CPP_TYPE_VOID:
                RETURN_DEFAULT_TYPE(void_class);
            case IL2CPP_TYPE_BOOLEAN:
                RETURN_DEFAULT_TYPE(boolean_class);
            case IL2CPP_TYPE_CHAR:
                RETURN_DEFAULT_TYPE(char_class);
            case IL2CPP_TYPE_I1:
                RETURN_DEFAULT_TYPE(sbyte_class);
            case IL2CPP_TYPE_U1:
                RETURN_DEFAULT_TYPE(byte_class);
            case IL2CPP_TYPE_I2:
                RETURN_DEFAULT_TYPE(int16_class);
            case IL2CPP_TYPE_U2:
                RETURN_DEFAULT_TYPE(uint16_class);
            case IL2CPP_TYPE_I4:
                RETURN_DEFAULT_TYPE(int32_class);
            case IL2CPP_TYPE_U4:
                RETURN_DEFAULT_TYPE(uint32_class);
            case IL2CPP_TYPE_I:
                RETURN_DEFAULT_TYPE(int_class);
            case IL2CPP_TYPE_U:
                RETURN_DEFAULT_TYPE(uint_class);
            case IL2CPP_TYPE_I8:
                RETURN_DEFAULT_TYPE(int64_class);
            case IL2CPP_TYPE_U8:
                RETURN_DEFAULT_TYPE(uint64_class);
            case IL2CPP_TYPE_R4:
                RETURN_DEFAULT_TYPE(single_class);
            case IL2CPP_TYPE_R8:
                RETURN_DEFAULT_TYPE(double_class);
            case IL2CPP_TYPE_STRING:
                RETURN_DEFAULT_TYPE(string_class);
            case IL2CPP_TYPE_TYPEDBYREF:
                RETURN_DEFAULT_TYPE(typed_reference_class);
            case IL2CPP_TYPE_ARRAY:
            {
                Il2CppClass* elementClass = FromIl2CppType(type->data.array->etype);
                return Class::GetBoundedArrayClass(elementClass, type->data.array->rank, true);
            }
            case IL2CPP_TYPE_PTR:
                return Class::GetPtrClass(type->data.type);
            case IL2CPP_TYPE_FNPTR:
                IL2CPP_NOT_IMPLEMENTED(Class::FromIl2CppType);
                return NULL; //mono_fnptr_class_get (type->data.method);
            case IL2CPP_TYPE_SZARRAY:
            {
                Il2CppClass* elementClass = FromIl2CppType(type->data.type);
                return Class::GetArrayClass(elementClass, 1);
            }
            case IL2CPP_TYPE_CLASS:
            case IL2CPP_TYPE_VALUETYPE:
                return Type::GetClass(type);
            case IL2CPP_TYPE_GENERICINST:
                return GenericClass::GetClass(type->data.generic_class);
            case IL2CPP_TYPE_VAR:
                return Class::FromGenericParameter(Type::GetGenericParameter(type));
            case IL2CPP_TYPE_MVAR:
                return Class::FromGenericParameter(Type::GetGenericParameter(type));
            default:
                IL2CPP_NOT_IMPLEMENTED(Class::FromIl2CppType);
        }

        return NULL;

#undef RETURN_DEFAULT_TYPE
    }

/* From ECMA-335, I.8.7 Assignment compatibility:

    The reduced type of a type T is the following:

    1. If the underlying type of T is:
        a. int8, or unsigned int8, then its reduced type is int8.
        b. int16, or unsigned int16, then its reduced type is int16.
        c. int32, or unsigned int32, then its reduced type is int32.
        d. int64, or unsigned int64, then its reduced type is int64.
        e. native int, or unsigned native int, then its reduced type is native int.
    2. Otherwise, the reduced type is itself.
*/
    static inline const Il2CppClass* GetReducedType(const Il2CppClass* type)
    {
        switch (type->byval_arg.type)
        {
            case IL2CPP_TYPE_I1:
            case IL2CPP_TYPE_U1:
                return il2cpp_defaults.sbyte_class;

            case IL2CPP_TYPE_I2:
            case IL2CPP_TYPE_U2:
                return il2cpp_defaults.int16_class;

            case IL2CPP_TYPE_I4:
            case IL2CPP_TYPE_U4:
                return il2cpp_defaults.int32_class;

            case IL2CPP_TYPE_I8:
            case IL2CPP_TYPE_U8:
                return il2cpp_defaults.int64_class;

            case IL2CPP_TYPE_I:
            case IL2CPP_TYPE_U:
                return il2cpp_defaults.int_class;

            default:
                return type;
        }
    }

    Il2CppClass* Class::FromSystemType(Il2CppReflectionType *type)
    {
        Il2CppClass *klass = Class::FromIl2CppType(type->type);
        Class::Init(klass);
        return klass;
    }

    static void SetupInterfacesLocked(Il2CppClass* klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->generic_class)
        {
            Il2CppClass* genericTypeDefinition = GenericClass::GetTypeDefinition(klass->generic_class);
            Il2CppGenericContext* context = &klass->generic_class->context;

            if (genericTypeDefinition->interfaces_count > 0 && klass->implementedInterfaces == NULL)
            {
                IL2CPP_ASSERT(genericTypeDefinition->interfaces_count == klass->interfaces_count);
                klass->implementedInterfaces = (Il2CppClass**)MetadataCalloc(genericTypeDefinition->interfaces_count, sizeof(Il2CppClass*));
                for (uint16_t i = 0; i < genericTypeDefinition->interfaces_count; i++)
                    klass->implementedInterfaces[i] = Class::FromIl2CppType(il2cpp::metadata::GenericMetadata::InflateIfNeeded(MetadataCache::GetInterfaceFromIndex(genericTypeDefinition->typeDefinition->interfacesStart + i), context, false));
            }
        }
        else if (klass->rank > 0)
        {
#if !IL2CPP_DOTS
            if (klass->implementedInterfaces == NULL)
                il2cpp::metadata::ArrayMetadata::SetupArrayInterfaces(klass, lock);
#endif
        }
        else
        {
            if (klass->interfaces_count > 0 && klass->implementedInterfaces == NULL)
            {
                klass->implementedInterfaces = (Il2CppClass**)MetadataCalloc(klass->interfaces_count, sizeof(Il2CppClass*));
                for (uint16_t i = 0; i < klass->interfaces_count; i++)
                    klass->implementedInterfaces[i] = Class::FromIl2CppType(MetadataCache::GetInterfaceFromIndex(klass->typeDefinition->interfacesStart + i));
            }
        }
    }

    typedef Il2CppHashMap<const Il2CppGenericParameter*, Il2CppClass*, il2cpp::utils::PointerHash<const Il2CppGenericParameter> > GenericParameterMap;
    static GenericParameterMap s_GenericParameterMap;

    Il2CppClass* Class::FromGenericParameter(const Il2CppGenericParameter *param)
    {
        IL2CPP_ASSERT(param->ownerIndex != kGenericContainerIndexInvalid);

        il2cpp::os::FastAutoLock lock(&g_MetadataLock);

        GenericParameterMap::const_iterator iter = s_GenericParameterMap.find(param);
        if (iter != s_GenericParameterMap.end())
            return iter->second;

        Il2CppClass* klass = (Il2CppClass*)MetadataCalloc(1, sizeof(Il2CppClass));
        klass->klass = klass;

        klass->name = MetadataCache::GetStringFromIndex(param->nameIndex);
        klass->namespaze = "";

        const Il2CppGenericContainer* container = MetadataCache::GetGenericContainerFromIndex(param->ownerIndex);
        klass->image = GenericContainer::GetDeclaringType(container)->image;

        klass->initialized = true;
        UpdateInitializedAndNoError(klass);

        klass->parent = il2cpp_defaults.object_class;
        klass->castClass = klass->element_class = klass;

        klass->flags = TYPE_ATTRIBUTE_PUBLIC;

        klass->this_arg.type = klass->byval_arg.type = container->is_method ? IL2CPP_TYPE_MVAR : IL2CPP_TYPE_VAR;
        klass->this_arg.data.genericParameterIndex = klass->byval_arg.data.genericParameterIndex = MetadataCache::GetIndexForGenericParameter(param);
        klass->this_arg.byref = true;

        klass->instance_size = sizeof(void*);
        klass->thread_static_fields_size = -1;
        klass->native_size = -1;
        klass->size_inited = true;

        s_GenericParameterMap.insert(std::make_pair(param, klass));

        return klass;
    }

    Il2CppClass* Class::GetElementClass(Il2CppClass *klass)
    {
        return klass->element_class;
    }

    const Il2CppType* Class::GetEnumBaseType(Il2CppClass *klass)
    {
        if (klass->element_class == klass)
            /* SRE or broken types */
            return NULL;
        else
            return &klass->element_class->byval_arg;
    }

    const EventInfo* Class::GetEvents(Il2CppClass *klass, void* *iter)
    {
        if (!iter)
            return NULL;

        if (!*iter)
        {
            Class::SetupEvents(klass);
            if (klass->event_count == 0)
                return NULL;

            *iter = const_cast<EventInfo*>(klass->events);
            return klass->events;
        }

        const EventInfo* eventInfo = (const EventInfo*)*iter;
        eventInfo++;
        if (eventInfo < klass->events + klass->event_count)
        {
            *iter = const_cast<EventInfo*>(eventInfo);
            return eventInfo;
        }

        return NULL;
    }

    FieldInfo* Class::GetFields(Il2CppClass *klass, void* *iter)
    {
        if (!iter)
            return NULL;

        if (!*iter)
        {
            Class::SetupFields(klass);
            if (klass->field_count == 0)
                return NULL;

            *iter = klass->fields;
            return klass->fields;
        }

        FieldInfo* fieldAddress = (FieldInfo*)*iter;
        fieldAddress++;
        if (fieldAddress < klass->fields + klass->field_count)
        {
            *iter = fieldAddress;
            return fieldAddress;
        }

        return NULL;
    }

    FieldInfo* Class::GetFieldFromName(Il2CppClass *klass, const char* name)
    {
        while (klass)
        {
            void* iter = NULL;
            FieldInfo* field;
            while ((field = GetFields(klass, &iter)))
            {
                if (strcmp(name, Field::GetName(field)) != 0)
                    continue;

                return field;
            }

            klass = klass->parent;
        }

        return NULL;
    }

    const MethodInfo* Class::GetFinalizer(Il2CppClass *klass)
    {
        if (!klass->initialized)
            Class::Init(klass);

        if (!klass->has_finalize)
            return NULL;

#if IL2CPP_DOTS
        IL2CPP_ASSERT(0 && "System.Object does not have a finalizer in the Dots mscorlib, so we don't have a finalizer slot.");
#endif
        return klass->vtable[s_FinalizerSlot].method;
    }

    int32_t Class::GetInstanceSize(const Il2CppClass *klass)
    {
        IL2CPP_ASSERT(klass->size_inited);
        return klass->instance_size;
    }

    Il2CppClass* Class::GetInterfaces(Il2CppClass *klass, void* *iter)
    {
        if (!iter)
            return NULL;

        if (!*iter)
        {
            Class::SetupInterfaces(klass);
            if (klass->interfaces_count == 0)
                return NULL;

            *iter = &klass->implementedInterfaces[0];
            return klass->implementedInterfaces[0];
        }

        Il2CppClass** interfaceAddress = (Il2CppClass**)*iter;
        interfaceAddress++;
        if (interfaceAddress < &klass->implementedInterfaces[klass->interfaces_count])
        {
            *iter = interfaceAddress;
            return *interfaceAddress;
        }

        return NULL;
    }

    const MethodInfo* Class::GetMethods(Il2CppClass *klass, void* *iter)
    {
        if (!iter)
            return NULL;

        if (!*iter)
        {
            Class::SetupMethods(klass);
            if (klass->method_count == 0)
                return NULL;

            *iter = &klass->methods[0];
            return klass->methods[0];
        }

        const MethodInfo** methodAddress = (const MethodInfo**)*iter;
        methodAddress++;
        if (methodAddress < &klass->methods[klass->method_count])
        {
            *iter = methodAddress;
            return *methodAddress;
        }

        return NULL;
    }

    const MethodInfo* Class::GetMethodFromName(Il2CppClass *klass, const char* name, int argsCount)
    {
        return GetMethodFromNameFlags(klass, name, argsCount, 0);
    }

    const MethodInfo* Class::GetMethodFromNameFlags(Il2CppClass *klass, const char* name, int argsCount, int32_t flags)
    {
        Class::Init(klass);

        while (klass != NULL)
        {
            void* iter = NULL;
            while (const MethodInfo* method = Class::GetMethods(klass, &iter))
            {
                if (method->name[0] == name[0] &&
                    !strcmp(name, method->name) &&
                    (argsCount == IgnoreNumberOfArguments || method->parameters_count == argsCount) &&
                    ((method->flags & flags) == flags))
                {
                    return method;
                }
            }

            klass = klass->parent;
        }

        return NULL;
    }

    const char* Class::GetName(Il2CppClass *klass)
    {
        return klass->name;
    }

    const char* Class::GetNamespace(Il2CppClass *klass)
    {
        return klass->namespaze;
    }

    Il2CppClass* Class::GetNestedTypes(Il2CppClass *klass, void* *iter)
    {
        if (!iter)
            return NULL;

        if (klass->generic_class)
        {
            IL2CPP_ASSERT(0 && "Class::GetNestedTypes should only be called on non-generic types and generic type definitions");
            return NULL;
        }

        if (!*iter)
        {
            Class::SetupNestedTypes(klass);
            if (klass->nested_type_count == 0)
                return NULL;

            *iter = &klass->nestedTypes[0];
            return klass->nestedTypes[0];
        }

        Il2CppClass** nestedTypeAddress = (Il2CppClass**)*iter;
        nestedTypeAddress++;
        if (nestedTypeAddress < &klass->nestedTypes[klass->nested_type_count])
        {
            *iter = nestedTypeAddress;
            return *nestedTypeAddress;
        }

        return NULL;
    }

    size_t Class::GetNumMethods(const Il2CppClass* klass)
    {
        return klass->method_count;
    }

    size_t Class::GetNumProperties(const Il2CppClass* klass)
    {
        return klass->property_count;
    }

    size_t Class::GetNumFields(const Il2CppClass* klass)
    {
        return klass->field_count;
    }

    Il2CppClass* Class::GetParent(Il2CppClass *klass)
    {
        return klass->parent;
    }

    const PropertyInfo* Class::GetProperties(Il2CppClass *klass, void* *iter)
    {
        if (!iter)
            return NULL;

        if (!*iter)
        {
            Class::SetupProperties(klass);
            if (klass->property_count == 0)
                return NULL;

            *iter = const_cast<PropertyInfo*>(klass->properties);
            return klass->properties;
        }

        const PropertyInfo* property = (const PropertyInfo*)*iter;
        property++;
        if (property < klass->properties + klass->property_count)
        {
            *iter = const_cast<PropertyInfo*>(property);
            return property;
        }

        return NULL;
    }

    const PropertyInfo* Class::GetPropertyFromName(Il2CppClass *klass, const char* name)
    {
        while (klass)
        {
            void* iter = NULL;
            while (const PropertyInfo* prop = GetProperties(klass, &iter))
            {
                if (strcmp(name, Property::GetName(prop)) != 0)
                    continue;

                return prop;
            }

            klass = klass->parent;
        }

        return NULL;
    }

    int32_t Class::GetValueSize(Il2CppClass *klass, uint32_t *align)
    {
        int32_t size;

        if (!klass->init_pending)
            Init(klass);

        IL2CPP_ASSERT(klass->valuetype);

        size = Class::GetInstanceSize(klass) - sizeof(Il2CppObject);

        if (align)
            *align = klass->minimumAlignment;

        return size;
    }

    bool Class::HasParent(Il2CppClass *klass, Il2CppClass *parent)
    {
        Class::SetupTypeHierarchy(klass);
        Class::SetupTypeHierarchy(parent);

        return ClassInlines::HasParentUnsafe(klass, parent);
    }

    bool Class::IsAssignableFrom(Il2CppClass *klass, Il2CppClass *oklass)
    {
        // Cast to original class - fast path
        if (klass == oklass)
            return true;

        Class::Init(klass);
        Class::Init(oklass);

        // Following checks are always going to fail for interfaces
        if (!IsInterface(klass))
        {
            // Array
            if (klass->rank)
            {
                if (oklass->rank != klass->rank)
                    return false;

                if (oklass->castClass->valuetype)
                {
                    // Full array covariance is defined only for reference types.
                    // For value types, array element reduced types must match
                    return GetReducedType(klass->castClass) == GetReducedType(oklass->castClass);
                }

                return Class::IsAssignableFrom(klass->castClass, oklass->castClass);
            }
            // System.Object
            else if (klass == il2cpp_defaults.object_class)
            {
                return true;
            }
            // Left is System.Nullable<>
            else if (Class::IsNullable(klass))
            {
                if (Class::IsNullable(oklass))
                    IL2CPP_NOT_IMPLEMENTED(Class::IsAssignableFrom);
                Il2CppClass* nullableArg = Class::GetNullableArgument(klass);
                return Class::IsAssignableFrom(nullableArg, oklass);
            }

#if NET_4_0
            if (klass->parent == il2cpp_defaults.multicastdelegate_class && klass->generic_class != NULL)
            {
                const Il2CppTypeDefinition* genericClass = MetadataCache::GetTypeDefinitionFromIndex(klass->generic_class->typeDefinitionIndex);
                const Il2CppGenericContainer* genericContainer = MetadataCache::GetGenericContainerFromIndex(genericClass->genericContainerIndex);

                if (IsGenericClassAssignableFrom(klass, oklass, genericContainer))
                    return true;
            }
#endif

            return ClassInlines::HasParentUnsafe(oklass, klass);
        }

#if NET_4_0
        if (klass->generic_class != NULL)
        {
            // checking for simple reference equality is not enough in this case because generic interface might have covariant and/or contravariant parameters

            const Il2CppTypeDefinition* genericInterface = MetadataCache::GetTypeDefinitionFromIndex(klass->generic_class->typeDefinitionIndex);
            const Il2CppGenericContainer* genericContainer = MetadataCache::GetGenericContainerFromIndex(genericInterface->genericContainerIndex);

            for (Il2CppClass* iter = oklass; iter != NULL; iter = iter->parent)
            {
                if (IsGenericClassAssignableFrom(klass, iter, genericContainer))
                    return true;

                for (uint16_t i = 0; i < iter->interfaces_count; ++i)
                {
                    if (IsGenericClassAssignableFrom(klass, iter->implementedInterfaces[i], genericContainer))
                        return true;
                }

                for (uint16_t i = 0; i < iter->interface_offsets_count; ++i)
                {
                    if (IsGenericClassAssignableFrom(klass, iter->interfaceOffsets[i].interfaceType, genericContainer))
                        return true;
                }
            }
        }
        else
#endif
        {
            for (Il2CppClass* iter = oklass; iter != NULL; iter = iter->parent)
            {
                for (uint16_t i = 0; i < iter->interfaces_count; ++i)
                {
                    if (iter->implementedInterfaces[i] == klass)
                        return true;
                }

                // Check the interfaces we may have grafted on to the type (e.g IList,
                // ICollection, IEnumerable for array types).
                for (uint16_t i = 0; i < iter->interface_offsets_count; ++i)
                {
                    if (iter->interfaceOffsets[i].interfaceType == klass)
                        return true;
                }
            }
        }

        return false;
    }

    bool Class::IsGeneric(const Il2CppClass *klass)
    {
        return klass->is_generic;
    }

    bool Class::IsInflated(const Il2CppClass *klass)
    {
        return klass->generic_class != NULL;
    }

    bool Class::IsSubclassOf(Il2CppClass *klass, Il2CppClass *klassc, bool check_interfaces)
    {
        Class::SetupTypeHierarchy(klass);
        Class::SetupTypeHierarchy(klassc);
        Class::SetupInterfaces(klass);

        if (check_interfaces && IsInterface(klassc) && !IsInterface(klass))
        {
            Il2CppClass *oklass = klass;

            while (oklass)
            {
                Class::SetupInterfaces(oklass);
                // TODO: we probably need to implement a faster check here
                for (uint16_t i = 0; i < oklass->interfaces_count; i++)
                {
                    if (oklass->implementedInterfaces[i] == klassc)
                        return true;
                }

                oklass = oklass->parent;
            }
        }
        else if (check_interfaces && IsInterface(klassc) && IsInterface(klass))
        {
            // TODO: we probably need to implement a faster check here
            for (uint16_t i = 0; i < klass->interfaces_count; i++)
            {
                if (klass->implementedInterfaces[i] == klassc)
                    return true;
            }
        }
        else
        {
            if (!IsInterface(klass) && ClassInlines::HasParentUnsafe(klass, klassc))
                return true;
        }

        /*
         * MS.NET thinks interfaces are a subclass of Object, so we think it as
         * well.
         */
        if (klassc == il2cpp_defaults.object_class)
            return true;

        return false;
    }

    bool Class::IsValuetype(const Il2CppClass *klass)
    {
        return klass->valuetype;
    }

    bool Class::IsBlittable(const Il2CppClass *klass)
    {
        return klass->is_blittable;
    }

    enum FieldLayoutKind
    {
        FIELD_LAYOUT_INSTANCE,
        FIELD_LAYOUT_STATIC,
        FIELD_LAYOUT_THREADSTATIC,
    };


    static void SetupFieldOffsetsLocked(FieldLayoutKind fieldLayoutKind, Il2CppClass* klass, size_t size, const std::vector<size_t>& fieldOffsets, const il2cpp::os::FastAutoLock& lock)
    {
        IL2CPP_ASSERT(size < std::numeric_limits<uint32_t>::max());
        if (fieldLayoutKind == FIELD_LAYOUT_INSTANCE)
            klass->instance_size = static_cast<uint32_t>(size);
        if (fieldLayoutKind == FIELD_LAYOUT_STATIC)
            klass->static_fields_size = static_cast<uint32_t>(size);
        if (fieldLayoutKind == FIELD_LAYOUT_THREADSTATIC)
            klass->thread_static_fields_size = static_cast<uint32_t>(size);

        if (!(klass->flags & TYPE_ATTRIBUTE_EXPLICIT_LAYOUT))
        {
            size_t fieldIndex = 0;
            for (uint16_t i = 0; i < klass->field_count; i++)
            {
                FieldInfo* field = klass->fields + i;
                if (fieldLayoutKind == FIELD_LAYOUT_INSTANCE && field->type->attrs & FIELD_ATTRIBUTE_STATIC)
                    continue;
                if (fieldLayoutKind == FIELD_LAYOUT_STATIC && !Field::IsNormalStatic(field))
                    continue;
                if (fieldLayoutKind == FIELD_LAYOUT_THREADSTATIC && !Field::IsThreadStatic(field))
                    continue;

                if (fieldLayoutKind == FIELD_LAYOUT_THREADSTATIC)
                {
                    field->offset = THREAD_STATIC_FIELD_OFFSET;
                    MetadataCache::AddThreadLocalStaticOffsetForFieldLocked(field, static_cast<int32_t>(fieldOffsets[fieldIndex]), lock);
                    fieldIndex++;
                    continue;
                }

                field->offset = static_cast<int32_t>(fieldOffsets[fieldIndex]);
                fieldIndex++;
            }
        }
    }

    static void ValidateFieldOffsets(FieldLayoutKind fieldLayoutKind, Il2CppClass* klass, size_t size, const std::vector<size_t>& fieldOffsets)
    {
        if (fieldLayoutKind == FIELD_LAYOUT_INSTANCE && klass->parent && !(klass->flags & TYPE_ATTRIBUTE_EXPLICIT_LAYOUT))
            IL2CPP_ASSERT(klass->instance_size == size);
        if (fieldLayoutKind == FIELD_LAYOUT_STATIC)
            IL2CPP_ASSERT(klass->static_fields_size == size);
        if (fieldLayoutKind == FIELD_LAYOUT_THREADSTATIC)
            IL2CPP_ASSERT(klass->thread_static_fields_size == size);

        if (!(klass->flags & TYPE_ATTRIBUTE_EXPLICIT_LAYOUT))
        {
            size_t fieldIndex = 0;
            for (uint16_t i = 0; i < klass->field_count; i++)
            {
                FieldInfo* field = klass->fields + i;
                if (fieldLayoutKind == FIELD_LAYOUT_INSTANCE && field->type->attrs & FIELD_ATTRIBUTE_STATIC)
                    continue;
                if (fieldLayoutKind == FIELD_LAYOUT_STATIC && !Field::IsNormalStatic(field))
                    continue;
                if (fieldLayoutKind == FIELD_LAYOUT_THREADSTATIC && !Field::IsThreadStatic(field))
                    continue;

                if (fieldLayoutKind == FIELD_LAYOUT_THREADSTATIC)
                {
                    IL2CPP_ASSERT(fieldOffsets[fieldIndex] == -1);
                    fieldIndex++;
                    continue;
                }

                IL2CPP_ASSERT(field->offset == fieldOffsets[fieldIndex]);
                fieldIndex++;
            }
        }
    }

    static void LayoutFieldsLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (Class::IsGeneric(klass))
            return;

        size_t instanceSize = 0;
        size_t actualSize = 0;
        if (klass->parent)
        {
            IL2CPP_ASSERT(klass->parent->size_inited);
            klass->has_references |= klass->parent->has_references;
            instanceSize = klass->parent->instance_size;
            actualSize = klass->parent->actualSize;
            if (klass->valuetype)
                klass->minimumAlignment = 1;
            else
                klass->minimumAlignment = klass->parent->minimumAlignment;
        }
        else
        {
            actualSize = instanceSize = sizeof(Il2CppObject);
            klass->minimumAlignment = sizeof(Il2CppObject*);
        }

        if (klass->field_count)
        {
            for (uint16_t i = 0; i < klass->field_count; i++)
            {
                FieldInfo* field = klass->fields + i;
                if (!Field::IsInstance(field))
                    continue;

                const Il2CppType* ftype = Type::GetUnderlyingType(field->type);

                if (Type::IsEmptyType(ftype))
                {
                    std::string message;
                    message += "The field '";
                    message += field->name;
                    message += "' in type '";
                    message += klass->name;
                    message += "' has a type which was not generated by il2cpp.exe. Consider using a generic type which is not nested so deeply.";
                    klass->has_initialization_error = true;
                    Class::UpdateInitializedAndNoError(klass);
                    klass->initializationExceptionGCHandle = gc::GCHandle::New(il2cpp::vm::Exception::GetExecutionEngineException(message.c_str()), false);
                    return;
                }

                if (Type::IsReference(ftype) || (Type::IsStruct(ftype) && Class::HasReferences(Class::FromIl2CppType(ftype))))
                    klass->has_references = true;
            }

            il2cpp::metadata::Il2CppTypeVector fieldTypes;
            il2cpp::metadata::Il2CppTypeVector staticFieldTypes;
            il2cpp::metadata::Il2CppTypeVector threadStaticFieldTypes;

            for (uint16_t i = 0; i < klass->field_count; i++)
            {
                FieldInfo* field = klass->fields + i;

                const Il2CppType* ftype = Type::GetUnderlyingType(field->type);

                if (Field::IsInstance(field))
                    fieldTypes.push_back(ftype);
                else if (Field::IsNormalStatic(field))
                    staticFieldTypes.push_back(ftype);
                else if (Field::IsThreadStatic(field))
                    threadStaticFieldTypes.push_back(ftype);
            }

            il2cpp::metadata::FieldLayout::FieldLayoutData layoutData;
            il2cpp::metadata::FieldLayout::FieldLayoutData staticLayoutData;
            il2cpp::metadata::FieldLayout::FieldLayoutData threadStaticLayoutData;

            il2cpp::metadata::FieldLayout::LayoutFields(instanceSize, actualSize, klass->minimumAlignment, klass->packingSize, fieldTypes, layoutData);

            klass->naturalAligment = layoutData.naturalAlignment;

            instanceSize = layoutData.classSize;

            // This is a value type with no instance fields, but at least one static field.
            if (klass->valuetype && fieldTypes.size() == 0)
            {
                instanceSize = IL2CPP_SIZEOF_STRUCT_WITH_NO_INSTANCE_FIELDS + sizeof(Il2CppObject);
                klass->actualSize = IL2CPP_SIZEOF_STRUCT_WITH_NO_INSTANCE_FIELDS + sizeof(Il2CppObject);
            }

            // need to set this in case there are no fields in a generic instance type
            if (klass->generic_class)
                klass->instance_size = static_cast<uint32_t>(instanceSize);

            klass->size_inited = true;

            il2cpp::metadata::FieldLayout::LayoutFields(0, 0, 1, 0, staticFieldTypes, staticLayoutData);
            il2cpp::metadata::FieldLayout::LayoutFields(0, 0, 1, 0, threadStaticFieldTypes, threadStaticLayoutData);

            klass->minimumAlignment = layoutData.minimumAlignment;
            klass->actualSize = static_cast<uint32_t>(layoutData.actualClassSize);

            size_t staticSize = staticLayoutData.classSize;
            size_t threadStaticSize = threadStaticLayoutData.classSize;

            if (klass->generic_class)
            {
                SetupFieldOffsetsLocked(FIELD_LAYOUT_INSTANCE, klass, instanceSize, layoutData.FieldOffsets, lock);
                SetupFieldOffsetsLocked(FIELD_LAYOUT_STATIC, klass, staticSize, staticLayoutData.FieldOffsets, lock);
                SetupFieldOffsetsLocked(FIELD_LAYOUT_THREADSTATIC, klass, threadStaticSize, threadStaticLayoutData.FieldOffsets, lock);
            }
            else
            {
#if IL2CPP_ENABLE_VALIDATE_FIELD_LAYOUT
                ValidateFieldOffsets(FIELD_LAYOUT_INSTANCE, klass, instanceSize, layoutData.FieldOffsets);
                ValidateFieldOffsets(FIELD_LAYOUT_STATIC, klass, staticSize, staticLayoutData.FieldOffsets);
                ValidateFieldOffsets(FIELD_LAYOUT_THREADSTATIC, klass, threadStaticSize, threadStaticLayoutData.FieldOffsets);
#endif
            }
        }
        else
        {
            // need to set this in case there are no fields in a generic instance type
            if (klass->generic_class)
                klass->instance_size = static_cast<uint32_t>(instanceSize);

            // Always set the actual size, as a derived class without fields could end up
            // with the wrong actual size (i.e. sizeof may be incorrect), if the last
            // field of the base class doesn't go to an alignment boundary and the compiler ABI
            // uses that extra space (as clang does).
            klass->actualSize = static_cast<uint32_t>(actualSize);
        }

        if (klass->static_fields_size)
        {
            klass->static_fields = il2cpp::gc::GarbageCollector::AllocateFixed(klass->static_fields_size, NULL);
            s_staticFieldData.push_back(klass);

            il2cpp_runtime_stats.class_static_data_size += klass->static_fields_size;
        }
        if (klass->thread_static_fields_size)
            klass->thread_static_fields_offset = il2cpp::vm::Thread::AllocThreadStaticData(klass->thread_static_fields_size);
    }

    static void SetupFieldsFromDefinitionLocked(Il2CppClass* klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->field_count == 0)
        {
            klass->fields = NULL;
            return;
        }

        FieldInfo* fields = (FieldInfo*)MetadataCalloc(klass->field_count, sizeof(FieldInfo));
        FieldInfo* newField = fields;

        FieldIndex start = klass->typeDefinition->fieldStart;
        IL2CPP_ASSERT(klass->typeDefinition->fieldStart != kFieldIndexInvalid);
        FieldIndex end = start + klass->field_count;

        for (FieldIndex fieldIndex = start; fieldIndex < end; ++fieldIndex)
        {
            const Il2CppFieldDefinition* fieldDefinition = MetadataCache::GetFieldDefinitionFromIndex(fieldIndex);

            newField->type = MetadataCache::GetIl2CppTypeFromIndex(fieldDefinition->typeIndex);
            newField->name = MetadataCache::GetStringFromIndex(fieldDefinition->nameIndex);
            newField->parent = klass;
            newField->offset = MetadataCache::GetFieldOffsetFromIndexLocked(MetadataCache::GetIndexForTypeDefinition(klass), fieldIndex - start, newField, lock);
            newField->token = fieldDefinition->token;

            newField++;
        }

        klass->fields = fields;
    }

// passing lock to ensure we have acquired it. We can add asserts later
    void SetupFieldsLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->size_inited)
            return;

        if (klass->parent && !klass->parent->size_inited)
            SetupFieldsLocked(klass->parent, lock);

        if (klass->generic_class)
        {
            // for generic instance types, they just inflate the fields of their generic type definition
            // initialize the generic type definition and delegate to the generic logic
            InitLocked(GenericClass::GetTypeDefinition(klass->generic_class), lock);
            GenericClass::SetupFields(klass);
        }
        else
        {
            SetupFieldsFromDefinitionLocked(klass, lock);
        }

        if (!Class::IsGeneric(klass))
            LayoutFieldsLocked(klass, lock);

        klass->size_inited = true;
    }

    void Class::SetupFields(Il2CppClass *klass)
    {
        if (!klass->size_inited)
        {
            il2cpp::os::FastAutoLock lock(&g_MetadataLock);
            SetupFieldsLocked(klass, lock);
        }
    }

// passing lock to ensure we have acquired it. We can add asserts later
    void SetupMethodsLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if ((!klass->method_count && !klass->rank) || klass->methods)
            return;

        if (klass->generic_class)
        {
            InitLocked(GenericClass::GetTypeDefinition(klass->generic_class), lock);
            GenericClass::SetupMethods(klass);
        }
        else if (klass->rank)
        {
            InitLocked(klass->element_class, lock);
            SetupVTable(klass, lock);
        }
        else
        {
            if (klass->method_count == 0)
            {
                klass->methods = NULL;
                return;
            }

            klass->methods = (const MethodInfo**)IL2CPP_CALLOC(klass->method_count, sizeof(MethodInfo*));
            MethodInfo* methods = (MethodInfo*)IL2CPP_CALLOC(klass->method_count, sizeof(MethodInfo));
            MethodInfo* newMethod = methods;

            MethodIndex start = klass->typeDefinition->methodStart;
            IL2CPP_ASSERT(start != kFieldIndexInvalid);
            MethodIndex end = start + klass->method_count;

            for (MethodIndex index = start; index < end; ++index)
            {
                const Il2CppMethodDefinition* methodDefinition = MetadataCache::GetMethodDefinitionFromIndex(index);

                newMethod->name = MetadataCache::GetStringFromIndex(methodDefinition->nameIndex);
                newMethod->methodPointer = MetadataCache::GetMethodPointer(klass->image, methodDefinition->token);
                newMethod->invoker_method = MetadataCache::GetMethodInvoker(klass->image, methodDefinition->token);
                newMethod->klass = klass;
                newMethod->return_type = MetadataCache::GetIl2CppTypeFromIndex(methodDefinition->returnType);

                ParameterInfo* parameters = (ParameterInfo*)IL2CPP_CALLOC(methodDefinition->parameterCount, sizeof(ParameterInfo));
                ParameterInfo* newParameter = parameters;
                for (uint16_t paramIndex = 0; paramIndex < methodDefinition->parameterCount; ++paramIndex)
                {
                    const Il2CppParameterDefinition* parameterDefinition = MetadataCache::GetParameterDefinitionFromIndex(methodDefinition->parameterStart + paramIndex);
                    newParameter->name = MetadataCache::GetStringFromIndex(parameterDefinition->nameIndex);
                    newParameter->position = paramIndex;
                    newParameter->token = parameterDefinition->token;
                    newParameter->parameter_type = MetadataCache::GetIl2CppTypeFromIndex(parameterDefinition->typeIndex);

                    newParameter++;
                }
                newMethod->parameters = parameters;

                newMethod->flags = methodDefinition->flags;
                newMethod->iflags = methodDefinition->iflags;
                newMethod->slot = methodDefinition->slot;
                newMethod->parameters_count = static_cast<const uint8_t>(methodDefinition->parameterCount);
                newMethod->is_inflated = false;
                newMethod->token = methodDefinition->token;
                newMethod->methodDefinition = methodDefinition;
                newMethod->genericContainer = MetadataCache::GetGenericContainerFromIndex(methodDefinition->genericContainerIndex);
                if (newMethod->genericContainer)
                    newMethod->is_generic = true;

                klass->methods[index - start] = newMethod;

                newMethod++;
            }
        }
    }

    void Class::SetupMethods(Il2CppClass *klass)
    {
        if (klass->method_count || klass->rank)
        {
            il2cpp::os::FastAutoLock lock(&g_MetadataLock);
            SetupMethodsLocked(klass, lock);
        }
    }

    void SetupNestedTypesLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->generic_class || klass->nestedTypes)
            return;

        if (klass->nested_type_count > 0)
        {
            klass->nestedTypes = (Il2CppClass**)MetadataCalloc(klass->nested_type_count, sizeof(Il2CppClass*));
            for (uint16_t i = 0; i < klass->nested_type_count; i++)
                klass->nestedTypes[i] = MetadataCache::GetNestedTypeFromIndex(klass->typeDefinition->nestedTypesStart + i);
        }
    }

    void Class::SetupNestedTypes(Il2CppClass *klass)
    {
        if (klass->generic_class || klass->nestedTypes)
            return;

        if (klass->nested_type_count)
        {
            il2cpp::os::FastAutoLock lock(&g_MetadataLock);
            SetupNestedTypesLocked(klass, lock);
        }
    }

    static void SetupVTable(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->is_vtable_initialized)
            return;

        if (klass->generic_class)
        {
            Il2CppClass* genericTypeDefinition = GenericClass::GetTypeDefinition(klass->generic_class);
            Il2CppGenericContext* context = &klass->generic_class->context;
            if (genericTypeDefinition->interface_offsets_count > 0 && klass->interfaceOffsets == NULL)
            {
                klass->interface_offsets_count = genericTypeDefinition->interface_offsets_count;
                klass->interfaceOffsets = (Il2CppRuntimeInterfaceOffsetPair*)MetadataCalloc(genericTypeDefinition->interface_offsets_count, sizeof(Il2CppRuntimeInterfaceOffsetPair));
                for (uint16_t i = 0; i < genericTypeDefinition->interface_offsets_count; i++)
                {
                    Il2CppInterfaceOffsetPair interfaceOffset = MetadataCache::GetInterfaceOffsetIndex(genericTypeDefinition->typeDefinition->interfaceOffsetsStart + i);
                    klass->interfaceOffsets[i].offset = interfaceOffset.offset;
                    klass->interfaceOffsets[i].interfaceType = Class::FromIl2CppType(il2cpp::metadata::GenericMetadata::InflateIfNeeded(MetadataCache::GetIl2CppTypeFromIndex(interfaceOffset.interfaceTypeIndex), context, false));
                }
            }

            if (genericTypeDefinition->vtable_count > 0)
            {
                klass->vtable_count = genericTypeDefinition->vtable_count;
                for (uint16_t i = 0; i < genericTypeDefinition->vtable_count; i++)
                {
                    EncodedMethodIndex vtableMethodIndex = MetadataCache::GetVTableMethodFromIndex(genericTypeDefinition->typeDefinition->vtableStart + i);
                    const MethodInfo* method = MetadataCache::GetMethodInfoFromIndex(vtableMethodIndex);
                    if (GetEncodedIndexType(vtableMethodIndex) == kIl2CppMetadataUsageMethodRef)
                    {
                        const Il2CppGenericMethod* genericMethod = il2cpp::metadata::GenericMetadata::Inflate(method->genericMethod, context);
                        method = il2cpp::metadata::GenericMethod::GetMethod(genericMethod);
                    }
                    else if (method && Class::IsGeneric(method->klass))
                    {
                        const Il2CppGenericMethod* gmethod = MetadataCache::GetGenericMethod(method, context->class_inst, NULL);
                        method = il2cpp::metadata::GenericMethod::GetMethod(gmethod);
                    }

                    klass->vtable[i].method = method;
                    if (method != NULL)
                    {
                        if (method->methodPointer)
                            klass->vtable[i].methodPtr = method->methodPointer;
                        else if (method->is_inflated && !method->is_generic && !method->genericMethod->context.method_inst)
                            klass->vtable[i].methodPtr = MetadataCache::GetUnresolvedVirtualCallStub(method);
                    }
                }
            }
        }
        else if (klass->rank)
        {
            InitLocked(klass->element_class, lock);
            il2cpp::metadata::ArrayMetadata::SetupArrayVTable(klass, lock);
        }
        else
        {
            if (klass->interface_offsets_count > 0 && klass->interfaceOffsets == NULL)
            {
                klass->interfaceOffsets = (Il2CppRuntimeInterfaceOffsetPair*)MetadataCalloc(klass->interface_offsets_count, sizeof(Il2CppRuntimeInterfaceOffsetPair));
                for (uint16_t i = 0; i < klass->interface_offsets_count; i++)
                {
                    Il2CppInterfaceOffsetPair interfaceOffset = MetadataCache::GetInterfaceOffsetIndex(klass->typeDefinition->interfaceOffsetsStart + i);
                    klass->interfaceOffsets[i].offset = interfaceOffset.offset;
                    klass->interfaceOffsets[i].interfaceType = Class::FromIl2CppType(MetadataCache::GetIl2CppTypeFromIndex(interfaceOffset.interfaceTypeIndex));
                }
            }

            if (klass->vtable_count > 0)
            {
                for (uint16_t i = 0; i < klass->vtable_count; i++)
                {
                    const MethodInfo* method = MetadataCache::GetMethodInfoFromIndex(MetadataCache::GetVTableMethodFromIndex(klass->typeDefinition->vtableStart + i));
                    klass->vtable[i].method = method;

                    if (method != NULL)
                        klass->vtable[i].methodPtr = method->methodPointer;
                }
            }
        }

        klass->is_vtable_initialized = 1;
    }

    static void SetupEventsLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->generic_class)
        {
            InitLocked(GenericClass::GetTypeDefinition(klass->generic_class), lock);
            GenericClass::SetupEvents(klass);
        }
        else if (klass->rank > 0)
        {
            // do nothing, arrays have no events
            IL2CPP_ASSERT(klass->event_count == 0);
        }
        else if (klass->event_count != 0)
        {
            // we need methods initialized since we reference them via index below
            SetupMethodsLocked(klass, lock);

            EventInfo* events = (EventInfo*)IL2CPP_CALLOC(klass->event_count, sizeof(EventInfo));
            EventInfo* newEvent = events;

            EventIndex start = klass->typeDefinition->eventStart;
            IL2CPP_ASSERT(klass->typeDefinition->eventStart != kEventIndexInvalid);
            EventIndex end = start + klass->event_count;

            for (EventIndex eventIndex = start; eventIndex < end; ++eventIndex)
            {
                const Il2CppEventDefinition* eventDefinition = MetadataCache::GetEventDefinitionFromIndex(eventIndex);

                newEvent->eventType = MetadataCache::GetIl2CppTypeFromIndex(eventDefinition->typeIndex);
                newEvent->name = MetadataCache::GetStringFromIndex(eventDefinition->nameIndex);
                newEvent->parent = klass;

                if (eventDefinition->add != kMethodIndexInvalid)
                    newEvent->add = klass->methods[eventDefinition->add];

                if (eventDefinition->remove != kMethodIndexInvalid)
                    newEvent->remove = klass->methods[eventDefinition->remove];

                if (eventDefinition->raise != kMethodIndexInvalid)
                    newEvent->raise = klass->methods[eventDefinition->raise];

                newEvent->token = eventDefinition->token;

                newEvent++;
            }

            klass->events = events;
        }
    }

    void Class::SetupEvents(Il2CppClass *klass)
    {
        if (!klass->events && klass->event_count)
        {
            il2cpp::os::FastAutoLock lock(&g_MetadataLock);
            SetupEventsLocked(klass, lock);
        }
    }

    static void SetupPropertiesLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->generic_class)
        {
            InitLocked(GenericClass::GetTypeDefinition(klass->generic_class), lock);
            GenericClass::SetupProperties(klass);
        }
        else if (klass->property_count != 0)
        {
            // we need methods initialized since we reference them via index below
            SetupMethodsLocked(klass, lock);

            PropertyInfo* properties = (PropertyInfo*)IL2CPP_CALLOC(klass->property_count, sizeof(PropertyInfo));
            PropertyInfo* newProperty = properties;

            PropertyIndex start = klass->typeDefinition->propertyStart;
            IL2CPP_ASSERT(klass->typeDefinition->propertyStart != kPropertyIndexInvalid);
            PropertyIndex end = start + klass->property_count;

            for (PropertyIndex propertyIndex = start; propertyIndex < end; ++propertyIndex)
            {
                const Il2CppPropertyDefinition* propertyDefinition = MetadataCache::GetPropertyDefinitionFromIndex(propertyIndex);

                newProperty->name = MetadataCache::GetStringFromIndex(propertyDefinition->nameIndex);
                newProperty->parent = klass;

                if (propertyDefinition->get != kMethodIndexInvalid)
                    newProperty->get = klass->methods[propertyDefinition->get];

                if (propertyDefinition->set != kMethodIndexInvalid)
                    newProperty->set = klass->methods[propertyDefinition->set];

                newProperty->attrs = propertyDefinition->attrs;
                newProperty->token = propertyDefinition->token;

                newProperty++;
            }

            klass->properties = properties;
        }
    }

    void Class::SetupProperties(Il2CppClass *klass)
    {
        if (!klass->properties && klass->property_count)
        {
            il2cpp::os::FastAutoLock lock(&g_MetadataLock);
            SetupPropertiesLocked(klass, lock);
        }
    }

    static void SetupTypeHierarchyLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->typeHierarchy != NULL)
            return;

        if (klass->parent && !klass->parent->typeHierarchy)
            SetupTypeHierarchyLocked(klass->parent, lock);
        if (klass->parent)
            klass->typeHierarchyDepth = klass->parent->typeHierarchyDepth + 1;
        else
            klass->typeHierarchyDepth = 1;

        klass->typeHierarchy = (Il2CppClass**)MetadataCalloc(klass->typeHierarchyDepth, sizeof(Il2CppClass*));

        if (klass->parent)
        {
            klass->typeHierarchy[klass->typeHierarchyDepth - 1] = klass;
            memcpy(klass->typeHierarchy, klass->parent->typeHierarchy, klass->parent->typeHierarchyDepth * sizeof(void*));
        }
        else
        {
            klass->typeHierarchy[0] = klass;
        }
    }

    void Class::SetupTypeHierarchy(Il2CppClass *klass)
    {
        il2cpp::os::FastAutoLock lock(&g_MetadataLock);
        SetupTypeHierarchyLocked(klass, lock);
    }

    void Class::SetupInterfaces(Il2CppClass *klass)
    {
        il2cpp::os::FastAutoLock lock(&g_MetadataLock);
        SetupInterfacesLocked(klass, lock);
    }

    static bool InitLocked(Il2CppClass *klass, const il2cpp::os::FastAutoLock& lock)
    {
        if (klass->initialized)
            return true;

        if (klass->generic_class && (klass->flags & TYPE_ATTRIBUTE_EXPLICIT_LAYOUT))
        {
            std::string message;
            message += "Could not load type '";
            message += klass->namespaze;
            message += ":";
            message += klass->name;
            message += "' because generic types cannot have explicit layout.";
            klass->has_initialization_error = true;
            Class::UpdateInitializedAndNoError(klass);
            klass->initializationExceptionGCHandle = gc::GCHandle::New(il2cpp::vm::Exception::GetTypeLoadException(message.c_str()), false);
            return false;
        }

        IL2CPP_NOT_IMPLEMENTED_NO_ASSERT(Class::Init, "Audit and compare to mono version");

        klass->init_pending = true;

        klass->genericRecursionDepth++;

        if (klass->generic_class)
            InitLocked(GenericClass::GetTypeDefinition(klass->generic_class), lock);

        if (klass->byval_arg.type == IL2CPP_TYPE_ARRAY || klass->byval_arg.type == IL2CPP_TYPE_SZARRAY)
        {
            Il2CppClass *element_class = klass->element_class;
            if (!element_class->initialized)
                InitLocked(element_class, lock);
        }

        SetupInterfacesLocked(klass, lock);

        if (klass->parent && !klass->parent->initialized)
            InitLocked(klass->parent, lock);

        SetupMethodsLocked(klass, lock);
        SetupTypeHierarchyLocked(klass, lock);
        SetupVTable(klass, lock);
        if (!klass->size_inited)
            SetupFieldsLocked(klass, lock);

        if (klass->has_initialization_error)
            return false;

        SetupEventsLocked(klass, lock);
        SetupPropertiesLocked(klass, lock);
        SetupNestedTypesLocked(klass, lock);

        if (klass == il2cpp_defaults.object_class)
        {
            for (uint16_t slot = 0; slot < klass->vtable_count; slot++)
            {
                const MethodInfo* vmethod = klass->vtable[slot].method;
                if (!strcmp(vmethod->name, "GetHashCode"))
                    s_GetHashCodeSlot = slot;
                else if (!strcmp(vmethod->name, "Finalize"))
                    s_FinalizerSlot = slot;
            }
#if !IL2CPP_DOTS
            IL2CPP_ASSERT(s_FinalizerSlot > 0);
            IL2CPP_ASSERT(s_GetHashCodeSlot > 0);
#endif
        }

        if (!Class::IsGeneric(klass))
            SetupGCDescriptor(klass);

        if (klass->generic_class)
        {
            if (klass->genericRecursionDepth < il2cpp::metadata::GenericMetadata::MaximumRuntimeGenericDepth)
                klass->rgctx_data = il2cpp::metadata::GenericMetadata::InflateRGCTX(klass->image, klass->token, &klass->generic_class->context);
        }

        klass->initialized = true;
        Class::UpdateInitializedAndNoError(klass);
        klass->init_pending = false;

        ++il2cpp_runtime_stats.initialized_class_count;

        return true;
    }

    bool Class::Init(Il2CppClass *klass)
    {
        IL2CPP_ASSERT(klass);

        if (!klass->initialized)
        {
            il2cpp::os::FastAutoLock lock(&g_MetadataLock);
            InitLocked(klass, lock);
        }

        return true;
    }

    void Class::UpdateInitializedAndNoError(Il2CppClass *klass)
    {
        klass->initialized_and_no_error = klass->initialized && !klass->has_initialization_error;
    }

    Il2CppClass* Class::FromName(const Il2CppImage* image, const char* namespaze, const char *name)
    {
        return Image::ClassFromName(image, namespaze, name);
    }

    Il2CppClass* Class::GetArrayClass(Il2CppClass *element_class, uint32_t rank)
    {
        return GetBoundedArrayClass(element_class, rank, false);
    }

    Il2CppClass* Class::GetBoundedArrayClass(Il2CppClass *eclass, uint32_t rank, bool bounded)
    {
        return il2cpp::metadata::ArrayMetadata::GetBoundedArrayClass(eclass, rank, bounded);
    }

    Il2CppClass* Class::GetInflatedGenericInstanceClass(Il2CppClass* klass, const il2cpp::metadata::Il2CppTypeVector& types)
    {
        return GetInflatedGenericInstanceClass(klass, MetadataCache::GetGenericInst(types));
    }

    Il2CppClass* Class::GetInflatedGenericInstanceClass(Il2CppClass* klass, const Il2CppGenericInst* genericInst)
    {
        IL2CPP_ASSERT(Class::IsGeneric(klass));

        Il2CppGenericClass* gclass = il2cpp::metadata::GenericMetadata::GetGenericClass(klass, genericInst);
        return GenericClass::GetClass(gclass);
    }

    Il2CppClass* Class::InflateGenericClass(Il2CppClass* klass, Il2CppGenericContext *context)
    {
        const Il2CppType* inflated = InflateGenericType(&klass->byval_arg, context);

        return FromIl2CppType(inflated);
    }

    const Il2CppType* Class::InflateGenericType(const Il2CppType* type, Il2CppGenericContext* context)
    {
        return il2cpp::metadata::GenericMetadata::InflateIfNeeded(type, context, true);
    }

    bool Class::HasDefaultConstructor(Il2CppClass* klass)
    {
        const char ctorName[] = ".ctor";
        void* iter = NULL;
        while (const MethodInfo* method = Class::GetMethods(klass, &iter))
        {
            if (strncmp(method->name, ctorName, utils::StringUtils::LiteralLength(ctorName)) == 0 && method->parameters_count == 0)
                return true;
        }

        return false;
    }

    int Class::GetFlags(const Il2CppClass *klass)
    {
        return klass->flags;
    }

    bool Class::IsAbstract(const Il2CppClass *klass)
    {
        return (klass->flags & TYPE_ATTRIBUTE_ABSTRACT) != 0;
    }

    bool Class::IsInterface(const Il2CppClass *klass)
    {
        return (klass->flags & TYPE_ATTRIBUTE_INTERFACE) || (klass->byval_arg.type == IL2CPP_TYPE_VAR) || (klass->byval_arg.type == IL2CPP_TYPE_MVAR);
    }

    bool Class::IsNullable(const Il2CppClass *klass)
    {
        return klass->generic_class != NULL &&
            GenericClass::GetTypeDefinition(klass->generic_class) == il2cpp_defaults.generic_nullable_class;
    }

    Il2CppClass* Class::GetNullableArgument(const Il2CppClass* klass)
    {
        return Class::FromIl2CppType(klass->generic_class->context.class_inst->type_argv[0]);
    }

    int Class::GetArrayElementSize(const Il2CppClass *klass)
    {
        const Il2CppType *type = &klass->byval_arg;

    handle_enum:
        switch (type->type)
        {
            case IL2CPP_TYPE_I1:
            case IL2CPP_TYPE_U1:
            case IL2CPP_TYPE_BOOLEAN:
                return 1;

            case IL2CPP_TYPE_I2:
            case IL2CPP_TYPE_U2:
            case IL2CPP_TYPE_CHAR:
                return 2;

            case IL2CPP_TYPE_I4:
            case IL2CPP_TYPE_U4:
            case IL2CPP_TYPE_R4:
                return 4;

            case IL2CPP_TYPE_I:
            case IL2CPP_TYPE_U:
            case IL2CPP_TYPE_PTR:
            case IL2CPP_TYPE_CLASS:
            case IL2CPP_TYPE_STRING:
            case IL2CPP_TYPE_OBJECT:
            case IL2CPP_TYPE_SZARRAY:
            case IL2CPP_TYPE_ARRAY:
            case IL2CPP_TYPE_VAR:
            case IL2CPP_TYPE_MVAR:
                return sizeof(Il2CppObject*);

            case IL2CPP_TYPE_I8:
            case IL2CPP_TYPE_U8:
            case IL2CPP_TYPE_R8:
                return 8;

            case IL2CPP_TYPE_VALUETYPE:
                if (Type::IsEnum(type))
                {
                    type = Class::GetEnumBaseType(Type::GetClass(type));
                    klass = klass->element_class;
                    goto handle_enum;
                }
                return Class::GetInstanceSize(klass) - sizeof(Il2CppObject);

            case IL2CPP_TYPE_GENERICINST:
                type = &GenericClass::GetTypeDefinition(type->data.generic_class)->byval_arg;
                goto handle_enum;

            case IL2CPP_TYPE_VOID:
                return 0;

            default:
                // g_error ("unknown type 0x%02x in mono_class_array_element_size", type->type);
                IL2CPP_NOT_IMPLEMENTED_NO_ASSERT(Class::GetArrayElementSize, "Should throw a nice error");
        }

        return -1;
    }

    const Il2CppType* Class::GetByrefType(Il2CppClass *klass)
    {
        return &klass->this_arg;
    }

    const Il2CppType* Class::GetType(Il2CppClass *klass)
    {
        return &klass->byval_arg;
    }

    const Il2CppType* Class::GetType(Il2CppClass *klass, const TypeNameParseInfo &info)
    {
        // Attempt to resolve a generic type definition.
        if (Class::IsGeneric(klass))
            klass = ResolveGenericInstanceType(klass, info, kTypeSearchFlagNone);

        bool bounded = false;

        std::vector<int32_t>::const_iterator it = info.modifiers().begin();

        if (info.is_bounded())
        {
            bounded = false;
        }

        while (it != info.modifiers().end())
        {
            if (*it == 0)
            {
                // byref is always the last modifier, so we can return here.
                return &klass->this_arg;
            }

            if (*it == -1)
            {
                klass = Class::GetPtrClass(klass);
            }
            else if (*it == -2)
            {
                bounded = true;
            }
            else
            {
                klass = Class::GetBoundedArrayClass(klass, *it, bounded);
            }

            ++it;
        }

        if (klass == NULL)
            return NULL;

        return &klass->byval_arg;
    }

    bool Class::HasAttribute(Il2CppClass *klass, Il2CppClass *attr_class)
    {
        return Reflection::HasAttribute(klass, attr_class);
    }

    bool Class::IsEnum(const Il2CppClass *klass)
    {
        return klass->enumtype;
    }

    const Il2CppImage* Class::GetImage(Il2CppClass *klass)
    {
        return klass->image;
    }

    const Il2CppGenericContainer* Class::GetGenericContainer(Il2CppClass *klass)
    {
        return MetadataCache::GetGenericContainerFromIndex(klass->genericContainerIndex);
    }

    const MethodInfo* Class::GetCCtor(Il2CppClass *klass)
    {
        if (!klass->has_cctor)
            return NULL;

        return GetMethodFromNameFlags(klass, ".cctor", IgnoreNumberOfArguments, METHOD_ATTRIBUTE_SPECIAL_NAME);
    }

    const char* Class::GetFieldDefaultValue(const FieldInfo *field, const Il2CppType** type)
    {
        const Il2CppFieldDefaultValue *entry = MetadataCache::GetFieldDefaultValueForField(field);
        if (entry != NULL)
        {
            *type = MetadataCache::GetIl2CppTypeFromIndex(entry->typeIndex);
            if (entry->dataIndex == kDefaultValueIndexNull)
                return NULL;

            return (const char*)MetadataCache::GetFieldDefaultValueDataFromIndex(entry->dataIndex);
        }
        return NULL;
    }

    int Class::GetFieldMarshaledSize(const FieldInfo *field)
    {
        int marshaledFieldSize = MetadataCache::GetFieldMarshaledSizeForField(field);
        if (marshaledFieldSize != -1)
            return marshaledFieldSize;

        if (field->type->type == IL2CPP_TYPE_BOOLEAN)
            return 4;
        if (field->type->type == IL2CPP_TYPE_CHAR)
            return 1;

        size_t size = il2cpp::metadata::FieldLayout::GetTypeSizeAndAlignment(field->type).size;
        IL2CPP_ASSERT(size < static_cast<size_t>(std::numeric_limits<int>::max()));
        return static_cast<int>(size);
    }

    Il2CppClass* Class::GetPtrClass(const Il2CppType* type)
    {
        return GetPtrClass(Class::FromIl2CppType(type));
    }

    Il2CppClass* Class::GetPtrClass(Il2CppClass* elementClass)
    {
        il2cpp::os::FastAutoLock lock(&g_MetadataLock);

        Il2CppClass* pointerClass = MetadataCache::GetPointerType(elementClass);
        if (pointerClass)
            return pointerClass;

        pointerClass = (Il2CppClass*)MetadataCalloc(1, sizeof(Il2CppClass));
        pointerClass->klass = pointerClass;

        pointerClass->namespaze = elementClass->namespaze;
        pointerClass->name = il2cpp::utils::StringUtils::StringDuplicate(il2cpp::utils::StringUtils::Printf("%s*", elementClass->name).c_str());

        pointerClass->image = elementClass->image;
        pointerClass->initialized = true;
        pointerClass->flags = TYPE_ATTRIBUTE_CLASS | (elementClass->flags & TYPE_ATTRIBUTE_VISIBILITY_MASK);
        pointerClass->instance_size = sizeof(void*);

        pointerClass->this_arg.type = pointerClass->byval_arg.type = IL2CPP_TYPE_PTR;
        pointerClass->this_arg.data.type = pointerClass->byval_arg.data.type = &elementClass->byval_arg;
        pointerClass->this_arg.byref = true;

        pointerClass->parent = NULL;
        pointerClass->castClass = pointerClass->element_class = elementClass;

        MetadataCache::AddPointerType(elementClass, pointerClass);

        return pointerClass;
    }

    bool Class::HasReferences(Il2CppClass *klass)
    {
        if (klass->init_pending)
        {
            /* Be conservative */
            return true;
        }
        else
        {
            Init(klass);

            return klass->has_references;
        }
    }

    const il2cpp::utils::dynamic_array<Il2CppClass*>& Class::GetStaticFieldData()
    {
        return s_staticFieldData;
    }

    const size_t kWordSize = (8 * sizeof(size_t));

    static inline void set_bit(size_t* bitmap, size_t index)
    {
        bitmap[index / kWordSize] |= (size_t)1 << (index % kWordSize);
    }

    size_t Class::GetBitmapSize(const Il2CppClass* klass)
    {
        size_t maxBits = klass->instance_size / sizeof(void*);
        size_t maxWords = 1 + (maxBits / sizeof(size_t));
        return sizeof(size_t) * maxWords;
    }

    void Class::GetBitmap(Il2CppClass* klass, size_t* bitmap, size_t& maxSetBit)
    {
        Class::Init(klass);
        return il2cpp::vm::GetBitmapNoInit(klass, bitmap, maxSetBit, 0);
    }

    const char *Class::GetAssemblyName(const Il2CppClass *klass)
    {
        return klass->image->name;
    }

    const char *Class::GetAssemblyNameNoExtension(const Il2CppClass *klass)
    {
        return klass->image->nameNoExt;
    }

    void GetBitmapNoInit(Il2CppClass* klass, size_t* bitmap, size_t& maxSetBit, size_t parentOffset)
    {
        Il2CppClass* currentClass = klass;

        while (currentClass)
        {
            for (uint16_t index = 0; index < currentClass->field_count; index++)
            {
                FieldInfo* field = currentClass->fields + index;
                if (field->type->attrs & (FIELD_ATTRIBUTE_STATIC | FIELD_ATTRIBUTE_HAS_FIELD_RVA))
                    continue;

                IL2CPP_ASSERT(!field->type->byref);

                size_t offset = parentOffset + field->offset;

                const Il2CppType* type = Type::GetUnderlyingType(field->type);

                switch (type->type)
                {
                    case IL2CPP_TYPE_I1:
                    case IL2CPP_TYPE_U1:
                    case IL2CPP_TYPE_BOOLEAN:
                    case IL2CPP_TYPE_I2:
                    case IL2CPP_TYPE_U2:
                    case IL2CPP_TYPE_CHAR:
                    case IL2CPP_TYPE_I4:
                    case IL2CPP_TYPE_U4:
                    case IL2CPP_TYPE_I8:
                    case IL2CPP_TYPE_U8:
                    case IL2CPP_TYPE_I:
                    case IL2CPP_TYPE_U:
                    case IL2CPP_TYPE_R4:
                    case IL2CPP_TYPE_R8:
                    case IL2CPP_TYPE_PTR:
                    case IL2CPP_TYPE_FNPTR:
                        break;
                    case IL2CPP_TYPE_STRING:
                    case IL2CPP_TYPE_SZARRAY:
                    case IL2CPP_TYPE_CLASS:
                    case IL2CPP_TYPE_OBJECT:
                    case IL2CPP_TYPE_ARRAY:
                    case IL2CPP_TYPE_VAR:
                    case IL2CPP_TYPE_MVAR:
                        IL2CPP_ASSERT(0 == (field->offset % sizeof(void*)));
                        set_bit(bitmap, offset / sizeof(void*));
                        maxSetBit = std::max(maxSetBit, offset / sizeof(void*));
                        break;
                    case IL2CPP_TYPE_GENERICINST:
                        if (!Type::GenericInstIsValuetype(type))
                        {
                            IL2CPP_ASSERT(0 == (field->offset % sizeof(void*)));
                            set_bit(bitmap, offset / sizeof(void*));
                            maxSetBit = std::max(maxSetBit, offset / sizeof(void*));
                            break;
                        }
                        else
                        {
                            /* fall through */
                        }
                    case IL2CPP_TYPE_VALUETYPE:
                    {
                        Il2CppClass* fieldClass = Class::FromIl2CppType(field->type);
                        Class::Init(fieldClass);
                        if (fieldClass->has_references)
                            GetBitmapNoInit(fieldClass, bitmap, maxSetBit, offset - sizeof(Il2CppObject) /* nested field offset includes padding for boxed structure. Remove for struct fields */);
                        break;
                    }
                    default:
                        IL2CPP_NOT_IMPLEMENTED(Class::GetClassBitmap);
                        break;
                }
            }

            currentClass = currentClass->parent;
        }
    }

    void SetupGCDescriptor(Il2CppClass* klass)
    {
        const size_t kMaxAllocaSize = 1024;
        size_t bitmapSize = Class::GetBitmapSize(klass);
        size_t* bitmap = NULL;
        std::vector<size_t> buffer(0);

        if (bitmapSize > kMaxAllocaSize)
        {
            buffer.resize(bitmapSize / sizeof(size_t));
            bitmap = (size_t*)&buffer[0];
        }
        else
        {
            bitmap = (size_t*)alloca(bitmapSize);
        }

        memset(bitmap, 0, bitmapSize);
        size_t maxSetBit = 0;
        GetBitmapNoInit(klass, bitmap, maxSetBit, 0);

        if (klass == il2cpp_defaults.string_class)
            klass->gc_desc = il2cpp::gc::GarbageCollector::MakeDescriptorForString();
        else if (klass->rank)
            klass->gc_desc = il2cpp::gc::GarbageCollector::MakeDescriptorForArray();
        else
            klass->gc_desc = il2cpp::gc::GarbageCollector::MakeDescriptorForObject(bitmap, (int)maxSetBit + 1);
    }

#define CHECK_IF_NULL(v)    \
    if ( (v) == NULL && (searchFlags & kTypeSearchFlagThrowOnError) != 0 ) \
        Exception::Raise (Exception::GetTypeLoadException (info)); \
    if ( (v) == NULL ) \
        return NULL;

    static Il2CppClass * resolve_generic_instance_internal(const il2cpp::vm::TypeNameParseInfo &info, Il2CppClass *generic_class, il2cpp::metadata::Il2CppTypeVector &generic_arguments, TypeSearchFlags searchFlags)
    {
        Il2CppClass *klass = NULL;

        const Il2CppGenericContainer* container = Class::GetGenericContainer(generic_class);
        if (container->type_argc != generic_arguments.size())
            il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentException("name", "The number of generic arguments provided doesn't equal the arity of the generic type definition."));

        if (info.assembly_name().name.empty())
        {
            const Il2CppImage* image = Image::GetExecutingImage();
            klass = MetadataCache::GetGenericInstanceType(generic_class, generic_arguments);

            if (klass == NULL && image != Image::GetCorlib())
            {
                // Try mscorlib
                image = (Il2CppImage*)Image::GetCorlib();
                klass = MetadataCache::GetGenericInstanceType(generic_class, generic_arguments);
            }
        }
        else
        {
            const Il2CppAssembly *assembly = Assembly::Load(info.assembly_name().name.c_str());

            CHECK_IF_NULL(assembly);

            Il2CppImage *image = (Il2CppImage*)Assembly::GetImage(assembly);

            CHECK_IF_NULL(image);

            klass = MetadataCache::GetGenericInstanceType(generic_class, generic_arguments);
        }

        return klass;
    }

    static Il2CppClass* ResolveGenericInstanceType(Il2CppClass* klass, const TypeNameParseInfo& info, TypeSearchFlags searchFlags)
    {
        if (info.has_generic_arguments())
        {
            il2cpp::metadata::Il2CppTypeVector generic_arguments;
            generic_arguments.reserve(info.type_arguments().size());

            std::vector<TypeNameParseInfo>::const_iterator it = info.type_arguments().begin();
            while (it != info.type_arguments().end())
            {
                const Il2CppType * generic_argument = Class::il2cpp_type_from_type_info(*it, searchFlags);

                CHECK_IF_NULL(generic_argument);

                generic_arguments.push_back(generic_argument);

                ++it;
            }

            klass = resolve_generic_instance_internal(info, klass, generic_arguments, searchFlags);

            CHECK_IF_NULL(klass);
        }

        if (klass != NULL)
            Class::Init(klass);

        return klass;
    }

    static Il2CppClass* resolve_parse_info_internal(const TypeNameParseInfo& info, TypeSearchFlags searchFlags)
    {
        Il2CppClass *klass = NULL;

        if (info.assembly_name().name.empty())
        {
            const Il2CppImage* image;
            if (searchFlags & kTypeSearchFlagDontUseExecutingImage)
            {
                image = Image::GetCorlib();
            }
            else
            {
                image = Image::GetExecutingImage();
            }

            klass = Image::FromTypeNameParseInfo(image, info, searchFlags & kTypeSearchFlagIgnoreCase);
            if (klass == NULL && image != Image::GetCorlib())
            {
                // Try mscorlib
                image = (Il2CppImage*)Image::GetCorlib();
                klass = Image::FromTypeNameParseInfo(image, info, searchFlags & kTypeSearchFlagIgnoreCase);
            }
        }
        else
        {
            const Il2CppAssembly *assembly = Assembly::Load(info.assembly_name().name.c_str());

            CHECK_IF_NULL(assembly);

            Il2CppImage *image = (Il2CppImage*)Assembly::GetImage(assembly);

            CHECK_IF_NULL(image);

            klass = Image::FromTypeNameParseInfo(image, info, searchFlags & kTypeSearchFlagIgnoreCase);
        }

        return klass;
    }

    const Il2CppType* Class::il2cpp_type_from_type_info(const TypeNameParseInfo& info, TypeSearchFlags searchFlags)
    {
        Il2CppClass *klass = resolve_parse_info_internal(info, searchFlags);

        CHECK_IF_NULL(klass);

        klass = ResolveGenericInstanceType(klass, info, searchFlags);

        CHECK_IF_NULL(klass);

        const Il2CppType *type = Class::GetType(klass, info);

        CHECK_IF_NULL(type);

        return type;
    }

    Il2CppClass* Class::GetDeclaringType(Il2CppClass* klass)
    {
        return klass->declaringType;
    }
} /* namespace vm */
} /* namespace il2cpp */
