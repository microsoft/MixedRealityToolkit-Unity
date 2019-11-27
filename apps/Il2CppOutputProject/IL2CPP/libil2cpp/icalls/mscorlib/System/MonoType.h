#pragma once

#include <stdint.h>
#include "il2cpp-config.h"

struct mscorlib_System_Reflection_MethodInfo;

struct Il2CppArray;
struct Il2CppString;
struct Il2CppReflectionAssembly;
struct Il2CppReflectionField;
struct Il2CppReflectionModule;
struct Il2CppReflectionMonoType;
struct Il2CppReflectionType;
struct Il2CppReflectionEvent;

typedef int32_t BindingFlags;

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    enum
    {
        BFLAGS_IgnoreCase = 1,
        BFLAGS_DeclaredOnly = 2,
        BFLAGS_Instance = 4,
        BFLAGS_Static = 8,
        BFLAGS_Public = 0x10,
        BFLAGS_NonPublic = 0x20,
        BFLAGS_FlattenHierarchy = 0x40,
        BFLAGS_InvokeMethod = 0x100,
        BFLAGS_CreateInstance = 0x200,
        BFLAGS_GetField = 0x400,
        BFLAGS_SetField = 0x800,
        BFLAGS_GetProperty = 0x1000,
        BFLAGS_SetProperty = 0x2000,
        BFLAGS_ExactBinding = 0x10000,
        BFLAGS_SuppressChangeType = 0x20000,
        BFLAGS_OptionalParamBinding = 0x40000
    };


    class LIBIL2CPP_CODEGEN_API MonoType
    {
    public:
        static Il2CppString* getFullName(Il2CppReflectionType* type, bool full_name, bool assembly_qualified);
        static Il2CppArray* GetFields_internal(Il2CppReflectionType* _this, int, Il2CppReflectionType* reflectedType);
        static Il2CppArray* GetFieldsByName(Il2CppReflectionType* _this, Il2CppString* name, int, Il2CppReflectionType* reflectedType);
        static int GetArrayRank(Il2CppReflectionType* type);
        static Il2CppArray* GetConstructors_internal(Il2CppReflectionType* type, int32_t bflags, Il2CppReflectionType* reftype);
        static Il2CppReflectionType* GetElementType(Il2CppReflectionType* type);
        static Il2CppArray* GetEvents_internal(Il2CppReflectionType* thisPtr, int32_t bindingFlags, Il2CppReflectionType* type);
        static Il2CppArray* GetEventsByName(Il2CppReflectionType* _this, Il2CppString* name, int bindingFlags, Il2CppReflectionType* reflectedType);
        static Il2CppReflectionField* GetField(Il2CppReflectionType* _this, Il2CppString* name, int32_t bindingFlags);
        static Il2CppArray* GetGenericArguments(Il2CppReflectionType* type);
        static Il2CppArray* GetInterfaces(Il2CppReflectionType* type);
        static Il2CppArray* GetMethodsByName(Il2CppReflectionType* _this, Il2CppString* name, int bindingFlags, bool ignoreCase, Il2CppReflectionType* type);
        static Il2CppReflectionType* GetNestedType(Il2CppReflectionType* type, Il2CppString* name, BindingFlags bindingFlags);
        static Il2CppArray* GetNestedTypesByName(Il2CppReflectionType* type, Il2CppString* name, int32_t bindingAttr);
        static Il2CppArray* GetNestedTypes(Il2CppReflectionType* type, int32_t bindingAttr);
        static Il2CppArray* GetPropertiesByName(Il2CppReflectionType* _this, Il2CppString* name, uint32_t bindingFlags, bool ignoreCase, Il2CppReflectionType* type);
        static Il2CppReflectionEvent* InternalGetEvent(Il2CppReflectionType* _this, Il2CppString* name, int32_t bindingFlags);
        static bool IsByRefImpl(Il2CppReflectionType* type);
        static bool IsCOMObjectImpl(Il2CppReflectionMonoType*);
        static bool IsPointerImpl(Il2CppReflectionType* type);
        static Il2CppReflectionAssembly* get_Assembly(Il2CppReflectionType*);
        static Il2CppReflectionType* get_BaseType(Il2CppReflectionType* type);
        static void* /* System.Reflection.MethodBase */ get_DeclaringMethod(void* /* System.MonoType */ self);
        static Il2CppReflectionType* get_DeclaringType(Il2CppReflectionMonoType*);
        static bool get_IsGenericParameter(Il2CppReflectionType*);
        static bool IsGenericParameter(const Il2CppType *type);
        static Il2CppReflectionModule* get_Module(Il2CppReflectionType* thisPtr);
        static Il2CppString* get_Name(Il2CppReflectionType* type);
        static Il2CppString* get_Namespace(Il2CppReflectionType* type);
        static int get_attributes(Il2CppReflectionType* type);
        static void type_from_obj(void* /* System.MonoType */ type, Il2CppObject* obj);
        static bool IsPrimitiveImpl(Il2CppReflectionType*);
        static bool PropertyEqual(const PropertyInfo* prop1, const PropertyInfo* prop2);
        static bool PropertyAccessorNonPublic(const MethodInfo* accessor, bool start_klass);
        static bool MethodNonPublic(const MethodInfo* method, bool start_klass);
    };
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
