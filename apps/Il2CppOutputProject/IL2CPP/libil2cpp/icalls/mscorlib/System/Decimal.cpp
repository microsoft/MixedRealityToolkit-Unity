#include "il2cpp-config.h"
#include "il2cpp-object-internals.h"
#include "il2cpp-class-internals.h"
#include "icalls/mscorlib/System/Decimal.h"
#include "utils/StringUtils.h"
#include "vm/Exception.h"
#include <cmath>
#include <algorithm>

#if !NET_4_0

#define DECIMAL_MAX_SCALE 28
#define DECIMAL_MAX_INTFACTORS 9

#define DECIMAL_SUCCESS 0
#define DECIMAL_FINISHED 1
#define DECIMAL_OVERFLOW 2
#define DECIMAL_INVALID_CHARACTER 2
#define DECIMAL_INTERNAL_ERROR 3
#define DECIMAL_INVALID_BITS 4
#define DECIMAL_DIVIDE_BY_ZERO 5
#define DECIMAL_BUFFER_OVERFLOW 6

#define LIT_GUINT32(x) x
#define LIT_GUINT64(x) x##LL

#define LIT_GUINT32_HIGHBIT 0x80000000
#define LIT_GUINT64_HIGHBIT LIT_GUINT64(0x8000000000000000)

#define DECIMAL_LOG_NEGINF -1000

#define DECINIT(src) memset(src, 0, sizeof(il2cpp_decimal_repr))

#define DECCOPY(dest, src) memcpy(dest, src, sizeof(il2cpp_decimal_repr))

#define DECNEGATE(p1) (p1)->u.signscale.sign = 1 - (p1)->u.signscale.sign

#define DECTO128(pd, lo, hi) \
    lo = (((uint64_t)(pd)->mid32) << 32) | (pd)->lo32; \
    hi = (pd)->hi32;

#ifndef MAX
#define MAX(a, b) (((a)>(b)) ? (a) : (b))
#endif

#ifdef _DEBUG
#include <assert.h>
#define PRECONDITION(flag) IL2CPP_ASSERT(flag)
#define POSTCONDITION(flag) IL2CPP_ASSERT(flag)
#else
#define PRECONDITION(flag)
#define POSTCONDITION(flag)
#endif /* _DEBUG */

typedef struct
{
    uint64_t lo;
    uint64_t hi;
} dec128_repr;

#define LIT_DEC128(hi, mid, lo) { (((uint64_t)mid)<<32 | lo), hi }

static const dec128_repr dec128decadeFactors[DECIMAL_MAX_SCALE + 1] =
{
    LIT_DEC128(0, 0, 1u), /* == 1 */
    LIT_DEC128(0, 0, 10u), /* == 10 */
    LIT_DEC128(0, 0, 100u), /* == 100 */
    LIT_DEC128(0, 0, 1000u), /* == 1e3m */
    LIT_DEC128(0, 0, 10000u), /* == 1e4m */
    LIT_DEC128(0, 0, 100000u), /* == 1e5m */
    LIT_DEC128(0, 0, 1000000u), /* == 1e6m */
    LIT_DEC128(0, 0, 10000000u), /* == 1e7m */
    LIT_DEC128(0, 0, 100000000u), /* == 1e8m */
    LIT_DEC128(0, 0, 1000000000u), /* == 1e9m */
    LIT_DEC128(0, 2u, 1410065408u), /* == 1e10m */
    LIT_DEC128(0, 23u, 1215752192u), /* == 1e11m */
    LIT_DEC128(0, 232u, 3567587328u), /* == 1e12m */
    LIT_DEC128(0, 2328u, 1316134912u), /* == 1e13m */
    LIT_DEC128(0, 23283u, 276447232u), /* == 1e14m */
    LIT_DEC128(0, 232830u, 2764472320u), /* == 1e15m */
    LIT_DEC128(0, 2328306u, 1874919424u), /* == 1e16m */
    LIT_DEC128(0, 23283064u, 1569325056u), /* == 1e17m */
    LIT_DEC128(0, 232830643u, 2808348672u), /* == 1e18m */
    LIT_DEC128(0, 2328306436u, 2313682944u), /* == 1e19m */
    LIT_DEC128(5u, 1808227885u, 1661992960u), /* == 1e20m */
    LIT_DEC128(54u, 902409669u, 3735027712u), /* == 1e21m */
    LIT_DEC128(542u, 434162106u, 2990538752u), /* == 1e22m */
    LIT_DEC128(5421u, 46653770u, 4135583744u), /* == 1e23m */
    LIT_DEC128(54210u, 466537709u, 2701131776u), /* == 1e24m */
    LIT_DEC128(542101u, 370409800u, 1241513984u), /* == 1e25m */
    LIT_DEC128(5421010u, 3704098002u, 3825205248u), /* == 1e26m */
    LIT_DEC128(54210108u, 2681241660u, 3892314112u), /* == 1e27m */
    LIT_DEC128(542101086u, 1042612833u, 268435456u), /* == 1e28m */
};

static const uint32_t constantsDecadeInt32Factors[DECIMAL_MAX_INTFACTORS + 1] =
{
    1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000
};

static int pack128toDecimal(il2cpp_decimal_repr* pA, uint64_t alo, uint64_t ahi, int scale, int sign)
{
    if (scale < 0 || scale > DECIMAL_MAX_SCALE || (ahi >> 32) != 0)
        return DECIMAL_OVERFLOW;

    pA->lo32 = (uint32_t)alo;
    pA->mid32 = (uint32_t)(alo >> 32);
    pA->hi32 = (uint32_t)ahi;
    pA->u.signscale.sign = sign;
    pA->u.signscale.scale = scale;

    return DECIMAL_SUCCESS;
}

static void add128(uint64_t alo, uint64_t ahi, uint64_t blo, uint64_t bhi, uint64_t* pclo, uint64_t* pchi)
{
    alo += blo;
    if (alo < blo)
        ahi++;
    ahi += bhi;

    *pclo = alo;
    *pchi = ahi;
}

static void sub128(uint64_t alo, uint64_t ahi, uint64_t blo, uint64_t bhi, uint64_t* pclo, uint64_t* pchi)
{
    uint64_t clo, chi;

    clo = alo - blo;
    chi = ahi - bhi;
    if (alo < blo)
        chi--;            /* borrow */

    *pclo = clo;
    *pchi = chi;
}

static int div128by32(uint64_t* plo, uint64_t* phi, uint32_t factor, uint32_t* pRest)
{
    uint64_t a, b, c, h;

    h = *phi;
    a = (uint32_t)(h >> 32);
    b = a / factor;
    a -= b * factor;
    a <<= 32;
    a |= (uint32_t)h;
    c = a / factor;
    a -= c * factor;
    a <<= 32;
    *phi = b << 32 | (uint32_t)c;

    h = *plo;
    a |= (uint32_t)(h >> 32);
    b = a / factor;
    a -= b * factor;
    a <<= 32;
    a |= (uint32_t)h;
    c = a / factor;
    a -= c * factor;
    *plo = b << 32 | (uint32_t)c;

    if (pRest)
        *pRest = (uint32_t)a;

    a <<= 1;
    return (a >= factor || (a == factor && (c & 1) == 1)) ? 1 : 0;
}

static int mult128by32(uint64_t* pclo, uint64_t* pchi, uint32_t factor, int roundBit)
{
    uint64_t a;
    uint32_t h0, h1;

    a = ((uint64_t)(uint32_t)(*pclo)) * factor;
    if (roundBit)
        a += factor / 2;
    h0 = (uint32_t)a;

    a >>= 32;
    a += (*pclo >> 32) * factor;
    h1 = (uint32_t)a;

    *pclo = ((uint64_t)h1) << 32 | h0;

    a >>= 32;
    a += ((uint64_t)(uint32_t)(*pchi)) * factor;
    h0 = (uint32_t)a;

    a >>= 32;
    a += (*pchi >> 32) * factor;
    h1 = (uint32_t)a;

    *pchi = ((uint64_t)h1) << 32 | h0;

    return ((a >> 32) == 0) ? DECIMAL_SUCCESS : DECIMAL_OVERFLOW;
}

static void mult96by96to192(uint32_t alo, uint32_t ami, uint32_t ahi, uint32_t blo, uint32_t bmi, uint32_t bhi, uint64_t* pclo, uint64_t* pcmi, uint64_t* pchi)
{
    uint64_t a, b, c, d;
    uint32_t h0, h1, h2, h3, h4, h5;
    int carry0, carry1;

    a = ((uint64_t)alo) * blo;
    h0 = (uint32_t)a;

    a >>= 32; carry0 = 0;
    b = ((uint64_t)alo) * bmi;
    c = ((uint64_t)ami) * blo;
    a += b; if (a < b)
        carry0++;
    a += c; if (a < c)
        carry0++;
    h1 = (uint32_t)a;

    a >>= 32; carry1 = 0;
    b = ((uint64_t)alo) * bhi;
    c = ((uint64_t)ami) * bmi;
    d = ((uint64_t)ahi) * blo;
    a += b; if (a < b)
        carry1++;
    a += c; if (a < c)
        carry1++;
    a += d; if (a < d)
        carry1++;
    h2 = (uint32_t)a;

    a >>= 32; a += carry0; carry0 = 0;
    b = ((uint64_t)ami) * bhi;
    c = ((uint64_t)ahi) * bmi;
    a += b; if (a < b)
        carry0++;
    a += c; if (a < c)
        carry0++;
    h3 = (uint32_t)a;

    a >>= 32; a += carry1;
    b = ((uint64_t)ahi) * bhi;
    a += b;
    h4 = (uint32_t)a;

    a >>= 32; a += carry0;
    h5 = (uint32_t)a;

    *pclo = ((uint64_t)h1) << 32 | h0;
    *pcmi = ((uint64_t)h3) << 32 | h2;
    *pchi = ((uint64_t)h5) << 32 | h4;
}

static void div192by32(uint64_t* plo, uint64_t* pmi, uint64_t* phi, uint32_t factor)
{
    uint64_t a, b, c, h;

    h = *phi;
    a = (uint32_t)(h >> 32);
    b = a / factor;
    a -= b * factor;
    a <<= 32;
    a |= (uint32_t)h;
    c = a / factor;
    a -= c * factor;
    a <<= 32;
    *phi = b << 32 | (uint32_t)c;

    h = *pmi;
    a |= (uint32_t)(h >> 32);
    b = a / factor;
    a -= b * factor;
    a <<= 32;
    a |= (uint32_t)h;
    c = a / factor;
    a -= c * factor;
    a <<= 32;
    *pmi = b << 32 | (uint32_t)c;

    h = *plo;
    a |= (uint32_t)(h >> 32);
    b = a / factor;
    a -= b * factor;
    a <<= 32;
    a |= (uint32_t)h;
    c = a / factor;
    a -= c * factor;
    a <<= 32;
    *plo = b << 32 | (uint32_t)c;
}

static int incMultConstant128(uint64_t* palo, uint64_t* pahi, int idx, int factor)
{
    uint64_t blo, bhi, h;

    IL2CPP_ASSERT(idx >= 0 && idx <= DECIMAL_MAX_SCALE);
    IL2CPP_ASSERT(factor > 0 && factor <= 9);

    blo = dec128decadeFactors[idx].lo;
    h = bhi = dec128decadeFactors[idx].hi;
    if (factor != 1)
    {
        mult128by32(&blo, &bhi, factor, 0);
        if (h > bhi)
            return DECIMAL_OVERFLOW;
    }
    h = *pahi;
    add128(*palo, *pahi, blo, bhi, palo, pahi);
    if (h > *pahi)
        return DECIMAL_OVERFLOW;

    return DECIMAL_SUCCESS;
}

static void roundUp128(uint64_t* pclo, uint64_t* pchi)
{
    if (++(*pclo) == 0)
        ++(*pchi);
}

static int decimalIsZero(il2cpp_decimal_repr* pA)
{
    return (pA->lo32 == 0 && pA->mid32 == 0 && pA->hi32 == 0);
}

static void sub192(uint64_t alo, uint64_t ami, uint64_t ahi , uint64_t blo, uint64_t bmi, uint64_t bhi, uint64_t* pclo, uint64_t* pcmi, uint64_t* pchi)
{
    uint64_t clo, cmi, chi;

    clo = alo - blo;
    cmi = ami - bmi;
    chi = ahi - bhi;
    if (alo < blo)
    {
        if (cmi == 0)
            chi--; /* borrow mid */
        cmi--; /* borrow low */
    }
    if (ami < bmi)
        chi--; /* borrow mid */
    *pclo = clo;
    *pcmi = cmi;
    *pchi = chi;
}

static void mult96by32to128(uint32_t alo, uint32_t ami, uint32_t ahi, uint32_t factor, uint64_t* pclo, uint64_t* pchi)
{
    uint64_t a;
    uint32_t h0, h1;

    a = ((uint64_t)alo) * factor;
    h0 = (uint32_t)a;

    a >>= 32;
    a += ((uint64_t)ami) * factor;
    h1 = (uint32_t)a;

    a >>= 32;
    a += ((uint64_t)ahi) * factor;

    *pclo = ((uint64_t)h1) << 32 | h0;
    *pchi = a;
}

static void add192(uint64_t alo, uint64_t ami, uint64_t ahi, uint64_t blo, uint64_t bmi, uint64_t bhi, uint64_t* pclo, uint64_t* pcmi, uint64_t* pchi)
{
    alo += blo;
    if (alo < blo)
    { /* carry low */
        ami++;
        if (ami == 0)
            ahi++; /* carry mid */
    }
    ami += bmi;
    if (ami < bmi)
        ahi++; /* carry mid */
    ahi += bhi;
    *pclo = alo;
    *pcmi = ami;
    *pchi = ahi;
}

static uint32_t div192by96to32withRest(uint64_t* palo, uint64_t* pami, uint64_t* pahi, uint32_t blo, uint32_t bmi, uint32_t bhi)
{
    uint64_t rlo, rmi, rhi; /* remainder */
    uint64_t tlo, thi; /* term */
    uint32_t c;

    rlo = *palo; rmi = *pami; rhi = *pahi;
    if (rhi >= (((uint64_t)bhi) << 32))
    {
        c = LIT_GUINT32(0xFFFFFFFF);
    }
    else
    {
        c = (uint32_t)(rhi / bhi);
    }
    mult96by32to128(blo, bmi, bhi, c, &tlo, &thi);
    sub192(rlo, rmi, rhi, 0, tlo, thi, &rlo, &rmi, &rhi);
    while (((int64_t)rhi) < 0)
    {
        c--;
        add192(rlo, rmi, rhi, 0, (((uint64_t)bmi) << 32) | blo, bhi, &rlo, &rmi, &rhi);
    }
    *palo = rlo; *pami = rmi; *pahi = rhi;

    POSTCONDITION(rhi >> 32 == 0);

    return c;
}

static void div192by96to128(uint64_t alo, uint64_t ami, uint64_t ahi, uint32_t blo, uint32_t bmi, uint32_t bhi, uint64_t* pclo, uint64_t* pchi)
{
    uint64_t rlo, rmi, rhi; /* remainder */
    uint32_t h, c;

    PRECONDITION(ahi < (((uint64_t)bhi) << 32 | bmi)
        || (ahi == (((uint64_t)bhi) << 32 | bmi) && (ami >> 32) > blo));

    /* high 32 bit*/
    rlo = alo; rmi = ami; rhi = ahi;
    h = div192by96to32withRest(&rlo, &rmi, &rhi, blo, bmi, bhi);

    /* mid 32 bit*/
    rhi = (rhi << 32) | (rmi >> 32); rmi = (rmi << 32) | (rlo >> 32); rlo <<= 32;
    *pchi = (((uint64_t)h) << 32) | div192by96to32withRest(&rlo, &rmi, &rhi, blo, bmi, bhi);

    /* low 32 bit */
    rhi = (rhi << 32) | (rmi >> 32); rmi = (rmi << 32) | (rlo >> 32); rlo <<= 32;
    h = div192by96to32withRest(&rlo, &rmi, &rhi, blo, bmi, bhi);

    /* estimate lowest 32 bit (two last bits may be wrong) */
    if (rhi >= bhi)
    {
        c = LIT_GUINT32(0xFFFFFFFF);
    }
    else
    {
        rhi <<= 32;
        c = (uint32_t)(rhi / bhi);
    }
    *pclo = (((uint64_t)h) << 32) | c;
}

static void rshift128(uint64_t* pclo, uint64_t* pchi)
{
    *pclo >>= 1;
    *pclo |= (*pchi & 1) << 63;
    *pchi >>= 1;
}

static void lshift128(uint64_t* pclo, uint64_t* pchi)
{
    *pchi <<= 1;
    *pchi |= (*pclo & LIT_GUINT64_HIGHBIT) >> 63;
    *pclo <<= 1;
}

static inline int
my_g_bit_nth_msf(uintptr_t mask)
{
    int i;

    // Mono uses some architecture and OS-specific code to implement this function.
    // This is the code from the #else case, which attempts to use sizeof(gsize),
    // which is sizeof(uintptr_t) for us. This seems to be wrong on 64-bit though,
    // So we will explicitly always use 4 here.
    //i = sizeof(gsize) * 8;
    i = 4 * 8;
    while (i > 0)
    {
        i--;
        if (mask & static_cast<uintptr_t>(1ULL << i))
            return i;
    }
    return -1;
}

