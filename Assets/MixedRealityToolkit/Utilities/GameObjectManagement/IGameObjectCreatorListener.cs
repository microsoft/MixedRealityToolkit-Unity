// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        /// Called before the GameObject's position and rotation are set (as well as it's active state) by the GameObjectPool
        /// when GetGameObject is called.
        /// </summary>
        void PrepareForUse();
    }
}