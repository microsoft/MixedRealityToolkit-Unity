// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities.GameObjectManagement
{
    /// <summary>
    /// Optional interface that GameObjects (instantiated and recycled by a
    /// GameObjectPool) can implement in order to handle preparation for
    /// recycling and reuse.
    /// </summary>
    public interface IGameObjectCreatorListener
    {
        /// <summary>
        /// Called when the GameObject is about to be recycled by the GameObjectPool. This allows you to potentially free
        /// up any resources before it is deactivated by the GameObjectPool.
        /// </summary>
        void PrepareForRecycle();

        /// <summary>
        /// Called before the GameObject's position and rotation are set (as well as its active state) by the GameObjectPool
        /// when GetGameObject is called.
        /// </summary>
        void PrepareForUse();
    }
}