static int decimalDivSub(il2cpp_decimal_repr* pA, il2cpp_decimal_repr* pB, uint64_t* pclo, uint64_t* pchi, int* pExp)
{
    uint64_t alo, ami, ahi;
    uint64_t tlo, tmi, thi;
    uint32_t blo, bmi, bhi;
    int ashift, bshift, extraBit, texp;

    ahi = (((uint64_t)(pA->hi32)) << 32) | pA->mid32;
    ami = ((uint64_t)(pA->lo32)) << 32;
    alo = 0;
    blo = pB->lo32;
    bmi = pB->mid32;
    bhi = pB->hi32;

    if (blo == 0 && bmi == 0 && bhi == 0)
    {
        return DECIMAL_DIVIDE_BY_ZERO;
    }

    if (ami == 0 && ahi == 0)
    {
        *pclo = *pchi = 0;
        return DECIMAL_FINISHED;
    }

    /* enlarge dividend to get maximal precision */
    if (ahi == 0)
    {
        ahi = ami;
        ami = 0;
        for (ashift = 64; (ahi & LIT_GUINT64_HIGHBIT) == 0; ++ashift)
        {
            ahi <<= 1;
        }
    }
    else
    {
        for (ashift = 0; (ahi & LIT_GUINT64_HIGHBIT) == 0; ++ashift)
        {
            lshift128(&ami, &ahi);
        }
    }

    /* ensure that divisor is at least 2^95 */
    if (bhi == 0)
    {
        if (bmi == 0)
        {
            uint32_t hi_shift;
            bhi = blo;
            bmi = 0;
            blo = 0;

            //IL2CPP_ASSERT(g_bit_nth_msf (bhi, 32) == my_g_bit_nth_msf (bhi));

            hi_shift = 31 - my_g_bit_nth_msf(bhi);
            bhi <<= hi_shift;
            bshift = 64 + hi_shift;
        }
        else
        {
            bhi = bmi;
            bmi = blo;
            blo = 0;

            for (bshift = 32; (bhi & LIT_GUINT32_HIGHBIT) == 0; ++bshift)
            {
                bhi <<= 1;
                bhi |= (bmi & LIT_GUINT32_HIGHBIT) >> 31;
                bmi <<= 1;
            }
        }
    }
    else
    {
        for (bshift = 0; (bhi & LIT_GUINT32_HIGHBIT) == 0; ++bshift)
        {
            bhi <<= 1;
            bhi |= (bmi & LIT_GUINT32_HIGHBIT) >> 31;
            bmi <<= 1;
            bmi |= (blo & LIT_GUINT32_HIGHBIT) >> 31;
            blo <<= 1;
        }
    }

    thi = ((uint64_t)bhi) << 32 | bmi;
    tmi = ((uint64_t)blo) << 32;
    tlo = 0;
    if (ahi > thi || (ahi == thi && ami >= tmi))
    {
        sub192(alo, ami, ahi, tlo, tmi, thi, &alo, &ami, &ahi);
        extraBit = 1;
    }
    else
    {
        extraBit = 0;
    }

    div192by96to128(alo, ami, ahi, blo, bmi, bhi, pclo, pchi);
    texp = 128 + ashift - bshift;

    if (extraBit)
    {
        rshift128(pclo, pchi);
        *pchi += LIT_GUINT64_HIGHBIT;
        texp--;
    }

    /* try loss free right shift */
    while (texp > 0 && (*pclo & 1) == 0)
    {
        /* right shift */
        rshift128(pclo, pchi);
        texp--;
    }

    *pExp = texp;

    return DECIMAL_SUCCESS;
}

static int log2_32(uint32_t a)
{
    if (a == 0)
        return DECIMAL_LOG_NEGINF;

    return my_g_bit_nth_msf(a) + 1;
}

static int log2_64(uint64_t a)
{
    if (a == 0)
        return DECIMAL_LOG_NEGINF;

#if SIZEOF_VOID_P == 8
    return my_g_bit_nth_msf(a) + 1;
#else
    if ((a >> 32) == 0)
        return my_g_bit_nth_msf((uint32_t)a) + 1;
    else
        return my_g_bit_nth_msf((uint32_t)(a >> 32)) + 1 + 32;
#endif
}

static int log2_128(uint64_t alo, uint64_t ahi)
{
    if (ahi == 0)
        return log2_64(alo);
    else
        return log2_64(ahi) + 64;
}

static int log2withScale_128(uint64_t alo, uint64_t ahi, int scale)
{
    int tlog2 = log2_128(alo, ahi);
    if (tlog2 < 0)
        tlog2 = 0;
    return tlog2 - (scale * 33219) / 10000;
}

static int decimalLog2(il2cpp_decimal_repr* pA)
{
    int tlog2;
    int scale = pA->u.signscale.scale;

    if (pA->hi32 != 0)
        tlog2 = 64 + log2_32(pA->hi32);
    else if (pA->mid32 != 0)
        tlog2 = 32 + log2_32(pA->mid32);
    else
        tlog2 = log2_32(pA->lo32);

    if (tlog2 != DECIMAL_LOG_NEGINF)
    {
        tlog2 -= (scale * 33219) / 10000;
    }

    return tlog2;
}

static int normalize128(uint64_t* pclo, uint64_t* pchi, int* pScale, int roundFlag, int roundBit)
{
    uint32_t overhang = (uint32_t)(*pchi >> 32);
    int scale = *pScale;
    int deltaScale;

    while (overhang != 0)
    {
        for (deltaScale = 1; deltaScale < DECIMAL_MAX_INTFACTORS; deltaScale++)
        {
            if (overhang < constantsDecadeInt32Factors[deltaScale])
                break;
        }

        scale -= deltaScale;

        if (scale < 0)
            return DECIMAL_OVERFLOW;

        roundBit = div128by32(pclo, pchi, constantsDecadeInt32Factors[deltaScale], 0);

        overhang = (uint32_t)(*pchi >> 32);
        if (roundFlag && roundBit && *pclo == (uint64_t)-1 && (int32_t)*pchi == (int32_t)-1)
            overhang = 1;
    }

    *pScale = scale;

    if (roundFlag && roundBit)
        roundUp128(pclo, pchi);

    return DECIMAL_SUCCESS;
}

static int adjustScale128(uint64_t* palo, uint64_t* pahi, int deltaScale)
{
    int idx, rc;

    if (deltaScale < 0)
    {
        deltaScale *= -1;
        if (deltaScale > DECIMAL_MAX_SCALE)
            return DECIMAL_INTERNAL_ERROR;
        while (deltaScale > 0)
        {
            idx = (deltaScale > DECIMAL_MAX_INTFACTORS) ? DECIMAL_MAX_INTFACTORS : deltaScale;
            deltaScale -= idx;
            div128by32(palo, pahi, constantsDecadeInt32Factors[idx], 0);
        }
    }
    else if (deltaScale > 0)
    {
        if (deltaScale > DECIMAL_MAX_SCALE)
            return DECIMAL_INTERNAL_ERROR;
        while (deltaScale > 0)
        {
            idx = (deltaScale > DECIMAL_MAX_INTFACTORS) ? DECIMAL_MAX_INTFACTORS : deltaScale;
            deltaScale -= idx;
            rc = mult128by32(palo, pahi, constantsDecadeInt32Factors[idx], 0);
            if (rc != DECIMAL_SUCCESS)
                return rc;
        }
    }

    return DECIMAL_SUCCESS;
}

static int rescale128(uint64_t* pclo, uint64_t* pchi, int* pScale, int texp,
    int minScale, int maxScale, int roundFlag)
{
    uint32_t factor, overhang;
    int scale, i, rc, roundBit = 0;

    scale = *pScale;

    if (texp > 0)
    {
        /* reduce exp */
        while (texp > 0 && scale <= maxScale)
        {
            overhang = (uint32_t)(*pchi >> 32);

            /* The original loop was this: */
            /*
            while (texp > 0 && (overhang > (2<<DECIMAL_MAX_INTFACTORS) || (*pclo & 1) == 0)) {
                if (--texp == 0)
                    roundBit = (int)(*pclo & 1);
                rshift128(pclo, pchi);
                overhang = (guint32)(*pchi >> 32);
            }
            */
            if (overhang > 0)
            {
                int msf = my_g_bit_nth_msf(overhang);
                int shift = msf - (DECIMAL_MAX_INTFACTORS + 2);

                if (shift >= texp)
                    shift = texp - 1;

                if (shift > 0)
                {
                    texp -= shift;
                    *pclo = (*pclo >> shift) | ((*pchi & ((1 << shift) - 1)) << (64 - shift));
                    *pchi >>= shift;
                    overhang >>= shift;

                    IL2CPP_ASSERT(texp > 0);
                    IL2CPP_ASSERT(overhang > (2 << DECIMAL_MAX_INTFACTORS));
                }
            }
            while (texp > 0 && (overhang > (2 << DECIMAL_MAX_INTFACTORS) || (*pclo & 1) == 0))
            {
                if (--texp == 0)
                    roundBit = (int)(*pclo & 1);
                rshift128(pclo, pchi);
                overhang >>= 1;
            }

            if (texp > DECIMAL_MAX_INTFACTORS)
                i = DECIMAL_MAX_INTFACTORS;
            else
                i = texp;
            if (scale + i > maxScale)
                i = maxScale - scale;
            if (i == 0)
                break;
            texp -= i;
            scale += i;
            factor = constantsDecadeInt32Factors[i] >> i; /* 10^i/2^i=5^i */
            mult128by32(pclo, pchi, factor, 0);
            /*printf("3: %.17e\n", (((double)chi) * pow(2,64) + clo) * pow(10, -scale) * pow(2, -texp));*/
        }

        while (texp > 0)
        {
            if (--texp == 0)
                roundBit = (int)(*pclo & 1);
            rshift128(pclo, pchi);
        }
    }

    while (scale > maxScale)
    {
        i = scale - maxScale;
        if (i > DECIMAL_MAX_INTFACTORS)
            i = DECIMAL_MAX_INTFACTORS;
        scale -= i;
        roundBit = div128by32(pclo, pchi, constantsDecadeInt32Factors[i], 0);
    }

    while (scale < minScale)
    {
        if (!roundFlag)
            roundBit = 0;
        i = minScale - scale;
        if (i > DECIMAL_MAX_INTFACTORS)
            i = DECIMAL_MAX_INTFACTORS;
        scale += i;
        rc = mult128by32(pclo, pchi, constantsDecadeInt32Factors[i], roundBit);
        if (rc != DECIMAL_SUCCESS)
            return rc;
        roundBit = 0;
    }

    *pScale = scale;

    return normalize128(pclo, pchi, pScale, roundFlag, roundBit);
}

static void div128DecadeFactor(uint64_t* palo, uint64_t* pahi, int powerOfTen)
{
    int idx, roundBit = 0;

    while (powerOfTen > 0)
    {
        idx = (powerOfTen > DECIMAL_MAX_INTFACTORS) ? DECIMAL_MAX_INTFACTORS : powerOfTen;
        powerOfTen -= idx;
        roundBit = div128by32(palo, pahi, constantsDecadeInt32Factors[idx], 0);
    }

    if (roundBit)
        roundUp128(palo, pahi);
}

static void buildIEEE754Double(double* pd, int sign, int texp, uint64_t mantisse)
{
    uint64_t* p = (uint64_t*)pd;

    *p = (((uint64_t)sign) << 63) | (((uint64_t)((1023 + texp) & 0x7ff)) << 52) | mantisse;

    IL2CPP_NOT_IMPLEMENTED_NO_ASSERT(buildIEEE754Double, "Endianess is not being considered");
}

#else // NET_4_0

typedef enum
{
    IL2CPP_DECIMAL_OK,
    IL2CPP_DECIMAL_OVERFLOW,
    IL2CPP_DECIMAL_INVALID_ARGUMENT,
    IL2CPP_DECIMAL_DIVBYZERO,
    IL2CPP_DECIMAL_ARGUMENT_OUT_OF_RANGE
} Il2CppDecimalStatus;

#ifndef FC_GC_POLL
#   define FC_GC_POLL()
#endif

// Double floating point Bias
#define IL2CPP_DOUBLE_BIAS 1022

// Single floating point Bias
#define IL2CPP_SINGLE_BIAS 126

static const uint32_t ten_to_nine = 1000000000U;
static const uint32_t ten_to_ten_div_4 = 2500000000U;
#define POWER10_MAX     9
#define DECIMAL_NEG ((uint8_t)0x80)
#define DECMAX 28
#define DECIMAL_SCALE(dec)       ((dec).u.u.scale)
#define DECIMAL_SIGN(dec)        ((dec).u.u.sign)
#define DECIMAL_SIGNSCALE(dec)   ((dec).u.signscale)
#define DECIMAL_LO32(dec)        ((dec).v.v.Lo32)
#define DECIMAL_MID32(dec)       ((dec).v.v.Mid32)
#define DECIMAL_HI32(dec)        ((dec).Hi32)
#if IL2CPP_BYTE_ORDER != IL2CPP_LITTLE_ENDIAN
# define DECIMAL_LO64_GET(dec)   (((uint64_t)((dec).v.v.Mid32) << 32) | (dec).v.v.Lo32)
# define DECIMAL_LO64_SET(dec, value)   {(dec).v.v.Lo32 = (value); (dec).v.v.Mid32 = ((value) >> 32); }
#else
# define DECIMAL_LO64_GET(dec)    ((dec).v.Lo64)
# define DECIMAL_LO64_SET(dec, value)   {(dec).v.Lo64 = value; }
#endif

#define DECIMAL_SETZERO(dec) {DECIMAL_LO32(dec) = 0; DECIMAL_MID32(dec) = 0; DECIMAL_HI32(dec) = 0; DECIMAL_SIGNSCALE(dec) = 0;}
#define COPYDEC(dest, src) {DECIMAL_SIGNSCALE(dest) = DECIMAL_SIGNSCALE(src); DECIMAL_HI32(dest) = DECIMAL_HI32(src); \
    DECIMAL_MID32(dest) = DECIMAL_MID32(src); DECIMAL_LO32(dest) = DECIMAL_LO32(src); }

#define DEC_SCALE_MAX   28
#define POWER10_MAX     9

#define OVFL_MAX_9_HI   4
#define OVFL_MAX_9_MID  1266874889
#define OVFL_MAX_9_LO   3047500985u

#define OVFL_MAX_5_HI   42949
#define OVFL_MAX_5_MID  2890341191

#define OVFL_MAX_1_HI   429496729

typedef union
{
    uint64_t int64;
    struct
    {
#if IL2CPP_BYTE_ORDER == IL2CPP_BIG_ENDIAN
        uint32_t Hi;
        uint32_t Lo;
#else
        uint32_t Lo;
        uint32_t Hi;
#endif
    } u;
} SPLIT64;

static const SPLIT64    ten_to_eighteen = { 1000000000000000000ULL };

#if IL2CPP_BYTE_ORDER == IL2CPP_BIG_ENDIAN
const Il2CppDouble_double ds2to64 = { { 0, IL2CPP_DOUBLE_BIAS + 65, 0, 0 } };
#else
const Il2CppDouble_double ds2to64 = { { 0, 0, IL2CPP_DOUBLE_BIAS + 65, 0 } };
#endif

//
// Data tables
//

static const uint32_t power10[POWER10_MAX + 1] =
{
    1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000
};


static const double double_power10[] =
{
    1, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9,
    1e10, 1e11, 1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18, 1e19,
    1e20, 1e21, 1e22, 1e23, 1e24, 1e25, 1e26, 1e27, 1e28, 1e29,
    1e30, 1e31, 1e32, 1e33, 1e34, 1e35, 1e36, 1e37, 1e38, 1e39,
    1e40, 1e41, 1e42, 1e43, 1e44, 1e45, 1e46, 1e47, 1e48, 1e49,
    1e50, 1e51, 1e52, 1e53, 1e54, 1e55, 1e56, 1e57, 1e58, 1e59,
    1e60, 1e61, 1e62, 1e63, 1e64, 1e65, 1e66, 1e67, 1e68, 1e69,
    1e70, 1e71, 1e72, 1e73, 1e74, 1e75, 1e76, 1e77, 1e78, 1e79,
    1e80
};

const SPLIT64 sdl_power10[] = { { 10000000000ULL },          // 1E10
                                { 100000000000ULL }, // 1E11
                                { 1000000000000ULL }, // 1E12
                                { 10000000000000ULL }, // 1E13
                                { 100000000000000ULL } }; // 1E14

static const uint64_t long_power10[] =
{
    1,
    10ULL,
    100ULL,
    1000ULL,
    10000ULL,
    100000ULL,
    1000000ULL,
    10000000ULL,
    100000000ULL,
    1000000000ULL,
    10000000000ULL,
    100000000000ULL,
    1000000000000ULL,
    10000000000000ULL,
    100000000000000ULL,
    1000000000000000ULL,
    10000000000000000ULL,
    100000000000000000ULL,
    1000000000000000000ULL,
    10000000000000000000ULL
};

