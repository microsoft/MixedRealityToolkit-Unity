using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    internal class BoundingBoxRotationHandles : BoundingBoxHandlesBase
    {


        internal const int NumEdges = 12;

        private Vector3[] edgeCenters = new Vector3[NumEdges];
        private CardinalAxisType[] edgeAxes;

        private int[] flattenedHandles;

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



        internal void ResetHandleVisibility(bool isVisible, Material handleMaterial)
        {
            //set balls visibility
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    handles[i].gameObject.SetActive(isVisible && ShouldRotateHandleBeVisible(edgeAxes[i]));
                    BoundingBox.ApplyMaterialToAllRenderers(handles[i].gameObject, handleMaterial);
                }
            }
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

        public override HandleType GetHandleType()
        {
            return HandleType.Rotation;
        }

        internal void CreateHandles(GameObject rotationHandlePrefab, Transform parent, float handleSize, 
            Material handleMaterial, RotationHandlePrefabCollider colliderType, Vector3 colliderPadding)
        {

            for (int i = 0; i < edgeCenters.Length; ++i)
            {
                GameObject midpoint = new GameObject();
                midpoint.name = "midpoint_" + i.ToString();
                midpoint.transform.position = edgeCenters[i];
                midpoint.transform.parent = parent;

                GameObject midpointVisual;
                if (rotationHandlePrefab != null)
                {
                    midpointVisual = GameObject.Instantiate(rotationHandlePrefab);
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
                float invScale = handleSize / maxDim;

                midpointVisual.transform.parent = midpoint.transform;
                midpointVisual.transform.localScale = new Vector3(invScale, invScale, invScale);
                midpointVisual.transform.localPosition = Vector3.zero;

                BoundingBoxHandleUtils.AddComponentsToAffordance(midpoint, new Bounds(midpointBounds.center * invScale, midpointBounds.size * invScale), colliderType, CursorContextInfo.CursorAction.Rotate, colliderPadding);

                handles.Add(midpoint.transform);

                if (handleMaterial != null)
                {
                    BoundingBox.ApplyMaterialToAllRenderers(midpointVisual, handleMaterial);
                }
            }

        }


        private bool ShouldRotateHandleBeVisible(CardinalAxisType axisType)
        {
            return
                (axisType == CardinalAxisType.X && showRotationHandleForX) ||
                (axisType == CardinalAxisType.Y && showRotationHandleForY) ||
                (axisType == CardinalAxisType.Z && showRotationHandleForZ);
        }

    }
}
