#if RUNTIME_MONO

#include <cassert>

#include "il2cpp-tokentype.h"
#include "il2cpp-class-internals.h"
#include "il2cpp-object-internals.h"

#include "il2cpp-mapping.h"
#include "il2cpp-mono-support.h"

#include "../libmono/mono-api.h"
#include "utils/dynamic_array.h"
#include "utils/StringUtils.h"
#if IL2CPP_ENABLE_NATIVE_STACKTRACES
#include "vm-utils/NativeSymbol.h"
#endif // IL2CPP_ENABLE_NATIVE_STACKTRACES
#include "utils/MarshalingUtils.h"
#include "../libmono/vm/MetadataCache.h"
#include <string>
#include "utils/Il2CppHashMap.h"
#include "utils/HashUtils.h"
#include "utils/PathUtils.h"
#include "utils/StringUtils.h"
#include "os/Mutex.h"

extern const Il2CppCodeRegistration g_CodeRegistration IL2CPP_ATTRIBUTE_WEAK;
extern "C" const Il2CppMethodSpec g_Il2CppMethodSpecTable[] IL2CPP_ATTRIBUTE_WEAK;
#if IL2CPP_ENABLE_NATIVE_STACKTRACES
#endif // IL2CPP_ENABLE_NATIVE_STACKTRACES

// Mono-specific metadata emitted by IL2CPP
extern "C" void** const g_MetadataUsages[] IL2CPP_ATTRIBUTE_WEAK;
extern "C" const MonoGenericInstMetadata* const g_MonoGenericInstMetadataTable[] IL2CPP_ATTRIBUTE_WEAK;
extern "C" const Il2CppCodeGenOptions s_Il2CppCodeGenOptions IL2CPP_ATTRIBUTE_WEAK;

extern const int g_Il2CppInteropDataCount IL2CPP_ATTRIBUTE_WEAK;
extern Il2CppInteropData g_Il2CppInteropData[] IL2CPP_ATTRIBUTE_WEAK;

static MonoGenericContext GetSharedContext(const MonoGenericContext* context);
static MonoAssembly** s_MonoAssemblies;
static int s_MonoAssembliesCount = 0;

MonoAssembly* il2cpp_mono_assembly_from_name(const char* name);

typedef Il2CppHashMap<uint64_t, const Il2CppInteropData*, il2cpp::utils::PassThroughHash<uint64_t> > InteropDataMap;
static InteropDataMap s_InteropDataMap;

typedef Il2CppHashMap<MonoClass*, const Il2CppInteropData*, il2cpp::utils::PointerHash<MonoClass> > InteropDataPointerCacheMap;
static InteropDataPointerCacheMap s_InteropDataPointerCacheMap;
static il2cpp::os::FastMutex s_InteropPointerCacheMutex;

MonoDomain *g_MonoDomain;

void il2cpp_mono_error_init(MonoError *oerror)
{
    MonoErrorInternal *error = (MonoErrorInternal*)oerror;
    error->error_code = MONO_ERROR_NONE;
    error->flags = 0;
}

bool il2cpp_mono_error_ok(MonoError *error)
{
    return error->error_code == MONO_ERROR_NONE;
}

static MonoGenericInst* GetSharedGenericInst(MonoGenericInst* inst)
{
    std::vector<MonoType*> types;
    unsigned int type_argc = mono_unity_generic_inst_get_type_argc(inst);
    for (uint32_t i = 0; i < type_argc; ++i)
    {
        MonoType *type = mono_unity_generic_inst_get_type_argument(inst, i);

        if (mono_type_is_reference(type))
        {
            types.push_back(mono_class_get_type(mono_get_object_class()));
        }
        else if (mono_unity_type_is_generic_instance(type))
        {
            MonoGenericClass *gclass =  mono_unity_type_get_generic_class(type);
            MonoGenericContext context = mono_unity_generic_class_get_context(gclass);
            MonoGenericContext sharedContext = GetSharedContext(&context);
            MonoClass *container_class = mono_unity_generic_class_get_container_class(gclass);
            MonoError unused;
            MonoClass *klass = mono_class_inflate_generic_class_checked(container_class, &sharedContext, &unused);
            types.push_back(mono_class_get_type(klass));
        }
        else if (mono_unity_type_is_enum_type(type) && s_Il2CppCodeGenOptions.enablePrimitiveValueTypeGenericSharing)
        {
            MonoType* underlyingType = mono_type_get_underlying_type(type);
            switch (underlyingType->type)
            {
                case IL2CPP_TYPE_I1:
                    type = mono_class_get_type(mono_class_from_name(mono_get_corlib(), "System", "SByteEnum"));
                    break;
                case IL2CPP_TYPE_I2:
                    type = mono_class_get_type(mono_class_from_name(mono_get_corlib(), "System", "Int16Enum"));
                    break;
                case IL2CPP_TYPE_I4:
                    type = mono_class_get_type(mono_class_from_name(mono_get_corlib(), "System", "Int32Enum"));
                    break;
                case IL2CPP_TYPE_I8:
                    type = mono_class_get_type(mono_class_from_name(mono_get_corlib(), "System", "Int64Enum"));
                    break;
                case IL2CPP_TYPE_U1:
                    type = mono_class_get_type(mono_class_from_name(mono_get_corlib(), "System", "ByteEnum"));
                    break;
                case IL2CPP_TYPE_U2:
                    type = mono_class_get_type(mono_class_from_name(mono_get_corlib(), "System", "UInt16Enum"));
                    break;
                case IL2CPP_TYPE_U4:
                    type = mono_class_get_type(mono_class_from_name(mono_get_corlib(), "System", "UInt32Enum"));
                    break;
                case IL2CPP_TYPE_U8:
                    type = mono_class_get_type(mono_class_from_name(mono_get_corlib(), "System", "UInt64Enum"));
                    break;
                default:
                    IL2CPP_ASSERT(0 && "Invalid enum underlying type");
                    break;
            }
            types.push_back(type);
        }
        else
        {
            types.push_back(mono_unity_generic_inst_get_type_argument(inst, i));
        }
    }

    return mono_metadata_get_generic_inst(type_argc, &types[0]);
}