typedef struct
{
    uint32_t Hi, Mid, Lo;
} DECOVFL;

const DECOVFL power_overflow[] =
{
    // This is a table of the largest values that can be in the upper two
    // ULONGs of a 96-bit number that will not overflow when multiplied
    // by a given power.  For the upper word, this is a table of
    // 2^32 / 10^n for 1 <= n <= 9.  For the lower word, this is the
    // remaining fraction part * 2^32.  2^32 = 4294967296.
    //
    { 429496729u, 2576980377u, 2576980377u }, // 10^1 remainder 0.6
    { 42949672u,  4123168604u, 687194767u }, // 10^2 remainder 0.16
    { 4294967u,   1271310319u, 2645699854u }, // 10^3 remainder 0.616
    { 429496u,    3133608139u, 694066715u }, // 10^4 remainder 0.1616
    { 42949u,     2890341191u, 2216890319u }, // 10^5 remainder 0.51616
    { 4294u,      4154504685u, 2369172679u }, // 10^6 remainder 0.551616
    { 429u,       2133437386u, 4102387834u }, // 10^7 remainder 0.9551616
    { 42u,        4078814305u, 410238783u }, // 10^8 remainder 0.09991616
    { 4u,         1266874889u, 3047500985u }, // 10^9 remainder 0.709551616
};


#define UInt32x32To64(a, b) ((uint64_t)((uint32_t)(a)) * (uint64_t)((uint32_t)(b)))
#define Div64by32(num, den) ((uint32_t)((uint64_t)(num) / (uint32_t)(den)))
#define Mod64by32(num, den) ((uint32_t)((uint64_t)(num) % (uint32_t)(den)))

static double
fnDblPower10(int ix)
{
    const int maxIx = (sizeof(double_power10) / sizeof(double_power10[0]));
    IL2CPP_ASSERT(ix >= 0);
    if (ix < maxIx)
        return double_power10[ix];
    return pow(10.0, ix);
} // double fnDblPower10()

static inline int64_t
DivMod32by32(int32_t num, int32_t den)
{
    SPLIT64  sdl;

    sdl.u.Lo = num / den;
    sdl.u.Hi = num % den;
    return sdl.int64;
}

static inline int64_t
DivMod64by32(int64_t num, int32_t den)
{
    SPLIT64  sdl;

    sdl.u.Lo = Div64by32(num, den);
    sdl.u.Hi = Mod64by32(num, den);
    return sdl.int64;
}

static uint64_t
UInt64x64To128(SPLIT64 op1, SPLIT64 op2, uint64_t *hi)
{
    SPLIT64  tmp1;
    SPLIT64  tmp2;
    SPLIT64  tmp3;

    tmp1.int64 = UInt32x32To64(op1.u.Lo, op2.u.Lo); // lo partial prod
    tmp2.int64 = UInt32x32To64(op1.u.Lo, op2.u.Hi); // mid 1 partial prod
    tmp1.u.Hi += tmp2.u.Lo;
    if (tmp1.u.Hi < tmp2.u.Lo)  // test for carry
        tmp2.u.Hi++;
    tmp3.int64 = UInt32x32To64(op1.u.Hi, op2.u.Hi) + (uint64_t)tmp2.u.Hi;
    tmp2.int64 = UInt32x32To64(op1.u.Hi, op2.u.Lo);
    tmp1.u.Hi += tmp2.u.Lo;
    if (tmp1.u.Hi < tmp2.u.Lo)  // test for carry
        tmp2.u.Hi++;
    tmp3.int64 += (uint64_t)tmp2.u.Hi;

    *hi = tmp3.int64;
    return tmp1.int64;
}

/**
* FullDiv64By32:
*
* Entry:
*   pdlNum  - Pointer to 64-bit dividend
*   ulDen   - 32-bit divisor
*
* Purpose:
*   Do full divide, yielding 64-bit result and 32-bit remainder.
*
* Exit:
*   Quotient overwrites dividend.
*   Returns remainder.
*
* Exceptions:
*   None.
*/
// Was: FullDiv64By32
static uint32_t
FullDiv64By32(uint64_t *num, uint32_t den)
{
    SPLIT64  tmp;
    SPLIT64  res;

    tmp.int64 = *num;
    res.u.Hi = 0;

    if (tmp.u.Hi >= den)
    {
        // DivMod64by32 returns quotient in Lo, remainder in Hi.
        //
        res.u.Lo = tmp.u.Hi;
        res.int64 = DivMod64by32(res.int64, den);
        tmp.u.Hi = res.u.Hi;
        res.u.Hi = res.u.Lo;
    }

    tmp.int64 = DivMod64by32(tmp.int64, den);
    res.u.Lo = tmp.u.Lo;
    *num = res.int64;
    return tmp.u.Hi;
}

/***
* SearchScale
*
* Entry:
*   res_hi - Top uint32_t of quotient
*   res_mid - Middle uint32_t of quotient
*   res_lo - Bottom uint32_t of quotient
*   scale  - Scale factor of quotient, range -DEC_SCALE_MAX to DEC_SCALE_MAX
*
* Purpose:
*   Determine the max power of 10, <= 9, that the quotient can be scaled
*   up by and still fit in 96 bits.
*
* Exit:
*   Returns power of 10 to scale by, -1 if overflow error.
*
***********************************************************************/

static int
SearchScale(uint32_t res_hi, uint32_t res_mid, uint32_t res_lo, int scale)
{
    int   cur_scale;

    // Quick check to stop us from trying to scale any more.
    //
    if (res_hi > OVFL_MAX_1_HI || scale >= DEC_SCALE_MAX)
    {
        cur_scale = 0;
        goto HaveScale;
    }

    if (scale > DEC_SCALE_MAX - 9)
    {
        // We can't scale by 10^9 without exceeding the max scale factor.
        // See if we can scale to the max.  If not, we'll fall into
        // standard search for scale factor.
        //
        cur_scale = DEC_SCALE_MAX - scale;
        if (res_hi < power_overflow[cur_scale - 1].Hi)
            goto HaveScale;

        if (res_hi == power_overflow[cur_scale - 1].Hi)
        {
        UpperEq:
            if (res_mid > power_overflow[cur_scale - 1].Mid ||
                (res_mid == power_overflow[cur_scale - 1].Mid && res_lo > power_overflow[cur_scale - 1].Lo))
            {
                cur_scale--;
            }
            goto HaveScale;
        }
    }
    else if (res_hi < OVFL_MAX_9_HI || (res_hi == OVFL_MAX_9_HI && res_mid < OVFL_MAX_9_MID) || (res_hi == OVFL_MAX_9_HI && res_mid == OVFL_MAX_9_MID && res_lo <= OVFL_MAX_9_LO))
        return 9;

    // Search for a power to scale by < 9.  Do a binary search
    // on power_overflow[].
    //
    cur_scale = 5;
    if (res_hi < OVFL_MAX_5_HI)
        cur_scale = 7;
    else if (res_hi > OVFL_MAX_5_HI)
        cur_scale = 3;
    else
        goto UpperEq;

    // cur_scale is 3 or 7.
    //
    if (res_hi < power_overflow[cur_scale - 1].Hi)
        cur_scale++;
    else if (res_hi > power_overflow[cur_scale - 1].Hi)
        cur_scale--;
    else
        goto UpperEq;

    // cur_scale is 2, 4, 6, or 8.
    //
    // In all cases, we already found we could not use the power one larger.
    // So if we can use this power, it is the biggest, and we're done.  If
    // we can't use this power, the one below it is correct for all cases
    // unless it's 10^1 -- we might have to go to 10^0 (no scaling).
    //
    if (res_hi > power_overflow[cur_scale - 1].Hi)
        cur_scale--;

    if (res_hi == power_overflow[cur_scale - 1].Hi)
        goto UpperEq;

HaveScale:
    // cur_scale = largest power of 10 we can scale by without overflow,
    // cur_scale < 9.  See if this is enough to make scale factor
    // positive if it isn't already.
    //
    if (cur_scale + scale < 0)
        cur_scale = -1;

    return cur_scale;
}

/**
* Div96By32
*
* Entry:
*   rgulNum - Pointer to 96-bit dividend as array of uint32_ts, least-sig first
*   ulDen   - 32-bit divisor.
*
* Purpose:
*   Do full divide, yielding 96-bit result and 32-bit remainder.
*
* Exit:
*   Quotient overwrites dividend.
*   Returns remainder.
*
* Exceptions:
*   None.
*
*/
static uint32_t
Div96By32(uint32_t *num, uint32_t den)
{
    SPLIT64  tmp;

    tmp.u.Hi = 0;

    if (num[2] != 0)
        goto Div3Word;

    if (num[1] >= den)
        goto Div2Word;

    tmp.u.Hi = num[1];
    num[1] = 0;
    goto Div1Word;

Div3Word:
    tmp.u.Lo = num[2];
    tmp.int64 = DivMod64by32(tmp.int64, den);
    num[2] = tmp.u.Lo;
Div2Word:
    tmp.u.Lo = num[1];
    tmp.int64 = DivMod64by32(tmp.int64, den);
    num[1] = tmp.u.Lo;
Div1Word:
    tmp.u.Lo = num[0];
    tmp.int64 = DivMod64by32(tmp.int64, den);
    num[0] = tmp.u.Lo;
    return tmp.u.Hi;
}

/***
* DecFixInt
*
* Entry:
*   pdecRes - Pointer to Decimal result location
*   operand  - Pointer to Decimal operand
*
* Purpose:
*   Chop the value to integer.  Return remainder so Int() function
*   can round down if non-zero.
*
* Exit:
*   Returns remainder.
*
* Exceptions:
*   None.
*
***********************************************************************/

static uint32_t
DecFixInt(Il2CppDecimal * result, Il2CppDecimal * operand)
{
    uint32_t   num[3];
    uint32_t   rem;
    uint32_t   pwr;
    int     scale;

    if (operand->u.u.scale > 0)
    {
        num[0] = operand->v.v.Lo32;
        num[1] = operand->v.v.Mid32;
        num[2] = operand->Hi32;
        scale = operand->u.u.scale;
        result->u.u.sign = operand->u.u.sign;
        rem = 0;

        do
        {
            if (scale > POWER10_MAX)
                pwr = ten_to_nine;
            else
                pwr = power10[scale];

            rem |= Div96By32(num, pwr);
            scale -= 9;
        }
        while (scale > 0);

        result->v.v.Lo32 = num[0];
        result->v.v.Mid32 = num[1];
        result->Hi32 = num[2];
        result->u.u.scale = 0;

        return rem;
    }

    COPYDEC(*result, *operand);
    // Odd, the Microsoft code does not set result->reserved to zero on this case
    return 0;
}

/**
* ScaleResult:
*
* Entry:
*   res - Array of uint32_ts with value, least-significant first.
*   hi_res  - Index of last non-zero value in res.
*   scale  - Scale factor for this value, range 0 - 2 * DEC_SCALE_MAX
*
* Purpose:
*   See if we need to scale the result to fit it in 96 bits.
*   Perform needed scaling.  Adjust scale factor accordingly.
*
* Exit:
*   res updated in place, always 3 uint32_ts.
*   New scale factor returned, -1 if overflow error.
*
*/
static int
ScaleResult(uint32_t *res, int hi_res, int scale)
{
    int     new_scale;
    int     cur;
    uint32_t   pwr;
    uint32_t   tmp;
    uint32_t   sticky;
    SPLIT64 sdlTmp;

    // See if we need to scale the result.  The combined scale must
    // be <= DEC_SCALE_MAX and the upper 96 bits must be zero.
    //
    // Start by figuring a lower bound on the scaling needed to make
    // the upper 96 bits zero.  hi_res is the index into res[]
    // of the highest non-zero uint32_t.
    //
    new_scale = hi_res * 32 - 64 - 1;
    if (new_scale > 0)
    {
        // Find the MSB.
        //
        tmp = res[hi_res];
        if (!(tmp & 0xFFFF0000))
        {
            new_scale -= 16;
            tmp <<= 16;
        }
        if (!(tmp & 0xFF000000))
        {
            new_scale -= 8;
            tmp <<= 8;
        }
        if (!(tmp & 0xF0000000))
        {
            new_scale -= 4;
            tmp <<= 4;
        }
        if (!(tmp & 0xC0000000))
        {
            new_scale -= 2;
            tmp <<= 2;
        }
        if (!(tmp & 0x80000000))
        {
            new_scale--;
            tmp <<= 1;
        }

        // Multiply bit position by log10(2) to figure it's power of 10.
        // We scale the log by 256.  log(2) = .30103, * 256 = 77.  Doing this
        // with a multiply saves a 96-byte lookup table.  The power returned
        // is <= the power of the number, so we must add one power of 10
        // to make it's integer part zero after dividing by 256.
        //
        // Note: the result of this multiplication by an approximation of
        // log10(2) have been exhaustively checked to verify it gives the
        // correct result.  (There were only 95 to check...)
        //
        new_scale = ((new_scale * 77) >> 8) + 1;

        // new_scale = min scale factor to make high 96 bits zero, 0 - 29.
        // This reduces the scale factor of the result.  If it exceeds the
        // current scale of the result, we'll overflow.
        //
        if (new_scale > scale)
            return -1;
    }
    else
        new_scale = 0;

    // Make sure we scale by enough to bring the current scale factor
    // into valid range.
    //
    if (new_scale < scale - DEC_SCALE_MAX)
        new_scale = scale - DEC_SCALE_MAX;

    if (new_scale != 0)
    {
        // Scale by the power of 10 given by new_scale.  Note that this is
        // NOT guaranteed to bring the number within 96 bits -- it could
        // be 1 power of 10 short.
        //
        scale -= new_scale;
        sticky = 0;
        sdlTmp.u.Hi = 0; // initialize remainder

        for (;;)
        {
            sticky |= sdlTmp.u.Hi; // record remainder as sticky bit

            if (new_scale > POWER10_MAX)
                pwr = ten_to_nine;
            else
                pwr = power10[new_scale];

            // Compute first quotient.
            // DivMod64by32 returns quotient in Lo, remainder in Hi.
            //
            sdlTmp.int64 = DivMod64by32(res[hi_res], pwr);
            res[hi_res] = sdlTmp.u.Lo;
            cur = hi_res - 1;

            if (cur >= 0)
            {
                // If first quotient was 0, update hi_res.
                //
                if (sdlTmp.u.Lo == 0)
                    hi_res--;

                // Compute subsequent quotients.
                //
                do
                {
                    sdlTmp.u.Lo = res[cur];
                    sdlTmp.int64 = DivMod64by32(sdlTmp.int64, pwr);
                    res[cur] = sdlTmp.u.Lo;
                    cur--;
                }
                while (cur >= 0);
            }

            new_scale -= POWER10_MAX;
            if (new_scale > 0)
                continue; // scale some more

            // If we scaled enough, hi_res would be 2 or less.  If not,
            // divide by 10 more.
            //
            if (hi_res > 2)
            {
                new_scale = 1;
                scale--;
                continue; // scale by 10
            }

            // Round final result.  See if remainder >= 1/2 of divisor.
            // If remainder == 1/2 divisor, round up if odd or sticky bit set.
            //
            pwr >>= 1;  // power of 10 always even
            if (pwr <= sdlTmp.u.Hi && (pwr < sdlTmp.u.Hi ||
                                       ((res[0] & 1) | sticky)))
            {
                cur = -1;
                while (++res[++cur] == 0)
                    ;

                if (cur > 2)
                {
                    // The rounding caused us to carry beyond 96 bits.
                    // Scale by 10 more.
                    //
                    hi_res = cur;
                    sticky = 0;  // no sticky bit
                    sdlTmp.u.Hi = 0; // or remainder
                    new_scale = 1;
                    scale--;
                    continue; // scale by 10
                }
            }

            // We may have scaled it more than we planned.  Make sure the scale
            // factor hasn't gone negative, indicating overflow.
            //
            if (scale < 0)
                return -1;

            return scale;
        } // for(;;)
    }
    return scale;
}

// Returns: IL2CPP_DECIMAL_OK, or IL2CPP_DECIMAL_INVALID_ARGUMENT
static Il2CppDecimalStatus
il2cpp_decimal_to_double_result(Il2CppDecimal *input, double *result)
{
    SPLIT64  tmp;
    double   dbl;

    if (DECIMAL_SCALE(*input) > DECMAX || (DECIMAL_SIGN(*input) & ~DECIMAL_NEG) != 0)
        return IL2CPP_DECIMAL_INVALID_ARGUMENT;

    tmp.u.Lo = DECIMAL_LO32(*input);
    tmp.u.Hi = DECIMAL_MID32(*input);

    if ((int32_t)DECIMAL_MID32(*input) < 0)
        dbl = (ds2to64.d + (double)(int64_t)tmp.int64 +
            (double)DECIMAL_HI32(*input) * ds2to64.d) / fnDblPower10(DECIMAL_SCALE(*input));
    else
        dbl = ((double)(int64_t)tmp.int64 +
            (double)DECIMAL_HI32(*input) * ds2to64.d) / fnDblPower10(DECIMAL_SCALE(*input));

    if (DECIMAL_SIGN(*input))
        dbl = -dbl;

    *result = dbl;
    return IL2CPP_DECIMAL_OK;
}

