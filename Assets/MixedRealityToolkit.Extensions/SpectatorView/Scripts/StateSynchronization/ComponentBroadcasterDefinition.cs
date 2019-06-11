// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Component which defines when and how to attach a ComponentBroadcaster to a GameObject.
    /// ComponentBroadcasters can be created unconditionally or due to certain requirements being
    /// present on the provided GameObject.
    /// </summary>
    public abstract class ComponentBroadcasterDefinition
    {
        /// <summary>
        /// Method which checks for the existence of requirements on the provided GameObject and either
        /// creates or destroys ComponentBroadcasters as necessary.
        /// </summary>
        /// <param name="gameObject">The GameObject to check the state of.</param>
        /// <param name="changesDetected">True if ComponentBroadcasters were added or removed, otherwise false.</param>
        public abstract void EnsureComponentBroadcastersCreated(GameObject gameObject, out bool changesDetected);

        /// <summary>
        /// Gets whether or not this ComponentBroadcasterDefinition may need to perform modifications
        /// to the GameObject before other ComponentBroadcasterDefinitions are allowed to create
        /// ComponentBroadcasters.
        /// </summary>
        public abstract bool IsTransformBroadcasterController { get; }
    }

    /// <summary>
    /// Attaches a ComponentBroadcaster of type TComponentBroadcaster to a GameObject if and only if
    /// a required set of Component types are present on the GameObject.
    /// </summary>
    /// <typeparam name="TComponentBroadcaster">The type of ComponentBroadcaster to attach when
    /// requirements are met.</typeparam>
    public class ComponentBroadcasterDefinition<TComponentBroadcaster> : ComponentBroadcasterDefinition where TComponentBroadcaster : Component
    {
        private readonly Type[] requiredComponents;
        private readonly bool isTransformBroadcasterController;

        /// <summary>
        /// Creates a new definition with a set of required Component types.
        /// </summary>
        /// <param name="requiredComponents">The set of Component types
        /// which must be present on the GameObject in order for the ComponentBroadcaster
        /// to be created. An empty list means that the ComponentBroadcaster will
        /// never be created.</param>
        public ComponentBroadcasterDefinition(params Type[] requiredComponents)
            : this(false, requiredComponents)
        {
        }

        public override bool IsTransformBroadcasterController
        {
            get { return isTransformBroadcasterController; }
        }

        /// <summary>
        /// Creates a new definition, specifying whether the component definition is a transform controller
        /// and which set of Component types are required.
        /// </summary>
        /// <param name="isTransformBroadcasterController">Whether or not this component is a transform controller.</param>
        /// <param name="requiredComponents">The set of Component types
        /// which must be present on the GameObject in order for the ComponentBroadcaster
        /// to be created. An empty list means that the ComponentBroadcaster will
        /// never be created.</param>
        public ComponentBroadcasterDefinition(bool isTransformBroadcasterController, params Type[] requiredComponents)
        {
            this.isTransformBroadcasterController = isTransformBroadcasterController;
            this.requiredComponents = requiredComponents;
        }

        /// <inheritdoc />
        public override void EnsureComponentBroadcastersCreated(GameObject gameObject, out bool changesDetected)
        {
            changesDetected = false;

            if (requiredComponents.Length > 0)
            {
                bool hasAllRequiredComponents = true;

                foreach (Type requiredComponent in requiredComponents)
                {
                    Component c = gameObject.GetComponent(requiredComponent);
                    if (c == null)
                    {
                        hasAllRequiredComponents = false;
                        break;
                    }
                }

                TComponentBroadcaster ComponentBroadcaster = gameObject.GetComponent<TComponentBroadcaster>();

                if (hasAllRequiredComponents && ComponentBroadcaster == null)
                {
                    changesDetected = true;
                    ComponentBroadcaster = gameObject.AddComponent<TComponentBroadcaster>();
                }
                else if (!hasAllRequiredComponents && ComponentBroadcaster != null)
                {
                    changesDetected = true;
                    Component.Destroy(ComponentBroadcaster);
                }
            }
        }
    }
}