static MonoGenericContext GetSharedContext(const MonoGenericContext* context)
{
    MonoGenericContext newContext = {0};
    if (context->class_inst != NULL)
        newContext.class_inst = GetSharedGenericInst(context->class_inst);

    if (context->method_inst != NULL)
        newContext.method_inst = GetSharedGenericInst(context->method_inst);

    return newContext;
}

static MonoMethod* InflateGenericMethodWithContext(MonoMethod* method, MonoGenericContext* context, bool useSharedVersion)
{
    if (useSharedVersion)
    {
        MonoGenericContext sharedContext = GetSharedContext(context);
        return mono_class_inflate_generic_method(method, &sharedContext);
    }

    return mono_class_inflate_generic_method(method, context);
}

void initialize_interop_data_map()
{
    for (int i = 0; i < g_Il2CppInteropDataCount; ++i)
        s_InteropDataMap.add(g_Il2CppInteropData[i].hash, &g_Il2CppInteropData[i]);
}

const Il2CppInteropData* FindInteropDataFor(MonoClass* klass)
{
    il2cpp::os::FastAutoLock cacheLock(&s_InteropPointerCacheMutex);
    InteropDataPointerCacheMap::const_iterator itCache = s_InteropDataPointerCacheMap.find(klass);
    if (itCache != s_InteropDataPointerCacheMap.end())
        return itCache->second;

    uint64_t hash = mono_unity_type_get_hash(mono_class_get_type(klass), true);

    InteropDataMap::const_iterator it = s_InteropDataMap.find(hash);
    if (it != s_InteropDataMap.end())
    {
        const Il2CppInteropData *value = it->second;
        s_InteropDataPointerCacheMap.add(klass, value);
        return value;
    }

    return NULL;
}

static void StructureToPtr(MonoObject* structure, void* ptr, bool deleteOld)
{
    if (structure == NULL)
        mono_raise_exception(mono_get_exception_argument_null("structure"));

    if (ptr == NULL)
        mono_raise_exception(mono_get_exception_argument_null("ptr"));

    MonoClass* klass = mono_object_get_class(structure);
    const Il2CppInteropData* interopData = FindInteropDataFor(klass);

    if (interopData != NULL && interopData->pinvokeMarshalToNativeFunction != NULL)
    {
        if (deleteOld)
            il2cpp::utils::MarshalingUtils::MarshalFreeStruct(ptr, interopData);

        void* objectPtr = mono_unity_class_is_class_type(klass) ? structure : mono_object_unbox(structure);
        il2cpp::utils::MarshalingUtils::MarshalStructToNative(objectPtr, ptr, interopData);
        return;
    }

    // If there's no custom marshal function, it means it's either a primitive, or invalid argument

    uint32_t nativeSize = mono_unity_class_get_native_size(klass);
    if (nativeSize != -1)
    {
        // StructureToPtr is supposed to throw on strings and enums
        if (!mono_class_is_enum(klass) && !mono_unity_class_is_string(klass))
        {
            memcpy(ptr, mono_object_unbox(structure), nativeSize);
            return;
        }
    }

    // If we got this far, throw an exception
    MonoException* exception;

    if (mono_class_is_generic(klass))
    {
        exception = mono_get_exception_argument("structure", "The specified object must not be an instance of a generic type.");
    }
    else
    {
        exception = mono_get_exception_argument("structure", "The specified structure must be blittable or have layout information.");
    }

    mono_raise_exception(exception);
}

static void* MarshalInvokerMethod(Il2CppMethodPointer func, const MonoMethod* method, void* thisArg, void** params)
{
    if (strcmp(mono_unity_method_get_name(method), "PtrToStructure") == 0)
    {
        const Il2CppInteropData* interopData = FindInteropDataFor(mono_unity_method_get_class(method));
        if (interopData != NULL && mono_unity_class_is_class_type(mono_unity_method_get_class(method)))
            il2cpp::utils::MarshalingUtils::MarshalStructFromNative((void*)(*(intptr_t*)params[0]), params[1], interopData);
        else if (interopData != NULL)
            il2cpp::utils::MarshalingUtils::MarshalStructFromNative((void*)(*(intptr_t*)params[0]), mono_object_unbox((MonoObject*)params[1]), interopData);
        else // The type is blittable
            memcpy(mono_object_unbox((MonoObject*)params[1]), (void*)(*(intptr_t*)params[0]), mono_unity_class_get_native_size(mono_unity_method_get_class(method)));
    }
    else if (strcmp(mono_unity_method_get_name(method), "StructureToPtr") == 0)
        StructureToPtr((MonoObject*)params[0], (void*)(*(intptr_t*)params[1]), *(bool*)params[2]);
    else
        assert(0 && "Handle another special marshaling method");

    return (MonoObject*)params[1];
}

