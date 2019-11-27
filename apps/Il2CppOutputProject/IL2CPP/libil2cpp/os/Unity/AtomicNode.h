#pragma once

#include "ExtendedAtomicTypes.h"

UNITY_PLATFORM_BEGIN_NAMESPACE;

class AtomicNode
{
    friend class AtomicStack;
    friend class AtomicQueue;
    friend class MutexLockedStack;
    friend class MutexLockedQueue;

    volatile atomic_word _next;

public:
    void* data[3];

    AtomicNode *Next() const
    {
        return (AtomicNode*)_next;
    }

    AtomicNode *Link(AtomicNode *next);
};

UNITY_PLATFORM_END_NAMESPACE;
