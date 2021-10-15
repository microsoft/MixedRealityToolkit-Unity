// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    ///
    public interface IDataObjectPool
    {
        void SetMaximumPoolSize(int maxSize, bool resizeNow);

        bool IsEmpty();

        bool ReturnObjectToPool(object objectToReturn);

        object GetObjectFromPool();

        bool AddPrefetchedObjectToPool(int id, object objectToReturn);

        bool TryGetPrefetchedObject(int id, out object returnedObject);
    }
}
