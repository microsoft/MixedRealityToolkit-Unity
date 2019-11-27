#define ASM_DMB_ISH         "dmb    ish\n\t"

#if defined(__ARM_ARCH_7S__)
// this is sufficient for Swift processors
#   define ASM_REL         "dmb    ishst\n\t"
#else
#   define ASM_REL         "dmb    ish\n\t"
#endif

static inline void atomic_thread_fence(memory_order_relaxed_t)
{
}

static inline void atomic_thread_fence(memory_order_acquire_t)
{
    __asm__ __volatile__ ("dmb ld\n\t" : : : "memory");
}

static inline void atomic_thread_fence(memory_order_release_t)
{
    __asm__ __volatile__ (ASM_REL : : : "memory");
}

static inline void atomic_thread_fence(memory_order_acq_rel_t)
{
    __asm__ __volatile__ (ASM_DMB_ISH : : : "memory");
}

static inline void atomic_thread_fence(int /* memory_order_seq_cst_t */)
{
    __asm__ __volatile__ (ASM_DMB_ISH : : : "memory");
}

#define ATOMIC_LOAD(opc) \
    atomic_word res; \
    __asm__ __volatile__ \
    ( \
        opc "   %0, %1\n\t" \
        : "=r" (res) \
        : "m" (*p) \
    ); \
    return res;

/*
 *  int support
 */

static inline int atomic_load_explicit(const volatile int* p, memory_order_relaxed_t)
{
    int res;
    __asm__ __volatile__
    (
        "ldr   %w0, %w1\n\t"
        : "=r" (res)
        : "m" (*p)
    );
    return res;
}

static inline int atomic_load_explicit(const volatile int* p, memory_order_acquire_t)
{
    int res;
    __asm__ __volatile__
    (
        "ldar   %w0, %w1\n\t"
        : "=r" (res)
        : "m" (*p)
    );
    return res;
}

static inline int atomic_load_explicit(const volatile int* p, int /* memory_order_seq_cst_t */)
{
    int res;
    __asm__ __volatile__
    (
        "ldar   %w0, %w1\n\t"
        : "=r" (res)
        : "m" (*p)
    );
    return res;
}

/*
 *  native word support
 */

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_relaxed_t)
{
    ATOMIC_LOAD("ldr")
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, memory_order_acquire_t)
{
    ATOMIC_LOAD("ldar")
}

static inline atomic_word atomic_load_explicit(const volatile atomic_word* p, int /* memory_order_seq_cst_t */)
{
    ATOMIC_LOAD("ldar")
}

#define ATOMIC_STORE(opc) \
    __asm__ __volatile__ \
    ( \
        opc "   %1, %0\n\t" \
        : "=m" (*p) \
        : "r" (v) \
        : "memory" \
    );

/*
 *  int support
 */

static inline void atomic_store_explicit(volatile int* p, int v, memory_order_relaxed_t)
{
    __asm__ __volatile__
    (
        "str   %w1, %w0\n\t"
        : "=m" (*p)
        : "r" (v)
        : "memory"
    );
}

static inline void atomic_store_explicit(volatile int* p, int v, memory_order_release_t)
{
    __asm__ __volatile__
    (
        "stlr   %w1, %w0\n\t"
        : "=m" (*p)
        : "r" (v)
        : "memory"
    );
}

static inline void atomic_store_explicit(volatile int* p, int v, int /* memory_order_seq_cst_t */)
{
    __asm__ __volatile__
    (
        "stlr   %w1, %w0\n\t"
        : "=m" (*p)
        : "r" (v)
        : "memory"
    );
}

/*
 *  native word support
 */

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    ATOMIC_STORE("str")
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
    ATOMIC_STORE("stlr")
}

static inline void atomic_store_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
    ATOMIC_STORE("stlr")
}

#define ATOMIC_XCHG(LD, ST) \
    atomic_word res; \
    atomic_word success; \
    __asm__ __volatile__ \
    ( \
    "0:\n\t" \
        LD "    %2, [%4]\n\t" \
        ST "    %w0, %3, [%4]\n\t" \
        "cbnz   %w0, 0b\n\t" \
        : "=&r" (success), "+m" (*p), "=&r" (res) \
        : "r" (v), "r" (p) \
        : "memory" \
    ); \
    return res;

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    ATOMIC_XCHG("ldxr", "stxr")
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
    ATOMIC_XCHG("ldaxr", "stxr")
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
    ATOMIC_XCHG("ldxr", "stlxr")
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
    ATOMIC_XCHG("ldaxr", "stlxr")
}

