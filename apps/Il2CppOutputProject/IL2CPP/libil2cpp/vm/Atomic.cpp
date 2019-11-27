#include "il2cpp-config.h"
#include "vm/Atomic.h"
#include "os/Atomic.h"

namespace il2cpp
{
namespace vm
{
    int32_t Atomic::Add(volatile int32_t* location1, int32_t value)
    {
        return os::Atomic::Add(location1, value);
    }

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
    int64_t Atomic::Add64(volatile int64_t* location1, int64_t value)
    {
        return os::Atomic::Add64(location1, value);
    }

#endif

    int32_t Atomic::Increment(volatile int32_t* value)
    {
        return os::Atomic::Increment(value);
    }

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
    int64_t Atomic::Increment64(volatile int64_t* value)
    {
        return os::Atomic::Increment64(value);
    }

#endif

    int32_t Atomic::Decrement(volatile int32_t* value)
    {
        return os::Atomic::Decrement(value);
    }

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
    int64_t Atomic::Decrement64(volatile int64_t* value)
    {
        return os::Atomic::Decrement64(value);
    }

#endif

    int32_t Atomic::CompareExchange(volatile int32_t* dest, int32_t exchange, int32_t comparand)
    {
        return os::Atomic::CompareExchange(dest, exchange, comparand);
    }

    int64_t Atomic::CompareExchange64(volatile int64_t* dest, int64_t exchange, int64_t comparand)
    {
        return os::Atomic::CompareExchange64(dest, exchange, comparand);
    }

    void* Atomic::CompareExchangePointer(void* volatile* dest, void* exchange, void* comparand)
    {
        return os::Atomic::CompareExchangePointer(dest, exchange, comparand);
    }

    int32_t Atomic::Exchange(volatile int32_t* dest, int32_t exchange)
    {
        return os::Atomic::Exchange(dest, exchange);
    }

#if IL2CPP_ENABLE_INTERLOCKED_64_REQUIRED_ALIGNMENT
    int64_t Atomic::Exchange64(volatile int64_t* dest, int64_t exchange)
    {
        return os::Atomic::Exchange64(dest, exchange);
    }

#endif

    void* Atomic::ExchangePointer(void* volatile* dest, void* exchange)
    {
        return os::Atomic::ExchangePointer(dest, exchange);
    }

    int64_t Atomic::Read64(volatile int64_t* addr)
    {
        return os::Atomic::Read64(addr);
    }

    void Atomic::FullMemoryBarrier()
    {
        os::Atomic::FullMemoryBarrier();
    }
} /* namespace vm */
} /* namespace il2pp */
