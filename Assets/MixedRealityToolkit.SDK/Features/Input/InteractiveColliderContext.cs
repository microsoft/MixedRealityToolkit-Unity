// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Use this component to provide information about colliders that respond to MRTK inputs.
    /// 
    /// You can use pointerHandlerTypes to specify the list of types that this collider wishes to activate.
    /// For example, you may want a collider to activate for BoundingBox but not ManipulationHandler.
    /// To do this, add BoundingBox and ManipulationHandler to pointerHandlerTypes list.
    /// Note that it's up to the classes to filter on those types. See ManipulationHandler for an example.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InteractiveColliderContext : MonoBehaviour
    {
        [Tooltip("List of types that this collider wishes to activate. It is up to classes to implement filtering on these types. See ManipulationHandler for an example.")]
        [SerializeField]
        [Implements(typeof(IMixedRealityPointerHandler), TypeGrouping.ByNamespaceFlat)]
        private SystemType[] pointerHandlerTypes = new SystemType[0];

        public SystemType[] PointerHandlerTypes { get => pointerHandlerTypes; set => pointerHandlerTypes = value; }

        /// <summary>
        /// Add this type to end of PointerHandlerTypes list. If list is null, new list will be created
        /// </summary>
        /// <param name="type"></param>
        public void AddPointerHandler(SystemType type)
        {
            if (pointerHandlerTypes == null)
            {
                pointerHandlerTypes = new SystemType[0];
            }
            var lst = new List<SystemType>(pointerHandlerTypes);
            lst.Add(type);
            pointerHandlerTypes = lst.ToArray();
        }

        /// <summary>
        /// Removes all instances of the given type from the PointerHandlerTypes list. If list is null, nothing happens.
        /// </summary>
        /// <param name="type"></param>
        public void RemovePointerHandler(SystemType type)
        {
            if (pointerHandlerTypes == null)
            {
                return;
            }
            var lst = new List<SystemType>(pointerHandlerTypes);
            lst.RemoveAll((x) => x == type);
            pointerHandlerTypes = lst.ToArray();
        }
    }
}
