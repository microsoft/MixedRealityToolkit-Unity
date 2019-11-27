#if RUNTIME_MONO

#include <cassert>
#include <cstring>
#include "mono-api.h"
#include "../libmono/icalls/mscorlib/System.Diagnostics/StackFrame.h"
#include "../libmono/icalls/mscorlib/System.Diagnostics/StackTrace.h"
#include "../libmono/icalls/mscorlib/System.Runtime.InteropServices/Marshal.h"
#include "../libmono/vm/StackTrace.h"
#include "../libmono/vm/MetadataCache.h"
#include "il2cpp-callbacks.h"
#include "il2cpp-mono-support.h"
#include "il2cpp-class-internals.h"
#include "il2cpp-object-internals.h"
#include <iterator>
#include "os/Image.h"
#include "os/Thread.h"
#include "os/c-api/Allocator.h"
#include "utils/PathUtils.h"
#include "utils/StringUtils.h"
#include "utils/Environment.h"
#include "utils/utf8-cpp/source/utf8.h"
#include "vm-utils/Debugger.h"
#include "vm-utils/NativeSymbol.h"

static void* il2cpp_get_vtable_trampoline(MonoVTable *vtable, int slot_index)
{
    return 0;
}

static void* il2cpp_get_imt_trampoline(MonoVTable *vtable, int imt_slot_index)
{
    return 0;
}

static void mono_thread_start_cb(intptr_t tid, void* stack_start, void* func)
{
    mono::vm::StackTrace::InitializeStackTracesForCurrentThread();
}

static void mono_thread_attach_cb(intptr_t tid, void* stack_start)
{
    mono::vm::StackTrace::InitializeStackTracesForCurrentThread();
}

static void mono_thread_cleanup_cb(MonoNativeThreadId tid)
{
    //assert(tid == il2cpp::os::Thread::CurrentThreadId());
    //mono::vm::StackTrace::CleanupStackTracesForCurrentThread();
}

static void il2cpp_throw_exception(MonoException* ex)
{
    if (mono_unity_exception_get_trace_ips(ex) == NULL)
    {
        // Only write the stack trace if there is not one already in the exception.
        // When we exit managed try/finally and try/catch blocks with an exception, this method is
        // called with the original exception which already has the proper stack trace.
        // Getting the stack trace again here will lose the frames between the original throw
        // and the finally or catch block.
        const mono::vm::StackFrames& frames = *mono::vm::StackTrace::GetStackFrames();
        size_t i = frames.size() - 1;
        MonoArray* ips = MonoArrayNew(mono_unity_defaults_get_int_class(), frames.size());
        for (mono::vm::StackFrames::const_iterator iter = frames.begin(); iter != frames.end(); ++iter, --i)
        {
            mono_array_set(ips, const MonoMethod*, i, (*iter).method);
        }

        mono_unity_exception_set_trace_ips(ex, ips);
    }

    throw Il2CppExceptionWrapper(ex);
}

static void il2cpp_walk_stack_with_ctx(MonoInternalStackWalk func, MonoContext *ctx, MonoUnwindOptions options, void *user_data)
{
    mono::vm::StackTrace::WalkFrameStack(func, ctx, user_data);
}

static int32_t il2cpp_exception_walk_trace(MonoException *ex, MonoInternalExceptionFrameWalk func, void* user_data)
{
    MonoArray *ta = mono_unity_exception_get_trace_ips(ex);
    int len, i;

    if (ta == NULL)
        return 0;

    len = (int)mono_array_length(ta) >> 1;
    for (i = 0; i < len; i++)
    {
        void* ip = mono_array_get(ta, void*, i * 2 + 0);
        void* generic_info = mono_array_get(ta, void*, i * 2 + 1);
        const MonoMethod* method = il2cpp::utils::NativeSymbol::GetMethodFromNativeSymbol((Il2CppMethodPointer)ip);

        if (method == NULL)
        {
            if (func(NULL, ip, 0, 0, user_data))
                return 1;
        }
        else
        {
            if (func(const_cast<MonoMethod*>(method), ip, 0, 1, user_data))
                return 1;
        }
    }

    return len > 0;
}

static int32_t il2cpp_current_thread_has_handle_block_guard()
{
    return 0;
}

static gboolean il2cpp_install_handler_block_guard(MonoThreadUnwindState *ctx)
{
    return false;
}

