using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    class BoundingBoxHandlesBase
    {
        internal protected List<Transform> handles = new List<Transform>();

        internal void SetHighlighted(Transform handleToHighlight, Material highlightMaterial)
        {
            BoundingBoxHandleUtils.SetHighlighted(handleToHighlight, handles, highlightMaterial);
        }

        internal void HandleIgnoreCollider(Collider handlesIgnoreCollider)
        {
            BoundingBoxHandleUtils.HandleIgnoreCollider(handlesIgnoreCollider, handles);
        }


        public virtual void Init()
        {
            handles = new List<Transform>();
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

        internal void AddProximityEffect(BoundingBoxProximityEffect proximityEffect)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                proximityEffect.AddHandle(GetHandleType(), handles[i].gameObject);
            }
        }


        public virtual HandleType GetHandleType()
        {
            return HandleType.None;
        }

        //public void CreateHandles()
        //{

        //}
        //public void ResetHandleVisibility()
        //{
        // todo?
        //}
    }
}
