using Microsoft.MixedReality.Toolkit.UI.Experimental.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    [Serializable]
    /// <summary>
    /// Base class for any type of <see cref="BoundsControl"/> handle.
    /// Handles are used for manipulating the BoundsControl by near or far user interaction.
    /// </summary>
    public abstract class BoundsControlHandlesBase : IProximityEffectObjectProvider
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
                    SetMaterials();
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
                    SetMaterials();
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

        internal void ResetHandleVisibility(bool isVisible)
        {
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    handles[i].gameObject.SetActive(isVisible && IsVisible(handles[i]));
                    BoundsControlVisualUtils.ApplyMaterialToAllRenderers(handles[i].gameObject, handleMaterial);
                }
            }
        }

        internal abstract bool IsVisible(Transform handle);
        

        internal protected List<Transform> handles = new List<Transform>();

        public IReadOnlyList<Transform> Handles
        {
            get { return handles; }
        }

        internal void SetHighlighted(Transform handleToHighlight)
        {
            // turn off all handles that aren't the handle we want to highlight
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    if (handles[i] != handleToHighlight)
                    {
                        handles[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        BoundsControlVisualUtils.ApplyMaterialToAllRenderers(handles[i].gameObject, handleGrabbedMaterial);
                    }
                }
            }
        }

        internal void HandleIgnoreCollider(Collider handlesIgnoreCollider)
        {
            BoundsControlVisualUtils.HandleIgnoreCollider(handlesIgnoreCollider, handles);
        }

        internal void DestroyHandles()
        {
            if (handles != null)
            {
                foreach (Transform transform in handles)
                {
                    GameObject.Destroy(transform.gameObject);
                }

                handles.Clear();
            }
        }

        internal bool IsHandleType(Transform handle)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                if (handle == handles[i])
                {
                    return true;
                }
            }

            return false;
        }


        internal virtual HandleType GetHandleType()
        {
            return HandleType.None;
        }

        internal protected void SetMaterials()
        {
            if (handleMaterial == null)
            {
                handleMaterial = BoundsControlVisualUtils.CreateDefaultMaterial();
            }
            if (handleGrabbedMaterial == null && handleGrabbedMaterial != handleMaterial)
            {
                handleGrabbedMaterial = BoundsControlVisualUtils.CreateDefaultMaterial();
            }
        }

        protected abstract Transform GetVisual(Transform handle);


        #region IProximityScaleObjectProvider 
        public abstract bool IsActive();

        public Material GetBaseMaterial()
        {
            return handleMaterial;
        }

        public Material GetHighlightedMaterial()
        {
            return HandleGrabbedMaterial;
        }

        public float GetObjectSize()
        {
            return HandleSize;
        }

        public void ForEachProximityObject(Action<Transform> action)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                action(GetVisual(handles[i]));
            }
        }

        #endregion IProximityScaleObjectProvider
    }
}
