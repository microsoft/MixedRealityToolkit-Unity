#pragma once

#include "il2cpp-codegen-common.h"
#include "il2cpp-mono-support.h"
#include "mono-api.h"

#include "vm/StackTrace.h"
#include "vm/PlatformInvoke.h"

struct ProfilerMethodSentry
{
    ProfilerMethodSentry(const RuntimeMethod* method)
#if IL2CPP_ENABLE_PROFILER
        : m_method(method)
#endif
    {
        IL2CPP_NOT_IMPLEMENTED("Unity profiler hooks are not implemented yet for the libmonoruntime backend.");
    }

    ~ProfilerMethodSentry()
    {
        IL2CPP_NOT_IMPLEMENTED("Unity profiler hooks are not implemented yet for the libmonoruntime backend.");
    }

private:
    const RuntimeMethod* m_method;
};

struct StackTraceSentry
{
    StackTraceSentry(RuntimeMethod* method) : m_method(method)
    {
        MonoStackFrameInfo frame_info;

        frame_info.method = method;
        frame_info.actual_method = method;
        frame_info.type = FRAME_TYPE_MANAGED;
        frame_info.managed = 1;
        frame_info.il_offset = 0;
        frame_info.native_offset = 0;
        frame_info.ji = (MonoJitInfo*)(void*)-1;

        mono::vm::StackTrace::PushFrame(frame_info);
    }

    ~StackTraceSentry()
    {
        mono::vm::StackTrace::PopFrame();
    }

private:
    const RuntimeMethod* m_method;
};

#define IL2CPP_FAKE_BOX_SENTRY (MonoThreadsSync*)UINTPTR_MAX

template<typename T>
struct Il2CppFakeBox : RuntimeObject
{
    T m_Value;

    Il2CppFakeBox(RuntimeClass* boxedType, T* value)
    {
        vtable = il2cpp_mono_class_vtable(g_MonoDomain, boxedType);
        synchronisation = IL2CPP_FAKE_BOX_SENTRY;
        m_Value = *value;
    }
};

inline bool il2cpp_codegen_is_fake_boxed_object(RuntimeObject* object)
{
    return object->synchronisation == IL2CPP_FAKE_BOX_SENTRY;
}

// TODO: This file should contain all the functions and type declarations needed for the generated code.
// Hopefully, we stop including everything in the generated code and know exactly what dependencies we have.
// Note that all parameter and return types should match the generated types not the runtime types.

// type registration

inline String_t* il2cpp_codegen_string_new_utf16(const il2cpp::utils::StringView<Il2CppChar>& str)
{
    return (String_t*)mono_string_new_utf16(g_MonoDomain, (const mono_unichar2*)str.Str(), (int32_t)str.Length());
}

inline NORETURN void il2cpp_codegen_raise_exception(Exception_t *ex, Il2CppSequencePoint *seqPoint = NULL, MethodInfo* lastManagedFrame = NULL)
{
    mono_raise_exception((RuntimeException*)ex);
    il2cpp_codegen_no_return();
}

inline void il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(const RuntimeMethod* method)
{
    il2cpp_mono_raise_execution_engine_exception_if_method_is_not_found(const_cast<RuntimeMethod*>(method));
}

inline void il2cpp_codegen_raise_execution_engine_exception(const RuntimeMethod* method)
{
    mono_raise_exception(mono_get_exception_execution_engine(mono_unity_method_get_name(method)));
}

inline Exception_t* il2cpp_codegen_get_argument_exception(const char* param, const char* msg)
{
    return (Exception_t*)mono_get_exception_argument(param, msg);
}

inline Exception_t* il2cpp_codegen_get_argument_null_exception(const char* param)
{
    return (Exception_t*)mono_get_exception_argument_null(param);
}

inline Exception_t* il2cpp_codegen_get_overflow_exception()
{
    return (Exception_t*)mono_get_exception_overflow();
}

inline Exception_t* il2cpp_codegen_get_not_supported_exception(const char* msg)
{
    return (Exception_t*)mono_get_exception_not_supported(msg);
}

inline Exception_t* il2cpp_codegen_get_array_type_mismatch_exception()
{
    return (Exception_t*)mono_get_exception_array_type_mismatch();
}

inline Exception_t* il2cpp_codegen_get_invalid_operation_exception(const char* msg)
{
    return (Exception_t*)mono_get_exception_invalid_operation(msg);
}

