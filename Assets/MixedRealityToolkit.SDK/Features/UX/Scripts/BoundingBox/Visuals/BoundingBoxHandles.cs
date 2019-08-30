
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [Serializable]
    class BoundingBoxHandles
    {


        [SerializeField]
        [Tooltip("Prefab used to display scale handles in corners. If not set, boxes will be displayed instead")]
        GameObject scaleHandlePrefab = null;

        [SerializeField]
        [Tooltip("Prefab used to display scale handles in corners for 2D slate. If not set, boxes will be displayed instead")]
        GameObject scaleHandleSlatePrefab = null;


        private List<Transform> corners = new List<Transform>();
        private Vector3[] boundsCorners = new Vector3[8];

        private List<Transform> balls;

        private bool m_isFlattened = false;
        private Transform m_parent = null;

        //BoundingBoxScaleHandles()
        //{
        //    boundsCorners = new Vector3[8];
        //}

        /// <summary>
        /// Prefab used to display scale handles in corners. If not set, boxes will be displayed instead
        /// </summary>
        public GameObject ScaleHandlePrefab
        {
            get { return scaleHandlePrefab; }
            set
            {
                if (scaleHandlePrefab != value)
                {
                    scaleHandlePrefab = value;
                    //DestroyCorners();
                    // AddCorners();
                    CreateRig();
                }
            }
        }


        /// <summary>
        /// Prefab used to display scale handles in corners for 2D slate. If not set, boxes will be displayed instead
        /// </summary>
        public GameObject ScaleHandleSlatePrefab
        {
            get { return scaleHandleSlatePrefab; }
            set
            {
                if (scaleHandleSlatePrefab != value)
                {
                    scaleHandleSlatePrefab = value;
                    CreateRig();
                }
            }
        }


        [SerializeField]
       // [FormerlySerializedAs("cornerRadius")]
        [Tooltip("Size of the cube collidable used in scale handles")]
        private float scaleHandleSize = 0.016f; // 1.6cm default handle size

        /// <summary>
        /// Size of the cube collidable used in scale handles
        /// </summary>
        public float ScaleHandleSize
        {
            get { return scaleHandleSize; }
            set
            {
                if (scaleHandleSize != value)
                {
                    scaleHandleSize = value;
                    CreateRig();
                }
            }
        }



        [SerializeField]
        [Tooltip("Additional padding to apply to the collider on scale handle to make handle easier to hit")]
        private Vector3 scaleHandleColliderPadding = new Vector3(0.016f, 0.016f, 0.016f);

        /// <summary>
        /// Additional padding to apply to the collider on scale handle to make handle easier to hit
        /// </summary>
        public Vector3 ScaleHandleColliderPadding
        {
            get { return scaleHandleColliderPadding; }
            set
            {
                if (scaleHandleColliderPadding != value)
                {
                    scaleHandleColliderPadding = value;
                    CreateRig();
                }
            }
        }

        public void UpdateVisibilityInInspector(bool hide)
        {
            HideFlags desiredFlags = hide ? HideFlags.HideInHierarchy | HideFlags.HideInInspector : HideFlags.None;
            if (corners != null)
            {
                foreach (var cube in corners)
                {
                    cube.hideFlags = desiredFlags;
                }
            }
        }

        private void SetHighlighted(Transform handleToHighlight, List<Transform> handles, Material highlightMaterial)
        {
            // turn off all handles that aren't the handle we want to highlight
            if (handles != null)
            {
                for (int i = 0; i < corners.Count; ++i)
                {
                    if (handles[i] != handleToHighlight)
                    {
                        handles[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        BoundingBox.ApplyMaterialToAllRenderers(handles[i].gameObject, highlightMaterial);
                    }
                }
            }
        }

        public void SetHighlighted(Transform handleToHighlight, Material highlightMaterial)
        {
            SetHighlighted(handleToHighlight, corners, highlightMaterial);
            SetHighlighted(handleToHighlight, balls, highlightMaterial);
        }

        public void UpdateRigHandles()
        {
            for (int i = 0; i < corners.Count; ++i)
            {
                corners[i].position = boundsCorners[i];
            }
        }

        public void HandleIgnoreCollider(Collider handlesIgnoreCollider)
        {
            foreach (Transform corner in corners)
            {
                Collider[] colliders = corner.gameObject.GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    UnityEngine.Physics.IgnoreCollision(collider, handlesIgnoreCollider);
                }
            }
        }

        private HandleType GetHandleType(Transform handle)
        {
            for (int i = 0; i < balls.Count; ++i)
            {
                if (handle == balls[i])
                {
                    return HandleType.Rotation;
                }
            }
            for (int i = 0; i < corners.Count; ++i)
            {
                if (handle == corners[i])
                {
                    return HandleType.Scale;
                }
            }

            return HandleType.None;
        }


        private void DestroyMidPoints()
        {

            if (balls != null)
            {
                foreach (Transform transform in balls)
                {
                    GameObject.Destroy(transform.gameObject);
                }

                balls.Clear();
            }
        }

        private void DestroyCorners()
        {
            if (corners != null)
            {
                foreach (Transform transform in corners)
                {
                    GameObject.Destroy(transform.gameObject);
                }

                corners.Clear();
            }
        }

        public void DestroyHandles()
        {
            DestroyCorners();

            DestroyMidPoints();
        }

        
        public int GetRotationHandleIdx(Transform handle)
        {
            for (int i = 0; i < balls.Count; ++i)
            {
                if (handle == balls[i])
                {
                    return i;
                }
            }

            return balls.Count;
        }


        public void ResetHandleVisibility(bool isVisible)
        {

            //set balls visibility
            if (balls != null)
            { 
                for (int i = 0; i < balls.Count; ++i)
                {
                    balls[i].gameObject.SetActive(isVisible && ShouldRotateHandleBeVisible(edgeAxes[i]));
                    BoundingBox.ApplyMaterialToAllRenderers(balls[i].gameObject, handleMaterial);
                }
            }

            //set corner visibility
            if (showScaleHandles && corners != null)
            {
                for (int i = 0; i < corners.Count; ++i)
                {
                    corners[i].gameObject.SetActive(isVisible);
                    BoundingBox.ApplyMaterialToAllRenderers(corners[i].gameObject, handleMaterial);
                }
            }
        }

        private void AddCorners(Transform parent, bool isFlattened, BoundingBoxProximityEffect proximityEffect)
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
                GameObject prefabToInstantiate = isFlattened ? scaleHandleSlatePrefab : scaleHandlePrefab;
                GameObject cornerVisual = null;

                if (prefabToInstantiate == null)
                {
                    // instantiate default prefab, a cube. Remove the box collider from it
                    cornerVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cornerVisual.transform.parent = visualsScale.transform;
                    cornerVisual.transform.localPosition = Vector3.zero;
                    GameObject.Destroy(cornerVisual.GetComponent<BoxCollider>());
                }
                else
                {
                    cornerVisual = GameObject.Instantiate(prefabToInstantiate, visualsScale.transform);
                }

                if (isFlattened)
                {
                    // Rotate 2D slate handle asset for proper orientation
                    cornerVisual.transform.Rotate(0, 0, -90);
                }

                cornerVisual.name = "visuals";

                // this is the size of the corner visuals
                var cornerbounds = BoundingBoxHelper.GetMaxBounds(cornerVisual);
                float maxDim = Mathf.Max(Mathf.Max(cornerbounds.size.x, cornerbounds.size.y), cornerbounds.size.z);
                cornerbounds.size = maxDim * Vector3.one;

                // we need to multiply by this amount to get to desired scale handle size
                var invScale = scaleHandleSize / cornerbounds.size.x;
                cornerVisual.transform.localScale = new Vector3(invScale, invScale, invScale);

                BoundingBox.ApplyMaterialToAllRenderers(cornerVisual, handleMaterial);

                BoundingBoxHelper.AddComponentsToAffordance(corner, new Bounds(cornerbounds.center * invScale, cornerbounds.size * invScale), RotationHandlePrefabCollider.Box, CursorContextInfo.CursorAction.Scale, scaleHandleColliderPadding);
                corners.Add(corner.transform);

                proximityEffect?.AddHandle(HandleType.Scale, cornerVisual);
            }
        }

    }
}
