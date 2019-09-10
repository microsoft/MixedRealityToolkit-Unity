using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class BoundingBoxRotationHandles : BoundingBoxHandlesBase
    {

        [SerializeField]
        [Tooltip("Determines the type of collider that will surround the rotation handle prefab.")]
        private RotationHandlePrefabCollider rotationHandlePrefabColliderType = RotationHandlePrefabCollider.Box;

        /// <summary>
        /// Determines the type of collider that will surround the rotation handle prefab.
        /// </summary>
        public RotationHandlePrefabCollider RotationHandlePrefabColliderType
        {
            get
            {
                return rotationHandlePrefabColliderType;
            }
            set
            {
                if (rotationHandlePrefabColliderType != value)
                {
                    rotationHandlePrefabColliderType = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show rotation handles for the X axis")]
        private bool showRotationHandleForX = true;

        /// <summary>
        /// Check to show rotation handles for the X axis
        /// </summary>
        public bool ShowRotationHandleForX
        {
            get
            {
                return showRotationHandleForX;
            }
            set
            {
                if (showRotationHandleForX != value)
                {
                    showRotationHandleForX = value;
                    visibilityChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show rotation handles for the Y axis")]
        private bool showRotationHandleForY = true;

        /// <summary>
        /// Check to show rotation handles for the Y axis
        /// </summary>
        public bool ShowRotationHandleForY
        {
            get
            {
                return showRotationHandleForY;
            }
            set
            {
                if (showRotationHandleForY != value)
                {
                    showRotationHandleForY = value;
                    visibilityChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show rotation handles for the Z axis")]
        private bool showRotationHandleForZ = true;

        /// <summary>
        /// Check to show rotation handles for the Z axis
        /// </summary>
        public bool ShowRotationHandleForZ
        {
            get
            {
                return showRotationHandleForZ;
            }
            set
            {
                if (showRotationHandleForZ != value)
                {
                    showRotationHandleForZ = value;
                    visibilityChanged.Invoke();
                }
            }
        }


        internal const int NumEdges = 12;

        private Vector3[] edgeCenters = new Vector3[NumEdges];
        private CardinalAxisType[] edgeAxes;

        // TODO CHANGE BACK TO PRIVATE
        internal int[] flattenedHandles;

        public int GetRotationHandleIdx(Transform handle)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                if (handle == handles[i])
                {
                    return i;
                }
            }

            return handles.Count;
        }


        public void Flatten(FlattenModeType flattenAxis)
        {
            if (flattenAxis == FlattenModeType.FlattenX)
            {
                flattenedHandles = new int[] { 0, 4, 2, 6 };
            }
            else if (flattenAxis == FlattenModeType.FlattenY)
            {
                flattenedHandles = new int[] { 1, 3, 5, 7 };
            }
            else if (flattenAxis == FlattenModeType.FlattenZ)
            {
                flattenedHandles = new int[] { 9, 10, 8, 11 };
            }
        }

        public Vector3 GetEdgeCenter(int index)
        {
            // TODO ASSERT IF not in Bounds
            return edgeCenters[index];
        }

        public CardinalAxisType GetAxisType(int index)
        {
            // TODO ASSERT IF NOT IN BOUNDS
            return edgeAxes[index];
        }

        public CardinalAxisType GetAxisType(Transform handle)
        {
            int index = GetRotationHandleIdx(handle);
            //TODO ASSERT VALID INDEX
            return GetAxisType(index);
        }

        internal void UpdateHandles()
        {
            for (int i = 0; i < NumEdges; ++i)
            {
                handles[i].position = GetEdgeCenter(i);
            }
        }

        internal void CalculateEdgeCenters(Vector3[] boundsCorners)
        {
            if (boundsCorners != null && edgeCenters != null)
            {
                edgeCenters[0] = (boundsCorners[0] + boundsCorners[1]) * 0.5f;
                edgeCenters[1] = (boundsCorners[0] + boundsCorners[2]) * 0.5f;
                edgeCenters[2] = (boundsCorners[3] + boundsCorners[2]) * 0.5f;
                edgeCenters[3] = (boundsCorners[3] + boundsCorners[1]) * 0.5f;

                edgeCenters[4] = (boundsCorners[4] + boundsCorners[5]) * 0.5f;
                edgeCenters[5] = (boundsCorners[4] + boundsCorners[6]) * 0.5f;
                edgeCenters[6] = (boundsCorners[7] + boundsCorners[6]) * 0.5f;
                edgeCenters[7] = (boundsCorners[7] + boundsCorners[5]) * 0.5f;

                edgeCenters[8] = (boundsCorners[0] + boundsCorners[4]) * 0.5f;
                edgeCenters[9] = (boundsCorners[1] + boundsCorners[5]) * 0.5f;
                edgeCenters[10] = (boundsCorners[2] + boundsCorners[6]) * 0.5f;
                edgeCenters[11] = (boundsCorners[3] + boundsCorners[7]) * 0.5f;
            }
        }


        internal void InitEdgeAxis()
        {

            edgeAxes = new CardinalAxisType[12];
            edgeAxes[0] = CardinalAxisType.X;
            edgeAxes[1] = CardinalAxisType.Y;
            edgeAxes[2] = CardinalAxisType.X;
            edgeAxes[3] = CardinalAxisType.Y;
            edgeAxes[4] = CardinalAxisType.X;
            edgeAxes[5] = CardinalAxisType.Y;
            edgeAxes[6] = CardinalAxisType.X;
            edgeAxes[7] = CardinalAxisType.Y;
            edgeAxes[8] = CardinalAxisType.Z;
            edgeAxes[9] = CardinalAxisType.Z;
            edgeAxes[10] = CardinalAxisType.Z;
            edgeAxes[11] = CardinalAxisType.Z;
        }

        internal void SetHiddenHandles()
        {
            if (flattenedHandles != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    handles[flattenedHandles[i]].gameObject.SetActive(false);
                }
            }
        }

        public override bool IsVisible(Transform handle)
        {
            CardinalAxisType axisType = GetAxisType(handle);
            return
                (axisType == CardinalAxisType.X && ShowRotationHandleForX) ||
                (axisType == CardinalAxisType.Y && ShowRotationHandleForY) ||
                (axisType == CardinalAxisType.Z && ShowRotationHandleForZ);
        }

        public override bool IsHandleTypeActive()
        {
            return ShowRotationHandleForX || ShowRotationHandleForY || ShowRotationHandleForZ;
        }

        public override HandleType GetHandleType()
        {
            return HandleType.Rotation;
        }

        internal void CreateHandles(Transform parent, bool drawManipulationTether, bool isFlattened)
        {
            for (int i = 0; i < edgeCenters.Length; ++i)
            {
                GameObject midpoint = new GameObject();
                midpoint.name = "midpoint_" + i.ToString();
                midpoint.transform.position = edgeCenters[i];
                midpoint.transform.parent = parent;

                GameObject midpointVisual;
                GameObject prefabType = isFlattened ? HandleSlatePrefab : HandlePrefab;
                if (prefabType != null)
                {
                    midpointVisual = GameObject.Instantiate(prefabType);
                }
                else
                {
                    midpointVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GameObject.Destroy(midpointVisual.GetComponent<SphereCollider>());
                }

                // Align handle with its edge assuming that the prefab is initially aligned with the up direction 
                if (edgeAxes[i] == CardinalAxisType.X)
                {
                    Quaternion realignment = Quaternion.FromToRotation(Vector3.up, Vector3.right);
                    midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
                }
                else if (edgeAxes[i] == CardinalAxisType.Z)
                {
                    Quaternion realignment = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                    midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
                }

                Bounds midpointBounds = BoundingBoxHandleUtils.GetMaxBounds(midpointVisual);
                float maxDim = Mathf.Max(
                    Mathf.Max(midpointBounds.size.x, midpointBounds.size.y),
                    midpointBounds.size.z);
                float invScale = HandleSize / maxDim;

                midpointVisual.transform.parent = midpoint.transform;
                midpointVisual.transform.localScale = new Vector3(invScale, invScale, invScale);
                midpointVisual.transform.localPosition = Vector3.zero;

                BoundingBoxHandleUtils.AddComponentsToAffordance(midpoint, new Bounds(midpointBounds.center * invScale, midpointBounds.size * invScale),
                    rotationHandlePrefabColliderType, CursorContextInfo.CursorAction.Rotate, ColliderPadding, parent, drawManipulationTether);

                handles.Add(midpoint.transform);

                if (HandleMaterial != null)
                {
                    BoundingBoxHandleUtils.ApplyMaterialToAllRenderers(midpointVisual, HandleMaterial);
                }
            }

        }



    }
}