static inline atomic_word atomic_exchange_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
    ATOMIC_XCHG("ldaxr", "stlxr")
}

// atomic_compare_exchange_weak_explicit: can fail spuriously even if *p == *oldval

#define ATOMIC_CMP_XCHG(LD, ST) \
    atomic_word res; \
    atomic_word success = 0; \
    __asm__ __volatile__ \
    ( \
        LD "    %2, [%4]\n\t" \
        "cmp    %2, %5\n\t" \
        "b.ne   1f\n\t" \
        ST "    %w0, %3, [%4]\n" \
    "1:\n\t" \
        "clrex\n\t" \
        : "=&r" (success), "+m" (*p), "=&r" (res) \
        : "r" (newval), "r" (p), "r" (*oldval) \
        : "cc", "memory" \
    ); \
    *oldval = res; \
    return success == 0;


static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldxr", "stxr")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stxr")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldxr", "stlxr")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stlxr")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stlxr")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_acquire_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stxr")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_release_t)
{
    ATOMIC_CMP_XCHG("ldxr", "stlxr")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stlxr")
}

static inline bool atomic_compare_exchange_weak_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
    ATOMIC_CMP_XCHG("ldaxr", "stlxr")
}

// atomic_compare_exchange_strong_explicit: does loop and only returns false if *p != *oldval

#undef ATOMIC_CMP_XCHG

#define ATOMIC_CMP_XCHG(LD, ST) \
    atomic_word res; \
    atomic_word success = 0; \
    __asm__ __volatile__ \
    ( \
    "0:\n\t" \
        LD "    %2, [%4]\n\t" \
        "cmp    %2, %5\n\t" \
        "b.ne   1f\n\t" \
        ST "    %w0, %3, [%4]\n" \
        "cbnz   %w0, 0b\n\t" \
    "1:\n\t" \
        "clrex\n\t" \
        : "=&r" (success), "+m" (*p), "=&r" (res) \
        : "r" (newval), "r" (p), "r" (*oldval) \
        : "cc", "memory" \
    ); \
    *oldval = res; \
    return success == 0;

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_relaxed_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldxr", "stxr")
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stxr")
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldxr", "stlxr")
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stlxr")
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, memory_order_relaxed_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stlxr")
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acquire_t, memory_order_acquire_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stxr")
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_release_t, memory_order_release_t)
{
    ATOMIC_CMP_XCHG("ldxr", "stlxr")
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, memory_order_acq_rel_t, memory_order_acq_rel_t)
{
    ATOMIC_CMP_XCHG("ldaxr", "stlxr")
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word* p, atomic_word *oldval, atomic_word newval, int /* memory_order_seq_cst_t */, int /* memory_order_seq_cst_t */)
{
    ATOMIC_CMP_XCHG("ldaxr", "stlxr")
}

#define ATOMIC_PFIX_int         "%w"
#define ATOMIC_PFIX_atomic_word "%"
#define ATOMIC_PFIX(WORD)       ATOMIC_PFIX_##WORD

#define ATOMIC_OP(WORD, LD, ST, OP) \
    long long res, tmp; \
    int success; \
    __asm__ __volatile__ \
    ( \
    "0:\n\t" \
        LD "    " ATOMIC_PFIX(WORD) "2, [%5]\n\t" \
        OP "    " ATOMIC_PFIX(WORD) "3, " ATOMIC_PFIX(WORD) "2, " ATOMIC_PFIX(WORD) "4\n\t"\
        ST "    %w0, " ATOMIC_PFIX(WORD) "3, [%5]\n" \
        "cbnz   %w0, 0b\n\t" \
        : "=&r" (success), "+m" (*p), "=&r" (res), "=&r" (tmp) \
        : "Ir" ((long long) v), "r" (p) \
        : "cc", "memory" \
    ); \
    return (WORD) res;