static MonoObject* il2cpp_mono_fake_box(void *ptr)
{
    return ((MonoObject*)ptr) - 1;
}

MonoObject* il2cpp_mono_determine_target_object(MonoMethod *method, MonoObject *obj)
{
    if (!obj)
        return NULL;

    if (mono_class_is_valuetype(mono_unity_method_get_class(method)))
        return obj;

    MonoClass *klass = mono_unity_object_get_class(obj);
    if (mono_unity_class_is_delegate(klass) && (strcmp(mono_unity_method_get_name(method), "Invoke") != 0))
        return mono_unity_delegate_get_target((MonoDelegate*)obj);

    return obj;
}

MonoObject* il2cpp_mono_runtime_invoke(MonoMethod *method, void *obj, void **params, MonoObject **exc, MonoError *error)
{
    il2cpp_mono_method_initialize_function_pointers(method, error);
    if (!il2cpp_mono_error_ok(error))
        return NULL;

    if (exc)
        *exc = NULL;

    MonoObject *target = il2cpp_mono_determine_target_object(method, (MonoObject*)obj);

    void **newParams = params;
    std::vector<void*> newParamsVec;
    std::vector<int> byRefParams;

    if (params && mono_method_signature(method))
    {
        int numParams = mono_unity_signature_num_parameters(mono_method_signature(method));
        if (numParams > 0)
        {
            newParamsVec.resize(numParams);
            for (int i = 0; i < numParams; ++i)
            {
                MonoClass *paramSigClass = mono_unity_signature_get_class_for_param(mono_method_signature(method), i);
                if (mono_class_is_nullable(paramSigClass))
                {
                    if (mono_unity_signature_param_is_byref(mono_method_signature(method), i))
                        byRefParams.push_back(i);

                    MonoObject *paramObj = (MonoObject*)params[i];
                    void *nullable = alloca(mono_unity_class_get_instance_size(paramSigClass));
                    mono_nullable_init((uint8_t*)nullable, paramObj, paramSigClass);
                    newParamsVec[i] = nullable;
                }
                else
                {
                    newParamsVec[i] = params[i];
                }
            }

            newParams = &newParamsVec[0];
        }
    }

    if (mono_class_is_nullable(mono_unity_method_get_class(method)))
        target = mono_value_box(g_MonoDomain, mono_unity_class_get_castclass(mono_unity_method_get_class(method)), target);
    else if (target != NULL && mono_class_is_valuetype(mono_unity_method_get_class(method)))
        target = il2cpp_mono_fake_box(target);

    try
    {
        MonoClass *klass = mono_method_get_class(method);

        if (strcmp(mono_unity_method_get_name(method), ".ctor") == 0 || (mono_unity_method_is_static(method) && mono_unity_class_has_cctor(klass)))
            mono_runtime_class_init_full(il2cpp_mono_class_vtable(g_MonoDomain, klass), error);

        void *retVal = ((InvokerMethod)mono_unity_method_get_invoke_pointer(method))((Il2CppMethodPointer)mono_unity_method_get_method_pointer(method), method, target, newParams);

        std::vector<int>::const_iterator end = byRefParams.end();
        for (std::vector<int>::const_iterator it = byRefParams.begin(); it != end; ++it)
        {
            MonoClass *paramSigClass = mono_unity_signature_get_class_for_param(mono_method_signature(method), *it);
            params[*it] = mono_value_box(g_MonoDomain, paramSigClass, newParamsVec[*it]);
        }

        return mono_unity_method_convert_return_type_if_needed(method, retVal);
    }
    catch (Il2CppExceptionWrapper& ex)
    {
        if (exc)
            *exc = (MonoObject*)ex.ex;
        else
            mono_set_pending_exception((MonoException*)ex.ex);

        return NULL;
    }
}

MonoObject* il2cpp_mono_finalize_runtime_invoke(MonoObject *this_obj, void **params, MonoObject **exc, void* compiled_method)
{
    MonoMethod* finalizer = mono_class_get_finalizer(mono_object_get_class(this_obj));
    if (finalizer == NULL)
    {
        assert(0 && "Why did we get a NULL finalizer? Something may not be correct.");
        return NULL;
    }

    MonoError unused;
    return il2cpp_mono_runtime_invoke(finalizer, this_obj, params, exc, &unused);
}

