using Microsoft.MixedReality.Toolkit.UI.Experimental.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    /// <summary>
    /// Base class for any type of <see cref="BoundsControl"/> handle.
    /// Handles are used for manipulating the BoundsControl by near or far user interaction.
    /// </summary>
    public abstract class BoundsControlHandlesBase : IProximityEffectObjectProvider
    {
        protected abstract BoundsControlHandlesBaseConfiguration BaseConfig
        {
            get;
        }

        internal void ResetHandleVisibility(bool isVisible)
        {
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    handles[i].gameObject.SetActive(isVisible && IsVisible(handles[i]));
                    BoundsControlVisualUtils.ApplyMaterialToAllRenderers(handles[i].gameObject, BaseConfig.HandleMaterial);
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
                        BoundsControlVisualUtils.ApplyMaterialToAllRenderers(handles[i].gameObject, BaseConfig.HandleGrabbedMaterial);
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

        protected abstract Transform GetVisual(Transform handle);


        #region IProximityScaleObjectProvider 
        public abstract bool IsActive();

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
