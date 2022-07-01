// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Tests.Utilities
{
    public class LRUCacheUnitTests
    {
        [Test]
        public void LRUCache_Capacity_Test()
        {
            // Arrange
            var capacity = 5;
            var cache = new LRUCache<int, string>(capacity);

            // Assert
            Assert.AreEqual(capacity, cache.Capacity, "Incorrect cache capacity");
        }

        [Test]
        public void LRUCache_AddAndGetNewItems_Test()
        {
            // Arrange
            var capacity = 10;
            var cache = new LRUCache<int, string>(capacity);

            // Act
            AddItems(cache, capacity);

            // Assert
            Assert.AreEqual(capacity, cache.Count, "Incorrect item count.");

            for (var i = 0; i < capacity; i++)
            {
                // Act
                var result = cache.TryGetValue(i, out var retrievedValue);

                // Assert
                Assert.IsTrue(result, "Get operation should be successful.");
                Assert.AreEqual(i.ToString(), retrievedValue, "Incorrect item value retrieved.");
            }
        }

        [Test]
        public void LRUCache_AddAndIndexNewItems_Test()
        {
            // Arrange
            var capacity = 10;
            var cache = new LRUCache<int, string>(capacity);

            // Act
            for (var i = 0; i < capacity; i++)
            {
                cache[i] = i.ToString();
            }

            // Assert
            Assert.AreEqual(capacity, cache.Count, "Incorrect item count.");

            for (var i = 0; i < capacity; i++)
            {
                string retrievedValue = string.Empty;
                Assert.DoesNotThrow(() => retrievedValue = cache[i]);
                Assert.AreEqual(i.ToString(), retrievedValue, "Incorrect item value retrieved.");
            }

            Assert.Throws<KeyNotFoundException>(() => _ = cache[capacity]);
        }

        [Test]
        public void LRUCache_GetUncachedItem_Test()
        {
            // Arrange
            var cache = new LRUCache<int, string>(5);

            // Act
            var result = cache.TryGetValue(0, out var retrievedValue);

            // Assert
            Assert.IsFalse(result, "Get operation should be unsuccessful.");
            Assert.Null(retrievedValue, "Retrieved value should be null.");
        }

        [Test]
        public void LRUCache_AddItem_Test()
        {
            // Arrange
            var cache = new LRUCache<int, string>(5);

            // Act
            var string1 = "test string 1";
            cache.Add(0, string1);
            var result = cache.TryGetValue(0, out var retrievedValue);

            // Assert
            Assert.IsTrue(result, "Get operation should be successful.");
            Assert.AreEqual(string1, retrievedValue, "Incorrect cached value.");

            // Act
            var string2 = "test string 2";
            cache.Add(0, string2);
            result = cache.TryGetValue(0, out retrievedValue);

            // Assert
            Assert.IsTrue(result, "Get operation should be successful.");
            Assert.AreEqual(1, cache.Count, "There should only be one item in the cache.");
            Assert.AreEqual(string2, retrievedValue, "Incorrect cached value.");
        }

        [Test]
        public void LRUCache_RemoveItem_Test()
        {
            // Arrange
            var capacity = 5;
            var cache = new LRUCache<int, string>(capacity);
            var inCacheIndex = 4;
            var outOfCacheIndex = 20;

            // Act
            AddItems(cache, capacity);
            var didRemoveFirstItem = cache.Remove(inCacheIndex);
            var didRemoveSecondItem = cache.Remove(outOfCacheIndex);
            var result = cache.TryGetValue(5, out var retrievedValue);

            // Assert
            Assert.IsFalse(result, "Get operation should be unsuccessful.");
            Assert.Null(retrievedValue, "Retrieved value should be null.");
            Assert.IsTrue(didRemoveFirstItem, "First remove operation should be successful for in cache item.");
            Assert.IsFalse(didRemoveSecondItem, "Second remove operations should be unsuccessful for out of cache item.");
        }

        [Test]
        public void LRUCache_Clear_Test()
        {
            // Arrange
            var capacity = 5;
            var cache = new LRUCache<int, string>(capacity);

            // Act
            AddItems(cache, capacity);
            cache.Clear();

            // Assert
            for (var i = 0; i < capacity; i++)
            {
                var result = cache.TryGetValue(i, out var retrievedValue);
                Assert.IsFalse(result, "Get operation should be unsuccessful.");
                Assert.Null(retrievedValue, "Retrieved value should be null after clearing cache.");
            }

            Assert.AreEqual(0, cache.Count, "Cache should be empty.");
        }

        [Test]
        public void LRUCache_EntryOrder_AddNewItem_Test()
        {
            // Arrange
            var capacity = 5;
            var cache = new LRUCache<int, string>(capacity);

            // Act
            // Fill cache with sequential from 0 - 4, then add another entry 10 to cause an eviction of least recent entry
            AddItems(cache, capacity);
            cache.Add(10, "10");

            // Assert
            // Expected in cache item (Most recent -> least recent): 10, 4, 3, 2, 1
            Assert.AreEqual(capacity, cache.Count, "Cache count should be full.");
            VerifyEntrySequence(cache, 10, 4, 3, 2, 1);
        }

        [Test]
        public void LRUCache_EntryOrder_GetLastEntryThenAddNewItem_Test()
        {
            // Arrange
            var capacity = 5;
            var cache = new LRUCache<int, string>(capacity);

            // Act
            // Access the current least recent entry to bring it to the front, then add another entry to push out the least recent
            // Expected in cache item (Most recent -> least recent): 10, 0, 4, 3, 2
            AddItems(cache, capacity);
            cache.TryGetValue(0, out var _);
            cache.Add(10, "10");

            // Assert
            // Expected in cache items: 10, 0, 4, 3, 2
            Assert.AreEqual(capacity, cache.Capacity, "Cache capacity doesn't match the constructor.");
            Assert.AreEqual(capacity, cache.Count, "Cache count should be full.");
            VerifyEntrySequence(cache, 10, 0, 4, 3, 2);
        }

        [Test]
        public void LRUCache_EntryOrder_ResetLastEntryThenAddNewItem_Test()
        {
            // Arrange
            var capacity = 5;
            var cache = new LRUCache<int, string>(capacity);

            // Act
            // Add the least recent entry to bring it to the front, then add another entry to push out the least recent.
            AddItems(cache, capacity);
            cache.Add(0, 0.ToString());
            cache.Add(10, "10");

            // Assert
            // Expected in cache item (Most recent -> least recent): 10, 0, 4, 3, 2
            Assert.AreEqual(capacity, cache.Capacity, "Cache capacity doesn't match the constructor.");
            Assert.AreEqual(capacity, cache.Count, "Cache should be at max capacity.");
            VerifyEntrySequence(cache, 10, 0, 4, 3, 2);
        }

        [Test]
        public void LRUCache_EntryOrder_ReverseEntryAccess_Test()
        {
            // Arrange
            var capacity = 10;
            var cache = new LRUCache<int, string>(capacity);

            // Act
            // Set items in sequential order then access in reverse
            AddItems(cache, capacity);
            for (var i = capacity - 1; i >= 0; i--)
            {
                var result = cache.TryGetValue(i, out var retrievedValue);

                // Assert
                Assert.IsTrue(result, "Get operation should be successful.");
                Assert.AreEqual(i.ToString(), retrievedValue, "Cache entry value mismatch.");
            }

            // Assert
            // Entry in cache should now be in increasing order
            VerifyEntrySequence(cache, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        }

        private void AddItems(LRUCache<int, string> cache, int numEntryToAdd)
        {
            for (var i = 0; i < numEntryToAdd; i++)
            {
                cache.Add(i, i.ToString());
            }
        }

        private void VerifyEntrySequence(LRUCache<int, string> cache, params int[] entryKeySequence)
        {
            var entryList = cache.ToList();

            Assert.AreEqual(entryKeySequence.Length, cache.Count, "Cached item count does not match expected item count.");

            var index = 0;
            foreach (var kvp in entryList)
            {
                Assert.AreEqual(entryKeySequence[index++], kvp.Key, "Cached item sequence does not meet expected sequence.");
            }
        }
    }
}
