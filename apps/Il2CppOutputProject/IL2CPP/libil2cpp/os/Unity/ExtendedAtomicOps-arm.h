#if defined(_MSC_VER)

#   include "os/Win32/WindowsHeaders.h"
#   include <intrin.h>

#else

#   define ASM_DMB_ISH         "dmb    ish\n\t"

#   if defined(__ARM_ARCH_7S__)
// this is sufficient for Swift processors
#       define ASM_REL         "dmb    ishst\n\t"
#   else
#       define ASM_REL         "dmb    ish\n\t"
#   endif

#   define ASM_CLREX        "clrex\n\t"

#   define ASM_ISB         "isb\n\t"
#   define ASM_LABEL(i)    #i ":\n\t"

#endif

static inline void atomic_thread_fence(memory_order_relaxed_t)
{
}

static inline void atomic_thread_fence(memory_order_release_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
#else
    __asm__ __volatile__ (ASM_REL : : : "memory");
#endif
}

static inline void atomic_thread_fence(memory_order_acquire_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
#else
    __asm__ __volatile__ (ASM_DMB_ISH : : : "memory");
#endif
}

static inline void atomic_thread_fence(memory_order_acq_rel_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
#else
    __asm__ __volatile__ (ASM_DMB_ISH : : : "memory");
#endif
}

static inline void atomic_thread_fence(int /* memory_order_seq_cst_t */)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
#else
    __asm__ __volatile__ (ASM_DMB_ISH : : : "memory");
#endif
}

#define ATOMIC_LOAD(PRE, POST) \
    atomic_word res; \
    __asm__ __volatile__ \
    ( \
        PRE \
        "ldr    %0, %1\n\t" \
        POST \
        : "=r" (res) \
        : "m" (*p) \
        : "memory" \
    ); \
    return res;

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    return (atomic_word)__iso_volatile_load32((const volatile __int32*)p);
#else
    ATOMIC_LOAD("", "")
#endif
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_acquire_t)
{
#if defined(_MSC_VER)
    atomic_word res = (atomic_word)__iso_volatile_load32((const volatile __int32*)p);
    __dmb(_ARM_BARRIER_ISH);
    return res;
#else
    ATOMIC_LOAD("", ASM_DMB_ISH)
#endif
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, int /* memory_order_seq_cst_t */)
{
#if defined(_MSC_VER)
    atomic_word res = (atomic_word)__iso_volatile_load32((const volatile __int32*)p);
    __dmb(_ARM_BARRIER_ISH);
    return res;
#else
    ATOMIC_LOAD("", ASM_DMB_ISH)
#endif
}

#define ATOMIC_STORE(PRE, POST) \
    __asm__ __volatile__ \
    ( \
        PRE \
        "str    %1, %0\n\t" \
        POST \
        : "=m" (*p) \
        : "r" (v) \
        : "memory" \
    );

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    __iso_volatile_store32((volatile __int32*)p, (__int32)v);
#else
    ATOMIC_STORE("", "")
#endif
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
    __iso_volatile_store32((volatile __int32*)p, (__int32)v);
#else
    ATOMIC_STORE(ASM_REL, "")
#endif
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
    __iso_volatile_store32((volatile __int32*)p, (__int32)v);
    __dmb(_ARM_BARRIER_ISH);
#else
    ATOMIC_STORE(ASM_REL, ASM_DMB_ISH)
#endif
}

#define ATOMIC_XCHG(PRE, POST) \
    atomic_word res; \
    atomic_word success; \
    __asm__ __volatile__ \
    ( \
        PRE \
    ASM_LABEL (0) \
        "ldrex  %2, [%4]\n\t" \
        "strex  %0, %3, [%4]\n\t" \
        "teq    %0, #0\n\t" \
        "bne    0b\n\t" \
        POST \
        : "=&r" (success), "+m" (*p), "=&r" (res) \
        : "r" (v), "r" (p) \
        : "cc", "memory" \
    ); \
    return res;

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    return (atomic_word)_InterlockedExchange_nf((long volatile*)p, (long)v);
#else
    ATOMIC_XCHG("", "")
