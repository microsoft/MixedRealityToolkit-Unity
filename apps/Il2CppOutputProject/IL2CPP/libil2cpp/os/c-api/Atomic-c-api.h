#pragma once

#include "il2cpp-config-platforms.h"
#include <stdint.h>

#if defined(__cplusplus)
extern "C" {
#endif

static inline int32_t UnityPalCompareExchange(volatile int32_t* dest, int32_t exchange, int32_t comparand);
static inline int64_t UnityPalCompareExchange64(volatile int64_t* dest, int64_t exchange, int64_t comparand);
static inline void* UnityPalCompareExchangePointer(void* volatile* dest, void* exchange, void* comparand);
static inline int32_t UnityPalAdd(volatile int32_t* location1, int32_t value);
static inline int32_t UnityPalIncrement(volatile int32_t* value);
static inline int32_t UnityPalDecrement(volatile int32_t* value);
static inline int32_t UnityPalExchange(volatile int32_t* dest, int32_t exchange);
static inline void* UnityPalExchangePointer(void* volatile* dest, void* exchange);
static inline int64_t UnityPalRead64(volatile int64_t* addr);

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
static inline int64_t UnityPalAdd64(volatile int64_t* location1, int64_t value);
static inline int64_t UnityPalIncrement64(volatile int64_t* value);
static inline int64_t UnityPalDecrement64(volatile int64_t* value);
static inline int64_t UnityPalExchange64(volatile int64_t* dest, int64_t exchange);
#endif

#if defined(__cplusplus)
}
#endif

#if !IL2CPP_SUPPORT_THREADS

inline int32_t UnityPalAdd(volatile int32_t* location1, int32_t value)
{
    return *location1 += value;
}

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
inline int64_t UnityPalAdd64(volatile int64_t* location1, int64_t value)
{
    return *location1 += value;
}

#endif

inline int32_t UnityPalIncrement(volatile int32_t* value)
{
    return ++(*value);
}

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
inline int64_t UnityPalIncrement64(volatile int64_t* value)
{
    return ++(*value);
}

#endif

inline int32_t UnityPalDecrement(volatile int32_t* value)
{
    return --(*value);
}

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
inline int64_t UnityPalDecrement64(volatile int64_t* value)
{
    return --(*value);
}

#endif

inline int32_t UnityPalCompareExchange(volatile int32_t* dest, int32_t exchange, int32_t comparand)
{
    int32_t orig = *dest;
    if (*dest == comparand)
        *dest = exchange;

    return orig;
}

inline int64_t UnityPalCompareExchange64(volatile int64_t* dest, int64_t exchange, int64_t comparand)
{
    int64_t orig = *dest;
    if (*dest == comparand)
        *dest = exchange;

    return orig;
}

inline void* UnityPalCompareExchangePointer(void* volatile* dest, void* exchange, void* comparand)
{
    void* orig = *dest;
    if (*dest == comparand)
        *dest = exchange;

    return orig;
}

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
inline int64_t UnityPalExchange64(volatile int64_t* dest, int64_t exchange)
{
    int64_t orig = *dest;
    *dest = exchange;
    return orig;
}

#endif

inline int32_t UnityPalExchange(volatile int32_t* dest, int32_t exchange)
{
    int32_t orig = *dest;
    *dest = exchange;
    return orig;
}

inline void* UnityPalExchangePointer(void* volatile* dest, void* exchange)
{
    void* orig = *dest;
    *dest = exchange;
    return orig;
}

int64_t UnityPalRead64(volatile int64_t* addr)
{
    return *addr;
}

#elif IL2CPP_TARGET_WINDOWS
#include "Win32/AtomicImpl-c-api.h"
#elif IL2CPP_TARGET_PS4
#include "PS4/AtomicImpl-c-api.h"  // has to come earlier than posix
#elif IL2CPP_TARGET_PSP2
#include "PSP2/AtomicImpl-c-api.h"
#elif IL2CPP_TARGET_POSIX
#include "Posix/AtomicImpl-c-api.h"
#else
#include "AtomicImpl-c-api.h"
#endif
