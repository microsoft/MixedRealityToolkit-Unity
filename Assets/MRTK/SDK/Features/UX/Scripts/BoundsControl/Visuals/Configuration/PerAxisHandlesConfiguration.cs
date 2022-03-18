// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Configuration for <see cref="PerAxisHandles"/> used in <see cref="BoundsControl"/>.
    /// This class provides all data members needed to create axis based handles for <see cref="BoundsControl"/>.
    /// </summary>
    public abstract class PerAxisHandlesConfiguration : HandlesBaseConfiguration
    {

        [SerializeField]
        [Tooltip("Determines the type of collider that will surround the handle prefab.")]
        private HandlePrefabCollider handlePrefabColliderType = HandlePrefabCollider.Box;

        /// <summary>
        /// Determines the type of collider that will surround the handle prefab.
        /// </summary>
        public HandlePrefabCollider HandlePrefabColliderType
        {
            get
            {
                return handlePrefabColliderType;
            }
            set
            {
                if (handlePrefabColliderType != value)
                {
                    handlePrefabColliderType = value;
                    colliderTypeChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Shows handles for the X axis.")]
        private bool showHandleForX = true;

        /// <summary>
        /// Shows handles for the X axis.
        /// </summary>
        public bool ShowHandleForX
        {
            get
            {
                return showHandleForX;
            }
            set
            {
                if (showHandleForX != value)
                {
                    showHandleForX = value;
                    handlesChanged.Invoke(HandlesChangedEventType.Visibility);
                }
            }
        }

        [SerializeField]
        [Tooltip("Shows handles for the Y axis.")]
        private bool showHandleForY = true;

        /// <summary>
        /// Shows handles for the Y axis.
        /// </summary>
        public bool ShowHandleForY
        {
            get
            {
                return showHandleForY;
            }
            set
            {
                if (showHandleForY != value)
                {
                    showHandleForY = value;
                    handlesChanged.Invoke(HandlesChangedEventType.Visibility);
                }
            }
        }

        [SerializeField]
        [Tooltip("Shows handles for the Z axis.")]
        private bool showHandleForZ = true;

        /// <summary>
        /// Shows handles for the Z axis.
        /// </summary>
        public bool ShowHandleForZ
        {
            get
            {
                return showHandleForZ;
            }
            set
            {
                if (showHandleForZ != value)
                {
                    showHandleForZ = value;
                    handlesChanged.Invoke(HandlesChangedEventType.Visibility);
                }
            }
        }

        internal UnityEvent colliderTypeChanged = new UnityEvent();
    }
}
