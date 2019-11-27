#if __cplusplus > 199711L
#define CompileTimeAssert(condition, message) static_assert(condition, message)
#else
#define CompileTimeAssert(condition, message)
#endif

static inline void atomic_pause()
{
#if defined(__i386__) || defined(__x86_64__)
    __asm__ __volatile__ ("pause");
#elif defined(__arm__) || defined(__arm64__)
    #if defined(__ARM_ARCH_5__) || defined(__ARM_ARCH_5T__) || defined(__ARM_ARCH_5E__) || defined(__ARM_ARCH_5TE__)
    // YIELD instruction is available only for ARMv6K and above.
    // http://infocenter.arm.com/help/index.jsp?topic=/com.arm.doc.dui0473j/dom1361289926796.html
    // Thus we do nothing here.
    #else
    __asm__ __volatile__("yield" : : : "memory");
    #endif
#else
    #error untested architecture
#endif
}

#ifndef UNITY_ATOMIC_FORCE_LOCKFREE_IMPLEMENTATION
#   define UNITY_ATOMIC_FORCE_LOCKFREE_IMPLEMENTATION 1
#endif

namespace detail
{
#if UNITY_ATOMIC_USE_CLANG_ATOMICS && UNITY_ATOMIC_USE_GCC_ATOMICS
#   error Cannot use both Clang and GCC atomic built-ins
#elif UNITY_ATOMIC_USE_CLANG_ATOMICS
#   if !__has_feature(c_atomic) && !__has_extension(c_atomic)
#       error "missing atomic built-in functions"
#   endif
#   define INTERNAL_UNITY_ATOMIC_THREAD_FENCE(memorder)                                         __c11_atomic_thread_fence(memorder)
#   define INTERNAL_UNITY_ATOMIC_LOAD(ptr, memorder)                                            __c11_atomic_load(ptr, memorder)
#   define INTERNAL_UNITY_ATOMIC_STORE(ptr, value, memorder)                                    __c11_atomic_store(ptr, value, memorder)
#   define INTERNAL_UNITY_ATOMIC_EXCHANGE(ptr, value, memorder)                                 __c11_atomic_exchange(ptr, value, memorder)
#   define INTERNAL_UNITY_ATOMIC_COMPARE_EXCHANGE_STRONG(ptr, oldval, newval, success, fail)    __c11_atomic_compare_exchange_strong(ptr, oldval, newval, success, fail)
#   define INTERNAL_UNITY_ATOMIC_COMPARE_EXCHANGE_WEAK(ptr, oldval, newval, success, fail)      __c11_atomic_compare_exchange_weak(ptr, oldval, newval, success, fail)
#   define INTERNAL_UNITY_ATOMIC_FETCH_ADD(ptr, value, memorder)                                __c11_atomic_fetch_add(ptr, value, memorder)
#   define INTERNAL_UNITY_ATOMIC_FETCH_SUB(ptr, value, memorder)                                __c11_atomic_fetch_sub(ptr, value, memorder)
#   define INTERNAL_UNITY_ATOMIC_TYPE(type)                                                     _Atomic(type)
#   define INTERNAL_UNITY_ATOMIC_IS_LOCK_FREE(type)                                             __c11_atomic_is_lock_free(sizeof(type))
#elif UNITY_ATOMIC_USE_GCC_ATOMICS
#   if (__GNUC__ < 4) || (__GNUC__ == 4 && __GNUC_MINOR__ < 7)
#       error "__atomic built-in functions not supported on GCC versions older than 4.7"
#   endif
#   if UNITY_ATOMIC_FORCE_LOCKFREE_IMPLEMENTATION
#       if __GCC_ATOMIC_INT_LOCK_FREE + 0 != 2 || __GCC_ATOMIC_LLONG_LOCK_FREE + 0 != 2
#           error "atomic ops are not lock-free for some required data types"
#       endif
#   endif
#   define INTERNAL_UNITY_ATOMIC_THREAD_FENCE(memorder)                                         __atomic_thread_fence(memorder)
#   define INTERNAL_UNITY_ATOMIC_LOAD(ptr, memorder)                                            __atomic_load_n(ptr, memorder)
#   define INTERNAL_UNITY_ATOMIC_STORE(ptr, value, memorder)                                    __atomic_store_n(ptr, value, memorder)
#   define INTERNAL_UNITY_ATOMIC_EXCHANGE(ptr, value, memorder)                                 __atomic_exchange_n(ptr, value, memorder)
#   define INTERNAL_UNITY_ATOMIC_COMPARE_EXCHANGE_STRONG(ptr, oldval, newval, success, fail)    __atomic_compare_exchange_n(ptr, oldval, newval, false, success, fail)
#   define INTERNAL_UNITY_ATOMIC_COMPARE_EXCHANGE_WEAK(ptr, oldval, newval, success, fail)      __atomic_compare_exchange_n(ptr, oldval, newval, true, success, fail)
#   define INTERNAL_UNITY_ATOMIC_FETCH_ADD(ptr, value, memorder)                                __atomic_fetch_add(ptr, value, memorder)
#   define INTERNAL_UNITY_ATOMIC_FETCH_SUB(ptr, value, memorder)                                __atomic_fetch_sub(ptr, value, memorder)
#   define INTERNAL_UNITY_ATOMIC_TYPE(type)                                                     type
#   if __GNUC__ >= 5
    // GCC pre-5 did not allow __atomic_always_lock_free in static expressions such as CompileTimeAssert
    // https://gcc.gnu.org/bugzilla/show_bug.cgi?id=62024
#       define INTERNAL_UNITY_ATOMIC_IS_LOCK_FREE(type)                                             __atomic_always_lock_free(sizeof(type), 0)
#   else
#       define INTERNAL_UNITY_ATOMIC_IS_LOCK_FREE(type)                                         true
#   endif
#else
#   error One of UNITY_ATOMIC_USE_CLANG_ATOMICS or UNITY_ATOMIC_USE_GCC_ATOMICS must be defined to 1
#endif