// Returns: IL2CPP_DECIMAL_OK, or IL2CPP_DECIMAL_INVALID_ARGUMENT
static Il2CppDecimalStatus
il2cpp_decimal_to_float_result(Il2CppDecimal *input, float *result)
{
    double   dbl;

    if (DECIMAL_SCALE(*input) > DECMAX || (DECIMAL_SIGN(*input) & ~DECIMAL_NEG) != 0)
        return IL2CPP_DECIMAL_INVALID_ARGUMENT;

    // Can't overflow; no errors possible.
    //
    il2cpp_decimal_to_double_result(input, &dbl);
    *result = (float)dbl;
    return IL2CPP_DECIMAL_OK;
}

static Il2CppDecimalStatus
DecAddSub(Il2CppDecimal *left, Il2CppDecimal *right, Il2CppDecimal *result, int8_t sign)
{
    uint32_t     num[6];
    uint32_t     pwr;
    int       scale;
    int       hi_prod;
    int       cur;
    SPLIT64   tmp;
    Il2CppDecimal decRes;
    Il2CppDecimal decTmp;
    Il2CppDecimal *pdecTmp;

    sign ^= (right->u.u.sign ^ left->u.u.sign) & DECIMAL_NEG;

    if (right->u.u.scale == left->u.u.scale)
    {
        // Scale factors are equal, no alignment necessary.
        //
        decRes.u.signscale = left->u.signscale;

    AlignedAdd:
        if (sign)
        {
            // Signs differ - subtract
            //
            DECIMAL_LO64_SET(decRes, DECIMAL_LO64_GET(*left) - DECIMAL_LO64_GET(*right));
            DECIMAL_HI32(decRes) = DECIMAL_HI32(*left) - DECIMAL_HI32(*right);

            // Propagate carry
            //
            if (DECIMAL_LO64_GET(decRes) > DECIMAL_LO64_GET(*left))
            {
                decRes.Hi32--;
                if (decRes.Hi32 >= left->Hi32)
                    goto SignFlip;
            }
            else if (decRes.Hi32 > left->Hi32)
            {
                // Got negative result.  Flip its sign.
                //
            SignFlip:
                DECIMAL_LO64_SET(decRes, (uint64_t)DECIMAL_LO64_GET(decRes));
                decRes.Hi32 = ~decRes.Hi32;
                if (DECIMAL_LO64_GET(decRes) == 0)
                    decRes.Hi32++;
                decRes.u.u.sign ^= DECIMAL_NEG;
            }
        }
        else
        {
            // Signs are the same - add
            //
            DECIMAL_LO64_SET(decRes, DECIMAL_LO64_GET(*left) + DECIMAL_LO64_GET(*right));
            decRes.Hi32 = left->Hi32 + right->Hi32;

            // Propagate carry
            //
            if (DECIMAL_LO64_GET(decRes) < DECIMAL_LO64_GET(*left))
            {
                decRes.Hi32++;
                if (decRes.Hi32 <= left->Hi32)
                    goto AlignedScale;
            }
            else if (decRes.Hi32 < left->Hi32)
            {
            AlignedScale:
                // The addition carried above 96 bits.  Divide the result by 10,
                // dropping the scale factor.
                //
                if (decRes.u.u.scale == 0)
                    return IL2CPP_DECIMAL_OVERFLOW;
                decRes.u.u.scale--;

                tmp.u.Lo = decRes.Hi32;
                tmp.u.Hi = 1;
                tmp.int64 = DivMod64by32(tmp.int64, 10);
                decRes.Hi32 = tmp.u.Lo;

                tmp.u.Lo = decRes.v.v.Mid32;
                tmp.int64 = DivMod64by32(tmp.int64, 10);
                decRes.v.v.Mid32 = tmp.u.Lo;

                tmp.u.Lo = decRes.v.v.Lo32;
                tmp.int64 = DivMod64by32(tmp.int64, 10);
                decRes.v.v.Lo32 = tmp.u.Lo;

                // See if we need to round up.
                //
                if (tmp.u.Hi >= 5 && (tmp.u.Hi > 5 || (decRes.v.v.Lo32 & 1)))
                {
                    DECIMAL_LO64_SET(decRes, DECIMAL_LO64_GET(decRes) + 1)
                    if (DECIMAL_LO64_GET(decRes) == 0)
                        decRes.Hi32++;
                }
            }
        }
    }
    else
    {
        // Scale factors are not equal.  Assume that a larger scale
        // factor (more decimal places) is likely to mean that number
        // is smaller.  Start by guessing that the right operand has
        // the larger scale factor.  The result will have the larger
        // scale factor.
        //
        decRes.u.u.scale = right->u.u.scale;  // scale factor of "smaller"
        decRes.u.u.sign = left->u.u.sign;    // but sign of "larger"
        scale = decRes.u.u.scale - left->u.u.scale;

        if (scale < 0)
        {
            // Guessed scale factor wrong. Swap operands.
            //
            scale = -scale;
            decRes.u.u.scale = left->u.u.scale;
            decRes.u.u.sign ^= sign;
            pdecTmp = right;
            right = left;
            left = pdecTmp;
        }

        // *left will need to be multiplied by 10^scale so
        // it will have the same scale as *right.  We could be
        // extending it to up to 192 bits of precision.
        //
        if (scale <= POWER10_MAX)
        {
            // Scaling won't make it larger than 4 uint32_ts
            //
            pwr = power10[scale];
            DECIMAL_LO64_SET(decTmp, UInt32x32To64(left->v.v.Lo32, pwr));
            tmp.int64 = UInt32x32To64(left->v.v.Mid32, pwr);
            tmp.int64 += decTmp.v.v.Mid32;
            decTmp.v.v.Mid32 = tmp.u.Lo;
            decTmp.Hi32 = tmp.u.Hi;
            tmp.int64 = UInt32x32To64(left->Hi32, pwr);
            tmp.int64 += decTmp.Hi32;
            if (tmp.u.Hi == 0)
            {
                // Result fits in 96 bits.  Use standard aligned add.
                //
                decTmp.Hi32 = tmp.u.Lo;
                left = &decTmp;
                goto AlignedAdd;
            }
            num[0] = decTmp.v.v.Lo32;
            num[1] = decTmp.v.v.Mid32;
            num[2] = tmp.u.Lo;
            num[3] = tmp.u.Hi;
            hi_prod = 3;
        }
        else
        {
            // Have to scale by a bunch.  Move the number to a buffer
            // where it has room to grow as it's scaled.
            //
            num[0] = left->v.v.Lo32;
            num[1] = left->v.v.Mid32;
            num[2] = left->Hi32;
            hi_prod = 2;

            // Scan for zeros in the upper words.
            //
            if (num[2] == 0)
            {
                hi_prod = 1;
                if (num[1] == 0)
                {
                    hi_prod = 0;
                    if (num[0] == 0)
                    {
                        // Left arg is zero, return right.
                        //
                        DECIMAL_LO64_SET(decRes, DECIMAL_LO64_GET(*right));
                        decRes.Hi32 = right->Hi32;
                        decRes.u.u.sign ^= sign;
                        goto RetDec;
                    }
                }
            }

            // Scaling loop, up to 10^9 at a time.  hi_prod stays updated
            // with index of highest non-zero uint32_t.
            //
            for (; scale > 0; scale -= POWER10_MAX)
            {
                if (scale > POWER10_MAX)
                    pwr = ten_to_nine;
                else
                    pwr = power10[scale];

                tmp.u.Hi = 0;
                for (cur = 0; cur <= hi_prod; cur++)
                {
                    tmp.int64 = UInt32x32To64(num[cur], pwr) + tmp.u.Hi;
                    num[cur] = tmp.u.Lo;
                }

                if (tmp.u.Hi != 0)
                    // We're extending the result by another uint32_t.
                    num[++hi_prod] = tmp.u.Hi;
            }
        }

        // Scaling complete, do the add.  Could be subtract if signs differ.
        //
        tmp.u.Lo = num[0];
        tmp.u.Hi = num[1];

        if (sign)
        {
            // Signs differ, subtract.
            //
            DECIMAL_LO64_SET(decRes, tmp.int64 - DECIMAL_LO64_GET(*right));
            decRes.Hi32 = num[2] - right->Hi32;

            // Propagate carry
            //
            if (DECIMAL_LO64_GET(decRes) > tmp.int64)
            {
                decRes.Hi32--;
                if (decRes.Hi32 >= num[2])
                    goto LongSub;
            }
            else if (decRes.Hi32 > num[2])
            {
            LongSub:
                // If num has more than 96 bits of precision, then we need to
                // carry the subtraction into the higher bits.  If it doesn't,
                // then we subtracted in the wrong order and have to flip the
                // sign of the result.
                //
                if (hi_prod <= 2)
                    goto SignFlip;

                cur = 3;
                while (num[cur++]-- == 0)
                    ;
                if (num[hi_prod] == 0)
                    hi_prod--;
            }
        }
        else
        {
            // Signs the same, add.
            //
            DECIMAL_LO64_SET(decRes, tmp.int64 + DECIMAL_LO64_GET(*right));
            decRes.Hi32 = num[2] + right->Hi32;

            // Propagate carry
            //
            if (DECIMAL_LO64_GET(decRes) < tmp.int64)
            {
                decRes.Hi32++;
                if (decRes.Hi32 <= num[2])
                    goto LongAdd;
            }
            else if (decRes.Hi32 < num[2])
            {
            LongAdd:
                // Had a carry above 96 bits.
                //
                cur = 3;
                do
                {
                    if (hi_prod < cur)
                    {
                        num[cur] = 1;
                        hi_prod = cur;
                        break;
                    }
                }
                while (++num[cur++] == 0);
            }
        }

        if (hi_prod > 2)
        {
            num[0] = decRes.v.v.Lo32;
            num[1] = decRes.v.v.Mid32;
            num[2] = decRes.Hi32;
            decRes.u.u.scale = ScaleResult(num, hi_prod, decRes.u.u.scale);
            if (decRes.u.u.scale == (uint8_t)-1)
                return IL2CPP_DECIMAL_OVERFLOW;

            decRes.v.v.Lo32 = num[0];
            decRes.v.v.Mid32 = num[1];
            decRes.Hi32 = num[2];
        }
    }

RetDec:
    COPYDEC(*result, decRes);
    // Odd, the Microsoft code does not set result->reserved to zero on this case
    return IL2CPP_DECIMAL_OK;
}

// Returns IL2CPP_DECIMAL_OK or IL2CPP_DECIMAL_OVERFLOW
static Il2CppDecimalStatus
il2cpp_decimal_from_double(double input_d, Il2CppDecimal *result)
{
    int         exp;    // number of bits to left of binary point
    int         power;  // power-of-10 scale factor
    SPLIT64     sdlMant;
    SPLIT64     sdlLo;
    double      dbl;
    int         lmax, cur;  // temps used during scale reduction
    uint32_t       pwr_cur;
    uint32_t       quo;
    Il2CppDouble_double input;
    input.d = input_d;

    // The most we can scale by is 10^28, which is just slightly more
    // than 2^93.  So a float with an exponent of -94 could just
    // barely reach 0.5, but smaller exponents will always round to zero.
    //
    if ((exp = input.s.exp - IL2CPP_DOUBLE_BIAS) < -94)
    {
        DECIMAL_SETZERO(*result);
        return IL2CPP_DECIMAL_OK;
    }

    if (exp > 96)
        return IL2CPP_DECIMAL_OVERFLOW;

    // Round the input to a 15-digit integer.  The R8 format has
    // only 15 digits of precision, and we want to keep garbage digits
    // out of the Decimal were making.
    //
    // Calculate max power of 10 input value could have by multiplying
    // the exponent by log10(2).  Using scaled integer multiplcation,
    // log10(2) * 2 ^ 16 = .30103 * 65536 = 19728.3.
    //
    dbl = fabs(input.d);
    power = 14 - ((exp * 19728) >> 16);

    if (power >= 0)
    {
        // We have less than 15 digits, scale input up.
        //
        if (power > DECMAX)
            power = DECMAX;

        dbl = dbl * double_power10[power];
    }
    else
    {
        if (power != -1 || dbl >= 1E15)
            dbl = dbl / fnDblPower10(-power);
        else
            power = 0; // didn't scale it
    }

    IL2CPP_ASSERT(dbl < 1E15);
    if (dbl < 1E14 && power < DECMAX)
    {
        dbl *= 10;
        power++;
        IL2CPP_ASSERT(dbl >= 1E14);
    }

    // Round to int64
    //
    sdlMant.int64 = (int64_t)dbl;
    dbl -= (double)(int64_t)sdlMant.int64;  // dif between input & integer
    if (dbl > 0.5 || (dbl == 0.5 && (sdlMant.u.Lo & 1)))
        sdlMant.int64++;

    if (sdlMant.int64 == 0)
    {
        DECIMAL_SETZERO(*result);
        return IL2CPP_DECIMAL_OK;
    }

    if (power < 0)
    {
        // Add -power factors of 10, -power <= (29 - 15) = 14.
        //
        power = -power;
        if (power < 10)
        {
            sdlLo.int64 = UInt32x32To64(sdlMant.u.Lo, (uint32_t)long_power10[power]);
            sdlMant.int64 = UInt32x32To64(sdlMant.u.Hi, (uint32_t)long_power10[power]);
            sdlMant.int64 += sdlLo.u.Hi;
            sdlLo.u.Hi = sdlMant.u.Lo;
            sdlMant.u.Lo = sdlMant.u.Hi;
        }
        else
        {
            // Have a big power of 10.
            //
            IL2CPP_ASSERT(power <= 14);
            sdlLo.int64 = UInt64x64To128(sdlMant, sdl_power10[power - 10], &sdlMant.int64);

            if (sdlMant.u.Hi != 0)
                return IL2CPP_DECIMAL_OVERFLOW;
        }
        DECIMAL_LO32(*result) = sdlLo.u.Lo;
        DECIMAL_MID32(*result) = sdlLo.u.Hi;
        DECIMAL_HI32(*result) = sdlMant.u.Lo;
        DECIMAL_SCALE(*result) = 0;
    }
    else
    {
        // Factor out powers of 10 to reduce the scale, if possible.
        // The maximum number we could factor out would be 14.  This
        // comes from the fact we have a 15-digit number, and the
        // MSD must be non-zero -- but the lower 14 digits could be
        // zero.  Note also the scale factor is never negative, so
        // we can't scale by any more than the power we used to
        // get the integer.
        //
        // DivMod64by32 returns the quotient in Lo, the remainder in Hi.
        //
        lmax = std::min(power, 14);

        // lmax is the largest power of 10 to try, lmax <= 14.
        // We'll try powers 8, 4, 2, and 1 unless they're too big.
        //
        for (cur = 8; cur > 0; cur >>= 1)
        {
            if (cur > lmax)
                continue;

            pwr_cur = (uint32_t)long_power10[cur];

            if (sdlMant.u.Hi >= pwr_cur)
            {
                // Overflow if we try to divide in one step.
                //
                sdlLo.int64 = DivMod64by32(sdlMant.u.Hi, pwr_cur);
                quo = sdlLo.u.Lo;
                sdlLo.u.Lo = sdlMant.u.Lo;
                sdlLo.int64 = DivMod64by32(sdlLo.int64, pwr_cur);
            }
            else
            {
                quo = 0;
                sdlLo.int64 = DivMod64by32(sdlMant.int64, pwr_cur);
            }

            if (sdlLo.u.Hi == 0)
            {
                sdlMant.u.Hi = quo;
                sdlMant.u.Lo = sdlLo.u.Lo;
                power -= cur;
                lmax -= cur;
            }
        }

        DECIMAL_HI32(*result) = 0;
        DECIMAL_SCALE(*result) = power;
        DECIMAL_LO32(*result) = sdlMant.u.Lo;
        DECIMAL_MID32(*result) = sdlMant.u.Hi;
    }

    DECIMAL_SIGN(*result) = (char)input.s.sign << 7;
    return IL2CPP_DECIMAL_OK;
}

