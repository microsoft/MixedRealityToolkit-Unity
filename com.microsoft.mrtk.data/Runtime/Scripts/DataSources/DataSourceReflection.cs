// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data source that uses reflection to determine the structure of an object,
    /// either a struct, a class or a primitive, and makes objects in that structure
    /// available using a keypath structure similar to what is used for the DataSourceObjects
    /// and DataSourceJson classes.
    /// </summary>
    public class DataSourceReflection : DataSourceBase
    {
        /// <summary>
        /// THe data items needed to allow for efficient lookup of
        /// recently requested keypaths.
        /// </summary>
        protected struct LRUItem : LRUValue<string>
        {
            public string Key { get; set; }
            public MemberInfo memberInfo;
            public object containingObject;
            public object value;
        }

        /// <summary>
        /// Manages ObservableCollection event handlers to know when a collection has been
        /// modified and propagate notifications as appropriate.
        ///
        /// If any data consumer attaches as a listener for a particular collection in this
        /// object, then one of these classes will be created to manage notifications for
        /// that collection.
        ///
        /// This allows for any number of collections to co-exist in this data source.
        /// </summary>
        protected class CollectionObserver
        {
            private readonly object collectionToObserve;
            private readonly string collectionKeyPath;
            private readonly DataSourceReflection dataSourceToNotify;

            public CollectionObserver(DataSourceReflection dataSource, string keyPath, object collection)
            {
                dataSourceToNotify = dataSource;
                collectionKeyPath = keyPath;
                collectionToObserve = collection;

                Delegate collectionChangedHandler = (NotifyCollectionChangedEventHandler)CollectionChangedHandler;

                EventInfo collectionChangedEventInfo = collection.GetType().GetEvent("CollectionChanged");

                collectionChangedEventInfo.AddEventHandler(collection, collectionChangedHandler);
            }

            protected void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs eventArgs)
            {
                switch (eventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int n = 0; n < eventArgs.NewItems.Count; n++)
                        {
                            int itemIdx = eventArgs.NewStartingIndex + n;
                            string itemKeyPath = dataSourceToNotify.GetNthCollectionKeyPathAt(collectionKeyPath, itemIdx);
                            CollectionItemIdentifier itemIdentifier = new CollectionItemIdentifier(itemKeyPath, itemIdx);

                            dataSourceToNotify.NotifyCollectionDataChanged(collectionKeyPath, itemIdentifier, DataChangeType.CollectionItemAdded, true);
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        // TODO @hoffhoffman: this is to compensate for situation where items are removed from 1st to last which causes index IDs to shift.
                        if (eventArgs.OldStartingIndex == 0)
                        {
                            dataSourceToNotify.NotifyCollectionDataChanged(collectionKeyPath, collectionToObserve, DataChangeType.CollectionReset, true);
                        }

                        for (int n = eventArgs.OldItems.Count - 1; n >= 0; n--)
                        {
                            int itemIdx = eventArgs.OldStartingIndex + n;
                            string itemKeyPath = dataSourceToNotify.GetNthCollectionKeyPathAt(collectionKeyPath, itemIdx);
                            CollectionItemIdentifier itemIdentifier = new CollectionItemIdentifier(itemKeyPath, itemIdx);
                            dataSourceToNotify.NotifyCollectionDataChanged(collectionKeyPath, itemIdentifier, DataChangeType.CollectionItemRemoved, true);
                        }

                        break;

                    case NotifyCollectionChangedAction.Replace:
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        dataSourceToNotify.NotifyCollectionDataChanged(collectionKeyPath, collectionToObserve, DataChangeType.CollectionReset, true);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected const string CollectionElementkeyPathPrefixFormat = "{0}[{1:d}]";
        protected const string DictionaryElementkeyPathPrefixFormat = "{0}[{1}]";

        protected object _dataSourceObject = null;
        protected Dictionary<string, CollectionObserver> _collectionObservers = new Dictionary<string, CollectionObserver>();

        protected static readonly int DefaultLRUCacheSize = 100;    // TODO: Should this be configurable via inspector property or is that overkill?

        protected LeastRecentlyUsedCache<string, LRUItem> _lruKeyPathToObjectCache = new LeastRecentlyUsedCache<string, LRUItem>(DefaultLRUCacheSize);

        // MemberInfo lookups cost a lot of time and GC, so we'll cache them in
        // this lookup dictionary instead. This is used by GetNamedFieldOrPropertyInfo
        private static Dictionary<Type, MemberInfo[]> memberInfoCache = new Dictionary<Type, MemberInfo[]>();

        public DataSourceReflection() { }

        public DataSourceReflection(object dataSourceObject)
        {
            _dataSourceObject = dataSourceObject;
        }

        public void SetDataSourceObject(object dataSourceObject)
        {
            _lruKeyPathToObjectCache.Clear();
            _dataSourceObject = dataSourceObject;
        }

        /// <inheritdoc/>
        public override bool IsCollectionAtKeyPath(string resolvedKeyPath)
        {
            return IsArrayOrList(KeyPathToObject(resolvedKeyPath)?.GetType());
        }

        /// <inheritdoc/>
        public override int GetCollectionCount(string resolvedKeyPath)
        {
            switch (KeyPathToObject(resolvedKeyPath))
            {
                case Array arr: return arr.Length;
                case IList list: return list.Count;
                default: return 0;
            }
        }

        public void NotifyCollectionDataChanged(string collectionKeyPath, object value, DataChangeType changeType, bool isAtomicChange)
        {
            if (changeType == DataChangeType.CollectionItemRemoved || changeType == DataChangeType.CollectionReset)
            {
                // TODO: Add further optimization to only clear out data related to this collection or a specific item in the collection. This is a bit harder
                // because it would mean any keypath that starts with the root keypath of the collection would need to be cleared out.
                // As an example, if the item in the collection at index = 3 is deleted, both of the following would need to be deleted:
                //    Response.Contacts[3].FirstName
                //    Response.Contacts[3].LastName
                //
                // For now, we just start over since in general entire collections are likely to come and go as a whole, in many use cases.
                _lruKeyPathToObjectCache.Clear();
            }

            NotifyDataChanged(collectionKeyPath, value, changeType, isAtomicChange);
        }


        /// <inheritdoc/>
        public override string GetNthCollectionKeyPathAt(string resolvedKeyPath, int n)
        {
            object value = KeyPathToObject(resolvedKeyPath);
            if (IsArrayOrList(value.GetType()))
            {
                return string.Format(CollectionElementkeyPathPrefixFormat, resolvedKeyPath, n);
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public override void SetValueInternal(string resolvedKeyPath, object value)
        {
            KeyPathToValueWithMemberInfo(resolvedKeyPath, out MemberInfo memberInfo, out object containingObject);
            if (memberInfo != null && containingObject != null)
            {
                SetValueFromFieldOrProperty(containingObject, memberInfo, value);
                _lruKeyPathToObjectCache.AddOrUpdateValue(resolvedKeyPath, MakeLRUItem(value, containingObject, memberInfo));
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetCollectionKeyPathRange(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            if (IsDataAvailable())
            {
                object objectAtKeyPath = KeyPathToObject(resolvedKeyPath);
                if (objectAtKeyPath != null)
                {
                    if (IsArrayOrList(objectAtKeyPath.GetType()))
                    {
                        return GetValueAsArrayKeyPaths(resolvedKeyPath, rangeStart, rangeCount);
                    }
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public override object GetValueInternal(string resolvedKeyPath)
        {
            if (IsDataAvailable())
            {
                object objectAtKeyPath = KeyPathToObject(resolvedKeyPath);
                if (objectAtKeyPath != null)
                {
                    if (objectAtKeyPath is IList)
                    {
                        return objectAtKeyPath;
                    }
                    else if (objectAtKeyPath is IDictionary)
                    {
                        return GetValueAsDictionaryKeyPaths(objectAtKeyPath as IDictionary, resolvedKeyPath);
                    }
                    else
                    {
                        return objectAtKeyPath;
                    }
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public override void RemoveDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer)
        {
            base.RemoveDataConsumerListener(resolvedKeyPath, dataConsumer);
            _lruKeyPathToObjectCache.TryRemove(resolvedKeyPath);
        }

        /// <summary>
        /// Provide an enumerable of resolvedKeypaths for items in a list
        /// </summary>
        /// <remarks>
        /// For use cases where the list is expected to be large, the keypaths are only fabricated as consumed.
        ///
        /// The arrayOrList argument is provide for cases where the resolvedKeyPath
        /// </remarks>
        /// <param name="resolvedKeyPath">The keypath of the collection</param>
        /// <param name="rangeStart">First desired entry in collection.</param>
        /// <param name="rangeCount">Number of entries to provide.</param>
        protected IEnumerable<string> GetValueAsArrayKeyPaths(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            return new KeyPathEnumerable(resolvedKeyPath, rangeStart, rangeCount);
        }

        protected IEnumerable<string> GetValueAsDictionaryKeyPaths(IDictionary dict, string resolvedKeyPath)
        {
            List<string> keyPaths = new List<string>();
            foreach (string key in dict.Keys)
            {
                keyPaths.Add(string.Format(DictionaryElementkeyPathPrefixFormat, resolvedKeyPath, key));
            }
            return keyPaths;
        }

        protected object KeyPathToObject(string resolvedKeyPath)
        {
            return KeyPathToValueWithMemberInfo(resolvedKeyPath, out _, out _);
        }

        protected object KeyPathToValueWithMemberInfo(string resolvedKeyPath, out MemberInfo memberInfoOut, out object containingObjectOut)
        {
            if (_lruKeyPathToObjectCache.ContainsKey(resolvedKeyPath))
            {
                LRUItem item = _lruKeyPathToObjectCache.FindByKey(resolvedKeyPath);
                memberInfoOut = item.memberInfo;
                containingObjectOut = item.containingObject;
                return item.value;
            }
            else
            {
                object currentObject = _dataSourceObject;
                string keyPath = resolvedKeyPath;

                containingObjectOut = null;
                memberInfoOut = null;

                while (currentObject != null && keyPath != null && keyPath != "")
                {
                    int amountToSkip = 0;
                    if (DataParser.FindKeypathArrayToken(keyPath, out int arrayStart, out int arrayEnd))
                    {
                        // This is a little better than Substring with respect
                        // to performance and memory usage, but uses a custom
                        // integer parser.
                        if (!DataParser.TryParseIntSubstring(keyPath, arrayStart, arrayEnd, out int arrayIndex))
                        {
                            Debug.LogWarning($"Unparsable DataBind keypath array token '{keyPath.Substring(arrayStart, arrayEnd - arrayStart)}' in '{keyPath}'");
                            return null;
                        }

                        // ReadOnlySpan is a fast, reliable way to do this, but
                        // is only accessible in more recent versions of Unity:
                        // ReadOnlySpan<char> arrayIndexText = keyPath.AsSpan();
                        // int                arrayIndex     = int.Parse(arrayIndexText.Slice(arrayStart, arrayEnd - arrayStart));

                        // Original code, Substring generates GC allocs, but
                        // it's probably the least called Substring in this
                        // function.
                        // string arrayIndexText = keyPath.Substring(arrayStart, arrayEnd - arrayStart);
                        // int    arrayIndex     = int.Parse(arrayIndexText);

                        memberInfoOut = null;

                        // During Detach, data source lists are often empty or of different sizes
                        // from what the keypaths expect.
                        Type currType = currentObject.GetType();
                        if (IsList(currType))
                        {
                            IList list = currentObject as IList;
                            currentObject = arrayIndex < list.Count
                                ? list[arrayIndex]
                                : null;
                        }
                        else if (currType.IsArray)
                        {
                            Array array = currentObject as Array;
                            currentObject = arrayIndex < array.Length
                                ? array.GetValue(arrayIndex)
                                : null;
                        }
                        else
                        {
                            currentObject = null;
                        }
                        amountToSkip = arrayEnd + 1;
                    }
                    else
                    {
                        if (DataParser.FindKeypathToken(keyPath, out int tokenStart, out int tokenEnd))
                        {
                            Type currType = currentObject.GetType();
                            if (IsStructOrClass(currType))
                            {
                                string key = keyPath.Substring(tokenStart, tokenEnd - tokenStart);
                                containingObjectOut = currentObject;
                                memberInfoOut = GetNamedFieldOrPropertyInfo(currType, key);
                                currentObject = GetValueFromFieldOrProperty(containingObjectOut, memberInfoOut);
                            }

                            amountToSkip = tokenEnd - tokenStart;
                        }
                    }

                    if (keyPath.Length > amountToSkip && keyPath[amountToSkip] == '.')
                    {
                        amountToSkip++;
                    }
                    if (amountToSkip > 0)
                    {
                        keyPath = keyPath.Substring(amountToSkip);
                    }
                    else
                    {
                        memberInfoOut = null;
                        currentObject = null;
                        containingObjectOut = null;
                        break;      // was not a valid path piece
                    }
                }

                if (currentObject != null)
                {
                    AddOrUpdateItemInLRUCache(resolvedKeyPath, currentObject, containingObjectOut, memberInfoOut);
                }

                return currentObject;
            }
        }

        private void AddOrUpdateItemInLRUCache(string keyPath, object value, object containingObject, MemberInfo memberInfo)
        {
            _lruKeyPathToObjectCache.AddOrUpdateValue(keyPath, MakeLRUItem(value, containingObject, memberInfo));
        }

        private static LRUItem MakeLRUItem(object value, object containingObject, MemberInfo memberInfo)
        {
            // TODO: Implement a method in LRU cache that can return one of the already pre-allocated values (in LL node)
            // so that we don't have to allocated one just to add it to the cache.

            LRUItem item = new LRUItem();
            item.memberInfo = memberInfo;
            item.containingObject = containingObject;
            item.value = value;
            return item;
        }

        protected static MemberInfo GetNamedFieldOrPropertyInfo(Type objType, string key)
        {
            try
            {
                // MemberInfo lookups cost a lot of time and GC, so we'll cache
                // them in a lookup dictionary instead.
                if (!memberInfoCache.TryGetValue(objType, out MemberInfo[] members))
                {
                    members = objType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    memberInfoCache[objType] = members;
                }

                for (int i = 0; i < members.Length; i++)
                {
                    if (string.Equals(members[i].Name, key))
                    {
                        return members[i];
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        protected static object GetValueFromFieldOrProperty(object containingObject, MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case PropertyInfo info: return info.GetValue(containingObject);
                case FieldInfo info: return info.GetValue(containingObject);
                default: return null;
            }
        }

        protected static void SetValueFromFieldOrProperty(object containingObject, MemberInfo memberInfo, object value)
        {
            switch (memberInfo)
            {
                case PropertyInfo info: info.SetValue(containingObject, value); break;
                case FieldInfo info: info.SetValue(containingObject, value); break;
            }
        }

        private static bool IsList(Type source)
            => source != null && typeof(IList).IsAssignableFrom(source);

        private static bool IsArrayOrList(Type source)
        {
            return source != null && (source.IsArray || IsList(source));
        }

        private static bool IsStructOrClass(Type source)
        {
            if (source != null)
            {
                // NOTE: using TypeInfo for backwards compatibility with .NET (pre IL2CPP)
                TypeInfo typeInfo = source.GetTypeInfo();
                return typeInfo.IsClass || (typeInfo.IsValueType && !typeInfo.IsPrimitive && !typeInfo.IsEnum);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override bool IsDataAvailable()
        {
            return _dataSourceObject != null;
        }

        /// <inheritdoc/>
        public override void OnCollectionListenerAdded(string resolvedKeyPath, object collection)
        {
            Type collectionType = collection.GetType();

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(collectionType))
            {
                CollectionObserver collectionObserver = new CollectionObserver(this, resolvedKeyPath, collection);
                _collectionObservers[resolvedKeyPath] = collectionObserver;
            }
        }

        /// <inheritdoc/>
        public override void OnCollectionListenerRemoved(string resolvedKeyPath)
        {
            _collectionObservers.Remove(resolvedKeyPath);
        }
    } // End of class DataSourceObjects
} // End of namespace Microsoft.MixedReality.Toolkit.Data