inline Exception_t* il2cpp_codegen_get_marshal_directive_exception(const char* msg)
{
    return (Exception_t*)mono_unity_exception_get_marshal_directive(msg);
}

inline Exception_t* il2cpp_codegen_get_missing_method_exception(const char* msg)
{
    return (Exception_t*)mono_get_exception_missing_method(msg, "ctor");
}

inline Exception_t* il2cpp_codegen_get_maximum_nested_generics_exception()
{
    return (Exception_t*)mono_get_exception_not_supported(MAXIMUM_NESTED_GENERICS_EXCEPTION_MESSAGE);
}

inline RuntimeClass* il2cpp_codegen_object_class(RuntimeObject* obj)
{
    return mono_object_get_class(obj);
}

// OpCode.IsInst

inline RuntimeObject* IsInst(RuntimeObject *obj, RuntimeClass* targetType)
{
    return mono_object_isinst(obj, targetType);
}

inline RuntimeObject* IsInstSealed(RuntimeObject *obj, RuntimeClass* targetType)
{
    if (!obj)
        return NULL;

    // optimized version to compare sealed classes
    return mono_unity_object_isinst_sealed(obj, targetType);
}

inline RuntimeObject* IsInstClass(RuntimeObject *obj, RuntimeClass* targetType)
{
    if (!obj)
        return NULL;

    return mono_unity_class_has_parent_unsafe(mono_object_get_class(obj), targetType) ? obj : NULL;
}

// OpCode.Castclass

inline RuntimeObject* Castclass(RuntimeObject *obj, RuntimeClass* targetType)
{
    if (!obj)
        return NULL;

    RuntimeObject* result = mono_object_isinst(obj, targetType);
    if (result)
        return result;

    mono_raise_exception(mono_get_exception_invalid_cast());
    return NULL;
}

inline RuntimeObject* CastclassSealed(RuntimeObject *obj, RuntimeClass* targetType)
{
    if (!obj)
        return NULL;

    RuntimeObject* result = IsInstSealed(obj, targetType);
    if (result)
        return result;

    mono_raise_exception(mono_get_exception_invalid_cast());
    return NULL;
}

inline RuntimeObject* CastclassClass(RuntimeObject *obj, RuntimeClass* targetType)
{
    if (!obj)
        return NULL;

    RuntimeObject* result = IsInstClass(obj, targetType);
    if (result)
        return result;

    mono_raise_exception(mono_get_exception_invalid_cast());
    return NULL;
}

// OpCode.Box

inline RuntimeObject* Box(RuntimeClass* type, void* data)
{
    return mono_value_box(g_MonoDomain, type, data);
}

// OpCode.UnBox

inline void* UnBox(RuntimeObject* obj)
{
    if (!obj)
    {
        mono_raise_exception(mono_get_exception_null_reference());
        il2cpp_codegen_no_return();
    }

    return mono_object_unbox(obj);
}

inline void* UnBox(RuntimeObject* obj, RuntimeClass* klass)
{
    if (!obj)
    {
        mono_raise_exception(mono_get_exception_null_reference());
        il2cpp_codegen_no_return();
    }

    if (!mono_unity_object_check_box_cast(obj, klass))
    {
        mono_raise_exception(mono_get_exception_invalid_cast());
        il2cpp_codegen_no_return();
    }

    return mono_object_unbox(obj);
}

inline void UnBoxNullable(RuntimeObject* obj, RuntimeClass* klass, void* storage)
{
    mono_unity_object_unbox_nullable(obj, klass, storage);
}

inline uint32_t il2cpp_codegen_sizeof(RuntimeClass* klass)
{
    if (!mono_class_is_valuetype(klass))
        return sizeof(void*);

    return mono_class_instance_size(klass) - sizeof(RuntimeObject);
}

inline bool il2cpp_codegen_method_is_virtual(RuntimeMethod* method)
{
    return method->slot != -1;
}

inline bool il2cpp_codegen_object_is_of_sealed_type(RuntimeObject* obj)
{
    return obj != NULL && mono_class_is_sealed(mono_object_get_class(obj));
}

inline bool il2cpp_codegen_method_is_generic_instance(RuntimeMethod* method)
{
    return unity_mono_method_is_generic(method);
}

inline bool il2cpp_codegen_method_is_interface_method(RuntimeMethod* method)
{
    return MONO_CLASS_IS_INTERFACE(mono_method_get_class(method));
}

inline uint16_t il2cpp_codegen_method_get_slot(RuntimeMethod* method)
{
    return method->slot;
}

