#pragma once

#include "ExtendedAtomicTypes.h"

UNITY_PLATFORM_BEGIN_NAMESPACE;

enum memory_order_relaxed_t { memory_order_relaxed = 0 };
enum memory_order_acquire_t { memory_order_acquire = 2, memory_order_consume = memory_order_acquire };
enum memory_order_release_t { memory_order_release = 3 };
enum memory_order_acq_rel_t { memory_order_acq_rel = 4 };
enum memory_order_seq_cst_t { memory_order_seq_cst = 5 };

/*
    Available atomic functions:

    // non-explicit versions, use sequential consistency semantic by default

    // atomic load
    atomic_word atomic_load (const volatile atomic_word* p);

    // atomic store
    void atomic_store (volatile atomic_word* p, atomic_word val);

    // atomic exchange, returns previous value
    atomic_word atomic_exchange (volatile atomic_word* p, atomic_word val);

    // atomic compare exchange (strong), returns if the operation succeeded and update *oldval with the previous value
    bool atomic_compare_exchange (volatile atomic_word* p, atomic_word* oldval, atomic_word newval);

    // atomic fetch then add, returns previous value
    atomic_word atomic_fetch_add (volatile atomic_word *p, atomic_word val);

    // atomic fetch then sub, returns previous value
    atomic_word atomic_fetch_sub (volatile atomic_word *p, atomic_word val);

    // explicit versions

    // memory fence with <mo> semantic
    void atomic_thread_fence (memory_order_t mo);

    // atomic load with <mo> semantic
    atomic_word atomic_load_explicit (const volatile atomic_word* p, memory_order_t mo);

    // atomic store with <mo> semantic
    void atomic_store_explicit (volatile atomic_word* p, atomic_word v, memory_order_t mo);

    // atomic exchange with <mo> semantic, returns previous value
    atomic_word atomic_exchange_explicit (volatile atomic_word* p, atomic_word v, memory_order_t mo);

    // on RISC platforms with LoadLinked-StoreConditional available:
    // atomic_compare_exchange_weak_explicit: can fail spuriously even if *p == *oldval
    // uses <success> memory barrier when it succeeds, <failure> otherwize
    // returns the state of the operation and updates *oldval with the previous value
    bool atomic_compare_exchange_weak_explicit (volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_t success, memory_order_t failure);

    // atomic_compare_exchange_strong_explicit: can loop and only returns false if *p != *oldval
    // uses <success> memory barrier when it succeeds, <failure> otherwise
    // returns the state of the operation and updates *oldval with the previous value
    bool atomic_compare_exchange_strong_explicit (volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_t success, memory_order_t failure);

    // atomic fetch then add with <mo> semantic, returns previous value
    int atomic_fetch_add_explicit (volatile int* p, int val, memory_order_t mo);
    atomic_word atomic_fetch_add_explicit (volatile atomic_word* p, atomic_word val, memory_order_t mo);

    // atomic fetch then sub with <mo> semantic, returns previous value
    int atomic_fetch_sub_explicit (volatile int* p, int val, memory_order_t mo);
    atomic_word atomic_fetch_sub_explicit (volatile atomic_word* p, atomic_word val, memory_order_t mo);

    // extensions to the C++0x11 standard:

    // atomic increment with relaxed semantic
    void atomic_retain (volatile int *p);

    // atomic decrement with acquire/release semantic, returns true if resulting value is zero, false otherwize
    bool atomic_release (volatile int *p);

    // on platforms with double word compare exchange (ABA safe atomic pointers):

    // atomic load
    atomic_word2 atomic_load_explicit (const volatile atomic_word2* p, memory_order_t mo);

    // atomic store
    void atomic_store_explicit (volatile atomic_word2* p, atomic_word2 v, memory_order_t mo);

    // atomic exchange
    atomic_word atomic_exchange_explicit (volatile atomic_word2* p, atomic_word2 newval, memory_order_t mo);

    // atomic compare exchange
    bool atomic_compare_exchange_strong_explicit (volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_t success, memory_order_t failure);
*/

#if IL2CPP_TARGET_HAS_EXTENDED_ATOMICS