// Decimal multiply
// Returns: IL2CPP_DECIMAL_OVERFLOW or IL2CPP_DECIMAL_OK
static Il2CppDecimalStatus il2cpp_decimal_multiply_result(Il2CppDecimal *left, Il2CppDecimal *right, Il2CppDecimal *result)
{
    SPLIT64 tmp;
    SPLIT64 tmp2;
    SPLIT64 tmp3;
    int     scale;
    int     hi_prod;
    uint32_t   pwr;
    uint32_t   rem_lo;
    uint32_t   rem_hi;
    uint32_t   prod[6];

    scale = left->u.u.scale + right->u.u.scale;

    if ((left->Hi32 | left->v.v.Mid32 | right->Hi32 | right->v.v.Mid32) == 0)
    {
        // Upper 64 bits are zero.
        //
        tmp.int64 = UInt32x32To64(left->v.v.Lo32, right->v.v.Lo32);
        if (scale > DEC_SCALE_MAX)
        {
            // Result scale is too big.  Divide result by power of 10 to reduce it.
            // If the amount to divide by is > 19 the result is guaranteed
            // less than 1/2.  [max value in 64 bits = 1.84E19]
            //
            scale -= DEC_SCALE_MAX;
            if (scale > 19)
            {
            ReturnZero:
                DECIMAL_SETZERO(*result);
                return IL2CPP_DECIMAL_OK;
            }

            if (scale > POWER10_MAX)
            {
                // Divide by 1E10 first, to get the power down to a 32-bit quantity.
                // 1E10 itself doesn't fit in 32 bits, so we'll divide by 2.5E9 now
                // then multiply the next divisor by 4 (which will be a max of 4E9).
                //
                rem_lo = FullDiv64By32(&tmp.int64, ten_to_ten_div_4);
                pwr = power10[scale - 10] << 2;
            }
            else
            {
                pwr = power10[scale];
                rem_lo = 0;
            }

            // Power to divide by fits in 32 bits.
            //
            rem_hi = FullDiv64By32(&tmp.int64, pwr);

            // Round result.  See if remainder >= 1/2 of divisor.
            // Divisor is a power of 10, so it is always even.
            //
            pwr >>= 1;
            if (rem_hi >= pwr && (rem_hi > pwr || (rem_lo | (tmp.u.Lo & 1))))
                tmp.int64++;

            scale = DEC_SCALE_MAX;
        }
        DECIMAL_LO32(*result) = tmp.u.Lo;
        DECIMAL_MID32(*result) = tmp.u.Hi;
        DECIMAL_HI32(*result) = 0;
    }
    else
    {
        // At least one operand has bits set in the upper 64 bits.
        //
        // Compute and accumulate the 9 partial products into a
        // 192-bit (24-byte) result.
        //
        //                [l-h][l-m][l-l]   left high, middle, low
        //             x  [r-h][r-m][r-l]   right high, middle, low
        // ------------------------------
        //
        //                     [0-h][0-l]   l-l * r-l
        //                [1ah][1al]        l-l * r-m
        //                [1bh][1bl]        l-m * r-l
        //           [2ah][2al]             l-m * r-m
        //           [2bh][2bl]             l-l * r-h
        //           [2ch][2cl]             l-h * r-l
        //      [3ah][3al]                  l-m * r-h
        //      [3bh][3bl]                  l-h * r-m
        // [4-h][4-l]                       l-h * r-h
        // ------------------------------
        // [p-5][p-4][p-3][p-2][p-1][p-0]   prod[] array
        //
        tmp.int64 = UInt32x32To64(left->v.v.Lo32, right->v.v.Lo32);
        prod[0] = tmp.u.Lo;

        tmp2.int64 = UInt32x32To64(left->v.v.Lo32, right->v.v.Mid32) + tmp.u.Hi;

        tmp.int64 = UInt32x32To64(left->v.v.Mid32, right->v.v.Lo32);
        tmp.int64 += tmp2.int64; // this could generate carry
        prod[1] = tmp.u.Lo;
        if (tmp.int64 < tmp2.int64) // detect carry
            tmp2.u.Hi = 1;
        else
            tmp2.u.Hi = 0;
        tmp2.u.Lo = tmp.u.Hi;

        tmp.int64 = UInt32x32To64(left->v.v.Mid32, right->v.v.Mid32) + tmp2.int64;

        if (left->Hi32 | right->Hi32)
        {
            // Highest 32 bits is non-zero.  Calculate 5 more partial products.
            //
            tmp2.int64 = UInt32x32To64(left->v.v.Lo32, right->Hi32);
            tmp.int64 += tmp2.int64; // this could generate carry
            if (tmp.int64 < tmp2.int64) // detect carry
                tmp3.u.Hi = 1;
            else
                tmp3.u.Hi = 0;

            tmp2.int64 = UInt32x32To64(left->Hi32, right->v.v.Lo32);
            tmp.int64 += tmp2.int64; // this could generate carry
            prod[2] = tmp.u.Lo;
            if (tmp.int64 < tmp2.int64) // detect carry
                tmp3.u.Hi++;
            tmp3.u.Lo = tmp.u.Hi;

            tmp.int64 = UInt32x32To64(left->v.v.Mid32, right->Hi32);
            tmp.int64 += tmp3.int64; // this could generate carry
            if (tmp.int64 < tmp3.int64) // detect carry
                tmp3.u.Hi = 1;
            else
                tmp3.u.Hi = 0;

            tmp2.int64 = UInt32x32To64(left->Hi32, right->v.v.Mid32);
            tmp.int64 += tmp2.int64; // this could generate carry
            prod[3] = tmp.u.Lo;
            if (tmp.int64 < tmp2.int64) // detect carry
                tmp3.u.Hi++;
            tmp3.u.Lo = tmp.u.Hi;

            tmp.int64 = UInt32x32To64(left->Hi32, right->Hi32) + tmp3.int64;
            prod[4] = tmp.u.Lo;
            prod[5] = tmp.u.Hi;

            hi_prod = 5;
        }
        else
        {
            prod[2] = tmp.u.Lo;
            prod[3] = tmp.u.Hi;
            hi_prod = 3;
        }

        // Check for leading zero uint32_ts on the product
        //
        while (prod[hi_prod] == 0)
        {
            hi_prod--;
            if (hi_prod < 0)
                goto ReturnZero;
        }

        scale = ScaleResult(prod, hi_prod, scale);
        if (scale == -1)
            return IL2CPP_DECIMAL_OVERFLOW;

        result->v.v.Lo32 = prod[0];
        result->v.v.Mid32 = prod[1];
        result->Hi32 = prod[2];
    }

    result->u.u.sign = right->u.u.sign ^ left->u.u.sign;
    result->u.u.scale = (char)scale;
    return IL2CPP_DECIMAL_OK;
}

// Add a 32 bit unsigned long to an array of 3 unsigned longs representing a 96 integer
// Returns FALSE if there is an overflow
static bool
Add32To96(uint32_t *num, uint32_t value)
{
    num[0] += value;
    if (num[0] < value)
    {
        if (++num[1] == 0)
        {
            if (++num[2] == 0)
            {
                return false;
            }
        }
    }
    return true;
}

static void
OverflowUnscale(uint32_t *quo, bool remainder)
{
    SPLIT64  sdlTmp;

    // We have overflown, so load the high bit with a one.
    sdlTmp.u.Hi = 1u;
    sdlTmp.u.Lo = quo[2];
    sdlTmp.int64 = DivMod64by32(sdlTmp.int64, 10u);
    quo[2] = sdlTmp.u.Lo;
    sdlTmp.u.Lo = quo[1];
    sdlTmp.int64 = DivMod64by32(sdlTmp.int64, 10u);
    quo[1] = sdlTmp.u.Lo;
    sdlTmp.u.Lo = quo[0];
    sdlTmp.int64 = DivMod64by32(sdlTmp.int64, 10u);
    quo[0] = sdlTmp.u.Lo;
    // The remainder is the last digit that does not fit, so we can use it to work out if we need to round up
    if ((sdlTmp.u.Hi > 5) || ((sdlTmp.u.Hi == 5) && (remainder || (quo[0] & 1))))
    {
        Add32To96(quo, 1u);
    }
}

/**
* IncreaseScale:
*
* Entry:
*   num - Pointer to 96-bit number as array of uint32_ts, least-sig first
*   pwr   - Scale factor to multiply by
*
* Purpose:
*   Multiply the two numbers.  The low 96 bits of the result overwrite
*   the input.  The last 32 bits of the product are the return value.
*
* Exit:
*   Returns highest 32 bits of product.
*
* Exceptions:
*   None.
*
*/
static uint32_t
IncreaseScale(uint32_t *num, uint32_t pwr)
{
    SPLIT64   sdlTmp;

    sdlTmp.int64 = UInt32x32To64(num[0], pwr);
    num[0] = sdlTmp.u.Lo;
    sdlTmp.int64 = UInt32x32To64(num[1], pwr) + sdlTmp.u.Hi;
    num[1] = sdlTmp.u.Lo;
    sdlTmp.int64 = UInt32x32To64(num[2], pwr) + sdlTmp.u.Hi;
    num[2] = sdlTmp.u.Lo;
    return sdlTmp.u.Hi;
}

/***
* Div128By96
*
* Entry:
*   rgulNum - Pointer to 128-bit dividend as array of uint32_ts, least-sig first
*   den - Pointer to 96-bit divisor.
*
* Purpose:
*   Do partial divide, yielding 32-bit result and 96-bit remainder.
*   Top divisor uint32_t must be larger than top dividend uint32_t.  This is
*   assured in the initial call because the divisor is normalized
*   and the dividend can't be.  In subsequent calls, the remainder
*   is multiplied by 10^9 (max), so it can be no more than 1/4 of
*   the divisor which is effectively multiplied by 2^32 (4 * 10^9).
*
* Exit:
*   Remainder overwrites lower 96-bits of dividend.
*   Returns quotient.
*
* Exceptions:
*   None.
*
***********************************************************************/

static uint32_t
Div128By96(uint32_t *num, uint32_t *den)
{
    SPLIT64 sdlQuo;
    SPLIT64 sdlNum;
    SPLIT64 sdlProd1;
    SPLIT64 sdlProd2;

    sdlNum.u.Lo = num[0];
    sdlNum.u.Hi = num[1];

    if (num[3] == 0 && num[2] < den[2])
    {
        // Result is zero.  Entire dividend is remainder.
        //
        return 0;
    }

    // DivMod64by32 returns quotient in Lo, remainder in Hi.
    //
    sdlQuo.u.Lo = num[2];
    sdlQuo.u.Hi = num[3];
    sdlQuo.int64 = DivMod64by32(sdlQuo.int64, den[2]);

    // Compute full remainder, rem = dividend - (quo * divisor).
    //
    sdlProd1.int64 = UInt32x32To64(sdlQuo.u.Lo, den[0]); // quo * lo divisor
    sdlProd2.int64 = UInt32x32To64(sdlQuo.u.Lo, den[1]); // quo * mid divisor
    sdlProd2.int64 += sdlProd1.u.Hi;
    sdlProd1.u.Hi = sdlProd2.u.Lo;

    sdlNum.int64 -= sdlProd1.int64;
    num[2] = sdlQuo.u.Hi - sdlProd2.u.Hi; // sdlQuo.Hi is remainder

    // Propagate carries
    //
    if (sdlNum.int64 > ~sdlProd1.int64)
    {
        num[2]--;
        if (num[2] >= ~sdlProd2.u.Hi)
            goto NegRem;
    }
    else if (num[2] > ~sdlProd2.u.Hi)
    {
    NegRem:
        // Remainder went negative.  Add divisor back in until it's positive,
        // a max of 2 times.
        //
        sdlProd1.u.Lo = den[0];
        sdlProd1.u.Hi = den[1];

        for (;;)
        {
            sdlQuo.u.Lo--;
            sdlNum.int64 += sdlProd1.int64;
            num[2] += den[2];

            if (sdlNum.int64 < sdlProd1.int64)
            {
                // Detected carry. Check for carry out of top
                // before adding it in.
                //
                if (num[2]++ < den[2])
                    break;
            }
            if (num[2] < den[2])
                break; // detected carry
        }
    }

    num[0] = sdlNum.u.Lo;
    num[1] = sdlNum.u.Hi;
    return sdlQuo.u.Lo;
}

/**
* Div96By64:
*
* Entry:
*   rgulNum - Pointer to 96-bit dividend as array of uint32_ts, least-sig first
*   sdlDen  - 64-bit divisor.
*
* Purpose:
*   Do partial divide, yielding 32-bit result and 64-bit remainder.
*   Divisor must be larger than upper 64 bits of dividend.
*
* Exit:
*   Remainder overwrites lower 64-bits of dividend.
*   Returns quotient.
*
* Exceptions:
*   None.
*
*/
static uint32_t
Div96By64(uint32_t *num, SPLIT64 den)
{
    SPLIT64 quo;
    SPLIT64 sdlNum;
    SPLIT64 prod;

    sdlNum.u.Lo = num[0];

    if (num[2] >= den.u.Hi)
    {
        // Divide would overflow.  Assume a quotient of 2^32, and set
        // up remainder accordingly.  Then jump to loop which reduces
        // the quotient.
        //
        sdlNum.u.Hi = num[1] - den.u.Lo;
        quo.u.Lo = 0;
        goto NegRem;
    }

    // Hardware divide won't overflow
    //
    if (num[2] == 0 && num[1] < den.u.Hi)
        // Result is zero.  Entire dividend is remainder.
        //
        return 0;

    // DivMod64by32 returns quotient in Lo, remainder in Hi.
    //
    quo.u.Lo = num[1];
    quo.u.Hi = num[2];
    quo.int64 = DivMod64by32(quo.int64, den.u.Hi);
    sdlNum.u.Hi = quo.u.Hi; // remainder

    // Compute full remainder, rem = dividend - (quo * divisor).
    //
    prod.int64 = UInt32x32To64(quo.u.Lo, den.u.Lo); // quo * lo divisor
    sdlNum.int64 -= prod.int64;

    if (sdlNum.int64 > ~prod.int64)
    {
    NegRem:
        // Remainder went negative.  Add divisor back in until it's positive,
        // a max of 2 times.
        //
        do
        {
            quo.u.Lo--;
            sdlNum.int64 += den.int64;
        }
        while (sdlNum.int64 >= den.int64);
    }

    num[0] = sdlNum.u.Lo;
    num[1] = sdlNum.u.Hi;
    return quo.u.Lo;
}

//
// Returns: IL2CPP_DECIMAL_INVALID_ARGUMENT, IL2CPP_DECIMAL_OK
//
static Il2CppDecimalStatus
il2cpp_decimal_round_result(Il2CppDecimal *input, int cDecimals, Il2CppDecimal *result)
{
    uint32_t num[3];
    uint32_t rem;
    uint32_t sticky;
    uint32_t pwr;
    int scale;

    if (cDecimals < 0)
        return IL2CPP_DECIMAL_INVALID_ARGUMENT;

    scale = input->u.u.scale - cDecimals;
    if (scale > 0)
    {
        num[0] = input->v.v.Lo32;
        num[1] = input->v.v.Mid32;
        num[2] = input->Hi32;
        result->u.u.sign = input->u.u.sign;
        rem = sticky = 0;

        do
        {
            sticky |= rem;
            if (scale > POWER10_MAX)
                pwr = ten_to_nine;
            else
                pwr = power10[scale];

            rem = Div96By32(num, pwr);
            scale -= 9;
        }
        while (scale > 0);

        // Now round.  rem has last remainder, sticky has sticky bits.
        // To do IEEE rounding, we add LSB of result to sticky bits so
        // either causes round up if remainder * 2 == last divisor.
        //
        sticky |= num[0] & 1;
        rem = (rem << 1) + (sticky != 0);
        if (pwr < rem &&
            ++num[0] == 0 &&
            ++num[1] == 0
        )
            ++num[2];

        result->v.v.Lo32 = num[0];
        result->v.v.Mid32 = num[1];
        result->Hi32 = num[2];
        result->u.u.scale = cDecimals;
        return IL2CPP_DECIMAL_OK;
    }

    COPYDEC(*result, *input);
    // Odd, the Microsoft source does not set the result->reserved to zero here.
    return IL2CPP_DECIMAL_OK;
}