void il2cpp_mono_set_cast_details(MonoClass *from, MonoClass *to)
{
}

MonoObject* il2cpp_mono_capture_context_runtime_invoke(MonoObject *this_obj, void **params, MonoObject **exc, void* compiled_method)
{
    return NULL;
}

void il2cpp_mono_dummy_callback()
{
}

void* il2cpp_mono_delegate_trampoline(MonoDomain *domain, MonoClass *klass)
{
    int vtableSize = mono_class_get_vtable_size(klass);
    MonoMethod *method = NULL;

    for (int i = 0; i < vtableSize; ++i)
    {
        MonoMethod *vtableMethod = mono_class_get_vtable_entry(klass, i);
        const char *vtableMethodName = mono_unity_method_get_name(vtableMethod);
        const char *substr = strstr(vtableMethodName, "Invoke");
        if (substr == vtableMethodName) //if the pointers are equal, vtableMethodName starts with "Invoke"
        {
            method = vtableMethod;
            break;
        }
    }

    if (!method)
        return NULL;

    MonoError unused;
    il2cpp_mono_method_initialize_function_pointers(method, &unused);
    return mono_unity_method_get_method_pointer(method);
}

void* il2cpp_mono_create_jump_trampoline(MonoDomain *domain, MonoMethod *method, gboolean add_sync_wrapper, MonoError *error)
{
    il2cpp_mono_method_initialize_function_pointers(method, error);
    return mono_unity_method_get_method_pointer(method);
}

void* il2cpp_mono_create_ftnptr(MonoDomain *domain, void* addr)
{
    return addr;
}

char* il2cpp_mono_get_runtime_build_info()
{
    return mono_unity_get_runtime_build_info(__DATE__, __TIME__);
}

#if IL2CPP_MONO_DEBUGGER
void il2cpp_debugger_save_thread_context(Il2CppThreadUnwindState* context, int frameCountAdjust)
{
    il2cpp::utils::Debugger::SaveThreadContext(context, frameCountAdjust);
}

#endif

void il2cpp_install_callbacks()
{
    MonoRuntimeCallbacks callbacks;

    g_MonoDomain = mono_domain_get();

    memset(&callbacks, 0, sizeof(callbacks));
    callbacks.get_vtable_trampoline = il2cpp_get_vtable_trampoline;
    callbacks.get_imt_trampoline = il2cpp_get_imt_trampoline;
    callbacks.runtime_invoke = il2cpp_mono_runtime_invoke;
    callbacks.set_cast_details = il2cpp_mono_set_cast_details;
    callbacks.create_jump_trampoline = il2cpp_mono_create_jump_trampoline;
    callbacks.create_ftnptr = il2cpp_mono_create_ftnptr;
    callbacks.get_runtime_build_info = il2cpp_mono_get_runtime_build_info;
    callbacks.create_delegate_trampoline = il2cpp_mono_delegate_trampoline;
#if IL2CPP_MONO_DEBUGGER
    // These don't exist in the Mono code used by default. We might need to add them (or something similar)
    // later. I'll comment them out to get the build working for now though.
    //callbacks.il2cpp_debugger_save_thread_context = il2cpp_debugger_save_thread_context;
    //callbacks.get_global_breakpoint_state_pointer =il2cpp::utils::Debugger::GetGlobalBreakpointPointer;
#endif

    mono_install_callbacks(&callbacks);

    MonoRuntimeExceptionHandlingCallbacks cbs;
    memset(&cbs, 0, sizeof(cbs));
    cbs.mono_raise_exception = il2cpp_throw_exception;
    cbs.mono_walk_stack_with_ctx = il2cpp_walk_stack_with_ctx;
    cbs.mono_install_handler_block_guard = il2cpp_install_handler_block_guard;
    cbs.mono_exception_walk_trace = il2cpp_exception_walk_trace;
    cbs.mono_current_thread_has_handle_block_guard = il2cpp_current_thread_has_handle_block_guard;
    mono_install_eh_callbacks(&cbs);

    mono_unity_domain_install_finalize_runtime_invoke(g_MonoDomain, il2cpp_mono_finalize_runtime_invoke);
    mono_unity_domain_install_capture_context_runtime_invoke(g_MonoDomain, il2cpp_mono_capture_context_runtime_invoke);
    mono_unity_domain_install_capture_context_method(g_MonoDomain, (void*)il2cpp_mono_dummy_callback);
    //mono_install_delegate_trampoline(il2cpp_mono_delegate_trampoline);

    MonoThreadInfoRuntimeCallbacks ticallbacks;
    memset(&ticallbacks, 0, sizeof(ticallbacks));
    ticallbacks.thread_state_init_from_handle = mono_unity_thread_state_init_from_handle;

    mono_thread_info_runtime_init(&ticallbacks);
}