#   include "os/ExtendedAtomicOps.h"

#elif UNITY_ATOMIC_USE_GCC_ATOMICS || UNITY_ATOMIC_USE_CLANG_ATOMICS

#   include "ExtendedAtomicOps-clang-gcc.h"

#elif defined(__x86_64__) || defined(_M_X64)

#   include "ExtendedAtomicOps-x86-64.h"
#   define UNITY_ATOMIC_INT_OVERLOAD

#elif defined(__x86__) || defined(__i386__) || defined(_M_IX86)

#   include "ExtendedAtomicOps-x86.h"

#elif (defined(__arm64__) || defined(__aarch64__)) && (defined(__clang__) || defined(__GNUC__))

#   include "ExtendedAtomicOps-arm64.h"
#   define UNITY_ATOMIC_INT_OVERLOAD

#elif defined(_M_ARM64)

#   include "ExtendedAtomicOps-arm64-windows.h"
#   define UNITY_ATOMIC_INT_OVERLOAD

#elif defined(_M_ARM) || (defined(__arm__) && (defined(__ARM_ARCH_7__) || defined(__ARM_ARCH_7A__) || defined(__ARM_ARCH_7R__) || defined(__ARM_ARCH_7M__) || defined(__ARM_ARCH_7S__)) && (!UNITY_STV_API) && (defined(__clang__) || defined(__GNUC__)))

#   include "ExtendedAtomicOps-arm.h"

#elif PLATFORM_WIIU

#   include "ExtendedAtomicOps-ppc.h"

#elif PLATFORM_PSVITA

#   include "PlatformExtendedAtomicOps.h"

#elif (defined(__ppc64__) || defined(_ARCH_PPC64)) && (defined(__clang__) || defined(__GNUC__))

#   include "ExtendedAtomicOps-ppc64.h"
#   define UNITY_ATOMIC_INT_OVERLOAD

//#elif defined (__ppc__) && (defined (__clang__) || defined (__GNUC__))

//# include "Runtime/Threads/ExtendedAtomicOps-ppc.h"
#else

    #define UNITY_NO_ATOMIC_OPS

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_relaxed_t)
{
    return *p;
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    *p = v;
}

#endif

#ifndef UNITY_NO_ATOMIC_OPS

// non-explicit versions, use sequential consistency semantic

static inline void atomic_thread_fence()
{
    atomic_thread_fence(memory_order_seq_cst);
}

static inline atomic_word atomic_load(const volatile atomic_word* p)
{
    return atomic_load_explicit(p, memory_order_seq_cst);
}

static inline void atomic_store(volatile atomic_word* p, atomic_word val)
{
    atomic_store_explicit(p, val, memory_order_seq_cst);
}

static inline atomic_word atomic_exchange(volatile atomic_word* p, atomic_word val)
{
    return atomic_exchange_explicit(p, val, memory_order_seq_cst);
}

static inline bool atomic_compare_exchange(volatile atomic_word* p, atomic_word* oldval, atomic_word newval)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_seq_cst, memory_order_seq_cst);
}

static inline atomic_word atomic_fetch_add(volatile atomic_word *p, atomic_word val)
{
    return atomic_fetch_add_explicit(p, val, memory_order_seq_cst);
}

static inline atomic_word atomic_fetch_sub(volatile atomic_word *p, atomic_word val)
{
    return atomic_fetch_sub_explicit(p, val, memory_order_seq_cst);
}

#if defined(UNITY_ATOMIC_INT_OVERLOAD)

static inline int atomic_load(const volatile int* p)
{
    return atomic_load_explicit(p, memory_order_seq_cst);
}

static inline void atomic_store(volatile int* p, int val)
{
    atomic_store_explicit(p, val, memory_order_seq_cst);
}

static inline int atomic_fetch_add(volatile int *p, int val)
{
    return static_cast<int>(atomic_fetch_add_explicit(p, val, memory_order_seq_cst));
}

static inline int atomic_fetch_sub(volatile int *p, int val)
{
    return static_cast<int>(atomic_fetch_sub_explicit(p, val, memory_order_seq_cst));
}

#endif

#endif

UNITY_PLATFORM_END_NAMESPACE;