#endif
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
#if defined(_MSC_VER)
    // _InterlockedExchange_rel is documented by Microsoft, but it doesn't seem to be defined
    __dmb(_ARM_BARRIER_ISH);
    return (atomic_word)_InterlockedExchange_nf((long volatile*)p, (long)v);
#else
    ATOMIC_XCHG(ASM_REL, "")
#endif
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
#if defined(_MSC_VER)
    return (atomic_word)_InterlockedExchange_acq((long volatile*)p, (long)v);
#else
    ATOMIC_XCHG("", ASM_ISB)
#endif
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
    return (atomic_word)_InterlockedExchange_acq((long volatile*)p, (long)v);
#else
    ATOMIC_XCHG(ASM_REL, ASM_ISB)
#endif
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
#if defined(_MSC_VER)
    // _InterlockedExchange_rel is documented by Microsoft, but it doesn't seem to be defined
    __dmb(_ARM_BARRIER_ISH);
    atomic_word res = (atomic_word)_InterlockedExchange_nf((long volatile*)p, (long)v);
    __dmb(_ARM_BARRIER_ISH);
    return res;
#else
    ATOMIC_XCHG(ASM_REL, ASM_DMB_ISH)
#endif
}

#if !defined(_MSC_VER)

// atomic_compare_exchange_weak_explicit: can fail spuriously even if *p == *oldval

#define ATOMIC_CMP_XCHG(PRE, POST) \
    atomic_word res; \
    atomic_word success = 0; \
    __asm__ __volatile__ \
    ( \
        PRE \
        "ldrex  %2, [%4]\n\t" \
        "teq    %2, %5\n\t" \
        "it     eq\n\t" \
        "strexeq %0, %3, [%4]\n\t" \
        POST \
        : "+r" (success), "+m" (*p), "=&r" (res) \
        : "r" (newval), "r" (p), "r" (*oldval) \
        : "cc", "memory" \
    ); \
    *oldval = res; \
    return success;

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("", "")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(ASM_REL, "")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("", "teq %0, #0\n\tbne 1f\n\t" ASM_ISB ASM_LABEL(1))
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(ASM_REL, "teq %0, #0\n\tbne 1f\n\t" ASM_ISB ASM_LABEL(1))
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(ASM_REL, "teq %0, #0\n\tbne 1f\n\t" ASM_DMB_ISH ASM_LABEL(1))
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_release_t)
{
    ATOMIC_CMP_XCHG(ASM_REL, "")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_acquire_t)
{
    ATOMIC_CMP_XCHG("", ASM_ISB)
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
    ATOMIC_CMP_XCHG(ASM_REL, ASM_ISB)
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
    ATOMIC_CMP_XCHG(ASM_REL, ASM_DMB_ISH)
}

#endif

// atomic_compare_exchange_strong_explicit: does loop and only returns false if *p != *oldval

#undef ATOMIC_CMP_XCHG
#define ATOMIC_CMP_XCHG(PRE, POST) \
    atomic_word res; \
    atomic_word success = 0; \
    __asm__ __volatile__ \
    ( \
        PRE \
    ASM_LABEL (0) \
        "ldrex  %2, [%4]\n\t" \
        "teq    %2, %5\n\t" \
        "bne    1f\n\t" \
        "strex  %0, %3, [%4]\n\t" \
        "teq    %0, #0\n\t" \
        "bne    0b\n\t" \
        POST \
        : "=&r" (success), "+m" (*p), "=&r" (res) \
        : "r" (newval), "r" (p), "r" (*oldval) \
        : "cc", "memory" \
    ); \
    *oldval = res; \
    return success;

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    long tmp = _InterlockedCompareExchange_nf((long volatile*)p, (long)newval, (long)*oldval);
    return *oldval == tmp ? true : (*oldval = tmp, false);