inline RuntimeClass* il2cpp_codegen_method_get_declaring_type(RuntimeMethod* method)
{
    return mono_method_get_class(method);
}

IL2CPP_FORCE_INLINE const VirtualInvokeData il2cpp_codegen_get_virtual_invoke_data(RuntimeMethod* method, void* obj)
{
    VirtualInvokeData invokeData;
    il2cpp_mono_get_virtual_invoke_data(method, obj, &invokeData);
    return invokeData;
}

IL2CPP_FORCE_INLINE const VirtualInvokeData il2cpp_codegen_get_interface_invoke_data(RuntimeMethod* method, void* obj, RuntimeClass* declaringInterface)
{
    VirtualInvokeData invokeData;
    il2cpp_mono_get_interface_invoke_data(method, obj, &invokeData);
    return invokeData;
}

IL2CPP_FORCE_INLINE const RuntimeMethod* il2cpp_codegen_get_generic_virtual_method(const RuntimeMethod* method, const RuntimeObject* obj)
{
    return il2cpp_mono_get_virtual_target_method(const_cast<RuntimeMethod*>(method), const_cast<RuntimeObject*>(obj));
}

IL2CPP_FORCE_INLINE void il2cpp_codegen_get_generic_virtual_invoke_data(const RuntimeMethod* method, void* obj, VirtualInvokeData* invokeData)
{
    il2cpp_mono_get_invoke_data(const_cast<RuntimeMethod*>(method), obj, invokeData);
}

IL2CPP_FORCE_INLINE const RuntimeMethod* il2cpp_codegen_get_generic_interface_method(const RuntimeMethod* method, const RuntimeObject* obj)
{
    return il2cpp_mono_get_virtual_target_method(const_cast<RuntimeMethod*>(method), const_cast<RuntimeObject*>(obj));
}

IL2CPP_FORCE_INLINE void il2cpp_codegen_get_generic_interface_invoke_data(RuntimeMethod* method, void* obj, VirtualInvokeData* invokeData)
{
    il2cpp_mono_get_invoke_data(method, obj, invokeData);
}

IL2CPP_FORCE_INLINE void il2cpp_codegen_get_generic_interface_invoke_data(const RuntimeMethod* method, void* obj, VirtualInvokeData* invokeData)
{
    il2cpp_codegen_get_generic_interface_invoke_data(const_cast<RuntimeMethod*>(method), obj, invokeData);
}

// OpCode.Ldtoken

inline RuntimeClass* il2cpp_codegen_class_from_type(RuntimeType* type)
{
    return mono_class_from_mono_type(type);
}

inline RuntimeClass* il2cpp_codegen_class_from_type(const RuntimeType *type)
{
    return il2cpp_codegen_class_from_type(const_cast<RuntimeType*>(type));
}

template<typename T>
inline T InterlockedCompareExchangeImpl(T* location, T value, T comparand)
{
    return (T)mono_unity_object_compare_exchange((RuntimeObject**)location, (RuntimeObject*)value, (RuntimeObject*)comparand);
}

template<typename T>
inline T InterlockedExchangeImpl(T* location, T value)
{
    return (T)mono_unity_object_exchange((RuntimeObject**)location, (RuntimeObject*)value);
}

inline void ArrayGetGenericValueImpl(RuntimeArray* __this, int32_t pos, void* value)
{
    int elementSize = mono_unity_array_get_element_size(__this);
    memcpy(value, mono_array_addr_with_size(__this, elementSize, pos), elementSize);
}

inline void ArraySetGenericValueImpl(RuntimeArray * __this, int32_t pos, void* value)
{
    int elementSize = mono_unity_array_get_element_size(__this);
    memcpy(mono_array_addr_with_size(__this, elementSize, pos), value, elementSize);
}

inline RuntimeArray* SZArrayNew(RuntimeClass* arrayType, uint32_t length)
{
    mono_class_init(arrayType);

    MonoError error;
    RuntimeArray *retVal = mono_array_new_specific_checked(il2cpp_mono_class_vtable(g_MonoDomain, arrayType), length, &error);
    RuntimeException *exc = mono_error_convert_to_exception(&error);
    if (exc)
        mono_raise_exception(exc);

    return retVal;
}