Il2CppCodeGenModule* InitializeCodeGenHandle(MonoImage* image)
{
    if (image->il2cpp_codegen_handle)
        return (Il2CppCodeGenModule*)image->il2cpp_codegen_handle;

    for (uint32_t codeGenModuleIndex = 0; codeGenModuleIndex < g_CodeRegistration.codeGenModulesCount; ++codeGenModuleIndex)
    {
        std::string name = il2cpp::utils::PathUtils::PathNoExtension(g_CodeRegistration.codeGenModules[codeGenModuleIndex]->moduleName);
        if (strcmp(image->assembly_name, name.c_str()) == 0)
        {
            image->il2cpp_codegen_handle = (void*)g_CodeRegistration.codeGenModules[codeGenModuleIndex];
            return (Il2CppCodeGenModule*)image->il2cpp_codegen_handle;
        }
    }

    return NULL;
}

static il2cpp::os::FastMutex s_MonoMethodFunctionPointerInitializationMutex;

void il2cpp_mono_method_initialize_function_pointers(MonoMethod* method, MonoError* error)
{
    il2cpp::os::FastAutoLock lock(&s_MonoMethodFunctionPointerInitializationMutex);

    bool* isSpecialMarshalingMethod = NULL;
    assert(method != NULL);

    int32_t invokerIndex = -1;
    int32_t methodPointerIndex = -1;

    il2cpp_mono_error_init(error);

    if (method->invoke_pointer || method->method_pointer)
        return;

    //char *methodName = mono_method_get_name_full(method, true, false, MONO_TYPE_NAME_FORMAT_IL);

    if (unity_mono_method_is_inflated(method))
    {
        // Use the fully shared version to look up the method pointer, as we will invoke the fully shared version.
        MonoMethod* definition = mono_method_get_method_definition(method);
        MonoMethod* sharedMethod = InflateGenericMethodWithContext(definition, mono_method_get_context(method), true);

        //char *newMethodName = mono_method_get_name_full(sharedMethod, true, false, MONO_TYPE_NAME_FORMAT_IL);

        uint64_t hash = mono_unity_method_get_hash(sharedMethod, true);
        const MonoMethodInfoMetadata *methodInfo = mono::vm::MetadataCache::GetMonoGenericMethodInfoFromMethodHash(hash);

        if (methodInfo)
        {
            invokerIndex = methodInfo->invoker_index;
            methodPointerIndex = methodInfo->method_pointer_index;
        }
        else
        {
            mono_error_set_execution_engine(error, "Unable to create the method '%s' at runtime", mono_unity_method_get_name(method));
            return;
        }

        assert(invokerIndex != -1 && "The method does not have an invoker, is it a generic method?");
        assert((uint32_t)invokerIndex < g_CodeRegistration.invokerPointersCount);
        mono_unity_method_set_invoke_pointer(method, (void*)g_CodeRegistration.invokerPointers[invokerIndex]);

        assert((uint32_t)methodPointerIndex < g_CodeRegistration.genericMethodPointersCount);
        mono_unity_method_set_method_pointer(method, (void*)g_CodeRegistration.genericMethodPointers[methodPointerIndex]);
    }
    else
    {
        InitializeCodeGenHandle(method->klass->image);
        Il2CppCodeGenModule* codeGenModule = (Il2CppCodeGenModule*)method->klass->image->il2cpp_codegen_handle;

        if (!codeGenModule)
        {
            // Throw a missing method exception if we did not convert it. Once we know this code works for all runtime invoke cases,
            // we should throw this exception more often. Until then, we will leave theassert below when we don't find a method.
            mono_error_set_method_load(error, mono_unity_method_get_class(method), mono_unity_method_get_name(method), NULL, "This method was not converted ahead-of-time by IL2CPP.");
            return;
        }
        else if (strcmp(mono_unity_method_get_name(method), "PtrToStructure") == 0 || strcmp(mono_unity_method_get_name(method), "StructureToPtr") == 0)
        {
            mono_unity_method_set_invoke_pointer(method, (void*)&MarshalInvokerMethod);
            return;
        }

        invokerIndex = method->token & 0x00FFFFFF;
        methodPointerIndex = method->token & 0x00FFFFFF;

        invokerIndex = method->token & 0x00FFFFFF;
        assert((uint32_t)invokerIndex <= codeGenModule->methodPointerCount);
        invokerIndex = codeGenModule->invokerIndices[invokerIndex - 1];
        methodPointerIndex = method->token & 0x00FFFFFF;

        assert(invokerIndex != -1 && "The method does not have an invoker, is it a generic method?");
        assert((uint32_t)invokerIndex < g_CodeRegistration.invokerPointersCount);
        mono_unity_method_set_invoke_pointer(method, (void*)g_CodeRegistration.invokerPointers[invokerIndex]);

        assert((uint32_t)methodPointerIndex <= codeGenModule->methodPointerCount);
        mono_unity_method_set_method_pointer(method, (void*)codeGenModule->methodPointers[methodPointerIndex - 1]);
    }
}

