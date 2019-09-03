
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [Serializable]
    internal class BoundingBoxScaleHandles : BoundingBoxHandlesBase
    {


       // private List<Transform> corners = new List<Transform>();
        private Vector3[] boundsCorners = new Vector3[8];
        public Vector3[] BoundsCorners { get; private set; }

        public ref Vector3[] GetBoundsCornersRef() { return ref boundsCorners; }

        private bool m_isFlattened = false;
        private Transform m_parent = null;



        internal void UpdateVisibilityInInspector(HideFlags flags)
        {
            if (handles != null)
            {
                foreach (var cube in handles)
                {
                    cube.hideFlags = flags;
                }
            }
        }


        public override void Init()
        {
            boundsCorners = new Vector3[8];
            base.Init();
        }
        


        internal void UpdateHandles()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                handles[i].position = boundsCorners[i];
            }
        }



        public override HandleType GetHandleType()
        {
            return HandleType.Scale;
        }



        internal void ResetHandleVisibility(bool isVisible, Material handleMaterial)
        { 
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    handles[i].gameObject.SetActive(isVisible);
                    BoundingBox.ApplyMaterialToAllRenderers(handles[i].gameObject, handleMaterial);
                }
            }
        }

        internal void CreateHandles(GameObject handlePrefab, Material handleMaterial, Transform parent, 
            float handleSize, bool isFlattened, Vector3 colliderPadding)
        {
            //bool isFlattened = (flattenAxis != FlattenModeType.DoNotFlatten);
            m_isFlattened = isFlattened;
            m_parent = parent;
            for (int i = 0; i < boundsCorners.Length; ++i)
            {
                GameObject corner = new GameObject
                {
                    name = "corner_" + i.ToString()
                };
                corner.transform.parent = parent; // rigRoot.transform;
                corner.transform.localPosition = boundsCorners[i];

                GameObject visualsScale = new GameObject();
                visualsScale.name = "visualsScale";
                visualsScale.transform.parent = corner.transform;
                visualsScale.transform.localPosition = Vector3.zero;

                // Compute mirroring scale
                {
                    Vector3 p = boundsCorners[i];
                    visualsScale.transform.localScale = new Vector3(Mathf.Sign(p[0]), Mathf.Sign(p[1]), Mathf.Sign(p[2]));
                }

                // figure out which prefab to instantiate
                GameObject cornerVisual = null;

                if (handlePrefab == null)
                {
                    // instantiate default prefab, a cube. Remove the box collider from it
                    cornerVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cornerVisual.transform.parent = visualsScale.transform;
                    cornerVisual.transform.localPosition = Vector3.zero;
                    GameObject.Destroy(cornerVisual.GetComponent<BoxCollider>());
                }
                else
                {
                    cornerVisual = GameObject.Instantiate(handlePrefab, visualsScale.transform);
                }

                if (isFlattened)
                {
                    // Rotate 2D slate handle asset for proper orientation
                    cornerVisual.transform.Rotate(0, 0, -90);
                }

                cornerVisual.name = "visuals";

                // this is the size of the corner visuals
                var cornerbounds = BoundingBoxHandleUtils.GetMaxBounds(cornerVisual);
                float maxDim = Mathf.Max(Mathf.Max(cornerbounds.size.x, cornerbounds.size.y), cornerbounds.size.z);
                cornerbounds.size = maxDim * Vector3.one;

                // we need to multiply by this amount to get to desired scale handle size
                var invScale = handleSize / cornerbounds.size.x;
                cornerVisual.transform.localScale = new Vector3(invScale, invScale, invScale);

                BoundingBox.ApplyMaterialToAllRenderers(cornerVisual, handleMaterial);

                BoundingBoxHandleUtils.AddComponentsToAffordance(corner, new Bounds(cornerbounds.center * invScale, cornerbounds.size * invScale), RotationHandlePrefabCollider.Box, CursorContextInfo.CursorAction.Scale, colliderPadding);
                handles.Add(corner.transform);

                
            }
        }


        

    }
}
