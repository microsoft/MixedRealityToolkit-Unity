#pragma once

#include <stdint.h>
#include "utils/NonCopyable.h"
#include "c-api/Atomic-c-api.h"

namespace il2cpp
{
namespace os
{
    class Atomic : public il2cpp::utils::NonCopyable
    {
    public:
        // All 32bit atomics must be performed on 4-byte aligned addresses. All 64bit atomics must be
        // performed on 8-byte aligned addresses.

        // Add and Add64 return the *result* of the addition, not the old value! (i.e. they work like
        // InterlockedAdd and __sync_add_and_fetch).

        static inline void FullMemoryBarrier();

        static inline int32_t Add(volatile int32_t* location1, int32_t value)
        {
            return UnityPalAdd(location1, value);
        }

        static inline uint32_t Add(volatile uint32_t* location1, uint32_t value)
        {
            return (uint32_t)Add((volatile int32_t*)location1, (int32_t)value);
        }

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
        static inline int64_t Add64(volatile int64_t* location1, int64_t value)
        {
            return UnityPalAdd64(location1, value);
        }

#endif

        template<typename T>
        static inline T* CompareExchangePointer(T* volatile* dest, T* newValue, T* oldValue)
        {
            return static_cast<T*>(UnityPalCompareExchangePointer((void*volatile*)dest, newValue, oldValue));
        }

        template<typename T>
        static inline T* ExchangePointer(T* volatile* dest, T* newValue)
        {
            return static_cast<T*>(UnityPalExchangePointer((void*volatile*)dest, newValue));
        }

        static inline int64_t Read64(volatile int64_t* addr)
        {
            return UnityPalRead64(addr);
        }

        static inline uint64_t Read64(volatile uint64_t* addr)
        {
            return (uint64_t)Read64((volatile int64_t*)addr);
        }

        template<typename T>
        static inline T* ReadPointer(T* volatile* pointer)
        {
        #if IL2CPP_SIZEOF_VOID_P == 4
            return reinterpret_cast<T*>(Add(reinterpret_cast<volatile int32_t*>(pointer), 0));
        #else
            return reinterpret_cast<T*>(Read64(reinterpret_cast<volatile int64_t*>(pointer)));
        #endif
        }

        static inline int32_t Increment(volatile int32_t* value)
        {
            return UnityPalIncrement(value);
        }

        static inline uint32_t Increment(volatile uint32_t* value)
        {
            return (uint32_t)Increment((volatile int32_t*)value);
        }

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
        static inline int64_t Increment64(volatile int64_t* value)
        {
            return UnityPalIncrement64(value);
        }

        static inline uint64_t Increment64(volatile uint64_t* value)
        {
            return (uint64_t)Increment64((volatile int64_t*)value);
        }

#endif

        static inline int32_t Decrement(volatile int32_t* value)
        {
            return UnityPalDecrement(value);
        }

        static inline uint32_t Decrement(volatile uint32_t* value)
        {
            return (uint32_t)Decrement((volatile int32_t*)value);
        }

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
        static inline int64_t Decrement64(volatile int64_t* value)
        {
            return UnityPalDecrement64(value);
        }

        static inline uint64_t Decrement64(volatile uint64_t* value)
        {
            return (uint64_t)Decrement64((volatile int64_t*)value);
        }

#endif

        static inline int32_t CompareExchange(volatile int32_t* dest, int32_t exchange, int32_t comparand)
        {
            return UnityPalCompareExchange(dest, exchange, comparand);
        }

        static inline uint32_t CompareExchange(volatile uint32_t* value, uint32_t newValue, uint32_t oldValue)
        {
            return (uint32_t)CompareExchange((volatile int32_t*)value, newValue, oldValue);
        }

        static inline int64_t CompareExchange64(volatile int64_t* dest, int64_t exchange, int64_t comparand)
        {
            return UnityPalCompareExchange64(dest, exchange, comparand);
        }

        static inline uint64_t CompareExchange64(volatile uint64_t* value, uint64_t newValue, uint64_t oldValue)
        {
            return (uint64_t)CompareExchange64((volatile int64_t*)value, newValue, oldValue);
        }

        static inline int32_t Exchange(volatile int32_t* dest, int32_t exchange)
        {
            return UnityPalExchange(dest, exchange);
        }

        static inline uint32_t Exchange(volatile uint32_t* value, uint32_t newValue)
        {
            return (uint32_t)Exchange((volatile int32_t*)value, newValue);
        }

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
        static inline int64_t Exchange64(volatile int64_t* dest, int64_t exchange)
        {
            return UnityPalExchange64(dest, exchange);
        }

        static inline uint64_t Exchange64(volatile uint64_t* value, uint64_t newValue)
        {
            return (uint64_t)Exchange64((volatile int64_t*)value, newValue);
        }

#endif
    };
}
}

#if !IL2CPP_SUPPORT_THREADS

namespace il2cpp
{
namespace os
{
    inline void Atomic::FullMemoryBarrier()
    {
        // Do nothing.
    }
}
}

#elif IL2CPP_TARGET_WINDOWS
#include "os/Win32/AtomicImpl.h"
#elif IL2CPP_TARGET_PS4
#include "os/AtomicImpl.h"  // has to come earlier than posix
#elif IL2CPP_TARGET_PSP2
#include "os/PSP2/AtomicImpl.h"
#elif IL2CPP_TARGET_POSIX
#include "os/Posix/AtomicImpl.h"
#else
#include "os/AtomicImpl.h"
#endif