void il2cpp_mono_init_assemblies()
{
    s_MonoAssembliesCount = mono::vm::MetadataCache::GetMonoAssemblyCount();
    s_MonoAssemblies = new MonoAssembly*[mono::vm::MetadataCache::GetMonoAssemblyCount()];
    for (int i = 0; i < s_MonoAssembliesCount; ++i)
        s_MonoAssemblies[i] = il2cpp_mono_assembly_from_name(mono::vm::MetadataCache::GetMonoAssemblyNameFromIndex(i));
}

MonoAssembly* il2cpp_mono_assembly_from_index(AssemblyIndex index)
{
    if (s_MonoAssembliesCount == 0)
        il2cpp_mono_init_assemblies();

    assert(index < s_MonoAssembliesCount && "assembly index is out of range");
    return s_MonoAssemblies[index];
}

MonoAssembly* il2cpp_mono_assembly_from_name(const char* name)
{
    // First look for the cached assembly in the domain.
    MonoAssembly* assembly = mono_domain_assembly_open(g_MonoDomain, name);
    if (assembly == NULL)
    {
        // If we can't find it, look in the path we've set to find assemblies.
        MonoImageOpenStatus unused;
        assembly = mono_assembly_load_with_partial_name(name, &unused);
    }

    return assembly;
}

MonoMethod* il2cpp_mono_get_virtual_target_method(MonoMethod* method, MonoObject* obj)
{
    // This is *very* slow, and likely not good enough for production. It is good enough for proof of concept though.
    MonoMethod* target_method = mono_object_get_virtual_method(obj, method);

    if (mono_unity_class_is_array(mono_unity_method_get_class(target_method)))
        target_method = mono_unity_method_get_aot_array_helper_from_wrapper(target_method);

    return target_method;
}

MonoMethod* il2cpp_mono_get_virtual_target_method_fast(MonoMethod* method, MonoObject* obj)
{
    if (method->flags & METHOD_ATTRIBUTE_FINAL)
        return method;

    MonoClass *klass = obj->vtable->klass;
    if (!klass->vtable)
        mono_class_setup_vtable(klass);

    MonoMethod* target_method = klass->vtable[method->slot];

    if (target_method->klass->rank > 0)
        target_method = mono_unity_method_get_aot_array_helper_from_wrapper(target_method);

    return target_method;
}

MonoMethod* il2cpp_mono_get_interface_target_method_fast(MonoMethod* method, MonoObject* obj)
{
    if (method->flags & METHOD_ATTRIBUTE_FINAL)
        return method;

    MonoClass *klass = obj->vtable->klass;
    if (!klass->vtable)
        mono_class_setup_vtable(klass);

    gboolean variance_used = 0;
    int iface_offset = mono_class_interface_offset_with_variance(klass, method->klass, &variance_used);
    MonoMethod* target_method = klass->vtable[iface_offset + method->slot];

    if (target_method->klass->rank > 0)
        target_method = mono_unity_method_get_aot_array_helper_from_wrapper(target_method);

    return target_method;
}

void il2cpp_mono_get_invoke_data(MonoMethod* method, void* obj, VirtualInvokeData* invokeData)
{
    MonoMethod* target_method = il2cpp_mono_get_virtual_target_method(method, (MonoObject*)obj);

    MonoError error;
    il2cpp_mono_method_initialize_function_pointers(target_method, &error);
    if (!il2cpp_mono_error_ok(&error))
        mono_error_raise_exception_deprecated(&error);
    invokeData->methodPtr = (Il2CppMethodPointer)mono_unity_method_get_method_pointer(target_method);
    invokeData->method = target_method;
}

void il2cpp_mono_get_virtual_invoke_data(MonoMethod* method, void* obj, VirtualInvokeData* invokeData)
{
    MonoMethod* target_method = il2cpp_mono_get_virtual_target_method_fast(method, (MonoObject*)obj);

    MonoError error;
    il2cpp_mono_method_initialize_function_pointers(target_method, &error);
    if (!il2cpp_mono_error_ok(&error))
        mono_error_raise_exception_deprecated(&error);
    invokeData->methodPtr = (Il2CppMethodPointer)mono_unity_method_get_method_pointer(target_method);
    invokeData->method = target_method;
}

void il2cpp_mono_get_interface_invoke_data(MonoMethod* method, void* obj, VirtualInvokeData* invokeData)
{
    MonoMethod* target_method = il2cpp_mono_get_interface_target_method_fast(method, (MonoObject*)obj);

    MonoError error;
    il2cpp_mono_method_initialize_function_pointers(target_method, &error);
    if (!il2cpp_mono_error_ok(&error))
        mono_error_raise_exception_deprecated(&error);
    invokeData->methodPtr = (Il2CppMethodPointer)mono_unity_method_get_method_pointer(target_method);
    invokeData->method = target_method;
}

static MonoClass* InflateGenericParameter(MonoGenericContainer* genericContainer, MonoGenericContext* context, const MonoRGCTXDefinition* rgctxDefintion)
{
    MonoClass* genericParameter = mono_unity_generic_container_get_parameter_class(genericContainer, rgctxDefintion->generic_parameter_index);
    return mono_unity_class_inflate_generic_class(genericParameter, context);
}

