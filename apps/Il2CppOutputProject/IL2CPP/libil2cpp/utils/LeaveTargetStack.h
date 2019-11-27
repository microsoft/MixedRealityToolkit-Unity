#pragma once

#include <stdint.h>

namespace il2cpp
{
namespace utils
{
    class LeaveTargetStack
    {
    public:
        LeaveTargetStack(void* storage) : m_Storage((int32_t*)storage), m_currentIndex(-1)
        {
        }

        void push(int32_t value)
        {
            // This function is rather unsafe. We don't track the size of storage,
            // and assume the caller will not push more values than it has allocated.
            // This function should only be used from generated code, where
            // we control the calls to this function.
            m_currentIndex++;
            m_Storage[m_currentIndex] = value;
        }

        void pop()
        {
            if (m_currentIndex >= 0)
                m_currentIndex--;
        }

        int32_t top() const
        {
            return m_Storage[m_currentIndex];
        }

        bool empty() const
        {
            return m_currentIndex == -1;
        }

    private:
        int32_t* m_Storage;
        int m_currentIndex;
    };
}
}
