#pragma once

#include "os/Unity/AtomicQueue.h"

namespace il2cpp
{
namespace utils
{
    struct ThreadSafeFreeListNode
    {
        ThreadSafeFreeListNode* nextFreeListNode;

        ThreadSafeFreeListNode()
            : nextFreeListNode(NULL) {}
    };

/// Lockless allocator that keeps instances of T on a free list.
///
/// T must be derived from ThreadSafeFreeListNode.
///
/// NOTE: T must have sizeof(T) >= sizeof(void*).
    template<typename T>
    struct ThreadSafeFreeList
    {
        T* Allocate()
        {
            T* instance = reinterpret_cast<T*>(m_FreeList.Pop());
            if (!instance)
                instance = new T();

            return instance;
        }

        void Release(T* instance)
        {
            ThreadSafeFreeListNode* node = static_cast<ThreadSafeFreeListNode*>(instance);
            m_FreeList.Push(reinterpret_cast<il2cpp::os::AtomicNode*>(node));
        }

        ~ThreadSafeFreeList()
        {
            T* instance;
            while ((instance = reinterpret_cast<T*>(m_FreeList.Pop())) != NULL)
                delete instance;
        }

    private:

        ALIGN_TYPE(64) il2cpp::os::AtomicStack m_FreeList;
    };
} /* utils */
} /* il2cpp */
