// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data pool of objects that can be used for object re-use.
    ///
    /// This is designed to reduce memory allocations and hence
    /// the frequency and duration of garbage collections. It can
    /// also reduce the instantiation time of a prefab when populating
    /// large lists.
    /// </summary>
    public class DataObjectPool : IDataObjectPool
    {

        protected class LRUIndexList
        {
            // Actual list of index values in least recently to most recently used order.
            private LinkedList<int> _lruList;

            // Makes it possible to efficiently find linked list node by index
            private Dictionary<int, LinkedListNode<int>> _lruIndexLookup;

            // Pre-allocated all nodes at initialize time to avoid allocs later.
            private Stack<LinkedListNode<int>> _lruNodeFreePool;

            private int _maxEntries = 0;

            public LRUIndexList(int capacity)
            {
                _lruList = new LinkedList<int>();

                // Make it efficient to look up an LRU entry by index without
                // walking the linked list
                _lruIndexLookup = new Dictionary<int, LinkedListNode<int>>();

                // create a pool of nodes in the free pool with index set to -1
                _lruNodeFreePool = new Stack<LinkedListNode<int>>(capacity);
                _maxEntries = 0;
                SetSize(capacity);
            }

            public void SetSize(int newSize)
            {
                while (newSize > _maxEntries)
                {
                    _lruNodeFreePool.Push(new LinkedListNode<int>(-1));
                    _maxEntries++;
                }

                // TODO: Consider adding ability to reduce size of pool
            }

            public void AddIndex(int index, bool addAsLast = true)
            {
                // only add as oldest (addAsLast = false) if there's room for a new item
                if (index >= 0 && (addAsLast || _lruNodeFreePool.Count > 0))
                {
                    LinkedListNode<int> indexNode = null;

                    if (_lruIndexLookup.ContainsKey(index))
                    {
                        // already there, but make it the most recently used.
                        indexNode = _lruIndexLookup[index];
                        _lruList.Remove(indexNode);
                    }
                    else if (_lruNodeFreePool.Count > 0)
                    {
                        indexNode = _lruNodeFreePool.Pop();
                    }
                    else
                    {
                        indexNode = _lruList.First;
                        _lruList.RemoveFirst(); // oldest
                    }

                    _lruIndexLookup[index] = indexNode;
                    indexNode.Value = index;
                    if (addAsLast)
                    {
                        _lruList.AddLast(indexNode);    // make it most recently used
                    }
                    else
                    {
                        _lruList.AddFirst(indexNode);
                    }
                }
            }

            public void RemoveIndex(int index)
            {
                if (_lruIndexLookup.ContainsKey(index))
                {
                    // already there, but make it the most recently used.
                    LinkedListNode<int> indexNode = _lruIndexLookup[index];
                    _lruList.Remove(indexNode);
                    _lruIndexLookup.Remove(index);
                    indexNode.Value = -1;
                    _lruNodeFreePool.Push(indexNode);
                }
            }

            public int GetLeastRecentlyUsedIndex()
            {
                int index = -1;

                if (_lruList.Count > 0)
                {
                    index = _lruList.First.Value;
                    RemoveIndex(index);
                }

                return index;
            }
        };


        private const int DefaultPoolSize = 50;

        protected Queue<object> _objectPoolObjects = new Queue<object>();
        protected Dictionary<int, object> _prefetchedObjects = new Dictionary<int, object>();

        protected int _poolMaximumSize = DefaultPoolSize;

        // Keeps track of collecting indices in least recently used to most recently used order.
        protected LRUIndexList _lruIndexList = null;

        public DataObjectPool(int poolSize = DefaultPoolSize)
        {
            _poolMaximumSize = poolSize;
            _lruIndexList = new LRUIndexList(poolSize);
        }

        /// <inheritdoc/>
        public void SetMaximumPoolSize(int maxSize, bool resizeNow)
        {
            if (maxSize != _poolMaximumSize)
            {
                _lruIndexList.SetSize(maxSize);
                if (resizeNow)
                {

                    while (_objectPoolObjects.Count > maxSize)
                    {
                        if (_objectPoolObjects.Count > 0)
                        {
                            _objectPoolObjects.Dequeue();
                            // TODO: These objects should be passed back to caller to be properly disposed of.
                        }
                        else if (_prefetchedObjects.Count > 0)
                        {
                            // TODO: Start removing from prefetched items if needed to meet new, smaller pool size.
                        }
                    }
                }
                _poolMaximumSize = maxSize;
            }
        }

        /// <inheritdoc/>
        public bool IsEmpty()
        {
            return _objectPoolObjects.Count == 0 && _prefetchedObjects.Count == 0;
        }

        /// <summary>
        /// Determine if the pool is currently full and can't accept more objects.
        /// </summary>
        /// <remarks>
        /// It is considered full if it contains the maximum allowed combination of unused and prefetched objects.
        ///
        /// Note that if there are prefetched objects, it may be possible to reuse these.
        /// </remarks>
        /// <returns>True if full.</returns>
        public bool IsFull()
        {
            return _objectPoolObjects.Count + _prefetchedObjects.Count >= _poolMaximumSize;
        }

        /// <summary>
        /// Get the maximum allowed number of unused and prefetched objects.
        /// </summary>
        /// <returns>The maximum allowed objects.</returns>
        public int MaximumObjectsCount()
        {
            return _poolMaximumSize;
        }

        /// <summary>
        /// Get the number of prefetched objects in the pool.
        /// </summary>
        /// <returns>Number of prefetched objects.</returns>
        public int PrefetchedObjectsCount()
        {
            return _prefetchedObjects.Count;
        }

        /// <summary>
        /// Get the number of unused objects in the pool.
        /// </summary>
        /// <remarks>
        /// This count does not include any prefetched objects. Note that it is possible
        /// to forfeit a prefetched object and re-use it at any time.
        /// </remarks>
        /// <returns>Number of prefetched objects.</returns>
        public int UnusedObjectsCount()
        {
            return _objectPoolObjects.Count;
        }

        /// <summary>
        /// Check if an object is already in the prefab prefetch pool.
        /// </summary>
        /// <param name="id">The id to check.</param>
        /// <returns>True if object with specified id was found in prefetch pool.</returns>
        public bool ObjectIsPrefetched(int id)
        {
            return _prefetchedObjects.ContainsKey(id);
        }

        /// <inheritdoc/>
        public bool AddPrefetchedObjectToPool(int id, object objectToAdd, bool asNewest = true)
        {
            if (ObjectIsPrefetched(id) || IsFull())
            {
                return false;
            }
            else
            {
                _prefetchedObjects[id] = objectToAdd;
                _lruIndexList.AddIndex(id, asNewest);
                return true;
            }
        }

        /// <inheritdoc/>
        public bool ReturnObjectToPool(object objectToReturn)
        {
            if (objectToReturn == null)
            {
                Debug.LogError("Returning a null object to the object pool.");
            }
            else
            {
                if (_objectPoolObjects.Count + _prefetchedObjects.Count < _poolMaximumSize)
                {
                    _objectPoolObjects.Enqueue(objectToReturn);
                    return true;
                }

            }
            return false;
        }

        public void ReturnAllPrefetchedObjects(Action<int, object> processObject)
        {
            foreach (KeyValuePair<int, object> kv in _prefetchedObjects)
            {
                processObject(kv.Key, kv.Value);
                _lruIndexList.RemoveIndex(kv.Key);
                _objectPoolObjects.Enqueue(kv.Value);
            }
            _prefetchedObjects.Clear();
        }

        /// <inheritdoc/>
        public bool TryGetPrefetchedObject(int id, out object returnedObject)
        {

            if (_prefetchedObjects.ContainsKey(id))
            {
                returnedObject = _prefetchedObjects[id];
                _prefetchedObjects.Remove(id);
                _lruIndexList.RemoveIndex(id);
                return true;
            }
            else
            {
                returnedObject = GetObjectFromPool();
                return false;
            }
        }

        /// <inheritdoc/>
        public object GetObjectFromPool(bool canReusePrefetchObject = true)
        {
            object objectToReturn = null;

            if (_objectPoolObjects.Count > 0)
            {
                objectToReturn = _objectPoolObjects.Dequeue();
            }
            else if (canReusePrefetchObject && _prefetchedObjects.Count > 0)
            {
                int oldestIndex = _lruIndexList.GetLeastRecentlyUsedIndex();

                if (oldestIndex >= 0 && _prefetchedObjects.ContainsKey(oldestIndex))
                {
                    objectToReturn = _prefetchedObjects[oldestIndex];
                    _prefetchedObjects.Remove(oldestIndex);
                    _lruIndexList.RemoveIndex(oldestIndex);
                }
            }

            return objectToReturn;
        }
    }
}
