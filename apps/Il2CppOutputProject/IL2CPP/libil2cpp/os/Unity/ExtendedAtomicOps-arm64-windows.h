#include <intrin.h>

static inline void atomic_thread_fence(memory_order_relaxed_t)
{
}

static inline void atomic_thread_fence(memory_order_acquire_t)
{
    __dmb(_ARM64_BARRIER_ISH);
}

static inline void atomic_thread_fence(memory_order_release_t)
{
    __dmb(_ARM64_BARRIER_ISH);
}

static inline void atomic_thread_fence(memory_order_acq_rel_t)
{
    __dmb(_ARM64_BARRIER_ISH);
}

static inline void atomic_thread_fence(int /* memory_order_seq_cst_t */)
{
    __dmb(_ARM64_BARRIER_ISH);
}

static inline int atomic_load_explicit(const volatile int* p, memory_order_relaxed_t)
{
    return __iso_volatile_load32(p);
}

static inline int atomic_load_explicit(const volatile int* p, memory_order_acquire_t)
{
    int res = __iso_volatile_load32(p);
    __dmb(_ARM64_BARRIER_ISH);
    return res;
}

static inline int atomic_load_explicit(const volatile int* p, int /* memory_order_seq_cst_t */)
{
    int res = __iso_volatile_load32(p);
    __dmb(_ARM64_BARRIER_ISH);
    return res;
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_relaxed_t)
{
    return __iso_volatile_load64(p);
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_acquire_t)
{
    atomic_word res = __iso_volatile_load64(p);
    __dmb(_ARM64_BARRIER_ISH);
    return res;
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, int /* memory_order_seq_cst_t */)
{
    atomic_word res = __iso_volatile_load64(p);
    __dmb(_ARM64_BARRIER_ISH);
    return res;
}

static inline void atomic_store_explicit(volatile int* p, int v, memory_order_relaxed_t)
{
    __iso_volatile_store32(p, v);
}

static inline void atomic_store_explicit(volatile int* p, int v, memory_order_release_t)
{
    __dmb(_ARM64_BARRIER_ISH);
    __iso_volatile_store32(p, v);
}

static inline void atomic_store_explicit(volatile int* p, int v, int /* memory_order_seq_cst_t */)
{
    __dmb(_ARM64_BARRIER_ISH);
    __iso_volatile_store32(p, v);
    __dmb(_ARM64_BARRIER_ISH);
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    __iso_volatile_store64(p, v);
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
    __dmb(_ARM64_BARRIER_ISH);
    __iso_volatile_store64(p, v);
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
    __dmb(_ARM64_BARRIER_ISH);
    __iso_volatile_store64(p, v);
    __dmb(_ARM64_BARRIER_ISH);
}

static inline int atomic_exchange_explicit(volatile int* p, int v, memory_order_relaxed_t)
{
    return _InterlockedExchange_nf(reinterpret_cast<volatile long*>(p), v);
}

static inline int atomic_exchange_explicit(volatile int* p, int v, memory_order_acquire_t)
{
    return _InterlockedExchange_acq(reinterpret_cast<volatile long*>(p), v);
}

static inline int atomic_exchange_explicit(volatile int* p, int v, memory_order_release_t)
{
    return _InterlockedExchange_rel(reinterpret_cast<volatile long*>(p), v);
}

static inline int atomic_exchange_explicit(volatile int* p, int v, memory_order_acq_rel_t)
{
    return _InterlockedExchange(reinterpret_cast<volatile long*>(p), v);
}

static inline int atomic_exchange_explicit(volatile int* p, int v, int /* memory_order_seq_cst_t */)
{
    return _InterlockedExchange(reinterpret_cast<volatile long*>(p), v);
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    return _InterlockedExchange64_nf(p, v);
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
    return _InterlockedExchange64_acq(p, v);
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
    return _InterlockedExchange64_rel(p, v);
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
    return _InterlockedExchange64(p, v);
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
    return _InterlockedExchange64(p, v);
}

// atomic_compare_exchange_weak_explicit: can fail spuriously even if *p == *oldval

#undef  ATOMIC_CMP_XCHG

#define ATOMIC_CMP_XCHG(valueType, p, oldval, newval, exchangeFunction)     \
    do                                                                      \
    {                                                                       \
        valueType oldValue = *oldval;                                       \
        valueType previousValue = exchangeFunction(p, newval, oldValue);    \
        if (previousValue == oldValue)                                      \
            return true;                                                    \
                                                                            \
        *oldval = previousValue;                                            \
        return false;                                                       \
    }                                                                       \
    while (false)

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_nf);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_acq);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, memory_order_release_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_rel);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, memory_order_acquire_t, memory_order_acquire_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_acq);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, memory_order_release_t, memory_order_release_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_rel);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile int* p, int *oldval, int newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_nf);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_acq);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_rel);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_acquire_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_acq);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_release_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_rel);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64);
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64);
}

