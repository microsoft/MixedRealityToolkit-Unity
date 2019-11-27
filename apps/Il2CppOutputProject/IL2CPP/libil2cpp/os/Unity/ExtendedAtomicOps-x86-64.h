#if defined(_MSC_VER)
#   include "os/Win32/WindowsHeaders.h"
#   include <intrin.h>
#endif

#if defined(__SSE2__)
#   include <emmintrin.h>
#endif

#include "il2cpp-sanitizers.h"

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
    volatile LONGLONG tmp;
    _InterlockedOr64(&tmp, 0);
#else
    __asm__ __volatile__ ("lock orl #0, 0(%%esp)" ::: "cc", "memory");
#endif
}

/*
 * int support
 */

static inline atomic_word atomic_load_explicit(const volatile int* p, memory_order_relaxed_t) IL2CPP_DISABLE_TSAN
{
    return *p;
}

static inline int atomic_load_explicit(const volatile int* p, int) IL2CPP_DISABLE_TSAN
{
    int v;
#if defined(_MSC_VER)
    v = *p;
    _ReadWriteBarrier();
#else
    __asm__ __volatile__ ("movl %1, %0" : "=r" (v) : "m" (*p) : "memory");
#endif
    return v;
}

static inline void atomic_store_explicit(volatile int* p, int v, memory_order_relaxed_t) IL2CPP_DISABLE_TSAN
{
    *p = v;
}

static inline void atomic_store_explicit(volatile int* p, int v, memory_order_release_t) IL2CPP_DISABLE_TSAN
{
#if defined(_MSC_VER)
    _ReadWriteBarrier();
    *p = v;
#else
    __asm__ __volatile__ ("movl %1, %0" : "=m" (*p) : "r" (v) : "memory");
#endif
}

static inline void atomic_store_explicit(volatile int* p, int val, int /* memory_order_seq_cst_t */) IL2CPP_DISABLE_TSAN
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

/*
 * native word support
 */

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_relaxed_t) IL2CPP_DISABLE_TSAN
{
    return *p;
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, int) IL2CPP_DISABLE_TSAN
{
    atomic_word v;
#if defined(_MSC_VER)
    v = *p;
    _ReadWriteBarrier();
#else
    __asm__ __volatile__ ("movq %1, %0" : "=r" (v) : "m" (*p) : "memory");
#endif
    return v;
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t) IL2CPP_DISABLE_TSAN
{
    *p = v;
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t) IL2CPP_DISABLE_TSAN
{
#if defined(_MSC_VER)
    _ReadWriteBarrier();
    *p = v;
#else
    __asm__ __volatile__ ("movq %1, %0" : "=m" (*p) : "r" (v) : "memory");
#endif
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word val, int /* memory_order_seq_cst_t */) IL2CPP_DISABLE_TSAN
{
#if defined(_MSC_VER)
    _InterlockedExchange64((volatile LONGLONG*)p, (LONGLONG)val);
#else
    // lock prefix is implicit
    __asm__ __volatile__
    (
/*lock*/ "xchgq  %1, %0"
        : "+m" (*p), "+r" (val)
        :
        : "memory"
    );
#endif
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word val, int)
{
#if defined(_MSC_VER)
    return (atomic_word)_InterlockedExchange64((volatile LONGLONG*)p, (LONGLONG)val);
#else
    // lock prefix is implicit
    __asm__ __volatile__
    (
/*lock*/ "xchgq  %1, %0"
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
    atomic_word tmp = (atomic_word)_InterlockedCompareExchange64((volatile LONGLONG*)p, (LONGLONG)newval, (LONGLONG)*oldval);
    return *oldval == tmp ? true : (*oldval = tmp, false);
#else
    char res;
    __asm__ __volatile__
    (
        "lock cmpxchgq %3, %0\n\t"
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

static inline atomic_word atomic_fetch_add_explicit(volatile int *p, int val, int)
{
#if defined(_MSC_VER)
    return _InterlockedExchangeAdd((LONG volatile*)p, (LONG)val);
#else
    __asm__ __volatile__
    (
        "lock xaddl\t%1, %0"
        : "+m" (*p), "+r" (val)
        :
        : "cc", "memory"
    );
    return val;
#endif
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word *p, atomic_word val, int)
{
#if defined(_MSC_VER)
    return _InterlockedExchangeAdd64((LONGLONG volatile*)p, (LONGLONG)val);
#else
    __asm__ __volatile__
    (
        "lock xaddq  %1, %0"
        : "+m" (*p), "+r" (val)
        :
        : "cc", "memory"
    );
    return val;
#endif
}

static inline atomic_word atomic_fetch_sub_explicit(volatile int *p, int val, int mo)
{
    return atomic_fetch_add_explicit(p, -val, mo);
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

/*
 *  double word
 */

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, int, int)
{
#if defined(_MSC_VER)
    return _InterlockedCompareExchange128((volatile LONGLONG*)p, (LONGLONG)newval.hi, (LONGLONG)newval.lo, (LONGLONG*)oldval) != 0;
#else
    char res;
    __asm__ __volatile__
    (
        "lock cmpxchg16b %0\n\t"
        "setz   %b1\n\t"

        : "+m" (*p), "=q" (res), "+a" (oldval->lo), "+d" (oldval->hi)
        : "b" (newval.lo), "c" (newval.hi)
        : "cc", "memory"
    );
    return res != 0;
#endif
}

static inline atomic_word2 atomic_load_explicit(const volatile atomic_word2* p, int o) IL2CPP_DISABLE_TSAN
{
/*
    atomic_word2 r = { 0, 0 };
    atomic_word2 c = { 0, 0 };
    atomic_compare_exchange_strong_explicit((volatile atomic_word2*) p, &r, c, o, o);
    return r;
*/
    atomic_word2 r;
    r.v = ::_mm_load_si128((const __m128i*)p);
    return r;
}

static inline void atomic_store_explicit(volatile atomic_word2* p, atomic_word2 v, int o) IL2CPP_DISABLE_TSAN
{
/*
    atomic_word2 c = v;
    while(!atomic_compare_exchange_strong_explicit(p, &c, v, o, o)) {};
*/
    ::_mm_store_si128((__m128i*)&p->v, v.v);
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
