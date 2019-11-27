#pragma once

#include <stdint.h>
#ifndef NOMINMAX
#define NOMINMAX
#endif
#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN 1
#endif
#define INC_OLE2 1
#include <intrin.h>

#if defined(__cplusplus)
extern "C" {
#endif

static inline int32_t UnityPalCompareExchange(volatile int32_t* dest, int32_t exchange, int32_t comparand)
{
    return _InterlockedCompareExchange((long volatile*)dest, exchange, comparand);
}

static inline int64_t UnityPalCompareExchange64(volatile int64_t* dest, int64_t exchange, int64_t comparand)
{
    return _InterlockedCompareExchange64((long long volatile*)dest, exchange, comparand);
}

static inline void* UnityPalCompareExchangePointer(void* volatile* dest, void* exchange, void* comparand)
{
  #if defined(_M_IX86)
    return (void*)_InterlockedCompareExchange((long volatile*)dest, (int32_t)exchange, (int32_t)comparand);
  #else
    return _InterlockedCompareExchangePointer(dest, exchange, comparand);
  #endif
}

static inline int32_t UnityPalAdd(volatile int32_t* location1, int32_t value)
{
    return (_InterlockedExchangeAdd((long volatile*)location1, value) + value);
}

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
static inline int64_t UnityPalAdd64(volatile int64_t* location1, int64_t value)
{
    return (_InterlockedExchangeAdd64((long long volatile*)location1, value) + value);
}

#endif

static inline int32_t UnityPalIncrement(volatile int32_t* value)
{
    return _InterlockedIncrement((long volatile*)value);
}

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
static inline int64_t UnityPalIncrement64(volatile int64_t* value)
{
    return _InterlockedIncrement64(value);
}

#endif

static inline int32_t UnityPalDecrement(volatile int32_t* value)
{
    return _InterlockedDecrement((long volatile*)value);
}

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
static inline int64_t UnityPalDecrement64(volatile int64_t* value)
{
    return _InterlockedDecrement64((long long volatile*)value);
}

static inline int64_t UnityPalExchange64(volatile int64_t* dest, int64_t exchange)
{
    return _InterlockedExchange64(dest, exchange);
}

#endif

static inline int32_t UnityPalExchange(volatile int32_t* dest, int32_t exchange)
{
    return _InterlockedExchange((long volatile*)dest, exchange);
}

static  inline void* UnityPalExchangePointer(void* volatile* dest, void* exchange)
{
#if defined(_M_IX86)
    return (void*)_InterlockedExchange((long volatile*)dest, (int32_t)exchange);
#else
    return _InterlockedExchangePointer(dest, exchange);
#endif
}

static inline int64_t UnityPalRead64(volatile int64_t* addr)
{
    return _InterlockedCompareExchange64((long long volatile*)addr, 0, 0);
}

#if defined(__cplusplus)
}
#endif