static MonoClass* LookupGenericClassMetadata(MonoGenericContext* context, const MonoRGCTXDefinition* rgctxDefintion)
{
    return mono_class_get_full(mono_assembly_get_image(il2cpp_mono_assembly_from_index(rgctxDefintion->assemblyIndex)), rgctxDefintion->token, context);
}

static void* LookupMetadataFromRGCTX(const MonoRGCTXDefinition* definition, bool useSharedVersion, MonoGenericContext* context, MonoGenericContainer* genericContainer)
{
    if (definition->type == IL2CPP_RGCTX_DATA_CLASS)
    {
        if (mono_metadata_token_table(definition->token) == MONO_TABLE_GENERICPARAM)
            return InflateGenericParameter(genericContainer, context, definition);
        else if (mono_metadata_token_table(definition->token) == MONO_TABLE_TYPESPEC)
            return LookupGenericClassMetadata(context, definition);
        else
            assert(0 && "RGCTX case is not implemented yet.");
    }
    else if (definition->type == IL2CPP_RGCTX_DATA_ARRAY)
    {
        if (mono_metadata_token_table(definition->token) == MONO_TABLE_GENERICPARAM)
            return mono_array_class_get(InflateGenericParameter(genericContainer, context, definition), 1);
        else if (mono_metadata_token_table(definition->token) == MONO_TABLE_TYPESPEC)
            return mono_array_class_get(LookupGenericClassMetadata(context, definition), 1);
        else
            assert(0 && "RGCTX case is not implemented yet.");
    }
    else if (definition->type == IL2CPP_RGCTX_DATA_METHOD)
    {
        MonoMethod* methodDefinition = mono_get_method(mono_assembly_get_image(il2cpp_mono_assembly_from_index(definition->assemblyIndex)), definition->token, NULL);
        return InflateGenericMethodWithContext(methodDefinition, context, useSharedVersion);
    }
    else if (definition->type == IL2CPP_RGCTX_DATA_TYPE)
    {
        if (mono_metadata_token_table(definition->token) == MONO_TABLE_GENERICPARAM)
            return mono_class_get_type(InflateGenericParameter(genericContainer, context, definition));
        else if (mono_metadata_token_table(definition->token) == MONO_TABLE_TYPESPEC)
            return mono_class_get_type(LookupGenericClassMetadata(context, definition));
        else
            assert(0 && "RGCTX case is not implemented yet.");
    }
    else
    {
        assert(0 && "RGCTX case is not implemented yet.");
    }

    return NULL;
}

static int CompareIl2CppTokenRangePair(const void* pkey, const void* pelem)
{
    return (int)(((Il2CppTokenRangePair*)pkey)->token - ((Il2CppTokenRangePair*)pelem)->token);
}

typedef struct
{
    int32_t count;
    const MonoRGCTXDefinition* items;
} RGCTXCollection;

static RGCTXCollection GetRGCTXs(MonoImage* image, uint32_t token)
{
    InitializeCodeGenHandle(image);
    Il2CppCodeGenModule* codegenModule = (Il2CppCodeGenModule*)image->il2cpp_codegen_handle;
    RGCTXCollection collection = { 0, NULL };
    if (codegenModule->rgctxRangesCount == 0)
        return collection;

    Il2CppTokenRangePair key;
    memset(&key, 0, sizeof(Il2CppTokenRangePair));
    key.token = token;

    const Il2CppTokenRangePair* res = (const Il2CppTokenRangePair*)bsearch(&key, codegenModule->rgctxRanges, codegenModule->rgctxRangesCount, sizeof(Il2CppTokenRangePair), CompareIl2CppTokenRangePair);

    if (res == NULL)
        return collection;

    collection.count = res->range.length;
    collection.items = codegenModule->rgctxs + res->range.start;

    return collection;
}

void* LookupMetadataFromHash(MonoImage* image, uint32_t token, Il2CppRGCTXDataType rgctxType, int rgctxIndex, bool useSharedVersion, MonoGenericContext* context, MonoGenericContainer* genericContainer)
{
    RGCTXCollection collection = GetRGCTXs(image, token);
    if (rgctxIndex >= 0 && rgctxIndex < collection.count)
    {
        const MonoRGCTXDefinition* definition = collection.items + rgctxIndex;
        if (definition->type == rgctxType)
            return LookupMetadataFromRGCTX(definition, useSharedVersion, context, genericContainer);
    }

    return NULL;
}

void* il2cpp_mono_class_rgctx(MonoClass* klass, Il2CppRGCTXDataType rgctxType, int rgctxIndex, bool useSharedVersion)
{
    return LookupMetadataFromHash(klass->image, klass->type_token, rgctxType, rgctxIndex, useSharedVersion,
        mono_class_get_context(klass), mono_class_get_generic_container(mono_unity_class_get_generic_definition(klass)));
}

void* il2cpp_mono_method_rgctx(MonoMethod* method, Il2CppRGCTXDataType rgctxType, int rgctxIndex, bool useSharedVersion)
{
    return LookupMetadataFromHash(method->klass->image, method->token, rgctxType, rgctxIndex, useSharedVersion,
        mono_method_get_context(method), mono_method_get_generic_container(mono_unity_method_get_generic_definition(method)));
}

static MonoClass* ClassFromIndex(TypeIndex index);

