// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class ComponentBroadcasterDefinition
    {
        public abstract void EnsureComponentBroadcastersCreated(GameObject gameObject, out bool changesDetected);
        public abstract bool IsTransformBroadcasterController { get; }
    }

    internal class ComponentBroadcasterDefinition<TComponentBroadcaster> : ComponentBroadcasterDefinition where TComponentBroadcaster : Component
    {
        private readonly Type[] requiredComponents;
        private readonly bool isTransformBroadcasterController;

        public ComponentBroadcasterDefinition(params Type[] requiredComponents)
            : this(false, requiredComponents)
        {
        }

        public override bool IsTransformBroadcasterController
        {
            get { return isTransformBroadcasterController; }
        }

        public ComponentBroadcasterDefinition(bool isTransformBroadcasterController, params Type[] requiredComponents)
        {
            this.isTransformBroadcasterController = isTransformBroadcasterController;
            this.requiredComponents = requiredComponents;
        }

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