inline RuntimeArray* GenArrayNew(RuntimeClass* arrayType, il2cpp_array_size_t* dimensions)
{
    MonoError error;
    RuntimeArray *retVal = mono_array_new_full_checked(g_MonoDomain, arrayType, dimensions, NULL, &error);
    RuntimeException *exc = mono_error_convert_to_exception(&error);
    if (exc)
        mono_raise_exception(exc);

    return retVal;
}

// Performance optimization as detailed here: http://blogs.msdn.com/b/clrcodegeneration/archive/2009/08/13/array-bounds-check-elimination-in-the-clr.aspx
// Since array size is a signed int32_t, a single unsigned check can be performed to determine if index is less than array size.
// Negative indices will map to a unsigned number greater than or equal to 2^31 which is larger than allowed for a valid array.
#define IL2CPP_ARRAY_BOUNDS_CHECK(index, length) \
    do { \
        if (((uint32_t)(index)) >= ((uint32_t)length)) mono_raise_exception(mono_get_exception_index_out_of_range()); \
    } while (0)

inline bool il2cpp_codegen_class_is_assignable_from(RuntimeClass *klass, RuntimeClass *oklass)
{
    return mono_class_is_assignable_from(klass, oklass);
}

inline RuntimeObject* il2cpp_codegen_object_new(RuntimeClass *klass)
{
    return mono_object_new(g_MonoDomain, klass);
}

inline Il2CppMethodPointer il2cpp_codegen_resolve_icall(const RuntimeMethod* icallMethod)
{
    return (Il2CppMethodPointer)mono_lookup_internal_call(const_cast<RuntimeMethod*>(icallMethod));
}

template<typename FunctionPointerType>
inline FunctionPointerType il2cpp_codegen_resolve_pinvoke(const RuntimeMethod* pinvokeMethod)
{
    const char *exc_class, *exc_arg;
    FunctionPointerType result = reinterpret_cast<FunctionPointerType>(mono_lookup_pinvoke_call(const_cast<RuntimeMethod*>(pinvokeMethod), &exc_class, &exc_arg));
    if (exc_class)
    {
        mono_raise_exception(mono_exception_from_name_msg(mono_unity_image_get_mscorlib(), "System", exc_class, exc_arg));
        il2cpp_codegen_no_return();
    }

    return result;
}

template<typename T>
inline T* il2cpp_codegen_marshal_allocate_array(size_t length)
{
    MonoError unused;
    return (T*)mono_marshal_alloc((il2cpp_array_size_t)(sizeof(T) * length), &unused);
}

template<typename T>
inline T* il2cpp_codegen_marshal_allocate()
{
    MonoError unused;
    return (T*)mono_marshal_alloc(sizeof(T), &unused);
}

inline char* il2cpp_codegen_marshal_string(String_t* string)
{
    return mono::vm::PlatformInvoke::MarshalCSharpStringToCppString((RuntimeString*)string);
}

inline void il2cpp_codegen_marshal_string_fixed(String_t* string, char* buffer, int numberOfCharacters)
{
    return mono::vm::PlatformInvoke::MarshalCSharpStringToCppStringFixed((RuntimeString*)string, buffer, numberOfCharacters);
}

inline Il2CppChar* il2cpp_codegen_marshal_wstring(String_t* string)
{
    return (Il2CppChar*)mono::vm::PlatformInvoke::MarshalCSharpStringToCppWString((RuntimeString*)string);
}

inline void il2cpp_codegen_marshal_wstring_fixed(String_t* string, Il2CppChar* buffer, int numberOfCharacters)
{
    return mono::vm::PlatformInvoke::MarshalCSharpStringToCppWStringFixed((RuntimeString*)string, (mono_unichar2*)buffer, numberOfCharacters);
}

inline Il2CppChar* il2cpp_codegen_marshal_bstring(String_t* string)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline String_t* il2cpp_codegen_marshal_string_result(const char* value)
{
    return (String_t*)mono::vm::PlatformInvoke::MarshalCppStringToCSharpStringResult(value);
}

inline String_t* il2cpp_codegen_marshal_wstring_result(const Il2CppChar* value)
{
    return (String_t*)mono::vm::PlatformInvoke::MarshalCppWStringToCSharpStringResult((const mono_unichar2*)value);
}

inline String_t* il2cpp_codegen_marshal_bstring_result(const Il2CppChar* value)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline void il2cpp_codegen_marshal_free_bstring(Il2CppChar* value)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

inline char* il2cpp_codegen_marshal_string_builder(StringBuilder_t* stringBuilder)
{
    return mono::vm::PlatformInvoke::MarshalStringBuilder((RuntimeStringBuilder*)stringBuilder);
}