static MonoClass* InflateGenericClass(MonoClass* generic_class, uint32_t num_indices, const TypeIndex *generic_indices)
{
    std::vector<MonoType*> arguments(num_indices);
    for (uint32_t i = 0; i < num_indices; ++i)
        arguments[i] = mono_class_get_type(ClassFromIndex(generic_indices[i]));

    MonoGenericInst* generic_instance = mono_metadata_get_generic_inst(num_indices, &arguments[0]);
    MonoGenericContext context;
    context.class_inst = generic_instance;
    context.method_inst = NULL;
    MonoError unused;
    MonoClass* inflated_class = mono_class_inflate_generic_class_checked(generic_class, &context, &unused);
    mono_class_init(inflated_class);

    return inflated_class;
}

static MonoClass* ClassFromMetadata(const MonoClassMetadata *classMetadata)
{
    if (classMetadata->rank > 0)
    {
        MonoClass *klass = ClassFromIndex(classMetadata->elementTypeIndex);
        MonoClass *aklass = mono_array_class_get(klass, classMetadata->rank);
        mono_class_init(aklass);
        return aklass;
    }

    MonoClass *klass = mono_class_get(mono_assembly_get_image(il2cpp_mono_assembly_from_index(classMetadata->metadataToken.assemblyIndex)), classMetadata->metadataToken.token);
    mono_class_init(klass);
    return klass;
}

static MonoClass* ClassFromIndex(TypeIndex index)
{
    if (index == kTypeIndexInvalid)
        return NULL;

    const MonoClassMetadata* classMetadata = mono::vm::MetadataCache::GetClassMetadataFromIndex(index);

    MonoClass* klass = ClassFromMetadata(classMetadata);
    if (classMetadata->genericParametersCount != 0)
        klass = InflateGenericClass(klass, classMetadata->genericParametersCount, mono::vm::MetadataCache::GetGenericArgumentIndices(classMetadata->genericParametersOffset));

    if (classMetadata->isPointer)
        klass = mono_ptr_class_get(mono_class_get_type(klass));

    return klass;
}

static MonoClassField* FieldFromIndex(EncodedMethodIndex index)
{
    const MonoFieldMetadata* fieldMetadata = mono::vm::MetadataCache::GetFieldMetadataFromIndex(index);
    return mono_class_get_field(ClassFromIndex(fieldMetadata->parentTypeIndex), fieldMetadata->token);
}

static MonoType* TypeFromIndex(TypeIndex index)
{
    if (index == kTypeIndexInvalid)
        return NULL;

    return mono_class_get_type(ClassFromIndex(index));
}

MonoMethod* MethodFromIndex(MethodIndex index)
{
    const MonoMetadataToken* method = &mono::vm::MetadataCache::GetMonoMethodMetadataFromIndex(index)->metadataToken;
    return mono_get_method(mono_assembly_get_image(il2cpp_mono_assembly_from_index(method->assemblyIndex)), method->token, NULL);
}

MonoMethod* MethodFromToken(uint32_t token, Il2CppMethodPointer methodPtr)
{
    uint32_t rid = 0x00FFFFFF & token;
    for (uint32_t codeGenModuleIndex = 0; codeGenModuleIndex < g_CodeRegistration.codeGenModulesCount; ++codeGenModuleIndex)
    {
        const Il2CppCodeGenModule* codeGenModule = g_CodeRegistration.codeGenModules[codeGenModuleIndex];
        if (rid <= codeGenModule->methodPointerCount && codeGenModule->methodPointers[rid - 1] == methodPtr)
        {
            MonoAssembly* assembly = il2cpp_mono_assembly_from_name(il2cpp::utils::PathUtils::PathNoExtension(codeGenModule->moduleName).c_str());

            return mono_get_method(mono_assembly_get_image(assembly), token, NULL);
        }
    }

    return NULL;
}

static std::vector<MonoType*> GenericArgumentsFromInst(const MonoGenericInstMetadata *inst)
{
    std::vector<MonoType*> arguments;

    uint32_t numberOfGenericArguments = inst->type_argc;
    if (numberOfGenericArguments == 0)
        return arguments;

    for (uint32_t i = 0; i < numberOfGenericArguments; ++i)
    {
        const TypeIndex argIndex = inst->type_argv_indices[i];
        arguments.push_back(mono_class_get_type(ClassFromIndex(argIndex)));
    }

    return arguments;
}

static MonoGenericInst* GenericInstFromIndex(GenericInstIndex index)
{
    if (index == -1)
        return NULL;

    const MonoGenericInstMetadata* inst = g_MonoGenericInstMetadataTable[index];

    // Replace this with dynamic_array later when we get the libil2cpp utilities build sorted out.
    std::vector<MonoType*> genericArguments = GenericArgumentsFromInst(inst);

    size_t numberOfGenericArguments = genericArguments.size();
    if (numberOfGenericArguments == 0)
        return NULL;

    return mono_metadata_get_generic_inst((int)numberOfGenericArguments, &genericArguments[0]);
}

MonoMethod* GenericMethodFromIndex(MethodIndex index)
{
    const Il2CppMethodSpec* methodSpec = &g_Il2CppMethodSpecTable[index];

    MonoMethod* methodDefinition = MethodFromIndex(methodSpec->methodDefinitionIndex);
    MonoGenericContext generic_context = { GenericInstFromIndex(methodSpec->classIndexIndex) , GenericInstFromIndex(methodSpec->methodIndexIndex) };

    return mono_class_inflate_generic_method(methodDefinition, &generic_context);
}