#else
    ATOMIC_CMP_XCHG("", ASM_LABEL(1) ASM_CLREX)
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_release_t)
{
#if defined(_MSC_VER)
    long tmp = _InterlockedCompareExchange_rel((long volatile*)p, (long)newval, (long)*oldval);
    return *oldval == tmp ? true : (*oldval = tmp, false);
#else
    ATOMIC_CMP_XCHG(ASM_REL, ASM_LABEL(1) ASM_CLREX)
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_acquire_t)
{
#if defined(_MSC_VER)
    long tmp = _InterlockedCompareExchange_acq((long volatile*)p, (long)newval, (long)*oldval);
    return *oldval == tmp ? true : (*oldval = tmp, false);
#else
    ATOMIC_CMP_XCHG("", ASM_LABEL(1) ASM_CLREX ASM_ISB)
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
    long tmp = _InterlockedCompareExchange_acq((long volatile*)p, (long)newval, (long)*oldval);
    return *oldval == tmp ? true : (*oldval = tmp, false);
#else
    ATOMIC_CMP_XCHG(ASM_REL, ASM_LABEL(1) ASM_CLREX ASM_ISB)
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
#if defined(_MSC_VER)
    long tmp = _InterlockedCompareExchange_rel((long volatile*)p, (long)newval, (long)*oldval);
    __dmb(_ARM_BARRIER_ISH);
    return *oldval == tmp ? true : (*oldval = tmp, false);
#else
    ATOMIC_CMP_XCHG(ASM_REL, ASM_LABEL(1) ASM_CLREX ASM_DMB_ISH)
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_release, memory_order_release);
#else
    ATOMIC_CMP_XCHG(ASM_REL, ASM_LABEL(1) ASM_CLREX)
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_acquire, memory_order_acquire);
#else
    ATOMIC_CMP_XCHG("", ASM_ISB ASM_LABEL(1) ASM_CLREX)
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_acq_rel, memory_order_acq_rel);
#else
    ATOMIC_CMP_XCHG(ASM_REL, ASM_ISB ASM_LABEL(1) ASM_CLREX)
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_seq_cst, memory_order_seq_cst);
#else
    ATOMIC_CMP_XCHG(ASM_REL, ASM_DMB_ISH ASM_LABEL(1) ASM_CLREX)
#endif
}

#if defined(_MSC_VER)

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_relaxed, memory_order_relaxed);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_relaxed_t)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_release, memory_order_relaxed);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_acquire, memory_order_relaxed);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_acq_rel, memory_order_relaxed);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_seq_cst, memory_order_relaxed);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_release_t)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_release, memory_order_release);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_acquire_t)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_acquire, memory_order_acquire);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_acq_rel, memory_order_acq_rel);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_seq_cst, memory_order_seq_cst);
}

#endif

#define ATOMIC_OP(PRE, OP, POST) \
    atomic_word res, tmp; \
    atomic_word success; \
    __asm__ __volatile__ \
    ( \
        PRE \
    ASM_LABEL (0) \
        "ldrex  %2, [%5]\n\t" \
        OP "    %3, %2, %4\n\t" \
        "strex  %0, %3, [%5]\n\t" \
        "teq    %0, #0\n\t" \
        "bne    0b\n\t" \
        POST \
        : "=&r" (success), "+m" (*p), "=&r" (res), "=&r" (tmp) \
        : "Ir" (v), "r" (p) \
        : "cc", "memory" \
    ); \
    return res;

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    return (atomic_word)_InterlockedExchangeAdd_nf((volatile long*)p, (long)v);
#else
    ATOMIC_OP("", "add", "")
#endif
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
#if defined(_MSC_VER)
    return (atomic_word)_InterlockedExchangeAdd_rel((volatile long*)p, (long)v);
#else
    ATOMIC_OP(ASM_REL, "add", "")
#endif
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
#if defined(_MSC_VER)
    return (atomic_word)_InterlockedExchangeAdd_acq((volatile long*)p, (long)v);
#else
    ATOMIC_OP("", "add", ASM_ISB)
#endif
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
    return (atomic_word)_InterlockedExchangeAdd_acq((volatile long*)p, (long)v);
#else
    ATOMIC_OP(ASM_REL, "add", ASM_ISB)