inline Il2CppChar* il2cpp_codegen_marshal_wstring_builder(StringBuilder_t* stringBuilder)
{
    return (Il2CppChar*)mono::vm::PlatformInvoke::MarshalWStringBuilder((RuntimeStringBuilder*)stringBuilder);
}

inline void il2cpp_codegen_marshal_string_builder_result(StringBuilder_t* stringBuilder, char* buffer)
{
    mono::vm::PlatformInvoke::MarshalStringBuilderResult((RuntimeStringBuilder*)stringBuilder, buffer);
}

inline void il2cpp_codegen_marshal_wstring_builder_result(StringBuilder_t* stringBuilder, Il2CppChar* buffer)
{
    mono::vm::PlatformInvoke::MarshalWStringBuilderResult((RuntimeStringBuilder*)stringBuilder, (mono_unichar2*)buffer);
}

inline Il2CppHString il2cpp_codegen_create_hstring(String_t* str)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline String_t* il2cpp_codegen_marshal_hstring_result(Il2CppHString hstring)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline void il2cpp_codegen_marshal_free_hstring(Il2CppHString hstring)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

inline void il2cpp_codegen_marshal_free(void* ptr)
{
    mono_marshal_free(ptr);
}

inline Il2CppMethodPointer il2cpp_codegen_marshal_delegate(MulticastDelegate_t* d)
{
    return (Il2CppMethodPointer)mono::vm::PlatformInvoke::MarshalDelegate((RuntimeDelegate*)d);
}

template<typename T>
inline T* il2cpp_codegen_marshal_function_ptr_to_delegate(Il2CppMethodPointer functionPtr, RuntimeClass* delegateType)
{
    return (T*)mono::vm::PlatformInvoke::MarshalFunctionPointerToDelegate(reinterpret_cast<void*>(functionPtr), delegateType);
}

inline void il2cpp_codegen_marshal_store_last_error()
{
    mono_marshal_set_last_error();
}

namespace il2cpp
{
namespace vm
{
    class ScopedThreadAttacher
    {
    public:
        ScopedThreadAttacher() :
            _threadWasAttached(false)
        {
            if (!mono_thread_is_attached())
            {
                mono_thread_attach(mono_get_root_domain());
                _threadWasAttached = true;
            }
        }

        ~ScopedThreadAttacher()
        {
            if (_threadWasAttached)
                mono_thread_detach(mono_thread_current());
        }

    private:
        bool _threadWasAttached;

        bool mono_thread_is_attached()
        {
            return mono_domain_get() != NULL;
        }
    };
}
}

#if _DEBUG
struct ScopedMarshallingAllocationCheck
{
};

struct ScopedMarshalingAllocationClearer
{
};
#endif

inline void NullCheck(void* this_ptr)
{
    if (this_ptr != NULL)
        return;

    mono_raise_exception(mono_get_exception_null_reference());
}

inline void DivideByZeroCheck(int64_t denominator)
{
    if (denominator != 0)
        return;

    mono_raise_exception(mono_get_exception_divide_by_zero());
}

inline void il2cpp_codegen_initobj(void* value, size_t size)
{
    memset(value, 0, size);
}

inline bool MethodIsStatic(const RuntimeMethod* method)
{
    return mono_unity_method_is_static(const_cast<RuntimeMethod*>(method));
}

inline bool MethodHasParameters(const RuntimeMethod* method)
{
    return mono_signature_get_param_count(mono_method_signature(const_cast<RuntimeMethod*>(method))) != 0;
}

//#define IL2CPP_RUNTIME_CLASS_INIT(klass) do { if((klass)->has_cctor && !(klass)->cctor_finished) il2cpp::vm::Runtime::ClassInit ((klass)); } while (0)
#define IL2CPP_RUNTIME_CLASS_INIT(klass) RuntimeInit(klass)

inline void* il2cpp_codegen_mono_class_rgctx(RuntimeClass* klass, Il2CppRGCTXDataType rgctxType, int rgctxIndex, bool useSharedVersion)
{
    return il2cpp_mono_class_rgctx(klass, rgctxType, rgctxIndex, useSharedVersion);
}

inline void* il2cpp_codegen_mono_method_rgctx(RuntimeMethod* method, Il2CppRGCTXDataType rgctxType, int rgctxIndex, bool useSharedVersion)
{
    return il2cpp_mono_method_rgctx(method, rgctxType, rgctxIndex, useSharedVersion);
}

