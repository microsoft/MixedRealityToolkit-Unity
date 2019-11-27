#include "il2cpp-config.h"
#include "StackTrace.h"
#include "il2cpp-object-internals.h"
#include "os/Event.h"
#include "os/StackTrace.h"
#include "os/Thread.h"
#include "os/ThreadLocalValue.h"
#include "vm-utils/NativeSymbol.h"

namespace il2cpp
{
namespace vm
{
#if IL2CPP_ENABLE_STACKTRACES

    class MethodStack
    {
    protected:
        os::ThreadLocalValue s_StackFrames;

        inline StackFrames* GetStackFramesRaw()
        {
            StackFrames* stackFrames = NULL;

            os::ErrorCode result = s_StackFrames.GetValue(reinterpret_cast<void**>(&stackFrames));
            Assert(result == os::kErrorCodeSuccess);

            return stackFrames;
        }

    public:
        inline void InitializeForCurrentThread()
        {
            if (GetStackFramesRaw() != NULL)
                return;

            StackFrames* stackFrames = new StackFrames();
            stackFrames->reserve(64);

            os::ErrorCode result = s_StackFrames.SetValue(stackFrames);
            Assert(result == os::kErrorCodeSuccess);
        }

        inline void CleanupForCurrentThread()
        {
            StackFrames* frames = GetStackFramesRaw();

            if (frames == NULL)
                return;

            delete frames;

            os::ErrorCode result = s_StackFrames.SetValue(NULL);
            Assert(result == os::kErrorCodeSuccess);
        }
    };

#if IL2CPP_ENABLE_STACKTRACE_SENTRIES

    class StacktraceSentryMethodStack : public MethodStack
    {
    public:
        inline const StackFrames* GetStackFrames()
        {
            return GetStackFramesRaw();
        }

        inline bool GetStackFrameAt(int32_t depth, Il2CppStackFrameInfo& frame)
        {
            const StackFrames& frames = *GetStackFramesRaw();

            if (static_cast<int>(frames.size()) + depth < 1)
                return false;

            frame = frames[frames.size() - 1 + depth];
            return true;
        }

        inline void PushFrame(Il2CppStackFrameInfo& frame)
        {
            GetStackFramesRaw()->push_back(frame);
        }

        inline void PopFrame()
        {
            StackFrames* stackFrames = GetStackFramesRaw();
            stackFrames->pop_back();
        }
    };

#endif // IL2CPP_ENABLE_STACKTRACE_SENTRIES

#if IL2CPP_ENABLE_NATIVE_STACKTRACES

    class NativeMethodStack : public MethodStack
    {
        static bool GetStackFramesCallback(Il2CppMethodPointer frame, void* context)
        {
            const MethodInfo* method = il2cpp::utils::NativeSymbol::GetMethodFromNativeSymbol(frame);
            StackFrames* stackFrames = static_cast<StackFrames*>(context);

            if (method != NULL)
            {
                Il2CppStackFrameInfo frameInfo = { 0 };
                frameInfo.method = method;
                stackFrames->push_back(frameInfo);
            }

            return true;
        }

        struct GetStackFrameAtContext
        {
            int32_t currentDepth;
            const MethodInfo* method;
        };

        static bool GetStackFrameAtCallback(Il2CppMethodPointer frame, void* context)
        {
            const MethodInfo* method = il2cpp::utils::NativeSymbol::GetMethodFromNativeSymbol(frame);
            GetStackFrameAtContext* ctx = static_cast<GetStackFrameAtContext*>(context);

            if (method != NULL)
            {
                if (ctx->currentDepth == 0)
                {
                    ctx->method = method;
                    return false;
                }

                ctx->currentDepth++;
            }

            return true;
        }

    public:
        inline const StackFrames* GetStackFrames()
        {
            StackFrames* stackFrames = GetStackFramesRaw();
            if (stackFrames == NULL)
                return stackFrames;
            stackFrames->clear();

            os::StackTrace::WalkStack(&NativeMethodStack::GetStackFramesCallback, stackFrames, os::StackTrace::kFirstCalledToLastCalled);

            return stackFrames;
        }