static inline int atomic_fetch_add_explicit(volatile int* p, int v, memory_order_relaxed_t)
{
    ATOMIC_OP(int, "ldxr", "stxr", "add")
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, memory_order_acquire_t)
{
    ATOMIC_OP(int, "ldaxr", "stxr", "add")
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, memory_order_release_t)
{
    ATOMIC_OP(int, "ldxr", "stlxr", "add")
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, memory_order_acq_rel_t)
{
    ATOMIC_OP(int, "ldaxr", "stlxr", "add")
}

static inline int atomic_fetch_add_explicit(volatile int* p, int v, int /* memory_order_seq_cst_t */)
{
    ATOMIC_OP(int, "ldaxr", "stlxr", "add")
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    ATOMIC_OP(atomic_word, "ldxr", "stxr", "add")
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
    ATOMIC_OP(atomic_word, "ldaxr", "stxr", "add")
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
    ATOMIC_OP(atomic_word, "ldxr", "stlxr", "add")
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
    ATOMIC_OP(atomic_word, "ldaxr", "stlxr", "add")
}

static inline atomic_word atomic_fetch_add_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
    ATOMIC_OP(atomic_word, "ldaxr", "stlxr", "add")
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, memory_order_relaxed_t)
{
    ATOMIC_OP(int, "ldxr", "stxr", "sub")
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, memory_order_acquire_t)
{
    ATOMIC_OP(int, "ldaxr", "stxr", "sub")
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, memory_order_release_t)
{
    ATOMIC_OP(int, "ldxr", "stlxr", "sub")
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, memory_order_acq_rel_t)
{
    ATOMIC_OP(int, "ldaxr", "stlxr", "sub")
}

static inline int atomic_fetch_sub_explicit(volatile int* p, int v, int /* memory_order_seq_cst_t */)
{
    ATOMIC_OP(int, "ldaxr", "stlxr", "sub")
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_relaxed_t)
{
    ATOMIC_OP(atomic_word, "ldxr", "stxr", "sub")
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_acquire_t)
{
    ATOMIC_OP(atomic_word, "ldaxr", "stxr", "sub")
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_release_t)
{
    ATOMIC_OP(atomic_word, "ldxr", "stlxr", "sub")
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, memory_order_acq_rel_t)
{
    ATOMIC_OP(atomic_word, "ldaxr", "stlxr", "sub")
}