#endif
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
#if defined(_MSC_VER)
    long oldval = _InterlockedExchangeAdd_rel((volatile long*)p, (long)v);
    __dmb(_ARM_BARRIER_ISH);
    return (atomic_word)oldval;
#else
    ATOMIC_OP(ASM_REL, "add", ASM_DMB_ISH)
#endif
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    return atomic_fetch_add_explicit(p, -v, memory_order_relaxed);
#else
    ATOMIC_OP("", "sub", "")
#endif
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
#if defined(_MSC_VER)
    return atomic_fetch_add_explicit(p, -v, memory_order_release);
#else
    ATOMIC_OP(ASM_REL, "sub", "")
#endif
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
#if defined(_MSC_VER)
    return atomic_fetch_add_explicit(p, -v, memory_order_acquire);
#else
    ATOMIC_OP("", "sub", ASM_ISB)
#endif
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
#if defined(_MSC_VER)
    return atomic_fetch_add_explicit(p, -v, memory_order_acq_rel);
#else
    ATOMIC_OP(ASM_REL, "sub", ASM_ISB)
#endif
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
#if defined(_MSC_VER)
    return atomic_fetch_add_explicit(p, -v, memory_order_seq_cst);
#else
    ATOMIC_OP(ASM_REL, "sub", ASM_DMB_ISH)
#endif
}

/*
 *  extensions
 */

static inline void atomic_retain(volatile int* p)
{
#if defined(_MSC_VER)
    _InterlockedIncrement_nf((volatile long*)p);
#else
    atomic_fetch_add_explicit(p, 1, memory_order_relaxed);
#endif
}

static inline bool atomic_release(volatile int* p)
{
#if defined(_MSC_VER)
    // _interlockedDecrement returns the resulting decremented value
    bool res = _InterlockedDecrement_rel((volatile long*)p) == 0;
    if (res)
    {
        __dmb(_ARM_BARRIER_ISH);
    }
#else
    bool res = atomic_fetch_sub_explicit(p, 1, memory_order_release) == 1;
    if (res)
    {
        atomic_thread_fence(memory_order_acquire);
    }
#endif
    return res;
}

/*
 *  double word
 */

// Note: the only way to get atomic 64-bit memory accesses on ARM is to use ldrexd/strexd with a loop
// (ldrd and strd instructions are not guaranteed to appear atomic)

static inline atomic_word2 atomic_load_explicit(const volatile atomic_word2* p, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
//  atomic_word2 r; r.v = __iso_volatile_load64 ((volatile __int64*) p); return r;
    atomic_word2 r;
    r.v = _InterlockedCompareExchange64_nf((volatile __int64*)p, (__int64)0, (__int64)0);
    return r;
#else
    register atomic_word lo __asm__ ("r2");
    register atomic_word hi __asm__ ("r3");
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_LABEL(0)
        "ldrexd\t%1, %2, [%3]\n\t"
        "strexd\t%0, %1, %2, [%3]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"

        : "=&r" (success), "=&r" (lo), "=&r" (hi)
        : "r" (p)
        : "cc", "r2", "r3"
    );
    atomic_word2 w;
    w.lo = lo;
    w.hi = hi;
    return w;
#endif
}

static inline atomic_word2 atomic_load_explicit(const volatile atomic_word2* p, memory_order_acquire_t)
{
#if defined(_MSC_VER)
    atomic_word2 r;
    r.v = _InterlockedCompareExchange64_acq((volatile __int64*)p, (__int64)0, (__int64)0);
    return r;
#else
    register atomic_word lo __asm__ ("r2");
    register atomic_word hi __asm__ ("r3");
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_LABEL(0)
        "ldrexd\t%1, %2, [%3]\n\t"
        "strexd\t%0, %1, %2, [%3]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"
        ASM_ISB

        : "=&r" (success), "=&r" (lo), "=&r" (hi)
        : "r" (p)
        : "cc", "memory", "r2", "r3"
    );
    atomic_word2 w;
    w.lo = lo;
    w.hi = hi;
    return w;
#endif
}