static MonoString* StringFromIndex(StringIndex index)
{
    const MonoMetadataToken* stringMetadata = mono::vm::MetadataCache::GetMonoStringTokenFromIndex(index);
    return mono_ldstr(g_MonoDomain, mono_assembly_get_image(il2cpp_mono_assembly_from_index(stringMetadata->assemblyIndex)), mono_metadata_token_index(stringMetadata->token));
}

void il2cpp_mono_initialize_method_metadata(uint32_t index)
{
    const Il2CppMetadataUsageList* usageList = mono::vm::MetadataCache::GetMetadataUsageList(index);

    uint32_t start = usageList->start;
    uint32_t count = usageList->count;

    for (uint32_t i = 0; i < count; i++)
    {
        uint32_t offset = start + i;

        const Il2CppMetadataUsagePair* usagePair = mono::vm::MetadataCache::GetMetadataUsagePair(offset);
        uint32_t destinationIndex = usagePair->destinationIndex;
        uint32_t encodedSourceIndex = usagePair->encodedSourceIndex;

        Il2CppMetadataUsage usage = GetEncodedIndexType(encodedSourceIndex);
        uint32_t decodedIndex = GetDecodedMethodIndex(encodedSourceIndex);
        switch (usage)
        {
            case kIl2CppMetadataUsageTypeInfo:
                *g_MetadataUsages[destinationIndex] = ClassFromIndex(decodedIndex);
                break;
            case kIl2CppMetadataUsageFieldInfo:
                *g_MetadataUsages[destinationIndex] = FieldFromIndex(decodedIndex);
                break;
            case kIl2CppMetadataUsageIl2CppType:
                *g_MetadataUsages[destinationIndex] = TypeFromIndex(decodedIndex);
                break;
            case kIl2CppMetadataUsageMethodDef:
                *g_MetadataUsages[destinationIndex] = MethodFromIndex(decodedIndex);
                break;
            case kIl2CppMetadataUsageMethodRef:
                *g_MetadataUsages[destinationIndex] = GenericMethodFromIndex(decodedIndex);
                break;
            case kIl2CppMetadataUsageStringLiteral:
                *g_MetadataUsages[destinationIndex] = StringFromIndex(decodedIndex);
                break;
            default:
                assert(0 && "Unimplemented case for method metadata usage.");
                break;
        }
    }
}

void il2cpp_mono_raise_execution_engine_exception_if_method_is_not_found(MonoMethod* method)
{
    MonoError unused;
    il2cpp_mono_method_initialize_function_pointers(method, &unused);

    if (mono_unity_method_get_method_pointer(method) == NULL)
    {
        MonoException *exc;

        if (mono_unity_method_get_class(method) != NULL)
            exc = mono_get_exception_execution_engine(mono_method_get_name_full(method, true, false, MONO_TYPE_NAME_FORMAT_IL));
        else
            exc = mono_get_exception_execution_engine(mono_unity_method_get_name(method));

        mono_raise_exception(exc);
    }
}

Il2CppAsyncResult* il2cpp_mono_delegate_begin_invoke(MonoDelegate* delegate, void** params, MonoDelegate* asyncCallback, MonoObject* state)
{
    int numParams = mono_signature_get_param_count(mono_method_signature((MonoMethod*)delegate->method));
    MonoObject *nullState = NULL;
    MonoDelegate *nullCallback = NULL;

    std::vector<void*> newParams(numParams + 2);
    for (int i = 0; i < numParams; ++i)
        newParams[i] = params[i];

    //If the state and the callback are both null and there are no other params, we will send null for the params array
    newParams[numParams] = &nullCallback;
    newParams[numParams + 1] = &nullState;
    int numNewParams = numParams;

    if (asyncCallback)
    {
        newParams[numParams] = &asyncCallback;
        ++numNewParams;
    }

    if (state)
    {
        newParams[numParams + 1] = &state;
        ++numNewParams;
    }

    MonoError unused;
    return (Il2CppAsyncResult*)mono_threadpool_begin_invoke(g_MonoDomain, (MonoObject*)delegate, (MonoMethod*)delegate->method, numNewParams != 0 ? &newParams[0] : NULL, &unused);
}

static void MonoArrayGetGenericValue(MonoArray* array, int32_t pos, void* value)
{
    MonoObject *obj = mono_array_get(array, MonoObject*, pos);
    MonoClass *klass = mono_unity_object_get_class(obj);

    if (mono_class_is_valuetype(klass))
        memcpy(value, mono_object_unbox(obj), mono_class_instance_size(klass) - sizeof(MonoObject));
    else
        memcpy(value, &obj, mono_unity_array_get_element_size(array));
}

MonoObject* il2cpp_mono_delegate_end_invoke(Il2CppAsyncResult* asyncResult, void **out_args)
{
    MonoError unused;
    MonoObject *exc = NULL;
    MonoArray *mono_out_args = NULL;
    MonoObject *retVal = (MonoObject*)mono_threadpool_end_invoke((MonoAsyncResult*)asyncResult, &mono_out_args, &exc, &unused);

    if (exc)
        mono_raise_exception((MonoException*)exc);

    uint32_t numArgs = mono_unity_array_get_max_length(mono_out_args);

    for (uint32_t i = 0; i < numArgs; ++i)
        MonoArrayGetGenericValue(mono_out_args, i, out_args[i]);

    return retVal;
}