        inline bool GetStackFrameAt(int32_t depth, Il2CppStackFrameInfo& frame)
        {
            GetStackFrameAtContext context = { depth, NULL };

            os::StackTrace::WalkStack(&NativeMethodStack::GetStackFrameAtCallback, &context, os::StackTrace::kLastCalledToFirstCalled);

            if (context.method != NULL)
            {
                frame.method = context.method;
                return true;
            }

            return false;
        }

        inline void PushFrame(Il2CppStackFrameInfo& frame)
        {
        }

        inline void PopFrame()
        {
        }
    };

#endif // IL2CPP_ENABLE_NATIVE_STACKTRACES

#else

    static StackFrames s_EmptyStack;

    class NoOpMethodStack
    {
    public:
        inline void InitializeForCurrentThread()
        {
        }

        inline void CleanupForCurrentThread()
        {
        }

        inline const StackFrames* GetStackFrames()
        {
            return &s_EmptyStack;
        }

        inline bool GetStackFrameAt(int32_t depth, Il2CppStackFrameInfo& frame)
        {
            return false;
        }

        inline void PushFrame(Il2CppStackFrameInfo& frame)
        {
        }

        inline void PopFrame()
        {
        }
    };

#endif // IL2CPP_ENABLE_STACKTRACES

#if IL2CPP_ENABLE_STACKTRACES

#if IL2CPP_ENABLE_STACKTRACE_SENTRIES

    StacktraceSentryMethodStack s_MethodStack;

#elif IL2CPP_ENABLE_NATIVE_STACKTRACES

    NativeMethodStack s_MethodStack;

#endif

#else

    NoOpMethodStack s_MethodStack;

#endif // IL2CPP_ENABLE_STACKTRACES

// Current thread functions

    void StackTrace::InitializeStackTracesForCurrentThread()
    {
        s_MethodStack.InitializeForCurrentThread();
    }

    void StackTrace::CleanupStackTracesForCurrentThread()
    {
        s_MethodStack.CleanupForCurrentThread();
    }

    const StackFrames* StackTrace::GetStackFrames()
    {
        return s_MethodStack.GetStackFrames();
    }

    bool StackTrace::GetStackFrameAt(int32_t depth, Il2CppStackFrameInfo& frame)
    {
        Assert(depth <= 0 && "Frame depth must be 0 or less");
        return s_MethodStack.GetStackFrameAt(depth, frame);
    }

    void StackTrace::WalkFrameStack(Il2CppFrameWalkFunc callback, void* context)
    {
        const StackFrames& frames = *GetStackFrames();

        for (StackFrames::const_iterator it = frames.begin(); it != frames.end(); it++)
            callback(&*it, context);
    }

#if IL2CPP_DOTS_DEBUGGER
    std::string StackTrace::GetStackTrace()
    {
        const StackFrames* frames = s_MethodStack.GetStackFrames();

        const size_t numberOfFramesToSkip = 1;
        int startFrame = (int)frames->size() - 1 - numberOfFramesToSkip;

        std::string stackTrace;
        for (int i = startFrame; i > 0; i--)
        {
            if (i == startFrame)
                stackTrace += "at ";
            else
                stackTrace += "  at ";
            Il2CppStackFrameInfo test = (*frames)[i];
            stackTrace += std::string((*frames)[i].method->name);
            if (i != 1)
                stackTrace += "\n";
        }

        return stackTrace;
    }

#endif

    void StackTrace::PushFrame(Il2CppStackFrameInfo& frame)
    {
        s_MethodStack.PushFrame(frame);
    }

    void StackTrace::PopFrame()
    {
        s_MethodStack.PopFrame();
    }

// Remote thread functions