static inline void atomic_store_explicit(volatile atomic_word2* p, atomic_word2 v, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    atomic_word2 w, x;
    w = v;
    x = v;
    do
    {
        w.v = _InterlockedCompareExchange64_nf((volatile __int64*)p, x.v, w.v);
    }
    while (w.v != x.v);
#else
    register atomic_word l __asm__ ("r2");
    register atomic_word h __asm__ ("r3");
    register atomic_word lo __asm__ ("r0") = v.lo;
    register atomic_word hi __asm__ ("r1") = v.hi;
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_LABEL(0)
        "ldrexd\t%2, %3, [%6]\n\t"
        "strexd\t%0, %4, %5, [%6]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"

        : "=&r" (success), "=m" (*p), "=&r" (l), "=&r" (h)
        : "r" (lo), "r" (hi), "r" (p)
        : "cc", "memory", "r2", "r3"
    );
#endif
}

static inline void atomic_store_explicit(volatile atomic_word2* p, atomic_word2 v, memory_order_release_t)
{
#if defined(_MSC_VER)
    atomic_word2 w, x;
    w = v;
    x = v;
    do
    {
        w.v = _InterlockedCompareExchange64_rel((volatile __int64*)p, x.v, w.v);
    }
    while (w.v != x.v);
#else
    register atomic_word l __asm__ ("r2");
    register atomic_word h __asm__ ("r3");
    register atomic_word lo __asm__ ("r0") = v.lo;
    register atomic_word hi __asm__ ("r1") = v.hi;
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_REL
        ASM_LABEL(0)
        "ldrexd\t%2, %3, [%6]\n\t"
        "strexd\t%0, %4, %5, [%6]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"

        : "=&r" (success), "=m" (*p), "=&r" (l), "=&r" (h)
        : "r" (lo), "r" (hi), "r" (p)
        : "cc", "memory", "r2", "r3"
    );
#endif
}

static inline atomic_word2 atomic_exchange_explicit(volatile atomic_word2* p, atomic_word2 val, memory_order_acq_rel_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
    atomic_word2 w;
    w.v = _InterlockedExchange64_acq((__int64 volatile*)p, (__int64)val.v);
    return w;
#else
    register atomic_word l __asm__ ("r0");
    register atomic_word h __asm__ ("r1");
    register atomic_word lo __asm__ ("r2") = val.lo;
    register atomic_word hi __asm__ ("r3") = val.hi;
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_REL
        ASM_LABEL(0)
        "ldrexd\t%2, %3, [%6]\n\t"
        "strexd\t%0, %5, %4, [%6]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"
        ASM_ISB

        : "=&r" (success), "=m" (*p), "=&r" (l), "=&r" (h)
        : "r" (hi), "r" (lo), "r" (p)
        : "cc", "memory", "r0", "r1", "r3"
    );
    val.lo = l;
    val.hi = h;

    return val;
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_acquire_t, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    __int64 tmp = _InterlockedCompareExchange64_acq((volatile __int64*)p, newval.v, oldval->v);
    return oldval->v == tmp ? true : (oldval->v = tmp, false);
#else
    register atomic_word l __asm__ ("r2");
    register atomic_word h __asm__ ("r3");
    register atomic_word lo __asm__ ("r0") = newval.lo;
    register atomic_word hi __asm__ ("r1") = newval.hi;
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_LABEL(0)
        "ldrexd\t%2, %3, [%8]\n\t"
        "teq\t%3, %5\n\t"
        "it\t\teq\n\t"
        "teqeq\t%2, %4\n\t"
        "bne\t1f\n\t"
        "strexd\t%0, %6, %7, [%8]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"
        ASM_ISB
        ASM_LABEL(1)
        "clrex\n\t"

        : "=&r" (success), "+m" (*p), "=&r" (l), "=&r" (h)
        : "r" (oldval->lo), "r" (oldval->hi), "r" (lo), "r" (hi), "r" (p), "0" (1)
        : "cc", "memory", "r2", "r3"
    );
    if (success != 0)
    {
        oldval->lo = l;
        oldval->hi = h;
    }
    return success == 0;
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_release_t, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    __int64 tmp = _InterlockedCompareExchange64_rel((volatile __int64*)p, newval.v, oldval->v);
    return oldval->v == tmp ? true : (oldval->v = tmp, false);
