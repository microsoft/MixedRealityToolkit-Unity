// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// An object cache where items can be accessed by key and the least recently used item is evicted when the cache exceeds
    /// its capacity.
    /// </summary>
    /// <remarks>
    /// Implementation involves a dictionary for fast entry lookup, and a doubly linked list to track recent access.
    /// </remarks>
    /// <typeparam name="TKey">The type that is used as key to look up values in the cache.</typeparam>
    /// <typeparam name="TValue">The type of values stored in the cache.</typeparam>
    internal class LRUCache<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Data structure used to construct a linked list of cached items in temporal order.
        /// </summary>
        private class CacheEntry
        {
            /// <summary>
            /// Cached item key.
            /// </summary>
            public TKey Key { get; set; }

            /// <summary>.
            /// Cached item value
            /// </summary>
            public TValue Value { get; set; }

            /// <summary>
            /// Reference to previous cache entry.
            /// </summary>
            public CacheEntry Previous { get; set; }

            /// <summary>
            /// Reference to next cache entry.
            /// </summary>
            public CacheEntry Next { get; set; }

            public CacheEntry(TKey key, TValue item)
            {
                Key = key;
                Value = item;
            }
        }

        // Dictionary for key value entry lookup
        private readonly Dictionary<TKey, CacheEntry> keyToEntryTable = new Dictionary<TKey, CacheEntry>();

        // The least recent entry, stored as the tail of the doubly linked list
        private CacheEntry leastRecentEntry = null;

        // The most recent entry, stored as the head of the doubly linked list
        private CacheEntry mostRecentEntry = null;

        /// <summary>
        /// Constructs a new <see cref="LRUCache"/> with a given capacity.
        /// </summary>
        /// <param name="capacity">The maximum number items that may be cached.</param>
        public LRUCache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException("Cache capacity must be greater than 0");
            }

            Capacity = capacity;
        }

        /// <summary>
        /// Gets the maximum number items that may be cached. If items are added beyond this capacity, then the least
        /// recently used entry is evicted.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Gets the number of items currently cached. This value will never exceed <see cref="Capacity"/>
        /// </summary>
        public int Count => keyToEntryTable.Count;

        /// <summary>
        /// Gets or sets the item with the associated key.
        /// </summary>
        /// <exception cref="KeyNotFoundException"/>
        /// The property is retrieved and <paramref name="key"/> is not present in the cache.
        /// </exception>
        /// <exception cref="ArgumentNullException"/>
        /// <paramref name="key"/> is null.
        /// </exception>
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new KeyNotFoundException($"Item with key '{key}' is not cached");
            }

            set => Add(key, value);
        }

        /// <summary>
        /// Gets a collection containing keys to cached items in order of most to least recently used.
        /// </summary>
        public ICollection<TKey> Keys => this.Select(kv => kv.Key).ToList();

        /// <summary>
        /// Gets a collection containing the cached items in order of most to least recently used.
        /// </summary>
        public ICollection<TValue> Values => this.Select(kv => kv.Value).ToList();

        /// <summary>
        /// Try to retrieve a cached item by key. Getting a cached item by key will update the most recent access status of
        /// the item, bringing it to the front of the recent access list.
        /// </summary>
        /// <param name="key">The key for the cached item.</param>
        /// <param name="value">The value for the cached item.</param>
        /// <returns>True if the cached item exists in the cache, false if the item does not exist.</returns>
        /// <exception>
        /// <see cref="ArgumentNullException"/> if <paramref name="key"/> is null.
        /// </exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (keyToEntryTable.TryGetValue(key, out CacheEntry cacheEntry))
            {
                MakeCacheEntryRecent(cacheEntry);
                value = cacheEntry.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public bool ContainsKey(TKey key) => keyToEntryTable.ContainsKey(key);

        /// <summary>
        /// Adds an item to the cache. If the item key doesn't exist, the cache generates a new entry to store the item.
        /// If the key already exists, then the cached entry is updated with the new item value. In either case, the entry
        /// is brought to the front of the recent access list and if the new <see cref="Count"/> exceeds
        /// <see cref="Capacity"/> then the item lest recently accessed is evicted.
        /// </summary>
        /// <param name="key">The key for the item to cache</param>
        /// <param name="value">The value for the item to cache</param>
        /// <exception>
        /// <see cref="ArgumentNullException"/> if <paramref name="key"/> is null.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            // Retrieve of create cached entry
            if (keyToEntryTable.TryGetValue(key, out CacheEntry cacheEntry))
            {
                cacheEntry.Value = value;
            }
            else
            {
                if (keyToEntryTable.Count >= Capacity && leastRecentEntry != null)
                {
                    // Handle overcapacity by removing least recent entry
                    keyToEntryTable.Remove(leastRecentEntry.Key);

                    // Cache the new leastRecentEntry before reuse
                    CacheEntry newLeastRecentEntry = leastRecentEntry.Previous;

                    // Reuse the leastRecentEntry CacheEntry to avoid new allocations
                    cacheEntry = leastRecentEntry;
                    cacheEntry.Key = key;
                    cacheEntry.Value = value;
                    cacheEntry.Next = null;
                    cacheEntry.Previous = null;

                    // Rotate leastRecentEntry
                    leastRecentEntry = newLeastRecentEntry;
                    leastRecentEntry.Next = null;
                }
                else
                {
                    cacheEntry = new CacheEntry(key, value);
                    // Assign least recent entry if this is the first entry in the cache
                    if (leastRecentEntry == null)
                    {
                        leastRecentEntry = cacheEntry;
                    }
                }
                keyToEntryTable.Add(key, cacheEntry);
            }

            // Make the entry the most recently accessed entry in the list
            MakeCacheEntryRecent(cacheEntry);
        }

        /// <summary>
        /// Removes an entry from the cache by its key.
        /// </summary>
        /// <param name="key">The key for the cached item.</param>
        /// <returns>True if the item exists, false if item does not exists in the cache.</returns>
        /// <exception>
        /// <see cref="ArgumentNullException"/> if <paramref name="key"/> is null.
        /// </exception>
        public bool Remove(TKey key)
        {
            if (keyToEntryTable.TryGetValue(key, out var cacheEntry))
            {
                keyToEntryTable.Remove(cacheEntry.Key);
                RemoveEntryFromList(cacheEntry);

                // update most recent entry
                if (cacheEntry == mostRecentEntry)
                {
                    mostRecentEntry = cacheEntry.Next;
                }

                // update least recent entry
                if (cacheEntry == leastRecentEntry)
                {
                    leastRecentEntry = cacheEntry.Previous;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Clear all entry out of the cache.
        /// </summary>
        public void Clear()
        {
            // Clear all entry stored in the dictionary.
            keyToEntryTable.Clear();

            // Iterate through the list and unhook all the pointers in the list.
            while (mostRecentEntry != null)
            {
                var nextEntry = mostRecentEntry.Next;
                mostRecentEntry.Next = null;

                if (nextEntry != null)
                {
                    nextEntry.Previous = null;
                }

                mostRecentEntry = nextEntry;
            }

            leastRecentEntry = null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through cached items in order of most to least recently used.
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (keyToEntryTable.Count == 0)
            {
                yield break;
            }

            for (var entry = mostRecentEntry; entry != null; entry = entry.Next)
            {
                yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Update the doubly linked list to bring the given entry to the front of the list, with the front being the
        /// most recently access entry.
        /// </summary>
        /// <param name="entry">The entry to bring to the front of the list</param>
        private void MakeCacheEntryRecent(CacheEntry entry)
        {
            // Only perform operation if the entry is not already the most recently accessed item
            if (entry != mostRecentEntry)
            {
                RemoveEntryFromList(entry);

                // Update leastRecentyEntry value if necessary
                if (entry == leastRecentEntry && entry.Previous != null)
                {
                    leastRecentEntry = entry.Previous;
                }

                // Move entry to the front of the list
                entry.Previous = null;
                entry.Next = mostRecentEntry;

                if (mostRecentEntry != null)
                {
                    mostRecentEntry.Previous = entry;
                }

                // Mark entry as the most recent entry
                mostRecentEntry = entry;
            }
        }

        /// <summary>
        /// Remove an entry from the doubly linked list.
        /// </summary>
        /// <param name="entry">The entry to remove from the list.</param>
        private void RemoveEntryFromList(CacheEntry entry)
        {
            // Point the next entry pointer in the previous entry to point to the next entry in the list
            if (entry.Previous != null)
            {
                entry.Previous.Next = entry.Next;
            }

            // Point the prev entry pointer in the next entry to the prev entry in the list
            if (entry.Next != null)
            {
                entry.Next.Previous = entry.Previous;
            }
        }

        // ICollection implementation
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get; } = false;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            => Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            => TryGetValue(item.Key, out var value) && Equals(value, item.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => Remove(item.Key);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            foreach (var kv in this)
            {
                array[index++] = kv;
            }
        }

        // IEnumerable implementation
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
