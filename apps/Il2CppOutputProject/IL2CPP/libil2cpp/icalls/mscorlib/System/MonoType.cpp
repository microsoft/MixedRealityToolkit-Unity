#include "il2cpp-config.h"
#include "il2cpp-class-internals.h"
#include "il2cpp-object-internals.h"
#include "il2cpp-tabledefs.h"
#include "gc/Allocator.h"
#include "icalls/mscorlib/System/MonoType.h"
#include "utils/Functional.h"
#include "utils/StringUtils.h"
#include "utils/Il2CppHashMap.h"
#include "vm/Array.h"
#include "vm/Class.h"
#include "vm/Field.h"
#include "vm/GenericClass.h"
#include "vm/GenericContainer.h"
#include "vm/Method.h"
#include "vm/MetadataCache.h"
#include "vm/Reflection.h"
#include "vm/String.h"
#include "vm/Type.h"
#include "vm/Exception.h"
#include "vm-utils/VmStringUtils.h"

#include <vector>
#include <set>

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    struct Il2CppEventInfoHash
    {
        size_t operator()(const EventInfo* eventInfo) const
        {
            return il2cpp::utils::StringUtils::Hash(eventInfo->name);
        }
    };

    struct Il2CppEventInfoCompare
    {
        bool operator()(const EventInfo* event1, const EventInfo* event2) const
        {
            // You can't overload events
            return strcmp(event1->name, event2->name) == 0;
        }
    };

    struct PropertyPair
    {
        const PropertyInfo *property;
        Il2CppClass* originalType;

        PropertyPair(const PropertyInfo *property, Il2CppClass* originalType) : property(property), originalType(originalType)
        {
        }
    };

    typedef std::vector<PropertyPair> PropertyPairVector;

    typedef Il2CppHashMap<const EventInfo*, Il2CppClass*, Il2CppEventInfoHash, Il2CppEventInfoCompare> EventMap;

    bool MonoType::PropertyEqual(const PropertyInfo* prop1, const PropertyInfo* prop2)
    {
        // Name check is not enough, property can be overloaded
        if (strcmp(prop1->name, prop2->name) != 0)
            return false;

        return vm::Method::IsSameOverloadSignature(prop1, prop2);
    }

    static bool PropertyPairVectorContains(const PropertyPairVector& properties, const PropertyInfo* property)
    {
        for (PropertyPairVector::const_iterator it = properties.begin(), end = properties.end(); it != end; ++it)
            if (MonoType::PropertyEqual(it->property, property))
                return true;

        return false;
    }

    static inline bool IsPublic(const FieldInfo* field)
    {
        return (field->type->attrs & FIELD_ATTRIBUTE_FIELD_ACCESS_MASK) == FIELD_ATTRIBUTE_PUBLIC;
    }

    static inline bool IsPrivate(const FieldInfo* field)
    {
        return (field->type->attrs & FIELD_ATTRIBUTE_FIELD_ACCESS_MASK) == FIELD_ATTRIBUTE_PRIVATE;
    }

    static inline bool IsStatic(const FieldInfo* field)
    {
        return (field->type->attrs & FIELD_ATTRIBUTE_STATIC) != 0;
    }

    static inline bool IsPublic(const PropertyInfo* property)
    {
        if (property->get != NULL && (property->get->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC)
            return true;

        if (property->set != NULL && (property->set->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC)
            return true;

        return false;
    }

    static inline bool IsPrivate(const PropertyInfo* property)
    {
        if (property->get != NULL && (property->get->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) != METHOD_ATTRIBUTE_PRIVATE)
            return false;

        if (property->set != NULL && (property->set->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) != METHOD_ATTRIBUTE_PRIVATE)
            return false;

        return true;
    }

    static inline bool IsStatic(const PropertyInfo* property)
    {
        if (property->get != NULL)
            return (property->get->flags & METHOD_ATTRIBUTE_STATIC) != 0;

        if (property->set != NULL)
            return (property->set->flags & METHOD_ATTRIBUTE_STATIC) != 0;

        return false;
    }

    static inline bool IsPublic(const MethodInfo* method)
    {
        return (method->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC;
    }

    static inline bool IsPrivate(const MethodInfo* method)
    {
        return (method->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PRIVATE;
    }

    static inline bool IsStatic(const MethodInfo* method)
    {
        return (method->flags & METHOD_ATTRIBUTE_STATIC) != 0;
    }

// From MSDN: An event is considered public to reflection if it has at least one method or accessor that is public.
    static inline bool IsPublic(const EventInfo* event)
    {
        if (event->add != NULL && (event->add->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC)
            return true;

        if (event->remove != NULL && (event->remove->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC)
            return true;

        if (event->raise != NULL && (event->raise->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC)
            return true;

        return false;
    }

    static inline bool IsPrivate(const EventInfo* event)
    {
        if (event->add != NULL && (event->add->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) != METHOD_ATTRIBUTE_PRIVATE)
            return false;

        if (event->remove != NULL && (event->remove->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) != METHOD_ATTRIBUTE_PRIVATE)
            return false;

        if (event->raise != NULL && (event->raise->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) != METHOD_ATTRIBUTE_PRIVATE)
            return false;

        return true;
    }

    static inline bool IsStatic(const EventInfo* event)
    {
        if (event->add != NULL)
            return (event->add->flags & METHOD_ATTRIBUTE_STATIC) != 0;

        if (event->remove != NULL)
            return (event->remove->flags & METHOD_ATTRIBUTE_STATIC) != 0;

        if (event->raise != NULL)
            return (event->raise->flags & METHOD_ATTRIBUTE_STATIC) != 0;

        return false;
    }

    template<typename MemberInfo, typename NameFilter>
    static bool CheckMemberMatch(const MemberInfo* member, const Il2CppClass* type, const Il2CppClass* originalType, int32_t bindingFlags, const NameFilter& nameFilter)
    {
        uint32_t accessBindingFlag = IsPublic(member) ? BFLAGS_Public : BFLAGS_NonPublic;

        if ((bindingFlags & accessBindingFlag) == 0)
            return false;

        if (type != originalType && IsPrivate(member)) // Private members are not part of derived class
            return false;

        if (IsStatic(member))
        {
            if ((bindingFlags & BFLAGS_Static) == 0)
                return false;

            if ((bindingFlags & BFLAGS_FlattenHierarchy) == 0 && type != originalType)
                return false;
        }
        else if ((bindingFlags & BFLAGS_Instance) == 0)
        {
            return false;
        }

        if (!nameFilter(member->name))
            return false;

        return true;
    }

    static inline bool ValidBindingFlagsForGetMember(uint32_t bindingFlags)
    {
        return (bindingFlags & BFLAGS_Static) != 0 || (bindingFlags & BFLAGS_Instance) != 0;
    }

    Il2CppReflectionAssembly* MonoType::get_Assembly(Il2CppReflectionType* type)
    {
        Il2CppClass* klass = vm::Class::FromIl2CppType(type->type);

        return il2cpp::vm::Reflection::GetAssemblyObject(klass->image->assembly);
    }

    int MonoType::get_attributes(Il2CppReflectionType *type)
    {
        Il2CppClass *klass = vm::Class::FromSystemType(type);

        return klass->flags;
    }

    int MonoType::GetArrayRank(Il2CppReflectionType *type)
    {
        if (type->type->type != IL2CPP_TYPE_ARRAY && type->type->type != IL2CPP_TYPE_SZARRAY)
            IL2CPP_ASSERT("Type must be an array type");

        Il2CppClass* klass = vm::Class::FromIl2CppType(type->type);
        return klass->rank;
    }

    Il2CppReflectionType* MonoType::get_DeclaringType(Il2CppReflectionMonoType *monoType)
    {
        return vm::Type::GetDeclaringType(monoType->GetIl2CppType());
    }

    bool MonoType::IsGenericParameter(const Il2CppType *type)
    {
        return !type->byref && (type->type == IL2CPP_TYPE_VAR || type->type == IL2CPP_TYPE_MVAR);
    }

    bool MonoType::get_IsGenericParameter(Il2CppReflectionType* type)
    {
        return IsGenericParameter(type->type);
    }

    Il2CppReflectionModule* MonoType::get_Module(Il2CppReflectionType* type)
    {
        Il2CppClass* klass = vm::Class::FromIl2CppType(type->type);

        return il2cpp::vm::Reflection::GetModuleObject(klass->image);
    }

    Il2CppString * MonoType::get_Name(Il2CppReflectionType * type)
    {
        Il2CppClass* typeInfo = vm::Class::FromIl2CppType(type->type);

        if (type->type->byref)
        {
            std::string n = il2cpp::utils::StringUtils::Printf("%s&", typeInfo->name);
            return vm::String::New(n.c_str());
        }
        else
        {
            return il2cpp::vm::String::NewWrapper(typeInfo->name);
        }
    }

    Il2CppString *  MonoType::get_Namespace(Il2CppReflectionType *type)
    {
        Il2CppClass* typeInfo = vm::Class::FromIl2CppType(type->type);

        while (Il2CppClass* declaringType = vm::Class::GetDeclaringType(typeInfo))
            typeInfo = declaringType;

        if (typeInfo->namespaze[0] == '\0')
            return NULL;
        else
            return il2cpp::vm::String::NewWrapper(typeInfo->namespaze);
    }

    Il2CppReflectionType * MonoType::get_BaseType(Il2CppReflectionType * type)
    {
        Il2CppClass* klass = vm::Class::FromIl2CppType(type->type);

        return klass->parent ? il2cpp::vm::Reflection::GetTypeObject(&klass->parent->byval_arg) : NULL;
    }

    Il2CppArray* MonoType::GetConstructors_internal(Il2CppReflectionType* type, int32_t bflags, Il2CppReflectionType *reftype)
    {
        static Il2CppClass *System_Reflection_ConstructorInfo = NULL;
        Il2CppClass *startklass, *klass, *refklass;
        Il2CppArray *res;
        const MethodInfo *method;
        int match;
        void* iter = NULL;
        typedef std::vector<std::pair<const MethodInfo*, Il2CppClass*> > MethodPairVector;
        MethodPairVector tmp_vec;

        if (type->type->byref)
            return il2cpp::vm::Array::NewCached(il2cpp_defaults.method_info_class, 0);
        klass = startklass = vm::Class::FromIl2CppType(type->type);

        refklass = vm::Class::FromIl2CppType(reftype->type);

        if (!System_Reflection_ConstructorInfo)
            System_Reflection_ConstructorInfo = vm::Class::FromName(il2cpp_defaults.corlib, "System.Reflection", "ConstructorInfo");

        iter = NULL;
        while ((method = vm::Class::GetMethods(klass, &iter)))
        {
            match = 0;
            if (strcmp(method->name, ".ctor") && strcmp(method->name, ".cctor"))
                continue;
            if ((method->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC)
            {
                if (bflags & BFLAGS_Public)
                    match++;
            }
            else
            {
                if (bflags & BFLAGS_NonPublic)
                    match++;
            }
            if (!match)
                continue;
            match = 0;
            if (method->flags & METHOD_ATTRIBUTE_STATIC)
            {
                if (bflags & BFLAGS_Static)
                    if ((bflags & BFLAGS_FlattenHierarchy) || (klass == startklass))
                        match++;
            }
            else
            {
                if (bflags & BFLAGS_Instance)
                    match++;
            }

            if (!match)
                continue;

            tmp_vec.push_back(std::make_pair(method, refklass));
        }

        res = il2cpp::vm::Array::NewCached(System_Reflection_ConstructorInfo, (il2cpp_array_size_t)tmp_vec.size());

        for (size_t i = 0; i < tmp_vec.size(); ++i)
            il2cpp_array_setref(res, i, (Il2CppObject*)vm::Reflection::GetMethodObject(tmp_vec[i].first, tmp_vec[i].second));

        return res;
    }

    Il2CppReflectionType* MonoType::GetElementType(Il2CppReflectionType * type)
    {
        Il2CppClass *klass;

        klass = vm::Class::FromIl2CppType(type->type);

        // GetElementType should only return a type for:
        // Array Pointer PassedByRef
        if (type->type->byref)
            return il2cpp::vm::Reflection::GetTypeObject(&klass->byval_arg);
        else if (klass->element_class && IL2CPP_CLASS_IS_ARRAY(klass))
            return il2cpp::vm::Reflection::GetTypeObject(&klass->element_class->byval_arg);
        else if (klass->element_class && type->type->type == IL2CPP_TYPE_PTR)
            return il2cpp::vm::Reflection::GetTypeObject(&klass->element_class->byval_arg);
        else
            return NULL;
    }

    template<typename NameFilter>
    static inline Il2CppReflectionField* GetFieldFromType(Il2CppClass* type, Il2CppClass* const originalType, int32_t bindingFlags, const NameFilter& nameFilter)
    {
        void* iter = NULL;
        while (FieldInfo* field = vm::Class::GetFields(type, &iter))
        {
            if (CheckMemberMatch(field, type, originalType, bindingFlags, nameFilter))
            {
                return vm::Reflection::GetFieldObject(originalType, field);
            }
        }

        return NULL;
    }

    template<typename NameFilter>
    static Il2CppReflectionField* GetFieldImpl(Il2CppClass* type, int32_t bindingFlags, const NameFilter& nameFilter)
    {
        Il2CppReflectionField* field = GetFieldFromType(type, type, bindingFlags, nameFilter);

        if (field == NULL && (bindingFlags & BFLAGS_DeclaredOnly) == 0)
        {
            Il2CppClass* const originalType = type;
            type = vm::Class::GetParent(type);

            while (field == NULL && type != NULL)
            {
                field = GetFieldFromType(type, originalType, bindingFlags, nameFilter);
                type = vm::Class::GetParent(type);
            }
        }

        return field;
    }

    Il2CppReflectionField* MonoType::GetField(Il2CppReflectionType* _this, Il2CppString* name, int32_t bindingFlags)
    {
        if (_this->type->byref)
            return NULL;

        Il2CppClass* type = vm::Class::FromIl2CppType(_this->type);

        if (bindingFlags & BFLAGS_IgnoreCase)
        {
            return GetFieldImpl(type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseInsensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
        }

        return GetFieldImpl(type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseSensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
    }

    template<typename NameFilter>
    static inline void CollectTypeFields(Il2CppClass* type, const Il2CppClass* const originalType, int32_t bindingFlags, std::vector<FieldInfo*>& fields, const NameFilter& nameFilter)
    {
        void* iterator = NULL;
        FieldInfo* field = NULL;
        while ((field = vm::Class::GetFields(type, &iterator)) != NULL)
        {
            if (CheckMemberMatch(field, type, originalType, bindingFlags, nameFilter))
                fields.push_back(field);
        }
    }

    template<typename NameFilter>
    static inline Il2CppArray* GetFieldsImpl(Il2CppReflectionType* _this, int bindingFlags, Il2CppReflectionType* reflectedType, const NameFilter& nameFilter)
    {
        if (reflectedType->type->byref || !ValidBindingFlagsForGetMember(bindingFlags))
            return vm::Array::New(il2cpp_defaults.field_info_class, 0);

        std::vector<FieldInfo*> fields;
        Il2CppClass* typeInfo = vm::Class::FromIl2CppType(reflectedType->type);
        Il2CppClass* const originalType = typeInfo;

        CollectTypeFields(typeInfo, typeInfo, bindingFlags, fields, nameFilter);

        if ((bindingFlags & BFLAGS_DeclaredOnly) == 0)
        {
            typeInfo = typeInfo->parent;

            while (typeInfo != NULL)
            {
                CollectTypeFields(typeInfo, originalType, bindingFlags, fields, nameFilter);
                typeInfo = typeInfo->parent;
            }
        }

        size_t fieldCount = fields.size();
        Il2CppArray* result = vm::Array::NewCached(il2cpp_defaults.field_info_class, (il2cpp_array_size_t)fieldCount);

        for (size_t i = 0; i < fieldCount; i++)
        {
            il2cpp_array_setref(result, i, vm::Reflection::GetFieldObject(originalType, fields[i]));
        }

        return result;
    }

    Il2CppArray* MonoType::GetFields_internal(Il2CppReflectionType* _this, int bindingFlags, Il2CppReflectionType* reflectedType)
    {
        return GetFieldsImpl(_this, bindingFlags, reflectedType, utils::functional::TrueFilter());
    }

    Il2CppArray* MonoType::GetFieldsByName(Il2CppReflectionType* _this, Il2CppString* name, int bindingFlags, Il2CppReflectionType* reflectedType)
    {
        if (name == NULL)
            return GetFieldsImpl(_this, bindingFlags, reflectedType, utils::functional::TrueFilter());

        if (bindingFlags & BFLAGS_IgnoreCase)
        {
            return GetFieldsImpl(_this, bindingFlags, reflectedType, utils::functional::Filter<std::string, utils::VmStringUtils::CaseInsensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
        }

        return GetFieldsImpl(_this, bindingFlags, reflectedType, utils::functional::Filter<std::string, utils::VmStringUtils::CaseSensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
    }

    Il2CppString * MonoType::getFullName(Il2CppReflectionType * type, bool full_name, bool assembly_qualified)
    {
        Il2CppTypeNameFormat format;

        if (full_name)
            format = assembly_qualified ?
                IL2CPP_TYPE_NAME_FORMAT_ASSEMBLY_QUALIFIED :
                IL2CPP_TYPE_NAME_FORMAT_FULL_NAME;
        else
            format = IL2CPP_TYPE_NAME_FORMAT_REFLECTION;

        std::string name(vm::Type::GetName(type->type, format));
        if (name.empty())
            return NULL;

        if (full_name && (type->type->type == IL2CPP_TYPE_VAR || type->type->type == IL2CPP_TYPE_MVAR))
        {
            return NULL;
        }

        return il2cpp::vm::String::NewWrapper(name.c_str());
    }

    Il2CppArray * MonoType::GetGenericArguments(Il2CppReflectionType* type)
    {
        return vm::Type::GetGenericArgumentsInternal(type, false);
    }

    Il2CppArray* MonoType::GetInterfaces(Il2CppReflectionType* type)
    {
        Il2CppClass* klass = vm::Class::FromIl2CppType(type->type);
        typedef std::set<Il2CppClass*> InterfaceVector;
        InterfaceVector itfs;

        Il2CppClass* currentType = klass;
        while (currentType != NULL)
        {
            void* iter = NULL;
            while (Il2CppClass* itf = vm::Class::GetInterfaces(currentType, &iter))
                itfs.insert(itf);

            currentType = vm::Class::GetParent(currentType);
        }

        Il2CppArray* res = vm::Array::New(il2cpp_defaults.systemtype_class, (il2cpp_array_size_t)itfs.size());
        int i = 0;
        for (InterfaceVector::const_iterator iter = itfs.begin(); iter != itfs.end(); ++iter, ++i)
            il2cpp_array_setref(res, i, vm::Reflection::GetTypeObject(&(*iter)->byval_arg));

        return res;
    }

    static bool
    method_public(MethodInfo* method)
    {
        return (method->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC;
    }

    bool MonoType::MethodNonPublic(const MethodInfo* method, bool start_klass)
    {
        switch (method->flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK)
        {
            case METHOD_ATTRIBUTE_ASSEM:
                return (start_klass || il2cpp_defaults.generic_ilist_class);
            case METHOD_ATTRIBUTE_PRIVATE:
                return start_klass;
            case METHOD_ATTRIBUTE_PUBLIC:
                return false;
            default:
                return true;
        }
    }

    static bool
    method_rt_special_name(MethodInfo* method)
    {
        return (method->flags & METHOD_ATTRIBUTE_RESERVED_MASK) == METHOD_ATTRIBUTE_RT_SPECIAL_NAME;
    }

    static bool
    method_static(MethodInfo* method)
    {
        return (method->flags & METHOD_ATTRIBUTE_STATIC) != 0;
    }

    template<typename NameFilter>
    void CollectTypeMethods(Il2CppClass* type, const Il2CppClass* originalType, uint32_t bindingFlags, const NameFilter& nameFilter, std::vector<const MethodInfo*>& methods, bool(&filledSlots)[65535])
    {
        void* iter = NULL;
        while (const MethodInfo* method = vm::Class::GetMethods(type, &iter))
        {
            if ((method->flags & METHOD_ATTRIBUTE_RT_SPECIAL_NAME) != 0 && (strcmp(method->name, ".ctor") == 0 || strcmp(method->name, ".cctor") == 0))
                continue;

            if (CheckMemberMatch(method, type, originalType, bindingFlags, nameFilter))
            {
                if ((method->flags & METHOD_ATTRIBUTE_VIRTUAL) != 0)
                {
                    if (filledSlots[method->slot])
                        continue;

                    filledSlots[method->slot] = true;
                }

                methods.push_back(method);
            }
        }
    }

    template<typename NameFilter>
    static Il2CppArray* GetMethodsByNameImpl(const Il2CppType* type, uint32_t bindingFlags, const NameFilter& nameFilter)
    {
        std::vector<const MethodInfo*> methods;
        bool filledSlots[65535] = { 0 };

        Il2CppClass* typeInfo = vm::Class::FromIl2CppType(type);
        Il2CppClass* const originalTypeInfo = typeInfo;

        CollectTypeMethods(typeInfo, typeInfo, bindingFlags, nameFilter, methods, filledSlots);

        if ((bindingFlags & BFLAGS_DeclaredOnly) == 0)
        {
            for (typeInfo = vm::Class::GetParent(typeInfo); typeInfo != NULL; typeInfo = vm::Class::GetParent(typeInfo))
            {
                CollectTypeMethods(typeInfo, originalTypeInfo, bindingFlags, nameFilter, methods, filledSlots);
            }
        }

        size_t methodCount = methods.size();
        Il2CppArray* result = vm::Array::NewCached(il2cpp_defaults.method_info_class, (il2cpp_array_size_t)methodCount);

        for (size_t i = 0; i < methodCount; i++)
        {
            Il2CppReflectionMethod* method = vm::Reflection::GetMethodObject(methods[i], originalTypeInfo);
            il2cpp_array_setref(result, i, method);
        }

        return result;
    }

    Il2CppArray* MonoType::GetMethodsByName(Il2CppReflectionType* _this, Il2CppString* name, int bindingFlags, bool ignoreCase, Il2CppReflectionType* type)
    {
        if (type->type->byref || !ValidBindingFlagsForGetMember(bindingFlags))
            return vm::Array::NewCached(il2cpp_defaults.property_info_class, 0);

        if (name != NULL)
        {
            if (ignoreCase)
            {
                return GetMethodsByNameImpl(type->type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseInsensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
            }

            return GetMethodsByNameImpl(type->type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseSensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
        }

        return GetMethodsByNameImpl(type->type, bindingFlags, utils::functional::TrueFilter());
    }

    template<typename NameFilter>
    static void CollectTypeProperties(Il2CppClass* type, uint32_t bindingFlags, const NameFilter& nameFilter, Il2CppClass* const originalType, PropertyPairVector& properties)
    {
        void* iter = NULL;
        while (const PropertyInfo* property = vm::Class::GetProperties(type, &iter))
        {
            if (CheckMemberMatch(property, type, originalType, bindingFlags, nameFilter))
            {
                if (PropertyPairVectorContains(properties, property))
                    continue;

                properties.push_back(PropertyPair(property, originalType));
            }
        }
    }

    template<typename NameFilter>
    static Il2CppArray* GetPropertiesByNameImpl(const Il2CppType* type, uint32_t bindingFlags, const NameFilter& nameFilter)
    {
        PropertyPairVector properties;
        Il2CppClass* typeInfo = vm::Class::FromIl2CppType(type);
        Il2CppClass* const originalTypeInfo = typeInfo;

        properties.reserve(typeInfo->property_count);
        CollectTypeProperties(typeInfo, bindingFlags, nameFilter, originalTypeInfo, properties);

        if ((bindingFlags & BFLAGS_DeclaredOnly) == 0)
        {
            for (typeInfo = typeInfo->parent; typeInfo != NULL; typeInfo = typeInfo->parent)
            {
                CollectTypeProperties(typeInfo, bindingFlags, nameFilter, originalTypeInfo, properties);
            }
        }

        int i = 0;
        Il2CppArray* res = vm::Array::NewCached(il2cpp_defaults.property_info_class, (il2cpp_array_size_t)properties.size());

        for (PropertyPairVector::const_iterator iter = properties.begin(); iter != properties.end(); iter++)
        {
            il2cpp_array_setref(res, i, vm::Reflection::GetPropertyObject(iter->originalType, iter->property));
            i++;
        }

        return res;
    }

    Il2CppArray* MonoType::GetPropertiesByName(Il2CppReflectionType* _this, Il2CppString* name, uint32_t bindingFlags, bool ignoreCase, Il2CppReflectionType* type)
    {
        if (type->type->byref || !ValidBindingFlagsForGetMember(bindingFlags))
            return vm::Array::NewCached(il2cpp_defaults.property_info_class, 0);

        if (name != NULL)
        {
            if (ignoreCase)
            {
                return GetPropertiesByNameImpl(type->type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseInsensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
            }

            return GetPropertiesByNameImpl(type->type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseSensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
        }

        return GetPropertiesByNameImpl(type->type, bindingFlags, utils::functional::TrueFilter());
    }

    bool MonoType::IsByRefImpl(Il2CppReflectionType* type)
    {
        return type->type->byref;
    }

    bool MonoType::IsCOMObjectImpl(Il2CppReflectionMonoType *)
    {
        return false; // il2cpp does not support COM objects, so this is always false
    }

    bool MonoType::IsPointerImpl(Il2CppReflectionType* type)
    {
        return type->type->type == IL2CPP_TYPE_PTR;
    }

    bool MonoType::IsPrimitiveImpl(Il2CppReflectionType *type)
    {
        return (!type->type->byref && (((type->type->type >= IL2CPP_TYPE_BOOLEAN) && (type->type->type <= IL2CPP_TYPE_R8)) || (type->type->type == IL2CPP_TYPE_I) || (type->type->type == IL2CPP_TYPE_U)));
    }

    template<typename NameFilter>
    static inline Il2CppReflectionEvent* GetEventFromType(Il2CppClass* const type, Il2CppClass* const originalType, int32_t bindingFlags, const NameFilter& nameFilter)
    {
        void* iter = NULL;
        while (const EventInfo* event = vm::Class::GetEvents(type, &iter))
        {
            if (CheckMemberMatch(event, type, originalType, bindingFlags, nameFilter))
            {
                return vm::Reflection::GetEventObject(originalType, event);
            }
        }

        return NULL;
    }

    template<typename NameFilter>
    static Il2CppReflectionEvent* GetEventImpl(Il2CppClass* type, int32_t bindingFlags, const NameFilter& nameFilter)
    {
        Il2CppReflectionEvent* event = GetEventFromType(type, type, bindingFlags, nameFilter);

        if (event == NULL && (bindingFlags & BFLAGS_DeclaredOnly) == 0)
        {
            Il2CppClass* const originalType = type;
            type = type->parent;

            while (event == NULL && type != NULL)
            {
                event = GetEventFromType(type, originalType, bindingFlags, nameFilter);
                type = type->parent;
            }
        }

        return event;
    }

    Il2CppReflectionEvent* MonoType::InternalGetEvent(Il2CppReflectionType* _this, Il2CppString* name, int32_t bindingFlags)
    {
        if (_this->type->byref || !ValidBindingFlagsForGetMember(bindingFlags))
            return NULL;

        Il2CppClass* type = vm::Class::FromIl2CppType(_this->type);

        if (bindingFlags & BFLAGS_IgnoreCase)
        {
            return GetEventImpl(type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseInsensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
        }

        return GetEventImpl(type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseSensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
    }

    template<typename NameFilter>
    static inline void CollectTypeEvents(Il2CppClass* type, Il2CppClass* const originalType, int32_t bindingFlags, EventMap& events, const NameFilter& nameFilter)
    {
        void* iter = NULL;
        while (const EventInfo* event = vm::Class::GetEvents(type, &iter))
        {
            if (CheckMemberMatch(event, type, originalType, bindingFlags, nameFilter))
            {
                if (events.find(event) != events.end())
                    continue;

                events[event] = originalType;
            }
        }
    }

    template<typename NameFilter>
    static inline Il2CppArray* GetEventsImpl(Il2CppReflectionType* type, int bindingFlags, Il2CppReflectionType* reflectedType, const NameFilter& nameFilter)
    {
        if (type->type->byref || !ValidBindingFlagsForGetMember(bindingFlags))
            return vm::Array::New(il2cpp_defaults.event_info_class, 0);

        EventMap events;
        Il2CppClass* typeInfo = vm::Class::FromIl2CppType(type->type);

        CollectTypeEvents(typeInfo, typeInfo, bindingFlags, events, nameFilter);

        if ((bindingFlags & BFLAGS_DeclaredOnly) == 0)
        {
            Il2CppClass* const originalType = typeInfo;
            typeInfo = vm::Class::GetParent(typeInfo);

            while (typeInfo != NULL)
            {
                CollectTypeEvents(typeInfo, originalType, bindingFlags, events, nameFilter);
                typeInfo = vm::Class::GetParent(typeInfo);
            }
        }

        int i = 0;
        Il2CppArray* result = vm::Array::NewCached(il2cpp_defaults.event_info_class, (il2cpp_array_size_t)events.size());

        for (EventMap::const_iterator iter = events.begin(); iter != events.end(); iter++)
        {
            il2cpp_array_setref(result, i, vm::Reflection::GetEventObject(iter->second, iter->first.key));
            i++;
        }

        return result;
    }

    Il2CppArray* MonoType::GetEvents_internal(Il2CppReflectionType* _this, int32_t bindingFlags, Il2CppReflectionType* type)
    {
        return GetEventsImpl(_this, bindingFlags, type, utils::functional::TrueFilter());
    }

    Il2CppArray* MonoType::GetEventsByName(Il2CppReflectionType* _this, Il2CppString* name, int bindingFlags, Il2CppReflectionType* reflectedType)
    {
        if (name == NULL)
            return GetEventsImpl(_this, bindingFlags, reflectedType, utils::functional::TrueFilter());

        if (bindingFlags & BFLAGS_IgnoreCase)
        {
            return GetEventsImpl(_this, bindingFlags, reflectedType, utils::functional::Filter<std::string, utils::VmStringUtils::CaseInsensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
        }

        return GetEventsImpl(_this, bindingFlags, reflectedType, utils::functional::Filter<std::string, utils::VmStringUtils::CaseSensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
    }

    void MonoType::type_from_obj(void* /* System.MonoType */ type, Il2CppObject* obj)
    {
        NOT_SUPPORTED_IL2CPP(MonoType::type_from_obj, "This icall is only used by System.MonoType constructor, which throws NotImplementedException right after this call.");
    }

    static inline bool CheckNestedTypeMatch(Il2CppClass* nestedType, BindingFlags bindingFlags)
    {
        uint32_t accessFlag = (nestedType->flags & TYPE_ATTRIBUTE_VISIBILITY_MASK) == TYPE_ATTRIBUTE_NESTED_PUBLIC ? BFLAGS_Public : BFLAGS_NonPublic;
        return (accessFlag & bindingFlags) != 0;
    }

    template<typename NameFilter>
    Il2CppReflectionType* GetNestedTypeImpl(Il2CppClass* typeInfo, BindingFlags bindingFlags, const NameFilter& nameFilter)
    {
        void* iter = NULL;
        while (Il2CppClass* nestedType = vm::Class::GetNestedTypes(typeInfo, &iter))
        {
            if (CheckNestedTypeMatch(nestedType, bindingFlags) && nameFilter(nestedType->name))
                return vm::Reflection::GetTypeObject(&nestedType->byval_arg);
        }

        return NULL;
    }

    Il2CppReflectionType* MonoType::GetNestedType(Il2CppReflectionType* type, Il2CppString* name, BindingFlags bindingFlags)
    {
        bool validBindingFlags = (bindingFlags & BFLAGS_NonPublic) != 0 || (bindingFlags & BFLAGS_Public) != 0;

        if (type->type->byref || !validBindingFlags)
            return NULL;

        Il2CppClass* typeInfo = vm::Class::FromIl2CppType(type->type);

        // nested types are always generic type definitions, even for inflated types. As such we only store/retrieve them on
        // type definitions and generic type definitions. If we are a generic instance, use our generic type definition instead.
        if (typeInfo->generic_class)
            typeInfo = vm::GenericClass::GetTypeDefinition(typeInfo->generic_class);

        if (bindingFlags & BFLAGS_IgnoreCase)
        {
            return GetNestedTypeImpl(typeInfo, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseInsensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
        }

        return GetNestedTypeImpl(typeInfo, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseSensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
    }

    template<typename NameFilter>
    static Il2CppArray* GetNestedTypesImpl(Il2CppReflectionType* type, int32_t bindingFlags, const NameFilter& nameFilter)
    {
        bool validBindingFlags = (bindingFlags & BFLAGS_NonPublic) != 0 || (bindingFlags & BFLAGS_Public) != 0;

        if (type->type->byref || !validBindingFlags)
            return vm::Array::New(il2cpp_defaults.monotype_class, 0);

        Il2CppClass* typeInfo = vm::Class::FromIl2CppType(type->type);

        // nested types are always generic type definitions, even for inflated types. As such we only store/retrieve them on
        // type definitions and generic type definitions. If we are a generic instance, use our generic type definition instead.
        if (typeInfo->generic_class)
            typeInfo = vm::GenericClass::GetTypeDefinition(typeInfo->generic_class);

        std::vector<Il2CppClass*> nestedTypes;

        void* iter = NULL;
        while (Il2CppClass* nestedType = vm::Class::GetNestedTypes(typeInfo, &iter))
        {
            if (CheckNestedTypeMatch(nestedType, bindingFlags) && nameFilter(nestedType->name))
                nestedTypes.push_back(nestedType);
        }

        size_t nestedTypeCount = nestedTypes.size();
        Il2CppArray* result = vm::Array::New(il2cpp_defaults.monotype_class, (il2cpp_array_size_t)nestedTypeCount);

        for (size_t i = 0; i < nestedTypeCount; i++)
        {
            il2cpp_array_setref(result, i, vm::Reflection::GetTypeObject(&nestedTypes[i]->byval_arg));
        }

        return result;
    }

    Il2CppArray* MonoType::GetNestedTypesByName(Il2CppReflectionType* type, Il2CppString* name, int32_t bindingFlags)
    {
        if (name == NULL)
            return GetNestedTypesImpl(type, bindingFlags, utils::functional::TrueFilter());

        if (bindingFlags & BFLAGS_IgnoreCase)
            return GetNestedTypesImpl(type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseInsensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));

        return GetNestedTypesImpl(type, bindingFlags, utils::functional::Filter<std::string, utils::VmStringUtils::CaseSensitiveComparer>(utils::StringUtils::Utf16ToUtf8(name->chars)));
    }

    Il2CppArray* MonoType::GetNestedTypes(Il2CppReflectionType* type, int32_t bindingFlags)
    {
        return GetNestedTypesImpl(type, bindingFlags, utils::functional::TrueFilter());
    }

    void* /* System.Reflection.MethodBase */ MonoType::get_DeclaringMethod(void* /* System.MonoType */ self)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(MonoType::get_DeclaringMethod);

        return 0;
    }

    bool MonoType::PropertyAccessorNonPublic(const MethodInfo* accessor, bool start_klass)
    {
        if (!accessor)
            return false;

        return MethodNonPublic(accessor, start_klass);
    }
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
