#pragma once

struct Il2CppGuid;
struct Il2CppIUnknown;
struct Il2CppObject;
struct Il2CppThread;

namespace il2cpp
{
namespace gc
{
    class LIBIL2CPP_CODEGEN_API GarbageCollector
    {
    public:
        static void Collect(int maxGeneration);
        static int32_t CollectALittle();
        static int32_t GetCollectionCount(int32_t generation);
        static int64_t GetUsedHeapSize();
#if IL2CPP_ENABLE_WRITE_BARRIERS
        static void SetWriteBarrier(void **ptr);
        static void SetWriteBarrier(void **ptr, size_t numBytes);
#else
        static inline void SetWriteBarrier(void **ptr) {}
        static inline void SetWriteBarrier(void **ptr, size_t numBytes) {}
#endif

    public:
        // internal
        typedef void (*FinalizerCallback)(void* object, void* client_data);

        // functions implemented in a GC agnostic manner
        static void UninitializeGC();
        static void AddMemoryPressure(int64_t value);
        static int32_t GetMaxGeneration();
        static int32_t GetGeneration(void* addr);
#if !IL2CPP_DOTS_WITHOUT_DEBUGGER
        static void InitializeFinalizer();
        static bool IsFinalizerThread(Il2CppThread* thread);
        static void UninitializeFinalizers();
        static void NotifyFinalizers();
        static void RunFinalizer(void *obj, void *data);
        static void RegisterFinalizerForNewObject(Il2CppObject* obj);
        static void RegisterFinalizer(Il2CppObject* obj);
        static void SuppressFinalizer(Il2CppObject* obj);
        static void WaitForPendingFinalizers();
        static Il2CppIUnknown* GetOrCreateCCW(Il2CppObject* obj, const Il2CppGuid& iid);
#endif

        // functions implemented in a GC specific manner
        static void Initialize();
        static void Enable();
        static void Disable();
        static bool IsDisabled();

        static bool IsIncremental();

        static int64_t GetMaxTimeSliceNs();
        static void SetMaxTimeSliceNs(int64_t maxTimeSlice);

        static FinalizerCallback RegisterFinalizerWithCallback(Il2CppObject* obj, FinalizerCallback callback);

        static int64_t GetAllocatedHeapSize();

        static void* MakeDescriptorForObject(size_t *bitmap, int numbits);
        static void* MakeDescriptorForString();
        static void* MakeDescriptorForArray();

#if IL2CPP_DOTS_WITHOUT_DEBUGGER
        static void* Allocate(size_t size);
#endif

        static void* AllocateFixed(size_t size, void *descr);
        static void FreeFixed(void* addr);

        static bool RegisterThread(void *baseptr);
        static bool UnregisterThread();

#if !IL2CPP_DOTS_WITHOUT_DEBUGGER
        static bool HasPendingFinalizers();
        static int32_t InvokeFinalizers();
#endif

        static void AddWeakLink(void **link_addr, Il2CppObject *obj, bool track);
        static void RemoveWeakLink(void **link_addr);
        static Il2CppObject *GetWeakLink(void **link_addr);

        /* Used by liveness code */
        static void StopWorld();
        static void StartWorld();

        typedef void (*HeapSectionCallback) (void* user_data, void* start, void* end);
        static void ForEachHeapSection(void* user_data, HeapSectionCallback callback);
        static size_t GetSectionCount();

        typedef void* (*GCCallWithAllocLockCallback)(void* user_data);
        static void* CallWithAllocLockHeld(GCCallWithAllocLockCallback callback, void* user_data);

        static void RegisterRoot(char *start, size_t size);
        static void UnregisterRoot(char* start);

#if NET_4_0
        static void SetSkipThread(bool skip);
#endif
    };
} /* namespace vm */
} /* namespace il2cpp */
