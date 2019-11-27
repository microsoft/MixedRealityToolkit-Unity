#pragma once

#include "../../external/google/sparsehash/dense_hash_map.h"
#include "KeyWrapper.h"

template<class Key, class T,
         class HashFcn,
         class EqualKey = std::equal_to<Key>,
         class Alloc = std::allocator<std::pair<const KeyWrapper<Key>, T> > >
class Il2CppHashMap : public dense_hash_map<KeyWrapper<Key>, T, HashFcn, typename KeyWrapper<Key>::template EqualsComparer<EqualKey>, Alloc>
{
private:
    typedef dense_hash_map<KeyWrapper<Key>, T, HashFcn, typename KeyWrapper<Key>::template EqualsComparer<EqualKey>, Alloc> Base;

public:
    typedef typename Base::size_type size_type;
    typedef typename Base::hasher hasher;
    typedef typename Base::key_equal key_equal;
    typedef typename Base::key_type key_type;

    explicit Il2CppHashMap(size_type n = 0,
                           const hasher& hf = hasher(),
                           const EqualKey& eql = EqualKey()) :
        Base(n, hf, key_equal(eql))
    {
        Base::set_empty_key(key_type(key_type::KeyType_Empty));
        Base::set_deleted_key(key_type(key_type::KeyType_Deleted));
    }

    template<class InputIterator>
    Il2CppHashMap(InputIterator f, InputIterator l,
                  size_type n = 0,
                  const hasher& hf = hasher(),
                  const EqualKey& eql = EqualKey()) :
        Base(f, l, n, hf, key_equal(eql))
    {
        Base::set_empty_key(key_type(key_type::KeyType_Empty));
        Base::set_deleted_key(key_type(key_type::KeyType_Deleted));
    }

    void add(const key_type& key, const T& value)
    {
        Base::insert(std::make_pair(key, value));
    }
};