inline void ArrayElementTypeCheck(RuntimeArray* array, void* value)
{
    if (!value)
        return;

    RuntimeClass *aclass = mono_unity_array_get_class(array);
    RuntimeClass *eclass = mono_unity_class_get_element_class(aclass);
    RuntimeClass *oclass = mono_unity_object_get_class((RuntimeObject*)value);

    if (!mono_class_is_assignable_from(eclass, oclass))
        mono_raise_exception(mono_get_exception_array_type_mismatch());
}

inline const RuntimeMethod* GetVirtualMethodInfo(RuntimeObject* pThis, const RuntimeMethod* method)
{
    if (!pThis)
        mono_raise_exception(mono_get_exception_null_reference());

    return mono_object_get_virtual_method(pThis, const_cast<RuntimeMethod*>(method));
}

inline const RuntimeMethod* GetInterfaceMethodInfo(RuntimeObject* pThis, RuntimeMethod *slot, RuntimeClass* declaringInterface)
{
    if (!pThis)
        mono_raise_exception(mono_get_exception_null_reference());

    return mono_object_get_virtual_method(pThis, slot);
}

inline void il2cpp_codegen_memory_barrier()
{
    mono_unity_memory_barrier();
}

inline void il2cpp_codegen_initialize_method(uint32_t index)
{
    il2cpp_mono_initialize_method_metadata(index);
}

inline bool il2cpp_codegen_type_implements_virtual_method(RuntimeClass* type, RuntimeMethod *slot)
{
    return mono_unity_method_get_class(slot) == type;
}

inline MethodBase_t* il2cpp_codegen_get_method_object(const RuntimeMethod* method)
{
    if (unity_mono_method_is_inflated(const_cast<RuntimeMethod*>(method)))
        method = mono_unity_method_get_generic_definition(const_cast<RuntimeMethod*>(method));

    return (MethodBase_t*)mono_unity_method_get_object(const_cast<RuntimeMethod*>(method));
}

inline Type_t* il2cpp_codegen_get_type(Il2CppMethodPointer getTypeFunction, String_t* typeName, const char* assemblyName)
{
    typedef Type_t* (*getTypeFuncType)(String_t*);
    MonoString* assemblyQualifiedTypeName = mono_unity_string_append_assembly_name_if_necessary((MonoString*)typeName, assemblyName);

    // Try to find the type using a hint about about calling assembly. If it is not found, fall back to calling GetType without the hint.
    Type_t* type = ((getTypeFuncType)getTypeFunction)((String_t*)assemblyQualifiedTypeName);
    if (type == NULL)
        return ((getTypeFuncType)getTypeFunction)(typeName);
    return type;
}

inline Type_t* il2cpp_codegen_get_type(Il2CppMethodPointer getTypeFunction, String_t* typeName, bool throwOnError, const char* assemblyName)
{
    typedef Type_t* (*getTypeFuncType)(String_t*, bool);
    MonoString* assemblyQualifiedTypeName = mono_unity_string_append_assembly_name_if_necessary((MonoString*)typeName, assemblyName);

    // Try to find the type using a hint about about calling assembly. If it is not found, fall back to calling GetType without the hint.
    Type_t* type = ((getTypeFuncType)getTypeFunction)((String_t*)assemblyQualifiedTypeName, throwOnError);
    if (type == NULL)
        return ((getTypeFuncType)getTypeFunction)(typeName, throwOnError);
    return type;
}

inline Type_t* il2cpp_codegen_get_type(Il2CppMethodPointer getTypeFunction, String_t* typeName, bool throwOnError, bool ignoreCase, const char* assemblyName)
{
    typedef Type_t* (*getTypeFuncType)(String_t*, bool, bool);
    MonoString* assemblyQualifiedTypeName = mono_unity_string_append_assembly_name_if_necessary((MonoString*)typeName, assemblyName);

    // Try to find the type using a hint about about calling assembly. If it is not found, fall back to calling GetType without the hint.
    Type_t* type = ((getTypeFuncType)getTypeFunction)((String_t*)assemblyQualifiedTypeName, throwOnError, ignoreCase);
    if (type == NULL)
        return ((getTypeFuncType)getTypeFunction)(typeName, throwOnError, ignoreCase);
    return type;
}

inline Assembly_t* il2cpp_codegen_get_executing_assembly(const RuntimeMethod* method)
{
    return (Assembly_t*)mono_assembly_get_object(g_MonoDomain, mono_unity_class_get_assembly(mono_unity_method_get_class(method)));
}

