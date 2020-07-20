// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.GameObjectManagement
{
    /// <summary>
    /// Creator for making prefab instances
    /// </summary>
    /// <seealso cref="Microsoft.MixedReality.Toolkit.Utilities.GameObjectManagement.GameObjectCreator" />
    public class GenericPrefabInstanceCreator : GameObjectCreator
    {
        /// <summary>
        /// The prefab to instantiate
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// Creates a GameObject for the GameObjectPool. The position and rotation of the GameObject
        /// is set by the GameObjectPool when GetGameObject is called.
        /// </summary>
        /// <returns>
        /// An instantiated GameObject.
        /// </returns>
        public override GameObject Instantiate()
        {
            return GameObject.Instantiate(Prefab);
        }
    }
}