//
// Returns IL2CPP_DECIMAL_OK or IL2CPP_DECIMAL_OVERFLOW
static Il2CppDecimalStatus
il2cpp_decimal_from_float(float input_f, Il2CppDecimal* result)
{
    int         exp;    // number of bits to left of binary point
    int         power;
    uint32_t       mant;
    double      dbl;
    SPLIT64     sdlLo;
    SPLIT64     sdlHi;
    int         lmax, cur;  // temps used during scale reduction
    Il2CppSingle_float input;
    input.f = input_f;

    // The most we can scale by is 10^28, which is just slightly more
    // than 2^93.  So a float with an exponent of -94 could just
    // barely reach 0.5, but smaller exponents will always round to zero.
    //
    if ((exp = input.s.exp - IL2CPP_SINGLE_BIAS) < -94)
    {
        DECIMAL_SETZERO(*result);
        return IL2CPP_DECIMAL_OK;
    }

    if (exp > 96)
        return IL2CPP_DECIMAL_OVERFLOW;

    // Round the input to a 7-digit integer.  The R4 format has
    // only 7 digits of precision, and we want to keep garbage digits
    // out of the Decimal were making.
    //
    // Calculate max power of 10 input value could have by multiplying
    // the exponent by log10(2).  Using scaled integer multiplcation,
    // log10(2) * 2 ^ 16 = .30103 * 65536 = 19728.3.
    //
    dbl = fabs(input.f);
    power = 6 - ((exp * 19728) >> 16);

    if (power >= 0)
    {
        // We have less than 7 digits, scale input up.
        //
        if (power > DECMAX)
            power = DECMAX;

        dbl = dbl * double_power10[power];
    }
    else
    {
        if (power != -1 || dbl >= 1E7)
            dbl = dbl / fnDblPower10(-power);
        else
            power = 0; // didn't scale it
    }

    IL2CPP_ASSERT(dbl < 1E7);
    if (dbl < 1E6 && power < DECMAX)
    {
        dbl *= 10;
        power++;
        IL2CPP_ASSERT(dbl >= 1E6);
    }

    // Round to integer
    //
    mant = (int32_t)dbl;
    dbl -= (double)mant;  // difference between input & integer
    if (dbl > 0.5 || (dbl == 0.5 && (mant & 1)))
        mant++;

    if (mant == 0)
    {
        DECIMAL_SETZERO(*result);
        return IL2CPP_DECIMAL_OK;
    }

    if (power < 0)
    {
        // Add -power factors of 10, -power <= (29 - 7) = 22.
        //
        power = -power;
        if (power < 10)
        {
            sdlLo.int64 = UInt32x32To64(mant, (uint32_t)long_power10[power]);

            DECIMAL_LO32(*result) = sdlLo.u.Lo;
            DECIMAL_MID32(*result) = sdlLo.u.Hi;
            DECIMAL_HI32(*result) = 0;
        }
        else
        {
            // Have a big power of 10.
            //
            if (power > 18)
            {
                sdlLo.int64 = UInt32x32To64(mant, (uint32_t)long_power10[power - 18]);
                sdlLo.int64 = UInt64x64To128(sdlLo, ten_to_eighteen, &sdlHi.int64);

                if (sdlHi.u.Hi != 0)
                    return IL2CPP_DECIMAL_OVERFLOW;
            }
            else
            {
                sdlLo.int64 = UInt32x32To64(mant, (uint32_t)long_power10[power - 9]);
                sdlHi.int64 = UInt32x32To64(ten_to_nine, sdlLo.u.Hi);
                sdlLo.int64 = UInt32x32To64(ten_to_nine, sdlLo.u.Lo);
                sdlHi.int64 += sdlLo.u.Hi;
                sdlLo.u.Hi = sdlHi.u.Lo;
                sdlHi.u.Lo = sdlHi.u.Hi;
            }
            DECIMAL_LO32(*result) = sdlLo.u.Lo;
            DECIMAL_MID32(*result) = sdlLo.u.Hi;
            DECIMAL_HI32(*result) = sdlHi.u.Lo;
        }
        DECIMAL_SCALE(*result) = 0;
    }
    else
    {
        // Factor out powers of 10 to reduce the scale, if possible.
        // The maximum number we could factor out would be 6.  This
        // comes from the fact we have a 7-digit number, and the
        // MSD must be non-zero -- but the lower 6 digits could be
        // zero.  Note also the scale factor is never negative, so
        // we can't scale by any more than the power we used to
        // get the integer.
        //
        // DivMod32by32 returns the quotient in Lo, the remainder in Hi.
        //
        lmax = std::min(power, 6);

        // lmax is the largest power of 10 to try, lmax <= 6.
        // We'll try powers 4, 2, and 1 unless they're too big.
        //
        for (cur = 4; cur > 0; cur >>= 1)
        {
            if (cur > lmax)
                continue;

            sdlLo.int64 = DivMod32by32(mant, (uint32_t)long_power10[cur]);

            if (sdlLo.u.Hi == 0)
            {
                mant = sdlLo.u.Lo;
                power -= cur;
                lmax -= cur;
            }
        }
        DECIMAL_LO32(*result) = mant;
        DECIMAL_MID32(*result) = 0;
        DECIMAL_HI32(*result) = 0;
        DECIMAL_SCALE(*result) = power;
    }

    DECIMAL_SIGN(*result) = (char)input.s.sign << 7;
    return IL2CPP_DECIMAL_OK;
}

// il2cpp_decimal_round_to_int - Decimal Int (round down to integer)
static void
il2cpp_decimal_round_to_int(Il2CppDecimal *pdecOprd, Il2CppDecimal *result)
{
    if (DecFixInt(result, pdecOprd) != 0 && (result->u.u.sign & DECIMAL_NEG))
    {
        // We have chopped off a non-zero amount from a negative value.  Since
        // we round toward -infinity, we must increase the integer result by
        // 1 to make it more negative.  This will never overflow because
        // in order to have a remainder, we must have had a non-zero scale factor.
        // Our scale factor is back to zero now.
        //
        DECIMAL_LO64_SET(*result, DECIMAL_LO64_GET(*result) + 1);
        if (DECIMAL_LO64_GET(*result) == 0)
            result->Hi32++;
    }
}

static void
il2cpp_decimal_fix(Il2CppDecimal *pdecOprd, Il2CppDecimal *result)
{
    DecFixInt(result, pdecOprd);
}

#endif // !NET_4_0

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
#if !NET_4_0
    int Decimal::decimalSetExponent(il2cpp_decimal_repr* pA, int texp)
    {
        int scale = pA->u.signscale.scale;

        scale -= texp;

        if (scale < 0 || scale > DECIMAL_MAX_SCALE)
        {
            uint64_t alo;
            uint64_t ahi;

            DECTO128(pA, alo, ahi);
            int status = rescale128(&alo, &ahi, &scale, 0, 0, DECIMAL_MAX_SCALE, 1);
            if (status != DECIMAL_SUCCESS)
                return status;

            return pack128toDecimal(pA, alo, ahi, scale, pA->u.signscale.sign);
        }
        else
        {
            pA->u.signscale.scale = scale;
            return DECIMAL_SUCCESS;
        }
    }

    int Decimal::string2decimal(il2cpp_decimal_repr *pA, Il2CppString *str, unsigned int decrDecimal, int sign)
    {
        Il2CppChar* buf = utils::StringUtils::GetChars(str);
        Il2CppChar* p;
        uint64_t alo, ahi;
        alo = ahi = 0;
        int n, rc, i, len, sigLen = -1, firstNonZero;
        int scale, roundBit = 0;

        DECINIT(pA);

        for (p = buf, len = 0; *p != 0; len++, p++)
        {
        }

        for (p = buf, i = 0; *p != 0; i++, p++)
        {
            n = *p - '0';
            if (n < 0 || n > 9)
                return DECIMAL_INVALID_CHARACTER;

            if (n)
            {
                if (sigLen < 0)
                {
                    firstNonZero = i;
                    sigLen = (len - firstNonZero > DECIMAL_MAX_SCALE + 1) ? DECIMAL_MAX_SCALE + 1 + firstNonZero : len;
                    if (decrDecimal > static_cast<uint32_t>(sigLen + 1))
                        return DECIMAL_OVERFLOW;
                }
                if (i >= sigLen)
                    break;
                rc = incMultConstant128(&alo, &ahi, sigLen - 1 - i, n);
                if (rc != DECIMAL_SUCCESS)
                    return rc;
            }
        }

        scale = sigLen - decrDecimal;

        if (i < len) /* too much digits, we must round */
        {
            n = buf[i] - '0';
            if (n < 0 || n > 9)
            {
                return DECIMAL_INVALID_CHARACTER;
            }
            if (n > 5)
                roundBit = 1;
            else if (n == 5) /* we must take a nearer look */
            {
                n = buf[i - 1] - '0';
                for (++i; i < len; ++i)
                {
                    if (buf[i] != '0')
                        break;            /* we are greater than .5 */
                }
                if (i < len /* greater than exactly .5 */
                    || n % 2 == 1) /* exactly .5, use banker's rule for rounding */
                {
                    roundBit = 1;
                }
            }
        }

        if (ahi != 0)
        {
            rc = normalize128(&alo, &ahi, &scale, 1, roundBit);
            if (rc != DECIMAL_SUCCESS)
                return rc;
        }

        if (alo == 0 && ahi == 0)
        {
            DECINIT(pA);
            return DECIMAL_SUCCESS;
        }
        else
        {
            return pack128toDecimal(pA, alo, ahi, sigLen - decrDecimal, sign);
        }
        return 0;
    }

    int Decimal::decimalCompare(il2cpp_decimal_repr *pA, il2cpp_decimal_repr *pB)
    {
        int log2a, log2b, delta, sign;
        il2cpp_decimal_repr aa;

        sign = (pA->u.signscale.sign) ? -1 : 1;

        if (pA->u.signscale.sign ^ pB->u.signscale.sign)
        {
            return (decimalIsZero(pA) && decimalIsZero(pB)) ? 0 : sign;
        }

        /* try fast comparison via log2 */
        log2a = decimalLog2(pA);
        log2b = decimalLog2(pB);
        delta = log2a - log2b;
        /* decimalLog2 is not exact, so we can say nothing
        if abs(delta) <= 1 */
        if (delta < -1)
            return -sign;
        if (delta > 1)
            return sign;

        DECCOPY(&aa, pA);
        DECNEGATE(&aa);
        decimalIncr(&aa, pB);

        if (decimalIsZero(&aa))
            return 0;

        return (aa.u.signscale.sign) ? 1 : -1;
    }

    int32_t Decimal::decimalIncr(il2cpp_decimal_repr * pA, il2cpp_decimal_repr * pB)
    {
        uint64_t alo, ahi, blo, bhi;
        int log2A, log2B, log2Result, log10Result, rc;
        int subFlag, sign, scaleA, scaleB;

        DECTO128(pA, alo, ahi);
        DECTO128(pB, blo, bhi);

        sign = pA->u.signscale.sign;
        subFlag = sign - (int)pB->u.signscale.sign;
        scaleA = pA->u.signscale.scale;
        scaleB = pB->u.signscale.scale;
        if (scaleA == scaleB)
        {
            /* same scale, that's easy */
            if (subFlag)
            {
                sub128(alo, ahi, blo, bhi, &alo, &ahi);
                if (ahi & LIT_GUINT64_HIGHBIT)
                {
                    alo--;
                    alo = ~alo;
                    if (alo == 0)
                        ahi--;
                    ahi = ~ahi;
                    sign = !sign;
                }
            }
            else
            {
                add128(alo, ahi, blo, bhi, &alo, &ahi);
            }
            rc = normalize128(&alo, &ahi, &scaleA, 1, 0);
        }
        else
        {
            /* scales must be adjusted */
            /* Estimate log10 and scale of result for adjusting scales */
            log2A = log2withScale_128(alo, ahi, scaleA);
            log2B = log2withScale_128(blo, bhi, scaleB);
            log2Result = MAX(log2A, log2B);
            if (!subFlag)
                log2Result++;       /* result can have one bit more */
            log10Result = (log2Result * 1000) / 3322 + 1;
            /* we will calculate in 128bit, so we may need to adjust scale */
            if (scaleB > scaleA)
                scaleA = scaleB;
            if (scaleA + log10Result > DECIMAL_MAX_SCALE + 7)
            {
                /* this may not fit in 128bit, so limit it */
                scaleA = DECIMAL_MAX_SCALE + 7 - log10Result;
            }

            rc = adjustScale128(&alo, &ahi, scaleA - (int)pA->u.signscale.scale);
            if (rc != DECIMAL_SUCCESS)
                return rc;
            rc = adjustScale128(&blo, &bhi, scaleA - scaleB);
            if (rc != DECIMAL_SUCCESS)
                return rc;

            if (subFlag)
            {
                sub128(alo, ahi, blo, bhi, &alo, &ahi);
                if (ahi & LIT_GUINT64_HIGHBIT)
                {
                    alo--;
                    alo = ~alo;
                    if (alo == 0)
                        ahi--;
                    ahi = ~ahi;
                    sign = !sign;
                }
            }
            else
            {
                add128(alo, ahi, blo, bhi, &alo, &ahi);
            }

            rc = rescale128(&alo, &ahi, &scaleA, 0, 0, DECIMAL_MAX_SCALE, 1);
        }

        if (rc != DECIMAL_SUCCESS)
            return rc;

        return pack128toDecimal(pA, alo, ahi, scaleA, sign);
    }

    void Decimal::decimalFloorAndTrunc(il2cpp_decimal_repr * pA, int32_t floorFlag)
    {
        uint64_t alo, ahi;
        uint32_t factor, rest;
        int scale, sign, idx;
        int hasRest = 0;

        scale = pA->u.signscale.scale;
        if (scale == 0)
            return;         /* nothing to do */

        DECTO128(pA, alo, ahi);
        sign = pA->u.signscale.sign;

        while (scale > 0)
        {
            idx = (scale > DECIMAL_MAX_INTFACTORS) ? DECIMAL_MAX_INTFACTORS : scale;
            factor = constantsDecadeInt32Factors[idx];
            scale -= idx;
            div128by32(&alo, &ahi, factor, &rest);
            hasRest = hasRest || (rest != 0);
        }

        if (floorFlag && hasRest && sign) /* floor: if negative, we must round up */
        {
            roundUp128(&alo, &ahi);
        }

        pack128toDecimal(pA, alo, ahi, 0, sign);
    }

    int32_t Decimal::decimal2UInt64(il2cpp_decimal_repr * pA, uint64_t* pResult)
    {
        uint64_t alo, ahi;
        int scale;

        DECTO128(pA, alo, ahi);
        scale = pA->u.signscale.scale;
        if (scale > 0)
        {
            div128DecadeFactor(&alo, &ahi, scale);
        }

        /* overflow if integer too large or < 0 */
        if (ahi != 0 || (alo != 0 && pA->u.signscale.sign))
            return DECIMAL_OVERFLOW;

        *pResult = alo;
        return DECIMAL_SUCCESS;
    }

    int32_t Decimal::decimal2Int64(il2cpp_decimal_repr * pA, int64_t* pResult)
    {
        uint64_t alo, ahi;
        int sign, scale;

        DECTO128(pA, alo, ahi);
        scale = pA->u.signscale.scale;
        if (scale > 0)
        {
            div128DecadeFactor(&alo, &ahi, scale);
        }

        if (ahi != 0)
            return DECIMAL_OVERFLOW;

        sign = pA->u.signscale.sign;
        if (sign && alo != 0)
        {
            if (alo > LIT_GUINT64_HIGHBIT)
                return DECIMAL_OVERFLOW;
            *pResult = (int64_t) ~(alo - 1);
        }
        else
        {
            if (alo & LIT_GUINT64_HIGHBIT)
                return DECIMAL_OVERFLOW;
            *pResult = (int64_t)alo;
        }

        return DECIMAL_SUCCESS;
    }

    int32_t Decimal::decimalMult(il2cpp_decimal_repr* pA, il2cpp_decimal_repr* pB)
    {
        uint64_t low, mid, high;
        uint32_t factor;
        int scale, sign;

        mult96by96to192(pA->lo32, pA->mid32, pA->hi32, pB->lo32, pB->mid32, pB->hi32, &low, &mid, &high);

        /* adjust scale and sign */
        scale = (int)pA->u.signscale.scale + (int)pB->u.signscale.scale;
        sign = pA->u.signscale.sign ^ pB->u.signscale.sign;

        /* first scaling step */
        factor = constantsDecadeInt32Factors[DECIMAL_MAX_INTFACTORS];
        while (high != 0 || (mid >> 32) >= factor)
        {
            if (high < 100)
            {
                factor /= 1000; /* we need some digits for final rounding */
                scale -= DECIMAL_MAX_INTFACTORS - 3;
            }
            else
            {
                scale -= DECIMAL_MAX_INTFACTORS;
            }

            div192by32(&low, &mid, &high, factor);
        }

        /* second and final scaling */
        int status = rescale128(&low, &mid, &scale, 0, 0, DECIMAL_MAX_SCALE, 1);
        if (status != DECIMAL_SUCCESS)
            return status;

        return pack128toDecimal(pA, low, mid, scale, sign);
    }

    int32_t Decimal::decimalDiv(il2cpp_decimal_repr* pC, il2cpp_decimal_repr* pA, il2cpp_decimal_repr* pB)
    {
        uint64_t clo, chi; /* result */
        int scale, texp;

        /* Check for common cases */
        if (decimalCompare(pA, pB) == 0)
        {
            /* One */
            return pack128toDecimal(pC, 1, 0, 0, 0);
        }
        pA->u.signscale.sign = pA->u.signscale.sign ? 0 : 1;
        if (decimalCompare(pA, pB) == 0)
        {
            /* Minus one */
            return pack128toDecimal(pC, 1, 0, 0, 1);
        }
        pA->u.signscale.sign = pA->u.signscale.sign ? 0 : 1;

        int status = decimalDivSub(pA, pB, &clo, &chi, &texp);
        if (status != DECIMAL_SUCCESS)
        {
            if (status == DECIMAL_FINISHED)
                status = DECIMAL_SUCCESS;
            return status;
        }

        /* adjust scale and sign */
        scale = (int)pA->u.signscale.scale - (int)pB->u.signscale.scale;

        /*test: printf("0: %.17e\n", (((double)chi) * pow(2,64) + clo) * pow(10, -scale) * pow(2, -exp));*/
        status = rescale128(&clo, &chi, &scale, texp, 0, DECIMAL_MAX_SCALE, 1);
        if (status != DECIMAL_SUCCESS)
            return status;

        return pack128toDecimal(pC, clo, chi, scale, pA->u.signscale.sign ^ pB->u.signscale.sign);
    }

    double Decimal::decimal2double(il2cpp_decimal_repr * pA)
    {
        double d;
        uint64_t alo, ahi, mantisse;
        uint32_t overhang, factor, roundBits;
        int scale, texp, log5, i;


        ahi = (((uint64_t)(pA->hi32)) << 32) | pA->mid32;
        alo = ((uint64_t)(pA->lo32)) << 32;

        /* special case zero */
        if (ahi == 0 && alo == 0)
            return 0.0;

        texp = 0;
        scale = pA->u.signscale.scale;

        /* transform n * 10^-scale and exp = 0 => m * 2^-exp and scale = 0 */
        while (scale > 0)
        {
            while ((ahi & LIT_GUINT64_HIGHBIT) == 0)
            {
                lshift128(&alo, &ahi);
                texp++;
            }

            overhang = (uint32_t)(ahi >> 32);
            if (overhang >= 5)
            {
                /* estimate log5 */
                log5 = (log2_32(overhang) * 1000) / 2322; /* ln(5)/ln(2) = 2.3219... */
                if (log5 < DECIMAL_MAX_INTFACTORS)
                {
                    /* get maximal factor=5^i, so that overhang / factor >= 1 */
                    factor = constantsDecadeInt32Factors[log5] >> log5; /* 5^n = 10^n/2^n */
                    i = log5 + overhang / factor;
                }
                else
                {
                    i = DECIMAL_MAX_INTFACTORS; /* we have only constants up to 10^DECIMAL_MAX_INTFACTORS */
                }
                if (i > scale)
                    i = scale;
                factor = constantsDecadeInt32Factors[i] >> i; /* 5^n = 10^n/2^n */
                /* n * 10^-scale * 2^-exp => m * 10^-(scale-i) * 2^-(exp+i) with m = n * 5^-i */
                div128by32(&alo, &ahi, factor, 0);
                scale -= i;
                texp += i;
            }
        }

        /* normalize significand (highest bit should be 1) */
        while ((ahi & LIT_GUINT64_HIGHBIT) == 0)
        {
            lshift128(&alo, &ahi);
            texp++;
        }

        /* round to nearest even */
        roundBits = (uint32_t)ahi & 0x7ff;
        ahi += 0x400;
        if ((ahi & LIT_GUINT64_HIGHBIT) == 0) /* overflow ? */
        {
            ahi >>= 1;
            texp--;
        }
        else if ((roundBits & 0x400) == 0)
            ahi &= ~1;

        /* 96 bit => 1 implizit bit and 52 explicit bits */
        mantisse = (ahi & ~LIT_GUINT64_HIGHBIT) >> 11;

        buildIEEE754Double(&d, pA->u.signscale.sign, -texp + 95, mantisse);

        return d;
    }

    int32_t Decimal::decimal2string(il2cpp_decimal_repr* val, int32_t digits, int32_t decimals, Il2CppArray* bufDigits, int32_t bufSize, int32_t* decPos, int32_t* sign)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(Decimal::decimal2string);

        return 0;
    }

    int32_t Decimal::decimalIntDiv(il2cpp_decimal_repr* pC, il2cpp_decimal_repr* pA, il2cpp_decimal_repr* pB)
    {
        uint64_t clo, chi; /* result */
        int scale, texp, rc;

        rc = decimalDivSub(pA, pB, &clo, &chi, &texp);
        if (rc != DECIMAL_SUCCESS)
        {
            if (rc == DECIMAL_FINISHED)
                rc = DECIMAL_SUCCESS;
            return rc;
        }

        /* calc scale  */
        scale = (int)pA->u.signscale.scale - (int)pB->u.signscale.scale;

        /* truncate result to integer */
        rc = rescale128(&clo, &chi, &scale, texp, 0, 0, 0);
        if (rc != DECIMAL_SUCCESS)
            return rc;

        return pack128toDecimal(pC, clo, chi, scale, pA->u.signscale.sign);
    }

