// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class SynchronizedComponentDefinition
    {
        public abstract void EnsureSynchronized(GameObject gameObject, out bool changesDetected);
        public abstract bool IsSynchronizedTransformController { get; }
    }

    internal class SynchronizedComponentDefinition<TSynchronizedComponent> : SynchronizedComponentDefinition where TSynchronizedComponent : Component
    {
        private readonly Type[] requiredComponents;
        private readonly bool isSynchronizedTransformController;

        public SynchronizedComponentDefinition(params Type[] requiredComponents)
            : this(false, requiredComponents)
        {
        }

        public override bool IsSynchronizedTransformController
        {
            get { return isSynchronizedTransformController; }
        }

        public SynchronizedComponentDefinition(bool isSynchronizedTransformController, params Type[] requiredComponents)
        {
            this.isSynchronizedTransformController = isSynchronizedTransformController;
            this.requiredComponents = requiredComponents;
        }

        public override void EnsureSynchronized(GameObject gameObject, out bool changesDetected)
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

                TSynchronizedComponent synchronizedComponent = gameObject.GetComponent<TSynchronizedComponent>();

                if (hasAllRequiredComponents && synchronizedComponent == null)
                {
                    changesDetected = true;
                    synchronizedComponent = gameObject.AddComponent<TSynchronizedComponent>();
                }
                else if (!hasAllRequiredComponents && synchronizedComponent != null)
                {
                    changesDetected = true;
                    Component.Destroy(synchronizedComponent);
                }
            }
        }
    }
}
