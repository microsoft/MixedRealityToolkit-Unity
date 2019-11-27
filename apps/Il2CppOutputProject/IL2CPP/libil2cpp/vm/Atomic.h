#pragma once

#include <stdint.h>
#include "il2cpp-config.h"

namespace il2cpp
{
namespace vm
{
    class LIBIL2CPP_CODEGEN_API Atomic
    {
    public:
        static int32_t Add(volatile int32_t* location1, int32_t value);
        static int64_t Add64(volatile int64_t* location1, int64_t value);
        static int32_t Increment(volatile int32_t* value);
        static int64_t Increment64(volatile int64_t* value);
        static int32_t Decrement(volatile int32_t* value);
        static int64_t Decrement64(volatile int64_t* value);
        static int32_t CompareExchange(volatile int32_t* dest, int32_t exchange, int32_t comparand);
        static int64_t CompareExchange64(volatile int64_t* dest, int64_t exchange, int64_t comparand);
        static void* CompareExchangePointer(void* volatile* dest, void* exchange, void* comparand);
        static int32_t Exchange(volatile int32_t* dest, int32_t exchange);
        static int64_t Exchange64(volatile int64_t* dest, int64_t exchange);
        static void* ExchangePointer(void* volatile* dest, void* exchange);
        static int64_t Read64(volatile int64_t* addr);
        static void FullMemoryBarrier();

        static inline uint32_t Add(volatile uint32_t* location1, uint32_t value)
        {
            return static_cast<uint32_t>(Add((volatile int32_t*)location1, (int32_t)value));
        }

        template<typename T>
        static inline T* CompareExchangePointer(T* volatile* dest, T* newValue, T* oldValue)
        {
            return static_cast<T*>(CompareExchangePointer((void*volatile*)dest, newValue, oldValue));
        }

        template<typename T>
        static inline T* ExchangePointer(T* volatile* dest, T* newValue)
        {
            return static_cast<T*>(ExchangePointer((void*volatile*)dest, newValue));
        }

        static inline uint64_t Read64(volatile uint64_t* addr)
        {
            return static_cast<uint64_t>(Read64((volatile int64_t*)addr));
        }

        template<typename T>
        static inline T* ReadPointer(T* volatile* pointer)
        {
#if IL2CPP_SIZEOF_VOID_P == 4
            return reinterpret_cast<T*>(Add(reinterpret_cast<volatile int32_t*>(pointer), 0));
#else
            return reinterpret_cast<T*>(Add64(reinterpret_cast<volatile int64_t*>(pointer), 0));
#endif
        }

        static inline uint32_t Increment(volatile uint32_t* value)
        {
            return static_cast<uint32_t>(Increment(reinterpret_cast<volatile int32_t*>(value)));
        }

        static inline uint64_t Increment64(volatile uint64_t* value)
        {
            return static_cast<uint64_t>(Increment64(reinterpret_cast<volatile int64_t*>(value)));
        }

        static inline uint32_t Decrement(volatile uint32_t* value)
        {
            return static_cast<uint32_t>(Decrement(reinterpret_cast<volatile int32_t*>(value)));
        }

        static inline uint64_t Decrement64(volatile uint64_t* value)
        {
            return static_cast<uint64_t>(Decrement64(reinterpret_cast<volatile int64_t*>(value)));
        }

        static inline uint32_t CompareExchange(volatile uint32_t* value, uint32_t newValue, uint32_t oldValue)
        {
            return static_cast<uint32_t>(CompareExchange(reinterpret_cast<volatile int32_t*>(value), newValue, oldValue));
        }

        static inline uint64_t CompareExchange64(volatile uint64_t* value, uint64_t newValue, uint64_t oldValue)
        {
            return static_cast<uint64_t>(CompareExchange64(reinterpret_cast<volatile int64_t*>(value), newValue, oldValue));
        }

        static inline uint32_t Exchange(volatile uint32_t* value, uint32_t newValue)
        {
            return static_cast<uint32_t>(Exchange(reinterpret_cast<volatile int32_t*>(value), newValue));
        }

        static inline uint64_t Exchange64(volatile uint64_t* value, uint64_t newValue)
        {
            return static_cast<uint64_t>(Exchange64(reinterpret_cast<volatile int64_t*>(value), newValue));
        }
    };
} /* namesapce vm */
} /* namespace il2cpp */
