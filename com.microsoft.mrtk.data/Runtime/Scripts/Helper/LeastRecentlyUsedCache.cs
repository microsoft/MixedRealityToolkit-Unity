// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    public interface LRUValue<KT>
    {
        KT Key { get; set; }
    }

    public class LeastRecentlyUsedCache<KT, VT> where VT : LRUValue<KT>
    {
        // Actual list of index values in least recently to most recently used order.
        private LinkedList<VT> _lruList;

        // Makes it possible to efficiently find linked list node by index
        private Dictionary<KT, LinkedListNode<VT>> _lruIndexLookup;

        // Pre-allocated all nodes at initialize time to avoid allocs later.
        private Stack<LinkedListNode<VT>> _lruNodeFreePool;

        private int _maxEntries = 0;

        /// <summary>
        /// Create LRU cache with specified capacity
        /// </summary>
        /// <param name="capacity">How many entries to allow.</param>
        /// <param name="preallocate">Whether to preallocate space to avoid cost later.</param>
        public LeastRecentlyUsedCache(int capacity, bool preallocate = true)
        {
            _lruList = new LinkedList<VT>();

            // Make it efficient to look up an LRU entry by index without
            // walking the linked list
            _lruIndexLookup = new Dictionary<KT, LinkedListNode<VT>>(capacity);

            // create a pool of nodes in the free pool with value set to default for that type
            _lruNodeFreePool = new Stack<LinkedListNode<VT>>(capacity);
            _maxEntries = 0;
            SetSize(capacity, preallocate);
        }

        public void Clear()
        {
            while (_lruIndexLookup.Count > 0)
            {
                TryRemove(_lruIndexLookup.First().Key);
            }

            if (_lruIndexLookup.Count > 0 || _lruList.Count > 0)
            {
                Debug.LogError("LRU Cache clear failed.");
            }
        }

        /// <summary>
        /// Change the size of the cache.
        ///
        /// NOTE: You can only currently increase the size.
        /// </summary>
        /// <param name="newSize">The new size to make cache.</param>
        public void SetSize(int newSize, bool preallocate = true)
        {
            if (preallocate)
            {
                while (newSize > _maxEntries)
                {
                    _lruNodeFreePool.Push(new LinkedListNode<VT>(default(VT)));
                    _maxEntries++;
                }

            }
            else
            {
                _maxEntries = newSize;
            }

            // TODO: Consider adding ability to reduce size of pool
        }


        /// <summary>
        /// Add or update a value in the cache with associated key
        /// </summary>
        /// <remarks>
        /// The key can be used for efficient retrieval of any value, which
        /// then makes it the most recently used entry.
        ///
        /// If the key already exists, the value is updated and it is moved
        /// to the most recently used (newest) slot.
        ///
        /// By default it will be positioned as the newest value, but if
        /// the addAsNewest argument is false, it is also possible to add it
        /// as the oldest (and hence first to be reused). This is useful
        /// for putting already allocated items back in the list instead of
        /// destroying them as a speculative possibility that it MAY get
        /// reused in the future even though you're actually done with it
        /// for now.
        /// </remarks>
        /// <param name="key">A key associated with the value.</param>
        /// <param name="value">The value to add.</param>
        /// <param name="addAsNewest">If true add as the most recently used item. If false, add as oldest item.</param>
        public void AddOrUpdateValue(KT key, VT value, bool makeNewest = true)
        {
            if (makeNewest == true || _lruNodeFreePool.Count > 0)
            {
                value.Key = key;
                LinkedListNode<VT> indexNode = null;

                if (_lruIndexLookup.ContainsKey(key))
                {
                    // already there, but make it's the most recently used and the
                    // value is updated.
                    indexNode = _lruIndexLookup[key];
                    indexNode.Value = value;

                    _lruList.Remove(indexNode);
                }
                else if (_lruNodeFreePool.Count > 0)
                {
                    indexNode = _lruNodeFreePool.Pop();
                }
                else if (_lruNodeFreePool.Count + _lruList.Count < _maxEntries)
                {
                    indexNode = new LinkedListNode<VT>(default(VT));
                }
                else
                {
                    indexNode = _lruList.First;
                    _lruIndexLookup.Remove(indexNode.Value.Key);
                    _lruList.RemoveFirst(); // oldest
                }

                _lruIndexLookup[key] = indexNode;
                indexNode.Value = value;
                if (makeNewest)
                {
                    _lruList.AddLast(indexNode);    // make it most recently used
                }
                else
                {
                    _lruList.AddFirst(indexNode);   // make oldest
                }
            }

            if (_lruList.Count != _lruIndexLookup.Count)
            {
                Debug.LogError("Linked list and lookup are out of sync.");
            }
        }

        /// <summary>
        /// See if a value for a particular key exists in the cache.
        /// </summary>
        /// <param name="key">The key to check.</param>
        public bool ContainsKey(KT key)
        {
            return _lruIndexLookup.ContainsKey(key);
        }


        /// <summary>
        /// Find and return value by its key and make newest.
        /// </summary>
        /// <remarks>
        /// If the value is found, it is made the most recently used item and the
        /// value associated with that key is returned.
        /// </remarks>
        /// <param name="key">The key to find and return.</param>
        /// <returns>The value associated with that key.</returns>
        public VT FindByKey(KT key)
        {
            LinkedListNode<VT> indexNode = _lruIndexLookup[key];

            // make it the most recently used.
            _lruList.Remove(indexNode);
            _lruList.AddLast(indexNode);    // make it most recently used

            return indexNode.Value;
        }

        /// <summary>
        /// Get a particular value from the cache and remove it
        /// from the cache.
        /// </summary>
        /// <remarks>
        /// To keep an item in the cache </remarks>
        public VT PopByKey(KT key)
        {
            LinkedListNode<VT> indexNode = _lruIndexLookup[key];
            VT value = indexNode.Value;
            TryRemove(key);
            return value;
        }

        /// <summary>
        /// If a key is in the LRU cache, remove the value.
        /// </summary>
        public bool TryRemove(KT key)
        {
            if (_lruIndexLookup.ContainsKey(key))
            {
                // already there, but make it the most recently used.
                LinkedListNode<VT> indexNode = _lruIndexLookup[key];
                _lruList.Remove(indexNode);
                _lruIndexLookup.Remove(key);
                indexNode.Value = default(VT);
                _lruNodeFreePool.Push(indexNode);

                if (_lruList.Count != _lruIndexLookup.Count)
                {
                    Debug.LogError("Linked list and lookup are out of sync.");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsEmpty()
        {
            return _lruList.Count == 0;
        }

        /// <summary>
        /// Get the value of the oldest item.
        /// </summary>
        /// <remarks>
        /// This is useful if you want to free up another external resource that is identified
        /// by the value stored in this LRU cache.  This cache cen then be used to simply store identifiers
        /// of objects that are actually managed elsewhere.</remarks>
        public VT PeekOldestValue()
        {
            return _lruList.First.Value;
        }
    };
}