static inline atomic_word atomic_fetch_sub_explicit(volatile atomic_word* p, atomic_word v, int /* memory_order_seq_cst_t */)
{
    ATOMIC_OP(atomic_word, "ldaxr", "stlxr", "sub")
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

// Note: the only way to get atomic 128-bit memory accesses on ARM64 is to use ldxp/stxp with a loop
// (ldxp and stxp instructions are not guaranteed to appear atomic)

static inline atomic_word2 atomic_load_explicit(const volatile atomic_word2* p, memory_order_relaxed_t)
{
    atomic_word2 v;
    atomic_word success;
    __asm__ __volatile__
    (
        "0:\n\t"
        "ldxp\t%1, %2, [%3]\n\t"
        "stxp\t%w0, %1, %2, [%3]\n\t"
        "cbnz\t%w0, 0b\n\t"

        : "=&r" (success), "=&r" (v.lo), "=&r" (v.hi)
        : "r" (p)
    );
    return v;
}

static inline atomic_word2 atomic_load_explicit(const volatile atomic_word2* p, memory_order_acquire_t)
{
    atomic_word2 v;
    atomic_word success;
    __asm__ __volatile__
    (
        "0:\n\t"
        "ldaxp\t%1, %2, [%3]\n\t"
        "stxp\t%w0, %1, %2, [%3]\n\t"
        "cbnz\t%w0, 0b\n\t"

        : "=&r" (success), "=&r" (v.lo), "=&r" (v.hi)
        : "r" (p)
    );
    return v;
}

static inline void atomic_store_explicit(volatile atomic_word2* p, atomic_word2 v, memory_order_relaxed_t)
{
    atomic_word lo;
    atomic_word hi;
    atomic_word success;
    __asm__ __volatile__
    (
        "0:\n\t"
        "ldxp\t%2, %3, [%6]\n\t"
        "stxp\t%w0, %4, %5, [%6]\n\t"
        "cbnz\t%w0, 0b\n\t"

        : "=&r" (success), "=m" (*p), "=&r" (lo), "=&r" (hi)
        : "r" (v.lo), "r" (v.hi), "r" (p)
        : "memory"
    );
}

static inline void atomic_store_explicit(volatile atomic_word2* p, atomic_word2 v, memory_order_release_t)
{
    atomic_word lo;
    atomic_word hi;
    atomic_word success;
    __asm__ __volatile__
    (
        "0:\n\t"
        "ldxp\t%2, %3, [%6]\n\t"
        "stlxp\t%w0, %4, %5, [%6]\n\t"
        "cbnz\t%w0, 0b\n\t"

        : "=&r" (success), "=m" (*p), "=&r" (lo), "=&r" (hi)
        : "r" (v.lo), "r" (v.hi), "r" (p)
        : "memory"
    );
}

static inline atomic_word2 atomic_exchange_explicit(volatile atomic_word2* p, atomic_word2 val, memory_order_acq_rel_t)
{
    atomic_word2 oldval;
    atomic_word success;
    __asm__ __volatile__
    (
        "0:\n\t"
        "ldaxp\t%2, %3, [%6]\n\t"
        "stlxp\t%w0, %5, %4, [%6]\n\t"
        "cbnz\t%w0, 0b\n\t"

        : "=&r" (success), "+m" (*p), "=&r" (oldval.lo), "=&r" (oldval.hi)
        : "r" (val.hi), "r" (val.lo), "r" (p)
        : "memory"
    );

    return oldval;
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_acquire_t, memory_order_relaxed_t)
{
    atomic_word lo = oldval->lo;
    atomic_word hi = oldval->hi;
    atomic_word success;
    __asm__ __volatile__
    (
        "0:\n\t"
        "ldaxp\t%2, %3, [%8]\n\t"
        "cmp\t%3, %5\n\t"
        "b.ne\t1f\n\t"
        "cmp\t%2, %4\n\t"
        "b.ne\t1f\n\t"
        "stxp\t%w0, %6, %7, [%8]\n\t"
        "cbnz\t%w0, 0b\n\t"
        "1:\n\t"
        "clrex\n\t"

        : "=&r" (success), "+m" (*p), "=&r" (oldval->lo), "=&r" (oldval->hi)
        : "r" (lo), "r" (hi), "r" (newval.lo), "r" (newval.hi), "r" (p), "0" (1)
        : "cc", "memory"
    );

    return success == 0;
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, memory_order_release_t, memory_order_relaxed_t)
{
    atomic_word lo = oldval->lo;
    atomic_word hi = oldval->hi;
    atomic_word success;
    __asm__ __volatile__
    (
        "0:\n\t"
        "ldxp\t%2, %3, [%8]\n\t"
        "cmp\t%3, %5\n\t"
        "b.ne\t1f\n\t"
        "cmp\t%2, %4\n\t"
        "b.ne\t1f\n\t"
        "stlxp\t%w0, %6, %7, [%8]\n\t"
        "cbnz\t%w0, 0b\n\t"
        "1:\n\t"
        "clrex\n\t"

        : "=&r" (success), "+m" (*p), "=&r" (oldval->lo), "=&r" (oldval->hi)
        : "r" (lo), "r" (hi), "r" (newval.lo), "r" (newval.hi), "r" (p), "0" (1)
        : "cc", "memory"
    );

    return success == 0;
}

static inline bool atomic_compare_exchange_strong_explicit(volatile atomic_word2* p, atomic_word2* oldval, atomic_word2 newval, int /*memory_order_acq_rel_t*/, memory_order_relaxed_t)
{
    atomic_word lo = oldval->lo;
    atomic_word hi = oldval->hi;
    atomic_word success;
    __asm__ __volatile__
    (
        "0:\n\t"
        "ldaxp\t%2, %3, [%8]\n\t"
        "cmp\t%3, %5\n\t"
        "b.ne\t1f\n\t"
        "cmp\t%2, %4\n\t"
        "b.ne\t1f\n\t"
        "stlxp\t%w0, %6, %7, [%8]\n\t"
        "cbnz\t%w0, 0b\n\t"
        "1:\n\t"
        "clrex\n\t"

        : "=&r" (success), "+m" (*p), "=&r" (oldval->lo), "=&r" (oldval->hi)
        : "r" (lo), "r" (hi), "r" (newval.lo), "r" (newval.hi), "r" (p), "0" (1)
        : "cc", "memory"
    );

    return success == 0;
}