MonoArray* MonoArrayNew(MonoClass* elementType, uintptr_t length)
{
    MonoError unused;
    return mono_array_new_specific_checked(il2cpp_mono_class_vtable(g_MonoDomain, mono_array_class_get(elementType, 1)), length, &unused);
}

#if IL2CPP_ENABLE_NATIVE_STACKTRACES
void RegisterAllManagedMethods()
{
    uint32_t numberOfMethods = 0;
    uint32_t numberOfGenericMethods = g_CodeRegistration.genericMethodPointersCount;

    for (uint32_t codeGenModuleIndex = 0; codeGenModuleIndex < g_CodeRegistration.codeGenModulesCount; ++codeGenModuleIndex)
    {
        numberOfMethods += g_CodeRegistration.codeGenModules[codeGenModuleIndex]->methodPointerCount;
    }

    std::vector<MethodDefinitionKey> managedMethods(numberOfMethods + numberOfGenericMethods);

    for (uint32_t codeGenModuleIndex = 0; codeGenModuleIndex < g_CodeRegistration.codeGenModulesCount; ++codeGenModuleIndex)
    {
        const Il2CppCodeGenModule* codeGenModule = g_CodeRegistration.codeGenModules[codeGenModuleIndex];
        for (uint32_t i = 0; i < codeGenModule->methodPointerCount; ++i)
        {
            MethodDefinitionKey currentMethod;
            currentMethod.methodIndex = IL2CPP_TOKEN_METHOD_DEF | (i + 1); // produce method token
            currentMethod.method = codeGenModule->methodPointers[i];
            currentMethod.isGeneric = false;
            managedMethods.push_back(currentMethod);
        }
    }

    for (uint32_t i = 0; i < numberOfGenericMethods; ++i)
    {
        MethodDefinitionKey currentMethod;
        currentMethod.methodIndex = mono::vm::MetadataCache::GetGenericMethodIndex(i);
        currentMethod.method = g_CodeRegistration.genericMethodPointers[i];
        currentMethod.isGeneric = true;
        managedMethods.push_back(currentMethod);
    }

    il2cpp::utils::NativeSymbol::RegisterMethods(managedMethods);
}

#endif // IL2CPP_ENABLE_NATIVE_STACKTRACES

void RuntimeInit(MonoClass* klass)
{
    MonoError error;
    mono_runtime_class_init_full(il2cpp_mono_class_vtable(g_MonoDomain, klass), &error);
    if (!il2cpp_mono_error_ok(&error))
        mono_error_raise_exception_deprecated(&error);
}

std::string il2cpp_mono_format_exception(const MonoException *exc)
{
    MonoClass *klass = mono_object_get_class((MonoObject*)exc);
    std::string exception_namespace = mono_class_get_namespace(klass);
    std::string exception_type = mono_class_get_name(klass);
    MonoString *message = mono_unity_exception_get_message(const_cast<MonoException*>(exc));
    if (message)
        return exception_namespace + "." + exception_type + ": " + mono_string_to_utf8(message);
    else
        return exception_namespace + "." + exception_type;
}

void* il2cpp_mono_get_static_field_address(MonoClass *klass, MonoClassField *field)
{
    MonoVTable *vt = il2cpp_mono_class_vtable(g_MonoDomain, klass);
    return (char*)vt->vtable[vt->klass->vtable_size] + field->offset;
}

void* il2cpp_mono_get_thread_static_field_address(MonoClass *klass, MonoClassField *field)
{
    MonoVTable *vt = il2cpp_mono_class_vtable(g_MonoDomain, klass);
    return mono_unity_get_field_address(NULL, vt, field);
}

MonoVTable* il2cpp_mono_class_vtable(MonoDomain *domain, MonoClass *klass)
{
    MonoClassRuntimeInfo *runtime_info;

    runtime_info = klass->runtime_info;
    if (runtime_info)
        return runtime_info->domain_vtables[0];

    MonoError unused;
    return mono_class_vtable_full(domain, klass, &unused);
}

ArgvMono il2cpp_mono_convert_args(int argc, const Il2CppChar* const* argv)
{
    ArgvMono args;

    args.argc = argc;

    args.argvMonoObj = new std::string*[argc];
    for (int i = 0; i < argc; ++i)
        args.argvMonoObj[i] = new std::string();

    for (int i = 0; i < argc; ++i)
        *(args.argvMonoObj[i]) = il2cpp::utils::StringUtils::Utf16ToUtf8(argv[i]);

    args.argvMono = new char*[argc];
    for (int i = 0; i < argc; ++i)
        args.argvMono[i] = const_cast<char*>(args.argvMonoObj[i]->c_str());

    return args;
}

void il2cpp_mono_free_args(ArgvMono& args)
{
    delete[] args.argvMono;

    for (int i = 0; i < args.argc; ++i)
        delete args.argvMonoObj[i];

    delete[] args.argvMonoObj;
}

extern "C"
{
    void il2cpp_set_temp_dir(char *temp_dir)
    {
        assert(0 && "This needs to be implemented for Android");
    }
}

#endif //RUNTIME_MONO
