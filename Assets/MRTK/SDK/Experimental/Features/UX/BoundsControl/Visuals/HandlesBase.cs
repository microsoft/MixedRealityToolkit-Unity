// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Base class for any type of <see cref="BoundsControl"/> handle.
    /// Handles are used for manipulating the BoundsControl by near or far user interaction.
    /// </summary>
    public abstract class HandlesBase : IProximityEffectObjectProvider
    {

        internal HandlesBase()
        {

        }
        protected abstract HandlesBaseConfiguration BaseConfig
        {
            get;
        }

        internal void HandlesChanged(HandlesBaseConfiguration.HandlesChangedEventType changedType)
        {
            switch (changedType)
            {
                case HandlesBaseConfiguration.HandlesChangedEventType.MATERIAL:
                    UpdateBaseMaterial();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.MATERIAL_GRABBED:
                    UpdateGrabbedMaterial();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.PREFAB:
                    RecreateVisuals();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.COLLIDER_SIZE:
                case HandlesBaseConfiguration.HandlesChangedEventType.COLLIDER_PADDING:
                    UpdateColliderBounds();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.MANIPULATION_TETHER:
                    UpdateDrawTether();
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.IGNORE_COLLIDER_REMOVE:
                    HandlesIgnoreConfigCollider(false);
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.IGNORE_COLLIDER_ADD:
                    HandlesIgnoreConfigCollider(true);
                    break;
                case HandlesBaseConfiguration.HandlesChangedEventType.VISIBILITY:
                    ResetHandles();
                    break;
            }
        }

        private void HandlesIgnoreConfigCollider(bool ignore)
        {
            VisualUtils.HandleIgnoreCollider(BaseConfig.HandlesIgnoreCollider, handles, ignore);
        }

        private void UpdateDrawTether()
        {
            // enable / disable tether in near interaction grabbable of handle
            foreach (var handle in handles)
            {
                var grabbable = handle.gameObject.EnsureComponent<NearInteractionGrabbable>();
                grabbable.ShowTetherWhenManipulating = BaseConfig.DrawTetherWhenManipulating;
            }
        }

        protected void UpdateColliderBounds()
        {
            foreach (var handle in handles)
            {
                var handleBounds = VisualUtils.GetMaxBounds(GetVisual(handle).gameObject);
                UpdateColliderBounds(handle, handleBounds.size);
            }
        }

        protected abstract void UpdateColliderBounds(Transform handle, Vector3 visualSize);
        protected abstract void RecreateVisuals();

        private void ResetHandles()
        {
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    bool isVisible = IsVisible(handles[i]);
                    handles[i].gameObject.SetActive(isVisible);
                    if (isVisible)
                    {
                        VisualUtils.ApplyMaterialToAllRenderers(handles[i].gameObject, BaseConfig.HandleMaterial);
                    }
                }
            }
            highlightedHandle = null;
        }

        internal abstract bool IsVisible(Transform handle);
        

        internal protected List<Transform> handles = new List<Transform>();
        private Transform highlightedHandle = null;

        ProximityObjectsChangedEvent IProximityEffectObjectProvider.ProximityObjectsChanged => objectsChangedEvent;
        protected ProximityObjectsChangedEvent objectsChangedEvent = new ProximityObjectsChangedEvent();

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
                        VisualUtils.ApplyMaterialToAllRenderers(handles[i].gameObject, BaseConfig.HandleGrabbedMaterial);
                        highlightedHandle = handleToHighlight;
                    }
                }
            }
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

        protected abstract Transform GetVisual(Transform handle);

        protected void UpdateBaseMaterial()
        {
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    if (handles[i] != highlightedHandle)
                    {
                        VisualUtils.ApplyMaterialToAllRenderers(handles[i].gameObject, BaseConfig.HandleMaterial);
                    }
                }
            }
        }

        protected void UpdateGrabbedMaterial()
        {
            if (highlightedHandle)
            {
                SetHighlighted(highlightedHandle);
            }
        }

        //protected void UpdateColliderPadding(HandlePrefabCollider colliderType, Vector3 size)
        //{
        //    foreach (var handle in handles)
        //    {
        //        if (colliderType == HandlePrefabCollider.Box)
        //        {
        //            BoxCollider collider = handle.gameObject.GetComponent<BoxCollider>();
        //            collider.size = size;
        //            collider.size += BaseConfig.ColliderPadding;
        //        }
        //        else
        //        {
        //            SphereCollider sphere = handle.gameObject.GetComponent<SphereCollider>();
        //            sphere.radius = size.x;
        //            sphere.radius += Mathf.Max(Mathf.Max(BaseConfig.ColliderPadding.x, BaseConfig.ColliderPadding.y), BaseConfig.ColliderPadding.z);
        //        }
        //    }
        //}

        #region IProximityScaleObjectProvider 

        private bool isActive = true;
        public virtual bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    ResetHandles();
                }
            }
        }

        public void ForEachProximityObject(Action<Transform> action)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                action(GetVisual(handles[i]));
            }
        }

        public Material GetBaseMaterial()
        {
            return BaseConfig.HandleMaterial;
        }

        public Material GetHighlightedMaterial()
        {
            return BaseConfig.HandleGrabbedMaterial;
        }

        public float GetObjectSize()
        {
            return BaseConfig.HandleSize;
        }

        #endregion IProximityScaleObjectProvider
    }
}
