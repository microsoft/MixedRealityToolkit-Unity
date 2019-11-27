#pragma once

#if RUNTIME_MONO

#include "il2cpp-metadata.h"
#include <vector>
#include <string>

struct MethodInfo;
struct VirtualInvokeData;
struct Il2CppInteropData;
struct Il2CppCodeGenModule;

void il2cpp_mono_method_initialize_function_pointers(MonoMethod* method, MonoError* error);
MonoAssembly* il2cpp_mono_assembly_from_index(AssemblyIndex index);
void* il2cpp_mono_class_rgctx(MonoClass* klass, Il2CppRGCTXDataType rgctxType, int rgctxIndex, bool useSharedVersion);
void* il2cpp_mono_method_rgctx(MonoMethod* method, Il2CppRGCTXDataType rgctxType, int rgctxIndex, bool useSharedVersion);
MonoMethod* il2cpp_mono_get_virtual_target_method(MonoMethod* method, MonoObject* obj);
void il2cpp_mono_get_invoke_data(MonoMethod* method, void* obj, VirtualInvokeData* invokeData);
void il2cpp_mono_get_virtual_invoke_data(MonoMethod* method, void* obj, VirtualInvokeData* invokeData);
void il2cpp_mono_get_interface_invoke_data(MonoMethod* method, void* obj, VirtualInvokeData* invokeData);
void il2cpp_mono_initialize_method_metadata(uint32_t index);
void il2cpp_mono_raise_execution_engine_exception_if_method_is_not_found(MonoMethod* method);
Il2CppAsyncResult* il2cpp_mono_delegate_begin_invoke(MonoDelegate* delegate, void** params, MonoDelegate* asyncCallback, MonoObject* state);
MonoObject* il2cpp_mono_delegate_end_invoke(Il2CppAsyncResult* asyncResult, void **out_args);
MonoArray* MonoArrayNew(MonoClass* elementType, uintptr_t length);
MonoMethod* MethodFromIndex(MethodIndex index);
MonoMethod* MethodFromToken(uint32_t token, Il2CppMethodPointer methodPtr);
MonoMethod* GenericMethodFromIndex(MethodIndex index);
const Il2CppInteropData* FindInteropDataFor(MonoClass* klass);
void RuntimeInit(MonoClass* klass);
std::string il2cpp_mono_format_exception(const MonoException *exc);
void initialize_interop_data_map();
void il2cpp_mono_error_init(MonoError *error);
bool il2cpp_mono_error_ok(MonoError *error);
MonoVTable* il2cpp_mono_class_vtable(MonoDomain *domain, MonoClass *klass);
void* il2cpp_mono_get_static_field_address(MonoClass *klass, MonoClassField *field);
void* il2cpp_mono_get_thread_static_field_address(MonoClass *klass, MonoClassField *field);

Il2CppCodeGenModule* InitializeCodeGenHandle(MonoImage* image);

struct ArgvMono
{
    int argc;
    std::string **argvMonoObj;
    char **argvMono;
};

ArgvMono il2cpp_mono_convert_args(int argc, const Il2CppChar* const* argv);
void il2cpp_mono_free_args(ArgvMono& args);

#if IL2CPP_ENABLE_NATIVE_STACKTRACES
struct MethodDefinitionKey
{
    Il2CppMethodPointer method;
    MethodIndex methodIndex;
    bool isGeneric;
};

void RegisterAllManagedMethods();
#endif // IL2CPP_ENABLE_NATIVE_STACKTRACES

extern MonoDomain *g_MonoDomain;

#endif
