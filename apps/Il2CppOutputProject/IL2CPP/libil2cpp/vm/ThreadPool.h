#pragma once

#include "il2cpp-config.h"
struct Il2CppObject;
struct Il2CppDelegate;
struct Il2CppAsyncResult;

#if NET_4_0

/* Keep in sync with System.IOOperation in mcs/class/System/System/IOSelector.cs */
enum Il2CppIOOperation
{
    EVENT_IN = 1 << 0,
    EVENT_OUT = 1 << 1,
    EVENT_ERR = 1 << 2, /* not in managed */
};

#endif

namespace il2cpp
{
namespace vm
{
    class LIBIL2CPP_CODEGEN_API ThreadPool
    {
    public:

        struct Configuration
        {
            int minThreads;
            int maxThreads;
            int minAsyncIOThreads;
            int maxAsyncIOThreads;

            // These are read-only.
            int availableThreads;
            int availableAsyncIOThreads;
        };

#if NET_4_0
        typedef struct
        {
            bool(*init)(int wakeup_pipe_fd);
            void(*register_fd)(int fd, int events, bool is_new);
            void(*remove_fd)(int fd);
            int(*event_wait)(void(*callback)(int fd, int events, void* user_data), void* user_data);
        } ThreadPoolIOBackend;
#endif

        static void Initialize();
        static void Shutdown();

        /// On a thread, call the given delegate with 'params' as arguments. Upon completion,
        /// call 'asyncCallback'.
        static Il2CppAsyncResult* Queue(Il2CppDelegate* delegate, void** params, Il2CppDelegate* asyncCallback, Il2CppObject* state);

        /// Wait for the execution of the given asynchronous call to have completed and return
        /// the value returned by the delegate wrapped in the call (or null if the delegate has
        /// a void return type).
        /// NOTE: Any AsyncResult can only be waited on once! Repeated or concurrent calls to Wait() on the same AsyncResult
        ///       will throw InvalidOperationExceptions.
        static Il2CppObject* Wait(Il2CppAsyncResult* asyncResult, void** outArgs);

        static Configuration GetConfiguration();
        static void SetConfiguration(const Configuration& configuration);
    };
} /* namespace vm */
} /* namespace il2cpp */