    struct GetThreadFrameAtContext
    {
        il2cpp::os::Event apcDoneEvent;
        int32_t depth;
        Il2CppStackFrameInfo* frame;
        bool hasResult;
    };

    struct WalkThreadFrameStackContext
    {
        il2cpp::os::Event apcDoneEvent;
        Il2CppFrameWalkFunc callback;
        void* userContext;
    };

    struct GetThreadStackDepthContext
    {
        il2cpp::os::Event apcDoneEvent;
        int32_t stackDepth;
    };

    struct GetThreadTopFrameContext
    {
        il2cpp::os::Event apcDoneEvent;
        Il2CppStackFrameInfo* frame;
        bool hasResult;
    };

    static void STDCALL GetThreadFrameAtCallback(void* context)
    {
        GetThreadFrameAtContext* ctx = static_cast<GetThreadFrameAtContext*>(context);

        ctx->hasResult = StackTrace::GetStackFrameAt(ctx->depth, *ctx->frame);
        ctx->apcDoneEvent.Set();
    }

    bool StackTrace::GetThreadStackFrameAt(Il2CppThread* thread, int32_t depth, Il2CppStackFrameInfo& frame)
    {
#if IL2CPP_ENABLE_STACKTRACES
        GetThreadFrameAtContext apcContext;

        apcContext.depth = depth;
        apcContext.frame = &frame;

        thread->GetInternalThread()->handle->QueueUserAPC(GetThreadFrameAtCallback, &apcContext);
        apcContext.apcDoneEvent.Wait();

        return apcContext.hasResult;
#else
        return false;
#endif
    }

    static void STDCALL WalkThreadFrameStackCallback(void* context)
    {
        WalkThreadFrameStackContext* ctx = static_cast<WalkThreadFrameStackContext*>(context);

        StackTrace::WalkFrameStack(ctx->callback, ctx->userContext);
        ctx->apcDoneEvent.Set();
    }

    void StackTrace::WalkThreadFrameStack(Il2CppThread* thread, Il2CppFrameWalkFunc callback, void* context)
    {
#if IL2CPP_ENABLE_STACKTRACES
        WalkThreadFrameStackContext apcContext;

        apcContext.callback = callback;
        apcContext.userContext = context;

        thread->GetInternalThread()->handle->QueueUserAPC(WalkThreadFrameStackCallback, &apcContext);
        apcContext.apcDoneEvent.Wait();
#endif
    }

    static void STDCALL GetThreadStackDepthCallback(void* context)
    {
        GetThreadStackDepthContext* ctx = static_cast<GetThreadStackDepthContext*>(context);

        ctx->stackDepth = static_cast<int32_t>(StackTrace::GetStackDepth());
        ctx->apcDoneEvent.Set();
    }

    int32_t StackTrace::GetThreadStackDepth(Il2CppThread* thread)
    {
#if IL2CPP_ENABLE_STACKTRACES
        GetThreadStackDepthContext apcContext;

        thread->GetInternalThread()->handle->QueueUserAPC(GetThreadStackDepthCallback, &apcContext);
        apcContext.apcDoneEvent.Wait();

        return apcContext.stackDepth;
#else
        return 0;
#endif
    }

    static void STDCALL GetThreadTopFrameCallback(void* context)
    {
        GetThreadTopFrameContext* ctx = static_cast<GetThreadTopFrameContext*>(context);

        ctx->hasResult = StackTrace::GetTopStackFrame(*ctx->frame);
        ctx->apcDoneEvent.Set();
    }

    bool StackTrace::GetThreadTopStackFrame(Il2CppThread* thread, Il2CppStackFrameInfo& frame)
    {
#if IL2CPP_ENABLE_STACKTRACES
        GetThreadTopFrameContext apcContext;
        apcContext.frame = &frame;

        thread->GetInternalThread()->handle->QueueUserAPC(GetThreadTopFrameCallback, &apcContext);
        apcContext.apcDoneEvent.Wait();

        return apcContext.hasResult;
#else
        return false;
#endif
    }
}
}