// Atomic

inline void* il2cpp_codegen_atomic_compare_exchange_pointer(void* volatile* dest, void* exchange, void* comparand)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

template<typename T>
inline T* il2cpp_codegen_atomic_compare_exchange_pointer(T* volatile* dest, T* newValue, T* oldValue)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

// COM

template<typename T>
inline T* il2cpp_codegen_com_query_interface(Il2CppComObject* rcw)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

template<typename T>
inline T* il2cpp_codegen_com_query_interface_no_throw(Il2CppComObject* rcw)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline void il2cpp_codegen_com_marshal_variant(RuntimeObject* obj, Il2CppVariant* variant)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

inline RuntimeObject* il2cpp_codegen_com_marshal_variant_result(Il2CppVariant* variant)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline void il2cpp_codegen_com_destroy_variant(Il2CppVariant* variant)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

inline Il2CppSafeArray* il2cpp_codegen_com_marshal_safe_array(Il2CppChar type, RuntimeArray* managedArray)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline RuntimeArray* il2cpp_codegen_com_marshal_safe_array_result(Il2CppChar variantType, RuntimeClass* type, Il2CppSafeArray* safeArray)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline Il2CppSafeArray* il2cpp_codegen_com_marshal_safe_array_bstring(RuntimeArray* managedArray)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline RuntimeArray* il2cpp_codegen_com_marshal_safe_array_bstring_result(RuntimeClass* type, Il2CppSafeArray* safeArray)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline void il2cpp_codegen_com_destroy_safe_array(Il2CppSafeArray* safeArray)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

inline void il2cpp_codegen_com_create_instance(const Il2CppGuid& clsid, Il2CppIUnknown** identity)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

inline void il2cpp_codegen_com_register_rcw(Il2CppComObject* rcw)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

template<typename T>
inline T* il2cpp_codegen_com_get_or_create_rcw_from_iunknown(Il2CppIUnknown* unknown, RuntimeClass* fallbackClass)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

template<typename T>
inline T* il2cpp_codegen_com_get_or_create_rcw_from_iinspectable(Il2CppIInspectable* unknown, RuntimeClass* fallbackClass)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

template<typename T>
inline T* il2cpp_codegen_com_get_or_create_rcw_for_sealed_class(Il2CppIUnknown* unknown, RuntimeClass* objectClass)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline void il2cpp_codegen_il2cpp_com_object_cleanup(Il2CppComObject* rcw)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

template<typename InterfaceType>
inline InterfaceType* il2cpp_codegen_com_get_or_create_ccw(RuntimeObject* obj)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline intptr_t il2cpp_codegen_com_get_iunknown_for_object(RuntimeObject* obj)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return 0;
}

inline void il2cpp_codegen_com_raise_exception(il2cpp_hresult_t hr)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

inline void il2cpp_codegen_com_raise_exception_if_failed(il2cpp_hresult_t hr, bool defaultToCOMException)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
}

inline RuntimeException* il2cpp_codegen_com_get_exception(il2cpp_hresult_t hr, bool defaultToCOMException)
{
    IL2CPP_NOT_IMPLEMENTED("Not implemented yet.");
    return NULL;
}

inline RuntimeException* il2cpp_codegen_com_get_exception_for_invalid_iproperty_cast(RuntimeObject* value, const char* a, const char* b)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline Il2CppIActivationFactory* il2cpp_codegen_windows_runtime_get_activation_factory(const il2cpp::utils::StringView<Il2CppNativeChar>& runtimeClassName)
{
    IL2CPP_NOT_IMPLEMENTED("COM is not yet supported with the libmonoruntime backend.");
    return NULL;
}

// delegate

inline Il2CppAsyncResult* il2cpp_codegen_delegate_begin_invoke(RuntimeDelegate* delegate, void** params, RuntimeDelegate* asyncCallback, RuntimeObject* state)
{
    return il2cpp_mono_delegate_begin_invoke(delegate, params, asyncCallback, state);
}

inline RuntimeObject* il2cpp_codegen_delegate_end_invoke(Il2CppAsyncResult* asyncResult, void **out_args)
{
    return il2cpp_mono_delegate_end_invoke(asyncResult, out_args);
}