#else
    double Decimal::ToDouble(Il2CppDecimal d)
    {
        double result = 0.0;
        // Note: this can fail if the input is an invalid decimal, but for compatibility we should return 0
        il2cpp_decimal_to_double_result(&d, &result);
        return result;
    }

    int32_t Decimal::FCallCompare(Il2CppDecimal* left, Il2CppDecimal* right)
    {
        uint32_t   left_sign;
        uint32_t   right_sign;
        Il2CppDecimal result;

        result.Hi32 = 0; // Just to shut up the compiler

        // First check signs and whether either are zero.  If both are
        // non-zero and of the same sign, just use subtraction to compare.
        //
        left_sign = left->v.v.Lo32 | left->v.v.Mid32 | left->Hi32;
        right_sign = right->v.v.Lo32 | right->v.v.Mid32 | right->Hi32;
        if (left_sign != 0)
            left_sign = (left->u.u.sign & DECIMAL_NEG) | 1;

        if (right_sign != 0)
            right_sign = (right->u.u.sign & DECIMAL_NEG) | 1;

        // left_sign & right_sign have values 1, 0, or 0x81 depending on if the left/right
        // operand is +, 0, or -.
        //
        if (left_sign == right_sign)
        {
            if (left_sign == 0) // both are zero
                return IL2CPP_DECIMAL_CMP_EQ; // return equal

            DecAddSub(left, right, &result, DECIMAL_NEG);
            if (DECIMAL_LO64_GET(result) == 0 && result.Hi32 == 0)
                return IL2CPP_DECIMAL_CMP_EQ;
            if (result.u.u.sign & DECIMAL_NEG)
                return IL2CPP_DECIMAL_CMP_LT;
            return IL2CPP_DECIMAL_CMP_GT;
        }

        //
        // Signs are different.  Use signed byte comparison
        //
        if ((signed char)left_sign > (signed char)right_sign)
            return IL2CPP_DECIMAL_CMP_GT;
        return IL2CPP_DECIMAL_CMP_LT;
    }

    int32_t Decimal::FCallToInt32(Il2CppDecimal d)
    {
        Il2CppDecimal result;

        // The following can not return an error, it only returns INVALID_ARG if the decimals is < 0
        il2cpp_decimal_round_result(&d, 0, &result);

        if (DECIMAL_SCALE(result) != 0)
        {
            d = result;
            il2cpp_decimal_fix(&d, &result);
        }

        if (DECIMAL_HI32(result) == 0 && DECIMAL_MID32(result) == 0)
        {
            int32_t i = DECIMAL_LO32(result);
            if ((int16_t)DECIMAL_SIGNSCALE(result) >= 0)
            {
                if (i >= 0)
                    return i;
            }
            else
            {
                i = -i;
                if (i <= 0)
                    return i;
            }
        }

        vm::Exception::RaiseOverflowException();
        return 0;
    }

    int32_t Decimal::GetHashCode(Il2CppDecimal* _this)
    {
        double dbl;

        if (il2cpp_decimal_to_double_result(_this, &dbl) != IL2CPP_DECIMAL_OK)
            return 0;

        if (dbl == 0.0)
        {
            // Ensure 0 and -0 have the same hash code
            return 0;
        }
        // conversion to double is lossy and produces rounding errors so we mask off the lowest 4 bits
        //
        // For example these two numerically equal decimals with different internal representations produce
        // slightly different results when converted to double:
        //
        // decimal a = new decimal(new int[] { 0x76969696, 0x2fdd49fa, 0x409783ff, 0x00160000 });
        //                     => (decimal)1999021.176470588235294117647000000000 => (double)1999021.176470588
        // decimal b = new decimal(new int[] { 0x3f0f0f0f, 0x1e62edcc, 0x06758d33, 0x00150000 });
        //                     => (decimal)1999021.176470588235294117647000000000 => (double)1999021.1764705882
        //
        return ((((int*)&dbl)[0]) & 0xFFFFFFF0) ^ ((int*)&dbl)[1];
    }

    float Decimal::ToSingle(Il2CppDecimal d)
    {
        float result = 0.0f;
        // Note: this can fail if the input is an invalid decimal, but for compatibility we should return 0
        il2cpp_decimal_to_float_result(&d, &result);
        return result;
    }

    void Decimal::ConstructorDouble(Il2CppDecimal* _this, double value)
    {
        if (il2cpp_decimal_from_double(value, _this) == IL2CPP_DECIMAL_OVERFLOW)
        {
            vm::Exception::RaiseOverflowException();
            return;
        }
        _this->reserved = 0;
    }

    void Decimal::ConstructorFloat(Il2CppDecimal* _this, float value)
    {
        if (il2cpp_decimal_from_float(value, _this) == IL2CPP_DECIMAL_OVERFLOW)
        {
            vm::Exception::RaiseOverflowException();
            return;
        }
        _this->reserved = 0;
    }

    void Decimal::FCallAddSub(Il2CppDecimal* left, Il2CppDecimal* right, uint8_t sign)
    {
        Il2CppDecimal result, decTmp;
        Il2CppDecimal *pdecTmp, *leftOriginal;
        uint32_t    num[6], pwr;
        int         scale, hi_prod, cur;
        SPLIT64     sdlTmp;

        IL2CPP_ASSERT(sign == 0 || sign == DECIMAL_NEG);

        leftOriginal = left;

        sign ^= (DECIMAL_SIGN(*right) ^ DECIMAL_SIGN(*left)) & DECIMAL_NEG;

        if (DECIMAL_SCALE(*right) == DECIMAL_SCALE(*left))
        {
            // Scale factors are equal, no alignment necessary.
            //
            DECIMAL_SIGNSCALE(result) = DECIMAL_SIGNSCALE(*left);

        AlignedAdd:
            if (sign)
            {
                // Signs differ - subtract
                //
                DECIMAL_LO64_SET(result, (DECIMAL_LO64_GET(*left) - DECIMAL_LO64_GET(*right)));
                DECIMAL_HI32(result) = DECIMAL_HI32(*left) - DECIMAL_HI32(*right);

                // Propagate carry
                //
                if (DECIMAL_LO64_GET(result) > DECIMAL_LO64_GET(*left))
                {
                    DECIMAL_HI32(result)--;
                    if (DECIMAL_HI32(result) >= DECIMAL_HI32(*left))
                        goto SignFlip;
                }
                else if (DECIMAL_HI32(result) > DECIMAL_HI32(*left))
                {
                    // Got negative result.  Flip its sign.
                    //
                SignFlip:
                    DECIMAL_LO64_SET(result, -(int64_t)DECIMAL_LO64_GET(result));
                    DECIMAL_HI32(result) = ~DECIMAL_HI32(result);
                    if (DECIMAL_LO64_GET(result) == 0)
                        DECIMAL_HI32(result)++;
                    DECIMAL_SIGN(result) ^= DECIMAL_NEG;
                }
            }
            else
            {
                // Signs are the same - add
                //
                DECIMAL_LO64_SET(result, (DECIMAL_LO64_GET(*left) + DECIMAL_LO64_GET(*right)));
                DECIMAL_HI32(result) = DECIMAL_HI32(*left) + DECIMAL_HI32(*right);

                // Propagate carry
                //
                if (DECIMAL_LO64_GET(result) < DECIMAL_LO64_GET(*left))
                {
                    DECIMAL_HI32(result)++;
                    if (DECIMAL_HI32(result) <= DECIMAL_HI32(*left))
                        goto AlignedScale;
                }
                else if (DECIMAL_HI32(result) < DECIMAL_HI32(*left))
                {
                AlignedScale:
                    // The addition carried above 96 bits.  Divide the result by 10,
                    // dropping the scale factor.
                    //
                    if (DECIMAL_SCALE(result) == 0)
                    {
                        vm::Exception::RaiseOverflowException();
                        return;
                    }
                    DECIMAL_SCALE(result)--;

                    sdlTmp.u.Lo = DECIMAL_HI32(result);
                    sdlTmp.u.Hi = 1;
                    sdlTmp.int64 = DivMod64by32(sdlTmp.int64, 10);
                    DECIMAL_HI32(result) = sdlTmp.u.Lo;

                    sdlTmp.u.Lo = DECIMAL_MID32(result);
                    sdlTmp.int64 = DivMod64by32(sdlTmp.int64, 10);
                    DECIMAL_MID32(result) = sdlTmp.u.Lo;

                    sdlTmp.u.Lo = DECIMAL_LO32(result);
                    sdlTmp.int64 = DivMod64by32(sdlTmp.int64, 10);
                    DECIMAL_LO32(result) = sdlTmp.u.Lo;

                    // See if we need to round up.
                    //
                    if (sdlTmp.u.Hi >= 5 && (sdlTmp.u.Hi > 5 || (DECIMAL_LO32(result) & 1)))
                    {
                        DECIMAL_LO64_SET(result, DECIMAL_LO64_GET(result) + 1);
                        if (DECIMAL_LO64_GET(result) == 0)
                            DECIMAL_HI32(result)++;
                    }
                }
            }
        }
        else
        {
            // Scale factors are not equal.  Assume that a larger scale
            // factor (more decimal places) is likely to mean that number
            // is smaller.  Start by guessing that the right operand has
            // the larger scale factor.  The result will have the larger
            // scale factor.
            //
            DECIMAL_SCALE(result) = DECIMAL_SCALE(*right); // scale factor of "smaller"
            DECIMAL_SIGN(result) = DECIMAL_SIGN(*left); // but sign of "larger"
            scale = DECIMAL_SCALE(result) - DECIMAL_SCALE(*left);

            if (scale < 0)
            {
                // Guessed scale factor wrong. Swap operands.
                //
                scale = -scale;
                DECIMAL_SCALE(result) = DECIMAL_SCALE(*left);
                DECIMAL_SIGN(result) ^= sign;
                pdecTmp = right;
                right = left;
                left = pdecTmp;
            }

            // *left will need to be multiplied by 10^scale so
            // it will have the same scale as *right.  We could be
            // extending it to up to 192 bits of precision.
            //
            if (scale <= POWER10_MAX)
            {
                // Scaling won't make it larger than 4 uint32_ts
                //
                pwr = power10[scale];
                DECIMAL_LO64_SET(decTmp, UInt32x32To64(DECIMAL_LO32(*left), pwr));
                sdlTmp.int64 = UInt32x32To64(DECIMAL_MID32(*left), pwr);
                sdlTmp.int64 += DECIMAL_MID32(decTmp);
                DECIMAL_MID32(decTmp) = sdlTmp.u.Lo;
                DECIMAL_HI32(decTmp) = sdlTmp.u.Hi;
                sdlTmp.int64 = UInt32x32To64(DECIMAL_HI32(*left), pwr);
                sdlTmp.int64 += DECIMAL_HI32(decTmp);
                if (sdlTmp.u.Hi == 0)
                {
                    // Result fits in 96 bits.  Use standard aligned add.
                    //
                    DECIMAL_HI32(decTmp) = sdlTmp.u.Lo;
                    left = &decTmp;
                    goto AlignedAdd;
                }
                num[0] = DECIMAL_LO32(decTmp);
                num[1] = DECIMAL_MID32(decTmp);
                num[2] = sdlTmp.u.Lo;
                num[3] = sdlTmp.u.Hi;
                hi_prod = 3;
            }
            else
            {
                // Have to scale by a bunch.  Move the number to a buffer
                // where it has room to grow as it's scaled.
                //
                num[0] = DECIMAL_LO32(*left);
                num[1] = DECIMAL_MID32(*left);
                num[2] = DECIMAL_HI32(*left);
                hi_prod = 2;

                // Scan for zeros in the upper words.
                //
                if (num[2] == 0)
                {
                    hi_prod = 1;
                    if (num[1] == 0)
                    {
                        hi_prod = 0;
                        if (num[0] == 0)
                        {
                            // Left arg is zero, return right.
                            //
                            DECIMAL_LO64_SET(result, DECIMAL_LO64_GET(*right));
                            DECIMAL_HI32(result) = DECIMAL_HI32(*right);
                            DECIMAL_SIGN(result) ^= sign;
                            goto RetDec;
                        }
                    }
                }

                // Scaling loop, up to 10^9 at a time.  hi_prod stays updated
                // with index of highest non-zero uint32_t.
                //
                for (; scale > 0; scale -= POWER10_MAX)
                {
                    if (scale > POWER10_MAX)
                        pwr = ten_to_nine;
                    else
                        pwr = power10[scale];

                    sdlTmp.u.Hi = 0;
                    for (cur = 0; cur <= hi_prod; cur++)
                    {
                        sdlTmp.int64 = UInt32x32To64(num[cur], pwr) + sdlTmp.u.Hi;
                        num[cur] = sdlTmp.u.Lo;
                    }

                    if (sdlTmp.u.Hi != 0)
                        // We're extending the result by another uint32_t.
                        num[++hi_prod] = sdlTmp.u.Hi;
                }
            }

            // Scaling complete, do the add.  Could be subtract if signs differ.
            //
            sdlTmp.u.Lo = num[0];
            sdlTmp.u.Hi = num[1];

            if (sign)
            {
                // Signs differ, subtract.
                //
                DECIMAL_LO64_SET(result, (sdlTmp.int64 - DECIMAL_LO64_GET(*right)));
                DECIMAL_HI32(result) = num[2] - DECIMAL_HI32(*right);

                // Propagate carry
                //
                if (DECIMAL_LO64_GET(result) > sdlTmp.int64)
                {
                    DECIMAL_HI32(result)--;
                    if (DECIMAL_HI32(result) >= num[2])
                        goto LongSub;
                }
                else if (DECIMAL_HI32(result) > num[2])
                {
                LongSub:
                    // If num has more than 96 bits of precision, then we need to
                    // carry the subtraction into the higher bits.  If it doesn't,
                    // then we subtracted in the wrong order and have to flip the
                    // sign of the result.
                    //
                    if (hi_prod <= 2)
                        goto SignFlip;

                    cur = 3;
                    while (num[cur++]-- == 0)
                        ;
                    if (num[hi_prod] == 0)
                        hi_prod--;
                }
            }
            else
            {
                // Signs the same, add.
                //
                DECIMAL_LO64_SET(result, (sdlTmp.int64 + DECIMAL_LO64_GET(*right)));
                DECIMAL_HI32(result) = num[2] + DECIMAL_HI32(*right);

                // Propagate carry
                //
                if (DECIMAL_LO64_GET(result) < sdlTmp.int64)
                {
                    DECIMAL_HI32(result)++;
                    if (DECIMAL_HI32(result) <= num[2])
                        goto LongAdd;
                }
                else if (DECIMAL_HI32(result) < num[2])
                {
                LongAdd:
                    // Had a carry above 96 bits.
                    //
                    cur = 3;
                    do
                    {
                        if (hi_prod < cur)
                        {
                            num[cur] = 1;
                            hi_prod = cur;
                            break;
                        }
                    }
                    while (++num[cur++] == 0);
                }
            }

            if (hi_prod > 2)
            {
                num[0] = DECIMAL_LO32(result);
                num[1] = DECIMAL_MID32(result);
                num[2] = DECIMAL_HI32(result);
                DECIMAL_SCALE(result) = (uint8_t)ScaleResult(num, hi_prod, DECIMAL_SCALE(result));
                if (DECIMAL_SCALE(result) == (uint8_t)-1)
                {
                    vm::Exception::RaiseOverflowException();
                    return;
                }

                DECIMAL_LO32(result) = num[0];
                DECIMAL_MID32(result) = num[1];
                DECIMAL_HI32(result) = num[2];
            }
        }

    RetDec:
        left = leftOriginal;
        COPYDEC(*left, result);
        left->reserved = 0;
    }

    void Decimal::FCallDivide(Il2CppDecimal* left, Il2CppDecimal* right)
    {
        uint32_t quo[3], quo_save[3], rem[4], divisor[3];
        uint32_t pwr, tmp, tmp1;
        SPLIT64  sdlTmp, sdlDivisor;
        int      scale, cur_scale;
        bool unscale;

        scale = DECIMAL_SCALE(*left) - DECIMAL_SCALE(*right);
        unscale = false;
        divisor[0] = DECIMAL_LO32(*right);
        divisor[1] = DECIMAL_MID32(*right);
        divisor[2] = DECIMAL_HI32(*right);

        if (divisor[1] == 0 && divisor[2] == 0)
        {
            // Divisor is only 32 bits.  Easy divide.
            //
            if (divisor[0] == 0)
            {
                vm::Exception::RaiseDivideByZeroException();
                return;
            }

            quo[0] = DECIMAL_LO32(*left);
            quo[1] = DECIMAL_MID32(*left);
            quo[2] = DECIMAL_HI32(*left);
            rem[0] = Div96By32(quo, divisor[0]);

            for (;;)
            {
                if (rem[0] == 0)
                {
                    if (scale < 0)
                    {
                        cur_scale = std::min(9, -scale);
                        goto HaveScale;
                    }
                    break;
                }
                // We need to unscale if and only if we have a non-zero remainder
                unscale = true;

                // We have computed a quotient based on the natural scale
                // ( <dividend scale> - <divisor scale> ).  We have a non-zero
                // remainder, so now we should increase the scale if possible to
                // include more quotient bits.
                //
                // If it doesn't cause overflow, we'll loop scaling by 10^9 and
                // computing more quotient bits as long as the remainder stays
                // non-zero.  If scaling by that much would cause overflow, we'll
                // drop out of the loop and scale by as much as we can.
                //
                // Scaling by 10^9 will overflow if quo[2].quo[1] >= 2^32 / 10^9
                // = 4.294 967 296.  So the upper limit is quo[2] == 4 and
                // quo[1] == 0.294 967 296 * 2^32 = 1,266,874,889.7+.  Since
                // quotient bits in quo[0] could be all 1's, then 1,266,874,888
                // is the largest value in quo[1] (when quo[2] == 4) that is
                // assured not to overflow.
                //
                cur_scale = SearchScale(quo[2], quo[1], quo[0], scale);
                if (cur_scale == 0)
                {
                    // No more scaling to be done, but remainder is non-zero.
                    // Round quotient.T
                    //
                    tmp = rem[0] << 1;
                    if (tmp < rem[0] || (tmp >= divisor[0] &&
                                         (tmp > divisor[0] || (quo[0] & 1))))
                    {
                    RoundUp:
                        if (!Add32To96(quo, 1))
                        {
                            if (scale == 0)
                            {
                                vm::Exception::RaiseOverflowException();
                                return;
                            }
                            scale--;
                            OverflowUnscale(quo, true);
                            break;
                        }
                    }
                    break;
                }

                if (cur_scale < 0)
                {
                    vm::Exception::RaiseOverflowException();
                    return;
                }

            HaveScale:
                pwr = power10[cur_scale];
                scale += cur_scale;

                if (IncreaseScale(quo, pwr) != 0)
                {
                    vm::Exception::RaiseOverflowException();
                    return;
                }

                sdlTmp.int64 = DivMod64by32(UInt32x32To64(rem[0], pwr), divisor[0]);
                rem[0] = sdlTmp.u.Hi;

                if (!Add32To96(quo, sdlTmp.u.Lo))
                {
                    if (scale == 0)
                    {
                        vm::Exception::RaiseOverflowException();
                        return;
                    }
                    scale--;
                    OverflowUnscale(quo, (rem[0] != 0));
                    break;
                }
            } // for (;;)
        }
        else
        {
            // Divisor has bits set in the upper 64 bits.
            //
            // Divisor must be fully normalized (shifted so bit 31 of the most
            // significant uint32_t is 1).  Locate the MSB so we know how much to
            // normalize by.  The dividend will be shifted by the same amount so
            // the quotient is not changed.
            //
            if (divisor[2] == 0)
                tmp = divisor[1];
            else
                tmp = divisor[2];

            cur_scale = 0;
            if (!(tmp & 0xFFFF0000))
            {
                cur_scale += 16;
                tmp <<= 16;
            }
            if (!(tmp & 0xFF000000))
            {
                cur_scale += 8;
                tmp <<= 8;
            }
            if (!(tmp & 0xF0000000))
            {
                cur_scale += 4;
                tmp <<= 4;
            }
            if (!(tmp & 0xC0000000))
            {
                cur_scale += 2;
                tmp <<= 2;
            }
            if (!(tmp & 0x80000000))
            {
                cur_scale++;
                tmp <<= 1;
            }

            // Shift both dividend and divisor left by cur_scale.
            //
            sdlTmp.int64 = DECIMAL_LO64_GET(*left) << cur_scale;
            rem[0] = sdlTmp.u.Lo;
            rem[1] = sdlTmp.u.Hi;
            sdlTmp.u.Lo = DECIMAL_MID32(*left);
            sdlTmp.u.Hi = DECIMAL_HI32(*left);
            sdlTmp.int64 <<= cur_scale;
            rem[2] = sdlTmp.u.Hi;
            rem[3] = (DECIMAL_HI32(*left) >> (31 - cur_scale)) >> 1;

            sdlDivisor.u.Lo = divisor[0];
            sdlDivisor.u.Hi = divisor[1];
            sdlDivisor.int64 <<= cur_scale;

            if (divisor[2] == 0)
            {
                // Have a 64-bit divisor in sdlDivisor.  The remainder
                // (currently 96 bits spread over 4 uint32_ts) will be < divisor.
                //
                sdlTmp.u.Lo = rem[2];
                sdlTmp.u.Hi = rem[3];

                quo[2] = 0;
                quo[1] = Div96By64(&rem[1], sdlDivisor);
                quo[0] = Div96By64(rem, sdlDivisor);

                for (;;)
                {
                    if ((rem[0] | rem[1]) == 0)
                    {
                        if (scale < 0)
                        {
                            cur_scale = std::min(9, -scale);
                            goto HaveScale64;
                        }
                        break;
                    }

                    // We need to unscale if and only if we have a non-zero remainder
                    unscale = true;

                    // Remainder is non-zero.  Scale up quotient and remainder by
                    // powers of 10 so we can compute more significant bits.
                    //
                    cur_scale = SearchScale(quo[2], quo[1], quo[0], scale);
                    if (cur_scale == 0)
                    {
                        // No more scaling to be done, but remainder is non-zero.
                        // Round quotient.
                        //
                        sdlTmp.u.Lo = rem[0];
                        sdlTmp.u.Hi = rem[1];
                        if (sdlTmp.u.Hi >= 0x80000000 || (sdlTmp.int64 <<= 1) > sdlDivisor.int64 ||
                            (sdlTmp.int64 == sdlDivisor.int64 && (quo[0] & 1)))
                            goto RoundUp;
                        break;
                    }

                    if (cur_scale < 0)
                    {
                        vm::Exception::RaiseOverflowException();
                        return;
                    }

                HaveScale64:
                    pwr = power10[cur_scale];
                    scale += cur_scale;

                    if (IncreaseScale(quo, pwr) != 0)
                    {
                        vm::Exception::RaiseOverflowException();
                        return;
                    }

                    rem[2] = 0; // rem is 64 bits, IncreaseScale uses 96
                    IncreaseScale(rem, pwr);
                    tmp = Div96By64(rem, sdlDivisor);
                    if (!Add32To96(quo, tmp))
                    {
                        if (scale == 0)
                        {
                            vm::Exception::RaiseOverflowException();
                            return;
                        }
                        scale--;
                        OverflowUnscale(quo, (rem[0] != 0 || rem[1] != 0));
                        break;
                    }
                } // for (;;)
            }
            else
            {
                // Have a 96-bit divisor in divisor[].
                //
                // Start by finishing the shift left by cur_scale.
                //
                sdlTmp.u.Lo = divisor[1];
                sdlTmp.u.Hi = divisor[2];
                sdlTmp.int64 <<= cur_scale;
                divisor[0] = sdlDivisor.u.Lo;
                divisor[1] = sdlDivisor.u.Hi;
                divisor[2] = sdlTmp.u.Hi;

                // The remainder (currently 96 bits spread over 4 uint32_ts)
                // will be < divisor.
                //
                quo[2] = 0;
                quo[1] = 0;
                quo[0] = Div128By96(rem, divisor);

                for (;;)
                {
                    if ((rem[0] | rem[1] | rem[2]) == 0)
                    {
                        if (scale < 0)
                        {
                            cur_scale = std::min(9, -scale);
                            goto HaveScale96;
                        }
                        break;
                    }

                    // We need to unscale if and only if we have a non-zero remainder
                    unscale = true;

                    // Remainder is non-zero.  Scale up quotient and remainder by
                    // powers of 10 so we can compute more significant bits.
                    //
                    cur_scale = SearchScale(quo[2], quo[1], quo[0], scale);
                    if (cur_scale == 0)
                    {
                        // No more scaling to be done, but remainder is non-zero.
                        // Round quotient.
                        //
                        if (rem[2] >= 0x80000000)
                            goto RoundUp;

                        tmp = rem[0] > 0x80000000;
                        tmp1 = rem[1] > 0x80000000;
                        rem[0] <<= 1;
                        rem[1] = (rem[1] << 1) + tmp;
                        rem[2] = (rem[2] << 1) + tmp1;

                        if (rem[2] > divisor[2] || (rem[2] == divisor[2] && (rem[1] > divisor[1] || rem[1] == (divisor[1] && (rem[0] > divisor[0] || (rem[0] == divisor[0] && (quo[0] & 1)))))))
                            goto RoundUp;
                        break;
                    }

                    if (cur_scale < 0)
                    {
                        vm::Exception::RaiseOverflowException();
                        return;
                    }

                HaveScale96:
                    pwr = power10[cur_scale];
                    scale += cur_scale;

                    if (IncreaseScale(quo, pwr) != 0)
                    {
                        vm::Exception::RaiseOverflowException();
                        return;
                    }

                    rem[3] = IncreaseScale(rem, pwr);
                    tmp = Div128By96(rem, divisor);
                    if (!Add32To96(quo, tmp))
                    {
                        if (scale == 0)
                        {
                            vm::Exception::RaiseOverflowException();
                            return;
                        }

                        scale--;
                        OverflowUnscale(quo, (rem[0] != 0 || rem[1] != 0 || rem[2] != 0 || rem[3] != 0));
                        break;
                    }
                } // for (;;)
            }
        }

        // We need to unscale if and only if we have a non-zero remainder
        if (unscale)
        {
            // Try extracting any extra powers of 10 we may have
            // added.  We do this by trying to divide out 10^8, 10^4, 10^2, and 10^1.
            // If a division by one of these powers returns a zero remainder, then
            // we keep the quotient.  If the remainder is not zero, then we restore
            // the previous value.
            //
            // Since 10 = 2 * 5, there must be a factor of 2 for every power of 10
            // we can extract.  We use this as a quick test on whether to try a
            // given power.
            //
            while ((quo[0] & 0xFF) == 0 && scale >= 8)
            {
                quo_save[0] = quo[0];
                quo_save[1] = quo[1];
                quo_save[2] = quo[2];

                if (Div96By32(quo_save, 100000000) == 0)
                {
                    quo[0] = quo_save[0];
                    quo[1] = quo_save[1];
                    quo[2] = quo_save[2];
                    scale -= 8;
                }
                else
                    break;
            }

            if ((quo[0] & 0xF) == 0 && scale >= 4)
            {
                quo_save[0] = quo[0];
                quo_save[1] = quo[1];
                quo_save[2] = quo[2];

                if (Div96By32(quo_save, 10000) == 0)
                {
                    quo[0] = quo_save[0];
                    quo[1] = quo_save[1];
                    quo[2] = quo_save[2];
                    scale -= 4;
                }
            }

            if ((quo[0] & 3) == 0 && scale >= 2)
            {
                quo_save[0] = quo[0];
                quo_save[1] = quo[1];
                quo_save[2] = quo[2];

                if (Div96By32(quo_save, 100) == 0)
                {
                    quo[0] = quo_save[0];
                    quo[1] = quo_save[1];
                    quo[2] = quo_save[2];
                    scale -= 2;
                }
            }

            if ((quo[0] & 1) == 0 && scale >= 1)
            {
                quo_save[0] = quo[0];
                quo_save[1] = quo[1];
                quo_save[2] = quo[2];

                if (Div96By32(quo_save, 10) == 0)
                {
                    quo[0] = quo_save[0];
                    quo[1] = quo_save[1];
                    quo[2] = quo_save[2];
                    scale -= 1;
                }
            }
        }

        DECIMAL_SIGN(*left) = DECIMAL_SIGN(*left) ^ DECIMAL_SIGN(*right);
        DECIMAL_HI32(*left) = quo[2];
        DECIMAL_MID32(*left) = quo[1];
        DECIMAL_LO32(*left) = quo[0];
        DECIMAL_SCALE(*left) = (uint8_t)scale;
        left->reserved = 0;
    }

    void Decimal::FCallFloor(Il2CppDecimal* d)
    {
        Il2CppDecimal decRes;

        il2cpp_decimal_round_to_int(d, &decRes);

        // copy decRes into d
        COPYDEC(*d, decRes);
        d->reserved = 0;
        FC_GC_POLL();
    }

    void Decimal::FCallMultiply(Il2CppDecimal* d1, Il2CppDecimal* d2)
    {
        Il2CppDecimal decRes;

        Il2CppDecimalStatus status = il2cpp_decimal_multiply_result(d1, d2, &decRes);
        if (status != IL2CPP_DECIMAL_OK)
        {
            vm::Exception::RaiseOverflowException();
            return;
        }

        COPYDEC(*d1, decRes);
        d1->reserved = 0;

        FC_GC_POLL();
    }

    void Decimal::FCallRound(Il2CppDecimal* d, int32_t decimals)
    {
        Il2CppDecimal decRes;

        // GC is only triggered for throwing, no need to protect result
        if (decimals < 0 || decimals > 28)
        {
            vm::Exception::RaiseArgumentOutOfRangeException("d");
            return;
        }

        il2cpp_decimal_round_result(d, decimals, &decRes);

        // copy decRes into d
        COPYDEC(*d, decRes);
        d->reserved = 0;

        FC_GC_POLL();
    }

    void Decimal::FCallTruncate(Il2CppDecimal* d)
    {
        Il2CppDecimal decRes;

        il2cpp_decimal_fix(d, &decRes);

        // copy decRes into d
        COPYDEC(*d, decRes);
        d->reserved = 0;
        FC_GC_POLL();
    }

#endif
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