// atomic_compare_exchange_strong_explicit: does loop and only returns false if *p != *oldval

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_nf);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_acq);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, memory_order_release_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_rel);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, memory_order_acquire_t, memory_order_acquire_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_acq);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, memory_order_release_t, memory_order_release_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange_rel);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile int* p, int *oldval, int newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
    ATOMIC_CMP_XCHG(long, reinterpret_cast<volatile long*>(p), oldval, newval, _InterlockedCompareExchange);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_nf);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_acq);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_rel);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_acquire_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_acq);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_release_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64_rel);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
    ATOMIC_CMP_XCHG(atomic_word, p, oldval, newval, _InterlockedCompareExchange64);
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, memory_order_relaxed_t)
{
    return _InterlockedExchangeAdd_nf(reinterpret_cast<volatile long*>(p), v);
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, memory_order_acquire_t)
{
    return _InterlockedExchangeAdd_acq(reinterpret_cast<volatile long*>(p), v);
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, memory_order_release_t)
{
    return _InterlockedExchangeAdd_rel(reinterpret_cast<volatile long*>(p), v);
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, memory_order_acq_rel_t)
{
    return _InterlockedExchangeAdd(reinterpret_cast<volatile long*>(p), v);
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, int /* memory_order_seq_cst_t */)
{
    return _InterlockedExchangeAdd(reinterpret_cast<volatile long*>(p), v);
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    return _InterlockedExchangeAdd64_nf(p, v);
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
    return _InterlockedExchangeAdd64_acq(p, v);
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
    return _InterlockedExchangeAdd64_rel(p, v);
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
    return _InterlockedExchangeAdd64(p, v);
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
    return _InterlockedExchangeAdd64(p, v);
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, memory_order_relaxed_t)
{
    return _InterlockedExchangeAdd_nf(reinterpret_cast<volatile long*>(p), -v);
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, memory_order_acquire_t)
{
    return _InterlockedExchangeAdd_acq(reinterpret_cast<volatile long*>(p), -v);
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, memory_order_release_t)
{
    return _InterlockedExchangeAdd_rel(reinterpret_cast<volatile long*>(p), -v);
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, memory_order_acq_rel_t)
{
    return _InterlockedExchangeAdd(reinterpret_cast<volatile long*>(p), -v);
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, int /* memory_order_seq_cst_t */)
{
    return _InterlockedExchangeAdd(reinterpret_cast<volatile long*>(p), -v);
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    return _InterlockedExchangeAdd64_nf(p, -v);
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
    return _InterlockedExchangeAdd64_acq(p, -v);
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
    return _InterlockedExchangeAdd64_rel(p, -v);
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
    return _InterlockedExchangeAdd64(p, -v);
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
    return _InterlockedExchangeAdd64(p, -v);
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
    bool res = atomic_fetch_sub_explicit(p, 1, memory_order_release) == 1;
    if (res)
    {
        atomic_thread_fence(memory_order_acquire);
    }
    return res;
}

/*
*  double word
*/

static inline atomic_word2 atomic_load_explicit(const volatile atomic_word2* p, memory_order_relaxed_t)
{
    atomic_word2 result = { 0, 0 };
    _InterlockedCompareExchange128_nf(/* _Destination */ const_cast<volatile atomic_word*>(&p->lo), /* _ExchangeHigh */ 0, /* _ExchangeLow */ 0, /* _ComparandResult */ &result.lo);
    return result;
}

static inline atomic_word2 atomic_load_explicit(const volatile atomic_word2* p, memory_order_acquire_t)
{
    atomic_word2 result = { 0, 0 };
    _InterlockedCompareExchange128_acq(/* _Destination */ const_cast<volatile atomic_word*>(&p->lo), /* _ExchangeHigh */ 0, /* _ExchangeLow */ 0, /* _ComparandResult */ &result.lo);
    return result;
}

static inline void atomic_store_explicit(volatile atomic_word2* p, atomic_word2 v, memory_order_relaxed_t)
{
    atomic_word2 comparand = v;
    while (!_InterlockedCompareExchange128_nf(/* _Destination */ &p->lo, /* _ExchangeHigh */ v.hi, /* _ExchangeLow */ v.lo, /* _ComparandResult */ &comparand.lo))
    {
    }
}

static inline void atomic_store_explicit(volatile atomic_word2* p, atomic_word2 v, memory_order_release_t)
{
    atomic_word2 comparand = v;
    while (!_InterlockedCompareExchange128_rel(/* _Destination */ &p->lo, /* _ExchangeHigh */ v.hi, /* _ExchangeLow */ v.lo, /* _ComparandResult */ &comparand.lo))
    {
    }
}

static inline atomic_word2 atomic_exchange_explicit(volatile atomic_word2* p, atomic_word2 val, memory_order_acq_rel_t)
{
    atomic_word2 comparand = val;
    while (!_InterlockedCompareExchange128(/* _Destination */ &p->lo, /* _ExchangeHigh */ val.hi, /* _ExchangeLow */ val.lo, /* _ComparandResult */ &comparand.lo))
    {
    }
    return comparand;
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    return _InterlockedCompareExchange128_acq(/* _Destination */ &p->lo, /* _ExchangeHigh */ newval.hi, /* _ExchangeLow */ newval.lo, /* _ComparandResult */ &oldval->lo);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_release_t, memory_order_relaxed_t)
{
    return _InterlockedCompareExchange128_rel(/* _Destination */ &p->lo, /* _ExchangeHigh */ newval.hi, /* _ExchangeLow */ newval.lo, /* _ComparandResult */ &oldval->lo);
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, int /*memory_order_acq_rel_t*/, memory_order_relaxed_t)
{
    return _InterlockedCompareExchange128(/* _Destination */ &p->lo, /* _ExchangeHigh */ newval.hi, /* _ExchangeLow */ newval.lo, /* _ComparandResult */ &oldval->lo);
}
