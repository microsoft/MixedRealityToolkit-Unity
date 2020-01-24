// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Rotation handles for <see cref="BoundsControl"/> that are used for rotating the
    /// Gameobject BoundsControl is attached to with near or far interaction
    /// </summary>
    public class RotationHandles : HandlesBase
    {
        protected override HandlesBaseConfiguration BaseConfig => config;
        private RotationHandlesConfiguration config;

        internal RotationHandles(RotationHandlesConfiguration configuration)
        {
            Debug.Assert(configuration != null, "Can't create BoundsControlRotationHandles without valid configuration");
            config = configuration;
        }

        internal const int NumEdges = 12;

        private Vector3[] edgeCenters = new Vector3[NumEdges];
        private CardinalAxisType[] edgeAxes;

        internal int GetRotationHandleIdx(Transform handle)
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

        internal Vector3 GetEdgeCenter(int index)
        {
            Debug.Assert(index >= 0 && index <= NumEdges, "Edge center index out of bounds");
            return edgeCenters[index];
        }

        internal CardinalAxisType GetAxisType(int index)
        {
            Debug.Assert(index >= 0 && index <= NumEdges, "Edge axes index out of bounds");
            return edgeAxes[index];
        }

        internal CardinalAxisType GetAxisType(Transform handle)
        {
            int index = GetRotationHandleIdx(handle);
            return GetAxisType(index);
        }

        private void UpdateHandles()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                handles[i].position = GetEdgeCenter(i);
            }
        }

        internal void CalculateEdgeCenters(ref Vector3[] boundsCorners)
        {
            if (boundsCorners != null && edgeCenters != null)
            {
                for (int i = 0; i < edgeCenters.Length; ++i)
                {
                    edgeCenters[i] = VisualUtils.GetLinkPosition(i, ref boundsCorners);
                }
            }

            UpdateHandles();
        }


        internal void InitEdgeAxis()
        { 
            edgeAxes = new CardinalAxisType[NumEdges];
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

        internal void FlattenHandles(ref int[] flattenedHandles)
        {
            if (flattenedHandles != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    handles[flattenedHandles[i]].gameObject.SetActive(false);
                }
            }
        }

        internal void Create(ref Vector3[] boundsCorners, Transform parent, bool drawManipulationTether)
        {
            edgeCenters = new Vector3[12];
            CalculateEdgeCenters(ref boundsCorners);
            InitEdgeAxis();
            CreateHandles(parent, drawManipulationTether);
        }
        
        private void CreateHandles(Transform parent, bool drawManipulationTether)
        {
            for (int i = 0; i < edgeCenters.Length; ++i)
            {
                GameObject midpoint = new GameObject();
                midpoint.name = "midpoint_" + i.ToString();
                midpoint.transform.position = edgeCenters[i];
                midpoint.transform.parent = parent;

                GameObject midpointVisual;
                GameObject prefabType = config.HandlePrefab;
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

                Bounds midpointBounds = VisualUtils.GetMaxBounds(midpointVisual);
                float maxDim = Mathf.Max(
                    Mathf.Max(midpointBounds.size.x, midpointBounds.size.y),
                    midpointBounds.size.z);
                float invScale = config.HandleSize / maxDim;

                midpointVisual.name = "visuals";
                midpointVisual.transform.parent = midpoint.transform;
                midpointVisual.transform.localScale = new Vector3(invScale, invScale, invScale);
                midpointVisual.transform.localPosition = Vector3.zero;

                VisualUtils.AddComponentsToAffordance(midpoint, new Bounds(midpointBounds.center * invScale, midpointBounds.size * invScale),
                    config.RotationHandlePrefabColliderType, CursorContextInfo.CursorAction.Rotate, config.ColliderPadding, parent, drawManipulationTether);

                handles.Add(midpoint.transform);

                if (config.HandleMaterial != null)
                {
                    VisualUtils.ApplyMaterialToAllRenderers(midpointVisual, config.HandleMaterial);
                }
            }
        }

        #region BoundsControlHandlerBase 
        internal override bool IsVisible(Transform handle)
        {
            CardinalAxisType axisType = GetAxisType(handle);
            return
                (axisType == CardinalAxisType.X && config.ShowRotationHandleForX) ||
                (axisType == CardinalAxisType.Y && config.ShowRotationHandleForY) ||
                (axisType == CardinalAxisType.Z && config.ShowRotationHandleForZ);
        }

        internal override HandleType GetHandleType()
        {
            return HandleType.Rotation;
        }

        protected override Transform GetVisual(Transform handle)
        {
            // visual is first child 
            Transform childTransform = handle.GetChild(0);
            if (childTransform != null && childTransform.name == "visuals")
            {
                return childTransform;
            }

            return null;
        }
        #endregion BoundsControlHandlerBase

        #region IProximityScaleObjectProvider 
        public override bool IsActive()
        {
            return config.ShowRotationHandleForX || config.ShowRotationHandleForY || config.ShowRotationHandleForZ;
        }

        #endregion IProximityScaleObjectProvider

    }
}