#else
    register atomic_word l __asm__ ("r2");
    register atomic_word h __asm__ ("r3");
    register atomic_word lo __asm__ ("r0") = newval.lo;
    register atomic_word hi __asm__ ("r1") = newval.hi;
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_REL
        ASM_LABEL(0)
        "ldrexd\t%2, %3, [%8]\n\t"
        "teq\t%3, %5\n\t"
        "it\t\teq\n\t"
        "teqeq\t%2, %4\n\t"
        "bne\t1f\n\t"
        "strexd\t%0, %6, %7, [%8]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"
        ASM_LABEL(1)
        "clrex\n\t"

        : "=&r" (success), "+m" (*p), "=&r" (l), "=&r" (h)
        : "r" (oldval->lo), "r" (oldval->hi), "r" (lo), "r" (hi), "r" (p), "0" (1)
        : "cc", "memory", "r2", "r3"
    );
    if (success != 0)
    {
        oldval->lo = l;
        oldval->hi = h;
    }
    return success == 0;
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    __dmb(_ARM_BARRIER_ISH);
    __int64 tmp = _InterlockedCompareExchange64_acq((volatile __int64*)p, newval.v, oldval->v);
    return oldval->v == tmp ? true : (oldval->v = tmp, false);
#else
    register atomic_word l __asm__ ("r2");
    register atomic_word h __asm__ ("r3");
    register atomic_word lo __asm__ ("r0") = newval.lo;
    register atomic_word hi __asm__ ("r1") = newval.hi;
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_REL
        ASM_LABEL(0)
        "ldrexd\t%2, %3, [%8]\n\t"
        "teq\t%3, %5\n\t"
        "it\t\teq\n\t"
        "teqeq\t%2, %4\n\t"
        "bne\t1f\n\t"
        "strexd\t%0, %6, %7, [%8]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"
        ASM_ISB
        ASM_LABEL(1)
        "clrex\n\t"

        : "=&r" (success), "+m" (*p), "=&r" (l), "=&r" (h)
        : "r" (oldval->lo), "r" (oldval->hi), "r" (lo), "r" (hi), "r" (p), "0" (1)
        : "cc", "memory", "r2", "r3"
    );
    if (success != 0)
    {
        oldval->lo = l;
        oldval->hi = h;
    }
    return success == 0;
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_seq_cst_t, memory_order_relaxed_t)
{
#if defined(_MSC_VER)
    __int64 tmp = _InterlockedCompareExchange64_rel((volatile __int64*)p, newval.v, oldval->v);
    __dmb(_ARM_BARRIER_ISH);
    return oldval->v == tmp ? true : (oldval->v = tmp, false);
#else
    register atomic_word l __asm__ ("r2");
    register atomic_word h __asm__ ("r3");
    register atomic_word lo __asm__ ("r0") = newval.lo;
    register atomic_word hi __asm__ ("r1") = newval.hi;
    atomic_word success;

    __asm__ __volatile__
    (
        ASM_REL
        ASM_LABEL(0)
        "ldrexd\t%2, %3, [%8]\n\t"
        "teq\t%3, %5\n\t"
        "it\t\teq\n\t"
        "teqeq\t%2, %4\n\t"
        "bne\t1f\n\t"
        "strexd\t%0, %6, %7, [%8]\n\t"
        "teq\t%0, #0\n\t"
        "bne\t0b\n\t"
        ASM_DMB_ISH
        ASM_LABEL(1)
        "clrex\n\t"

        : "=&r" (success), "+m" (*p), "=&r" (l), "=&r" (h)
        : "r" (oldval->lo), "r" (oldval->hi), "r" (lo), "r" (hi), "r" (p), "0" (1)
        : "cc", "memory", "r2", "r3"
    );
    if (success != 0)
    {
        oldval->lo = l;
        oldval->hi = h;
    }
    return success == 0;
#endif
}
