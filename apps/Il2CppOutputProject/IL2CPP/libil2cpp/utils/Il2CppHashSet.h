#pragma once

#include "../../external/google/sparsehash/dense_hash_set.h"
#include "KeyWrapper.h"

template<class Value,
         class HashFcn,
         class EqualKey = std::equal_to<Value>,
         class Alloc = std::allocator<KeyWrapper<Value> > >
class Il2CppHashSet : public dense_hash_set<KeyWrapper<Value>, HashFcn, typename KeyWrapper<Value>::template EqualsComparer<EqualKey>, Alloc>
{
private:
    typedef dense_hash_set<KeyWrapper<Value>, HashFcn, typename KeyWrapper<Value>::template EqualsComparer<EqualKey>, Alloc> Base;

public:
    typedef typename Base::size_type size_type;
    typedef typename Base::hasher hasher;
    typedef typename Base::key_equal key_equal;
    typedef typename Base::key_type key_type;

    explicit Il2CppHashSet(size_type n = 0,
                           const hasher& hf = hasher(),
                           const EqualKey& eql = EqualKey()) :
        Base(n, hf, key_equal(eql))
    {
        Base::set_empty_key(key_type(key_type::KeyType_Empty));
        Base::set_deleted_key(key_type(key_type::KeyType_Deleted));
    }

    template<class InputIterator>
    Il2CppHashSet(InputIterator f, InputIterator l,
                  size_type n = 0,
                  const hasher& hf = hasher(),
                  const EqualKey& eql = EqualKey()) :
        Base(f, l, n, hf, key_equal(eql))
    {
        Base::set_empty_key(key_type(key_type::KeyType_Empty));
        Base::set_deleted_key(key_type(key_type::KeyType_Deleted));
    }
};
