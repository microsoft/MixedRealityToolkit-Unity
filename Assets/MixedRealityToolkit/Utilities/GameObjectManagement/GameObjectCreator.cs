// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.GameObjectManagement
{
    /// <summary>
    /// An abstract class used by the GameObjectPool for creating and recycling game objects.
    /// </summary>
    public abstract class GameObjectCreator
    {
        /// <summary>
        /// Creates a GameObject for the GameObjectPool. The position and rotation of the GameObject
        /// is set by the GameObjectPool when GetGameObject is called.
        /// </summary>
        /// <returns>An instantiated GameObject.</returns>
        public abstract GameObject Instantiate();

        /// <summary>
        /// Called when the GameObject is about to be recycled by the GameObjectPool. This allows you to potentially free
        /// up any resources before it is deactivated by the GameObjectPool. If the GameObject has a component that implements
        /// the IGameObjectCreatorHandler interface, it will call its PrepareForRecycle function;
        /// </summary>
        /// <param name="obj">The GameObject that is about to be recycled.</param>
        public virtual void PrepareForRecycle(GameObject obj)
        {
            var components = obj.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component is IGameObjectCreatorListener)
                {
                    (component as IGameObjectCreatorListener).PrepareForRecycle();
                }
            }
        }

        /// <summary>
        /// Called before the GameObject's position and rotation are set (as well as it's active state) by the GameObjectPool
        /// when GetGameObject is called. If the GameObject has a component that implements
        /// the IGameObjectCreatorHandler interface, it will call its PrepareForUse function;
        /// </summary>
        /// <param name="obj">The GameObject that is about to be used.</param>
        public virtual void PrepareForUse(GameObject obj)
        {
            var components = obj.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component is IGameObjectCreatorListener)
                {
                    (component as IGameObjectCreatorListener).PrepareForUse();
                }
            }
        }
    }
}
