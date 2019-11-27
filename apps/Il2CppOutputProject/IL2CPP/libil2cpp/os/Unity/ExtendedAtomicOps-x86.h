#if defined(_MSC_VER)
#   include "os/Win32/WindowsHeaders.h"
#   include <intrin.h>
#endif

#if defined(__SSE2__)
#   include <emmintrin.h>
#endif

static inline void atomic_thread_fence(memory_order_relaxed_t)
{
}

static inline void atomic_thread_fence(memory_order_release_t)
{
#if defined(_MSC_VER)
    _ReadWriteBarrier();
#else
    __asm__ __volatile__ ("" : : : "memory");
#endif
}

static inline void atomic_thread_fence(memory_order_acquire_t)
{
#if defined(_MSC_VER)
    _ReadWriteBarrier();
#else
    __asm__ __volatile__ ("" : : : "memory");
#endif
}

static inline void atomic_thread_fence(memory_order_acq_rel_t)
{
#if defined(_MSC_VER)
    _ReadWriteBarrier();
#else
    __asm__ __volatile__ ("" : : : "memory");
#endif
}

static inline void atomic_thread_fence(int /* memory_order_seq_cst_t */)
{
#if defined(__SSE2__)
    _mm_mfence();
#elif defined(_MSC_VER)
    volatile LONG tmp;
    _InterlockedOr(&tmp, 0);
#else
    __asm__ __volatile__ ("lock orl #0, 0(%%esp)" ::: "cc", "memory");
#endif
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_relaxed_t)
{
    return *p;
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, int)
{
    atomic_word v;
#if defined(_MSC_VER)
    v = *p;
    _ReadWriteBarrier();
#else
    __asm__ __volatile__ ("movl %1, %0" : "=r" (v) : "m" (*p) : "memory");
#endif
    return v;
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    *p = v;
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
#if defined(_MSC_VER)
    _ReadWriteBarrier();
    *p = v;
#else
    __asm__ __volatile__ ("movl %1, %0" : "=m" (*p) : "r" (v) : "memory");
#endif
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word val, int /* memory_order_seq_cst_t */)
{
#if defined(_MSC_VER)
    _InterlockedExchange((volatile LONG*)p, (LONG)val);
#else
    // lock prefix is implicit
    __asm__ __volatile__
    (
/*lock*/ "xchgl  %1, %0"

        : "+m" (*p), "+r" (val)
        :
        : "memory"
    );
#endif
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word val, int)
{
#if defined(_MSC_VER)
    return (atomic_word)_InterlockedExchange((volatile LONG*)p, (LONG)val);
#else
    // lock prefix is implicit
    __asm__ __volatile__
    (
/*lock*/ "xchgl  %1, %0"

        : "+m" (*p), "+r" (val)
        :
        : "memory"
    );
    return val;
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word* oldval, atomic_word newval, int, int)
{
#if defined(_MSC_VER)
    atomic_word tmp = (atomic_word)_InterlockedCompareExchange((volatile LONG*)p, (LONG)newval, (LONG)*oldval);
    return *oldval == tmp ? true : (*oldval = tmp, false);
#else
    char res;
    __asm__ __volatile__
    (
        "lock cmpxchgl %3, %0\n\t"
        "setz %b1"

        : "+m" (*p), "=q" (res), "+a" (*oldval)
        : "r" (newval)
        : "cc", "memory"
    );
    return res != 0;
#endif
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word* oldval, atomic_word newval, int, int)
{
    return atomic_compare_exchange_strong_explicit(p, oldval, newval, memory_order_seq_cst, memory_order_seq_cst);
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word *p, atomic_word val, int)
{
#if defined(_MSC_VER)
    return _InterlockedExchangeAdd((LONG volatile*)p, (LONG)val);
#else
    __asm__ __volatile__
    (
        "lock xaddl  %1, %0"
        : "+m" (*p), "+r" (val)
        :
        : "cc", "memory"
    );
    return val;
#endif
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word *p, atomic_word val, int mo)
{
    return atomic_fetch_add_explicit(p, -val, mo);
}

/*
 *  extensions
 */

static inline void atomic_retain(volatile int *p)
{
#if defined(_MSC_VER)
    _InterlockedIncrement((LONG volatile*)p);
#else
    __asm__ (
        "lock incl  %0\n\t"
        : "+m" (*p)
        :
        : "cc", "memory"
    );
#endif
}

static inline bool atomic_release(volatile int *p)
{
#if defined(_MSC_VER)
    return _InterlockedDecrement((LONG volatile*)p) == 0;
#else
    bool res;
    __asm__ (
        "lock decl  %0\n\t"
        "setz %b1"
        : "+m" (*p), "=q" (res)
        :
        : "cc", "memory"
    );
    return res;
#endif
}

// double word

static inline atomic_word2 atomic_load_explicit(const volatile atomic_word2* p, int)
{
    atomic_word2 r;
#if defined(__SSE2__)
    _mm_store_sd((double*)&r, _mm_load_sd((const double*)p));
#else
    // using the FPU is the only way to do a 64 bit atomic load if SSE is not available
    r.d = p->d;
#endif
    return r;
}

static inline void atomic_store_explicit(volatile atomic_word2* p, atomic_word2 v, int)
{
#if defined(__SSE2__)
    _mm_store_sd((double*)p, _mm_load_sd((const double*)&v));
#else
    // using the FPU is the only way to do a 64 bit atomic store if SSE is not available
    p->d = v.d;
#endif
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, int, int)
{
#if defined(_MSC_VER)
    LONGLONG tmp = _InterlockedCompareExchange64((volatile LONGLONG*)p, newval.v, oldval->v);
    return oldval->v == tmp ? true : (oldval->v = tmp, false);
#else
    char res;
    __asm__ __volatile__
    (
        "lock cmpxchg8b %0\n\t"
        "setz   %b1\n\t"

        : "+m" (*p), "=q" (res), "+a" (oldval->lo), "+d" (oldval->hi)
        : "b" (newval.lo), "c" (newval.hi)
        : "cc", "memory"
    );
    return res != 0;
#endif
}

static inline atomic_word2 atomic_exchange_explicit(volatile atomic_word2* p, atomic_word2 newval, int)
{
    atomic_word2 oldval;
    oldval.lo = 0;
    oldval.hi = newval.hi - 1;
    while (!atomic_compare_exchange_strong_explicit(p, &oldval, newval, memory_order_seq_cst, memory_order_seq_cst))
        ;
    return oldval;
}