    inline int MemOrder(memory_order_relaxed_t) { return __ATOMIC_RELAXED; }
    inline int MemOrder(memory_order_release_t) { return __ATOMIC_RELEASE; }
    inline int MemOrder(memory_order_acquire_t) { return __ATOMIC_ACQUIRE; }
    inline int MemOrder(memory_order_acq_rel_t) { return __ATOMIC_ACQ_REL; }
    inline int MemOrder(memory_order_seq_cst_t) { return __ATOMIC_SEQ_CST; }
    int MemOrder(...); // generate link error on unsupported mem order types

#define INTERNAL_UNITY_ATOMIC_TYPEDEF(nonatomic, atomic) \
    typedef INTERNAL_UNITY_ATOMIC_TYPE(nonatomic) atomic; \
    CompileTimeAssert(!UNITY_ATOMIC_FORCE_LOCKFREE_IMPLEMENTATION || INTERNAL_UNITY_ATOMIC_IS_LOCK_FREE(atomic), #atomic " is not lock-free on this platform")

    INTERNAL_UNITY_ATOMIC_TYPEDEF(non_atomic_word, native_atomic_word);
    INTERNAL_UNITY_ATOMIC_TYPEDEF(non_atomic_word2, native_atomic_word2);
    INTERNAL_UNITY_ATOMIC_TYPEDEF(int, native_atomic_int);

#if UNITY_ATOMIC_FORCE_LOCKFREE_IMPLEMENTATION
    CompileTimeAssert(__GCC_HAVE_SYNC_COMPARE_AND_SWAP_4 + 0, "requires 32bit CAS");
    CompileTimeAssert(__GCC_HAVE_SYNC_COMPARE_AND_SWAP_8 + 0, "requires 64bit CAS");
#   if __SIZEOF_POINTER__ == 8
    CompileTimeAssert(__GCC_HAVE_SYNC_COMPARE_AND_SWAP_16 + 0, "requires 128bit CAS");
#   endif
#endif

#undef INTERNAL_UNITY_ATOMIC_TYPEDEF

    inline native_atomic_word* AtomicPtr(atomic_word* p) { return reinterpret_cast<native_atomic_word*>(p); }
    inline volatile native_atomic_word* AtomicPtr(volatile atomic_word* p) { return reinterpret_cast<volatile native_atomic_word*>(p); }

    inline native_atomic_word2* AtomicPtr(atomic_word2* p) { return reinterpret_cast<native_atomic_word2*>(&p->v); }
    inline volatile native_atomic_word2* AtomicPtr(volatile atomic_word2* p) { return reinterpret_cast<volatile native_atomic_word2*>(&p->v); }

    inline non_atomic_word* NonAtomicPtr(atomic_word* v) { return v; }
    // same as above: inline non_atomic_word* NonAtomicPtr(non_atomic_word* v) { return v; }
    inline non_atomic_word2* NonAtomicPtr(atomic_word2* v) { return &v->v; }
    inline non_atomic_word2* NonAtomicPtr(non_atomic_word2* v) { return v; }

    inline non_atomic_word NonAtomicValue(atomic_word v) { return v; }
    // same as above: inline non_atomic_word NonAtomicValue(non_atomic_word v) { return v; }
    inline non_atomic_word2 NonAtomicValue(atomic_word2 v) { return v.v; }
    inline non_atomic_word2 NonAtomicValue(non_atomic_word2 v) { return v; }

    inline atomic_word UnityAtomicValue(non_atomic_word v) { return v; }
    inline atomic_word2 UnityAtomicValue(non_atomic_word2 v) { atomic_word2 r; r.v = v; return r; }

#ifdef UNITY_ATOMIC_INT_OVERLOAD
    inline native_atomic_int* AtomicPtr(int* p) { return reinterpret_cast<native_atomic_int*>(p); }
    inline volatile native_atomic_int* AtomicPtr(volatile int* p) { return reinterpret_cast<volatile native_atomic_int*>(p); }
    inline int* NonAtomicPtr(int* v) { return v; }
    inline int NonAtomicValue(int v) { return v; }
    inline int UnityAtomicValue(int v) { return v; }
#endif

    template<typename T> struct Identity { typedef T type; };
} // namespace detail

template<typename MemOrder>
static inline void atomic_thread_fence(MemOrder memOrder)
{
    INTERNAL_UNITY_ATOMIC_THREAD_FENCE(detail::MemOrder(memOrder));
}

template<typename T, typename MemOrder>
static inline T atomic_load_explicit(const volatile T* p, MemOrder memOrder)
{
    return detail::UnityAtomicValue(INTERNAL_UNITY_ATOMIC_LOAD(detail::AtomicPtr(const_cast<T*>(p)), detail::MemOrder(memOrder)));
}

template<typename T, typename MemOrder>
static inline void atomic_store_explicit(volatile T* p, typename detail::Identity<T>::type v, MemOrder memOrder)
{
    INTERNAL_UNITY_ATOMIC_STORE(detail::AtomicPtr(p), detail::NonAtomicValue(v), detail::MemOrder(memOrder));
}

template<typename T, typename MemOrder>
static inline T atomic_exchange_explicit(volatile T* p, typename detail::Identity<T>::type v, MemOrder memOrder)
{
    return detail::UnityAtomicValue(INTERNAL_UNITY_ATOMIC_EXCHANGE(detail::AtomicPtr(p), detail::NonAtomicValue(v), detail::MemOrder(memOrder)));
}

template<typename T, typename MemOrderSuccess, typename MemOrderFail>
static inline bool atomic_compare_exchange_weak_explicit(volatile T* p, T* oldval, typename detail::Identity<T>::type newval,
    MemOrderSuccess memOrderSuccess, MemOrderFail memOrderFail)
{
    return INTERNAL_UNITY_ATOMIC_COMPARE_EXCHANGE_WEAK(detail::AtomicPtr(p), detail::NonAtomicPtr(oldval), detail::NonAtomicValue(newval),
        detail::MemOrder(memOrderSuccess), detail::MemOrder(memOrderFail));
}

template<typename T, typename MemOrderSuccess, typename MemOrderFail>
static inline bool atomic_compare_exchange_strong_explicit(volatile T* p, T* oldval, typename detail::Identity<T>::type newval,
    MemOrderSuccess memOrderSuccess, MemOrderFail memOrderFail)
{
    return INTERNAL_UNITY_ATOMIC_COMPARE_EXCHANGE_STRONG(detail::AtomicPtr(p), detail::NonAtomicPtr(oldval), detail::NonAtomicValue(newval),
        detail::MemOrder(memOrderSuccess), detail::MemOrder(memOrderFail));
}

template<typename T, typename MemOrder>
static inline T atomic_fetch_add_explicit(volatile T* p, typename detail::Identity<T>::type v, MemOrder memOrder)
{
    return detail::UnityAtomicValue(INTERNAL_UNITY_ATOMIC_FETCH_ADD(detail::AtomicPtr(p), detail::NonAtomicValue(v), detail::MemOrder(memOrder)));
}

template<typename T, typename MemOrder>
static inline T atomic_fetch_sub_explicit(volatile T* p, typename detail::Identity<T>::type v, MemOrder memOrder)
{
    return detail::UnityAtomicValue(INTERNAL_UNITY_ATOMIC_FETCH_SUB(detail::AtomicPtr(p), detail::NonAtomicValue(v), detail::MemOrder(memOrder)));
}

/*
 *  extensions
 */
static inline void atomic_retain(volatile int* p)
{
    atomic_fetch_add_explicit(p, 1, memory_order_relaxed);
}

static inline bool atomic_release(volatile int* p)
{
    // Both paths here should be correct on any platform
    // On architectures where read-modify-write with memory_order_acq_rel is more expensive than memory_order_release
    // the idea is to use a global memory_order_acquire fence instead, but only when the reference count drops to 0.
    // Only then the acquire/release synchronization is needed to make sure everything prior to atomic_release happens before running a d'tor.
#if defined(__arm__) || defined(__arm64__)
    bool res = atomic_fetch_sub_explicit(p, 1, memory_order_release) == 1;
    if (res)
    {
        atomic_thread_fence(memory_order_acquire);
    }
    return res;
#else
    return atomic_fetch_sub_explicit(p, 1, memory_order_acq_rel) == 1;
#endif
}

#undef INTERNAL_UNITY_ATOMIC_THREAD_FENCE
#undef INTERNAL_UNITY_ATOMIC_LOAD
#undef INTERNAL_UNITY_ATOMIC_STORE
#undef INTERNAL_UNITY_ATOMIC_EXCHANGE
#undef INTERNAL_UNITY_ATOMIC_COMPARE_EXCHANGE_STRONG
#undef INTERNAL_UNITY_ATOMIC_COMPARE_EXCHANGE_WEAK
#undef INTERNAL_UNITY_ATOMIC_FETCH_ADD
#undef INTERNAL_UNITY_ATOMIC_FETCH_SUB
#undef INTERNAL_UNITY_ATOMIC_TYPE
#undef INTERNAL_UNITY_ATOMIC_IS_LOCK_FREE