void il2cpp_mono_runtime_init()
{
    register_allocator(mono_unity_alloc);
    mono_runtime_init(g_MonoDomain, mono_thread_start_cb, mono_thread_attach_cb);
    mono_threads_install_cleanup(mono_thread_cleanup_cb);
    mono_thread_attach(g_MonoDomain);
    mono_class_set_allow_gc_aware_layout(false);

    mono_add_internal_call("System.Diagnostics.StackFrame::get_frame_info", (void*)mono::icalls::mscorlib::System::Diagnostics::StackFrame::get_frame_info);
    mono_add_internal_call("System.Diagnostics.StackTrace::get_trace", (void*)mono::icalls::mscorlib::System::Diagnostics::StackTrace::get_trace);
    mono_add_internal_call("System.Runtime.InteropServices.Marshal::GetFunctionPointerForDelegateInternal", (void*)mono::icalls::mscorlib::System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegateInternal);
    mono_add_internal_call("System.Runtime.InteropServices.Marshal::GetDelegateForFunctionPointerInternal", (void*)mono::icalls::mscorlib::System::Runtime::InteropServices::Marshal::GetDelegateForFunctionPointerInternal);

    RegisterAllManagedMethods();
    initialize_interop_data_map();

    il2cpp::os::Image::Initialize();

    mono_profiler_started();
}

static void MonoSetConfigStr(const std::string& executablePath)
{
    std::string appBase = il2cpp::utils::PathUtils::DirectoryName(executablePath);
    std::string configFileName = il2cpp::utils::PathUtils::Basename(executablePath);
    configFileName.append(".config");
    mono_domain_set_config(g_MonoDomain, appBase.c_str(), configFileName.c_str());
}

void il2cpp_mono_set_config_utf16(const Il2CppChar* executablePath)
{
    IL2CPP_ASSERT(executablePath);
    std::string exePathUtf8 = il2cpp::utils::StringUtils::Utf16ToUtf8(executablePath, -1);
    MonoSetConfigStr(exePathUtf8);
}

void il2cpp_mono_set_config(const char* executablePath)
{
    IL2CPP_ASSERT(executablePath);
    std::string executablePathStr(executablePath);
    MonoSetConfigStr(executablePathStr);
}

void il2cpp_mono_set_commandline_arguments_utf16(int argc, const Il2CppChar* const* argv)
{
    std::vector<std::string> args(argc);
    for (int i = 0; i < argc; ++i)
        args[i] = il2cpp::utils::StringUtils::Utf16ToUtf8(argv[i], -1);

    std::vector<const char*> cargs(argc);
    for (int i = 0; i < argc; ++i)
        cargs[i] = args[i].c_str();

    mono_runtime_set_main_args(argc, const_cast<char**>(&cargs[0]));

    for (int i = 0; i < argc; ++i)
    {
        /* TODO: uncomment after mono debugger changes merged
        if (strncmp(args[i].c_str(), "--debugger-agent=", 17) == 0)
        {
            mono_debugger_set_il2cpp_breakpoints(g_Il2CppSequencePointCount, (Il2CppSequencePoint**)g_Il2CppSequencePoints);
            mono_debugger_agent_parse_options((char*)(args[i].c_str() + 17));
            //opt->mdb_optimizations = TRUE;
            //enable_debugging = TRUE;
           il2cpp::utils::Debugger::RegisterCallbacks(breakpoint_callback);
        }
        */
    }
    il2cpp::utils::Environment::SetMainArgs(argv, argc);
}

void il2cpp_mono_set_commandline_arguments(int argc, const char* const* argv)
{
    mono_runtime_set_main_args(argc, const_cast<char**>(argv));
    il2cpp::utils::Environment::SetMainArgs(argv, argc);
}

void il2cpp_mono_initialize_metadata()
{
    mono::vm::MetadataCache::Initialize();
}

#endif
