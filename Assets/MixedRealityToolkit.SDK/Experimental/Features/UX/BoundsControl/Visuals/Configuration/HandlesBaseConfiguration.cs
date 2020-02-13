// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Configuration base class for any <see cref="BoundsControl"/> handle type deriving from <see cref="HandlesBase"/>
    /// </summary>
    public abstract class HandlesBaseConfiguration : ScriptableObject
    { 
        [SerializeField]
        [Tooltip("Material applied to handles when they are not in a grabbed state")]
        private Material handleMaterial;

        /// <summary>
        /// Material applied to handles when they are not in a grabbed state
        /// </summary>
        public Material HandleMaterial
        {
            get { return handleMaterial; }
            set
            {
                if (handleMaterial != value)
                {
                    handleMaterial = value;
                    TrySetDefaultMaterial();
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material applied to handles while they are a grabbed")]
        private Material handleGrabbedMaterial;

        /// <summary>
        /// Material applied to handles while they are a grabbed
        /// </summary>
        public Material HandleGrabbedMaterial
        {
            get { return handleGrabbedMaterial; }
            set
            {
                if (handleGrabbedMaterial != value)
                {
                    handleGrabbedMaterial = value;
                    TrySetDefaultMaterial();
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Prefab used to display this type of bounds control handle. If not set, default shape will be used (scale default: boxes, rotation default: spheres)")]
        GameObject handlePrefab = null;

        /// <summary>
        /// Prefab used to display this type of bounds control handle. If not set, default shape will be used (scale default: boxes, rotation default: spheres)
        /// </summary>
        public GameObject HandlePrefab
        {
            get { return handlePrefab; }
            set
            {
                if (handlePrefab != value)
                {
                    handlePrefab = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Size of the handle collidable")]
        private float handleSize = 0.016f; // 1.6cm default handle size

        /// <summary>
        /// Size of the handle collidable
        /// </summary>
        public float HandleSize
        {
            get { return handleSize; }
            set
            {
                if (handleSize != value)
                {
                    handleSize = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Additional padding to apply to the handle collider to make handle easier to hit")]
        private Vector3 colliderPadding = new Vector3(0.016f, 0.016f, 0.016f);

        /// <summary>
        /// Additional padding to apply to the handle collider to make handle easier to hit
        /// </summary>
        public Vector3 ColliderPadding
        {
            get { return colliderPadding; }
            set
            {
                if (colliderPadding != value)
                {
                    colliderPadding = value;
                    configurationChanged.Invoke();
                }
            }
        }

        internal protected UnityEvent configurationChanged = new UnityEvent();
        internal protected UnityEvent visibilityChanged = new UnityEvent();

        private void Awake()
        {
            TrySetDefaultMaterial();
        }

        internal protected void TrySetDefaultMaterial()
        {
            if (handleMaterial == null)
            {
                handleMaterial = VisualUtils.CreateDefaultMaterial();
            }
            if (handleGrabbedMaterial == null && handleGrabbedMaterial != handleMaterial)
            {
                handleGrabbedMaterial = VisualUtils.CreateDefaultMaterial();
            }
        }


    }
}