inline const Il2CppGenericInst* il2cpp_codegen_get_generic_class_inst(RuntimeClass* genericClass)
{
    IL2CPP_NOT_IMPLEMENTED("Windows runtime is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline RuntimeClass* il2cpp_codegen_inflate_generic_class(RuntimeClass* genericClassDefinition, const Il2CppGenericInst* genericInst)
{
    //return il2cpp::vm::Class::GetInflatedGenericInstanceClass(genericClassDefinition, genericInst);
    IL2CPP_NOT_IMPLEMENTED("Windows runtime is not yet supported with the libmonoruntime backend.");
    return NULL;
}

inline RuntimeAssembly* il2cpp_codegen_mono_corlib()
{
    return mono_unity_assembly_get_mscorlib();
}

inline RuntimeClass* il2cpp_codegen_mono_class(AssemblyIndex assemblyIndex, uint32_t classToken)
{
    return mono_class_get(mono_assembly_get_image(il2cpp_mono_assembly_from_index(assemblyIndex)), classToken);
}

inline RuntimeClass* il2cpp_codegen_mono_class(RuntimeAssembly* assembly, uint32_t classToken)
{
    return mono_class_get(mono_assembly_get_image(assembly), classToken);
}

inline RuntimeMethod* il2cpp_codegen_mono_method(AssemblyIndex index, uint32_t methodToken)
{
    return mono_get_method(mono_assembly_get_image(il2cpp_mono_assembly_from_index(index)), methodToken, NULL);
}

inline RuntimeMethod* il2cpp_codegen_mono_method(RuntimeAssembly* assembly, uint32_t methodToken)
{
    return mono_get_method(mono_assembly_get_image(assembly), methodToken, NULL);
}

inline void* il2cpp_codegen_mono_get_static_field_address(RuntimeClass* klass, RuntimeField* field)
{
    return il2cpp_mono_get_static_field_address(klass, field);
}

inline void* il2cpp_codegen_mono_get_thread_static_field_address(RuntimeClass* klass, RuntimeField* field)
{
    return il2cpp_mono_get_thread_static_field_address(klass, field);
}

inline RuntimeField* il2cpp_codegen_mono_class_get_field(RuntimeClass* klass, uint32_t fieldToken)
{
    return mono_class_get_field(klass, fieldToken);
}

inline Il2CppMethodPointer il2cpp_codegen_get_method_pointer(const RuntimeMethod* method)
{
    MonoError unused;
    il2cpp_mono_method_initialize_function_pointers(const_cast<RuntimeMethod*>(method), &unused);
    return (Il2CppMethodPointer)mono_unity_method_get_method_pointer(const_cast<RuntimeMethod*>(method));
}

inline RuntimeType* il2cpp_codegen_method_return_type(const RuntimeMethod* method)
{
    return mono_signature_get_return_type(mono_method_signature(const_cast<RuntimeMethod*>(method)));
}

inline int il2cpp_codegen_method_parameter_count(const RuntimeMethod* method)
{
    return mono_signature_get_param_count(mono_method_signature(const_cast<RuntimeMethod*>(method)));
}

template<class T>
T il2cpp_mono_cast_nullable_method_param(const RuntimeMethod *method, int index, void *value)
{
    if (value)
        return *((T*)value);

    T retVal;
    RuntimeClass *klass = mono_unity_signature_get_class_for_param(mono_method_signature(const_cast<RuntimeMethod*>(method)), index);
    mono_nullable_init((uint8_t*)&retVal, NULL, klass);
    return retVal;
}

inline const RuntimeMethod* il2cpp_codegen_vtable_slot_method(const RuntimeClass* klass, RuntimeMethod* slot)
{
    return slot;
}

inline Il2CppMethodPointer il2cpp_codegen_vtable_slot_method_pointer(const RuntimeClass* klass, RuntimeMethod* slot)
{
    MonoError unused;
    il2cpp_mono_method_initialize_function_pointers(slot, &unused);
    return (Il2CppMethodPointer)mono_unity_method_get_method_pointer(slot);
}

inline bool il2cpp_codegen_is_import_or_windows_runtime(const RuntimeObject *object)
{
    assert(0 && "Not implemented yet.");
    return false;
}

inline intptr_t il2cpp_codegen_get_com_interface_for_object(RuntimeObject* object, Type_t* type)
{
    assert(0 && "Not implemented yet.");
    return 0;
}

inline NORETURN void il2cpp_codegen_raise_profile_exception(const RuntimeMethod* method)
{
    il2cpp_codegen_raise_exception(il2cpp_codegen_get_not_supported_exception(mono_unity_method_get_name(method)));
}
