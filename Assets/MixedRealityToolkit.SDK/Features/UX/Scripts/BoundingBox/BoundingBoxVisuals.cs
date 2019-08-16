using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    class BoundingBoxVisuals
    {

        #region Enums

        /// <summary>
        /// This enum defines which of the axes a given rotation handle revolves about.
        /// </summary>
        private enum CardinalAxisType
        {
            X = 0,
            Y,
            Z
        }
        /// <summary>
        /// Internal state tracking for proximity of a handle
        /// </summary>
        private enum HandleProximityState
        {
            FullsizeNoProximity = 0,
            MediumProximity,
            CloseProximity
        }

        /// <summary>
        /// Container for handle references and states (including scale and rotation type handles)
        /// </summary>
        private class Handle
        {
            public Transform HandleVisual;
            public Renderer HandleVisualRenderer;
            public HandleType Type = HandleType.None;
            public HandleProximityState ProximityState = HandleProximityState.FullsizeNoProximity;
        }

        #endregion Enums

        private Vector3[] boundsCorners;

        private Vector3[] edgeCenters;

        private List<Handle> handles;

        private List<Transform> corners;

        private List<Transform> links;
        private List<Renderer> linkRenderers;



        /// <summary>
        /// Returns list of transforms pointing to the scale handles of the bounding box.
        /// </summary>
        public IReadOnlyList<Transform> ScaleCorners
        {
            get { return corners; }
        }

        private List<Transform> balls;

        /// <summary>
        /// Returns list of transforms pointing to the rotation handles of the bounding box.
        /// </summary>
        public IReadOnlyList<Transform> RotateMidpoints
        {
            get { return balls; }
        }

        private void DestroyRig()
        {
            if (boundsOverride == null)
            {
                Destroy(TargetBounds);
            }
            else
            {
                boundsOverride.size -= boxPadding;

                if (TargetBounds != null)
                {
                    if (TargetBounds.gameObject.GetComponent<NearInteractionGrabbable>())
                    {
                        Destroy(TargetBounds.gameObject.GetComponent<NearInteractionGrabbable>());
                    }
                }
            }

            if (handles != null)
            {
                handles.Clear();
            }

            if (balls != null)
            {
                foreach (Transform transform in balls)
                {
                    Destroy(transform.gameObject);
                }

                balls.Clear();
            }

            if (links != null)
            {
                foreach (Transform transform in links)
                {
                    Destroy(transform.gameObject);
                }
                links.Clear();
                links = null;
            }

            if (corners != null)
            {
                foreach (Transform transform in corners)
                {
                    Destroy(transform.gameObject);
                }

                corners.Clear();
            }

            if (rigRoot != null)
            {
                Destroy(rigRoot.gameObject);
                rigRoot = null;
            }

        }

        private void UpdateRigVisibilityInInspector()
        {
            HideFlags desiredFlags = hideElementsInInspector ? HideFlags.HideInHierarchy | HideFlags.HideInInspector : HideFlags.None;
            if (corners != null)
            {
                foreach (var cube in corners)
                {
                    cube.hideFlags = desiredFlags;
                }
            }

            if (boxDisplay != null)
            {
                boxDisplay.hideFlags = desiredFlags;
            }

            if (rigRoot != null)
            {
                rigRoot.hideFlags = desiredFlags;
            }

            if (links != null)
            {
                foreach (var link in links)
                {
                    link.hideFlags = desiredFlags;
                }
            }
        }

        private void SetHighlighted(Transform activeHandle)
        {
            //turn off all balls
            if (balls != null)
            {
                for (int i = 0; i < balls.Count; ++i)
                {
                    if (balls[i] != activeHandle)
                    {
                        balls[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        ApplyMaterialToAllRenderers(balls[i].gameObject, handleGrabbedMaterial);
                    }
                }
            }

            //turn off all corners
            if (corners != null)
            {
                for (int i = 0; i < corners.Count; ++i)
                {
                    if (corners[i] != activeHandle)
                    {
                        corners[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        ApplyMaterialToAllRenderers(corners[i].gameObject, handleGrabbedMaterial);
                    }
                }
            }

            //update the box material to the grabbed material
            if (boxDisplay != null)
            {
                ApplyMaterialToAllRenderers(boxDisplay, boxGrabbedMaterial);
            }
        }


        private void HandleProximityScaling()
        {
            //only use proximity effect if nothing is being dragged or grabbed
            if (currentPointer == null)
            {
                proximityPointers.Clear();
                proximityPoints.Clear();

                // Find all valid pointers
                foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
                {
                    foreach (var pointer in inputSource.Pointers)
                    {
                        if (pointer.IsInteractionEnabled && !proximityPointers.Contains(pointer))
                        {
                            proximityPointers.Add(pointer);
                        }
                    }
                }

                // Get the max radius possible of our current bounds plus the proximity
                float maxRadius = Mathf.Max(Mathf.Max(currentBoundsExtents.x, currentBoundsExtents.y), currentBoundsExtents.z);
                maxRadius *= maxRadius;
                maxRadius += handleCloseProximity + handleMediumProximity;

                // Grab points within sphere of inluence from valid pointers
                foreach (var pointer in proximityPointers)
                {
                    if (IsPointWithinBounds(pointer.Position, maxRadius))
                    {
                        proximityPoints.Add(pointer.Position);
                    }

                    if (IsPointWithinBounds(pointer.Result.Details.Point, maxRadius))
                    {
                        proximityPoints.Add(pointer.Result.Details.Point);
                    }
                }

                // Loop through all handles and find closest one
                int closestHandleIdx = -1;
                float closestDistanceSqr = float.MaxValue;
                foreach (var point in proximityPoints)
                {
                    for (int i = 0; i < handles.Count; ++i)
                    {
                        // If handle can't be visisble, skip calculations
                        if (!IsHandleTypeVisible(handles[i].Type))
                            continue;

                        // Perform comparison on sqr distance since sqrt() operation is expensive in Vector3.Distance()
                        float sqrDistance = (handles[i].HandleVisual.position - point).sqrMagnitude;
                        if (sqrDistance < closestDistanceSqr)
                        {
                            closestHandleIdx = i;
                            closestDistanceSqr = sqrDistance;
                        }
                    }
                }

                // Loop through all handles and update visual state based on closest point
                for (int i = 0; i < handles.Count; ++i)
                {
                    Handle h = handles[i];

                    HandleProximityState newState = i == closestHandleIdx ? GetProximityState(closestDistanceSqr) : HandleProximityState.FullsizeNoProximity;

                    // Only apply updates if handle is in a new state or closest handle needs to lerp scaling
                    if (h.ProximityState != newState)
                    {
                        // Update and save new state
                        h.ProximityState = newState;

                        if (h.HandleVisualRenderer != null)
                        {
                            h.HandleVisualRenderer.material = newState == HandleProximityState.CloseProximity ? handleGrabbedMaterial : handleMaterial;
                        }
                    }

                    ScaleHandle(h, true);
                }
            }
        }



        /// <summary>
        /// Get the ProximityState value based on the distanced provided
        /// </summary>
        /// <param name="sqrDistance">distance squared in proximity in meters</param>
        /// <returns>HandleProximityState for given distance</returns>
        private HandleProximityState GetProximityState(float sqrDistance)
        {
            if (sqrDistance < handleCloseProximity * handleCloseProximity)
            {
                return HandleProximityState.CloseProximity;
            }
            else if (sqrDistance < handleMediumProximity * handleMediumProximity)
            {
                return HandleProximityState.MediumProximity;
            }
            else
            {
                return HandleProximityState.FullsizeNoProximity;
            }
        }


        private void ScaleHandle(Handle handle, bool lerp = false)
        {
            float handleSize = handle.Type == HandleType.Scale ? scaleHandleSize : rotationHandleSize;
            float targetScale = 1.0f, weight = 0.0f;

            switch (handle.ProximityState)
            {
                case HandleProximityState.FullsizeNoProximity:
                    targetScale = farScale;
                    weight = lerp ? farGrowRate : 1.0f;
                    break;
                case HandleProximityState.MediumProximity:
                    targetScale = mediumScale;
                    weight = lerp ? mediumGrowRate : 1.0f;
                    break;
                case HandleProximityState.CloseProximity:
                    targetScale = closeScale;
                    weight = lerp ? closeGrowRate : 1.0f;
                    break;
            }

            float newLocalScale = (handle.HandleVisual.localScale.x * (1.0f - weight)) + (handleSize * targetScale * weight);
            handle.HandleVisual.localScale = new Vector3(newLocalScale, newLocalScale, newLocalScale);
        }


        /// <summary>
        /// Helper method to check if handle type may be visible based on configuration
        /// </summary>
        /// <param name="h">handle reference to check</param>
        /// <returns>true if potentially visible, false otherwise</returns>
        private bool IsHandleTypeVisible(HandleType type)
        {
            return (type == HandleType.Scale && ShowScaleHandles) ||
                (type == HandleType.Rotation && (ShowRotationHandleForX || ShowRotationHandleForY || ShowRotationHandleForZ));
        }

        private void ResetHandleProximityScale()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                Handle h = handles[i];
                if (h.ProximityState != HandleProximityState.FullsizeNoProximity)
                {
                    h.ProximityState = HandleProximityState.FullsizeNoProximity;

                    if (h.HandleVisualRenderer != null)
                    {
                        h.HandleVisualRenderer.material = handleMaterial;
                    }

                    ScaleHandle(h);
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

        private void Flatten()
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

            if (flattenedHandles != null && linkRenderers != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    linkRenderers[flattenedHandles[i]].enabled = false;
                }
            }
        }

        private void SetHiddenHandles()
        {
            if (flattenedHandles != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    balls[flattenedHandles[i]].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Make the handle colliders ignore specified collider. (e.g. spatial mapping's floor collider to avoid the object get lifted up)
        /// </summary>
        private void HandleIgnoreCollider()
        {
            if (handlesIgnoreCollider != null)
            {
                foreach (Transform corner in corners)
                {
                    Collider[] colliders = corner.gameObject.GetComponents<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        UnityEngine.Physics.IgnoreCollision(collider, handlesIgnoreCollider);
                    }
                }

                foreach (Transform ball in balls)
                {
                    Collider[] colliders = ball.gameObject.GetComponents<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        UnityEngine.Physics.IgnoreCollision(collider, handlesIgnoreCollider);
                    }
                }
            }
        }

        private void SetMaterials()
        {
            //ensure materials
            if (wireframeMaterial == null)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                Shader shader = Shader.Find("Mixed Reality Toolkit/Standard");

                wireframeMaterial = new Material(shader);
                wireframeMaterial.EnableKeyword("_InnerGlow");
                wireframeMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                wireframeMaterial.SetFloat("_InnerGlow", 1.0f);
                wireframeMaterial.SetFloatArray("_InnerGlowColor", color);
            }
            if (handleMaterial == null && handleMaterial != wireframeMaterial)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                Shader shader = Shader.Find("Mixed Reality Toolkit/Standard");

                handleMaterial = new Material(shader);
                handleMaterial.EnableKeyword("_InnerGlow");
                handleMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                handleMaterial.SetFloat("_InnerGlow", 1.0f);
                handleMaterial.SetFloatArray("_InnerGlowColor", color);
            }
            if (handleGrabbedMaterial == null && handleGrabbedMaterial != handleMaterial && handleGrabbedMaterial != wireframeMaterial)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                Shader shader = Shader.Find("Mixed Reality Toolkit/Standard");

                handleGrabbedMaterial = new Material(shader);
                handleGrabbedMaterial.EnableKeyword("_InnerGlow");
                handleGrabbedMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                handleGrabbedMaterial.SetFloat("_InnerGlow", 1.0f);
                handleGrabbedMaterial.SetFloatArray("_InnerGlowColor", color);
            }
        }

        private void InitializeDataStructures()
        {
            boundsCorners = new Vector3[8];

            corners = new List<Transform>();
            balls = new List<Transform>();

            handles = new List<Handle>();

            if (showWireframe)
            {
                links = new List<Transform>();
                linkRenderers = new List<Renderer>();
            }

            sourcesDetected = new List<IMixedRealityController>();
        }

        private void CalculateEdgeCenters()
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

        private void ResetHandleVisibility()
        {
            if (currentPointer != null)
            {
                return;
            }

            bool isVisible;

            //set balls visibility
            if (balls != null)
            {
                isVisible = (active == true && wireframeOnly == false);
                for (int i = 0; i < balls.Count; ++i)
                {
                    balls[i].gameObject.SetActive(isVisible && ShouldRotateHandleBeVisible(edgeAxes[i]));
                    ApplyMaterialToAllRenderers(balls[i].gameObject, handleMaterial);
                }
            }

            //set link visibility
            if (links != null)
            {
                isVisible = active == true;
                for (int i = 0; i < linkRenderers.Count; ++i)
                {
                    if (linkRenderers[i] != null)
                    {
                        linkRenderers[i].enabled = isVisible;
                    }
                }
            }

            //set box display visibility
            if (boxDisplay != null)
            {
                boxDisplay.SetActive(active);
                ApplyMaterialToAllRenderers(boxDisplay, boxMaterial);
            }

            //set corner visibility
            if (corners != null)
            {
                isVisible = (active == true && wireframeOnly == false && showScaleHandles == true);

                for (int i = 0; i < corners.Count; ++i)
                {
                    corners[i].gameObject.SetActive(isVisible);
                    ApplyMaterialToAllRenderers(corners[i].gameObject, handleMaterial);
                }
            }

            SetHiddenHandles();
        }


        private static void ApplyMaterialToAllRenderers(GameObject root, Material material)
        {
            if (material != null)
            {
                Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

                for (int i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].material = material;
                }
            }
        }


        /// <summary>
        /// Destroys and re-creates the rig around the bounding box
        /// </summary>
        public void CreateRig()
        {
            DestroyRig();
            SetMaterials();
            InitializeRigRoot();
            InitializeDataStructures();
            SetBoundingBoxCollider();
            UpdateBounds();
            AddCorners();
            AddLinks();
            HandleIgnoreCollider();
            AddBoxDisplay();
            UpdateRigHandles();
            Flatten();
            ResetHandleVisibility();
            rigRoot.gameObject.SetActive(active);
            UpdateRigVisibilityInInspector();
        }


        private void InitializeRigRoot()
        {
            var rigRootObj = new GameObject(rigRootName);
            rigRoot = rigRootObj.transform;
            rigRoot.parent = transform;

            var pH = rigRootObj.AddComponent<PointerHandler>();
            pH.OnPointerDown.AddListener(OnPointerDown);
            pH.OnPointerDragged.AddListener(OnPointerDragged);
            pH.OnPointerUp.AddListener(OnPointerUp);
        }

        private void UpdateRigHandles()
        {
            if (rigRoot != null && Target != null)
            {
                // We move the rigRoot to the scene root to ensure that non-uniform scaling performed
                // anywhere above the rigRoot does not impact the position of rig corners / edges
                rigRoot.parent = null;

                rigRoot.rotation = Quaternion.identity;
                rigRoot.position = Vector3.zero;
                rigRoot.localScale = Vector3.one;

                for (int i = 0; i < corners.Count; ++i)
                {
                    corners[i].position = boundsCorners[i];
                }

                Vector3 rootScale = rigRoot.lossyScale;
                Vector3 invRootScale = new Vector3(1.0f / rootScale[0], 1.0f / rootScale[1], 1.0f / rootScale[2]);

                // Compute the local scale that produces the desired world space dimensions
                Vector3 linkDimensions = Vector3.Scale(GetLinkDimensions(), invRootScale);

                for (int i = 0; i < edgeCenters.Length; ++i)
                {
                    balls[i].position = edgeCenters[i];

                    if (links != null)
                    {
                        links[i].position = edgeCenters[i];

                        if (edgeAxes[i] == CardinalAxisType.X)
                        {
                            links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.x, wireframeEdgeRadius);
                        }
                        else if (edgeAxes[i] == CardinalAxisType.Y)
                        {
                            links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.y, wireframeEdgeRadius);
                        }
                        else//Z
                        {
                            links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.z, wireframeEdgeRadius);
                        }
                    }
                }

                if (boxDisplay != null)
                {
                    // Compute the local scale that produces the desired world space size
                    boxDisplay.transform.localScale = Vector3.Scale(GetBoxDisplayScale(), invRootScale);
                }

                //move rig into position and rotation
                rigRoot.position = TargetBounds.bounds.center;
                rigRoot.rotation = Target.transform.rotation;
                rigRoot.parent = transform;
            }
        }


        /// <summary>
        /// Allows to manually enable wire (edge) highlighting (edges) of the bounding box.
        /// This is useful if connected to the Manipulation events of a
        /// <see cref="Microsoft.MixedReality.Toolkit.UI.ManipulationHandler"/> 
        /// when used in conjunction with this MonoBehavior.
        /// </summary>
        public void HighlightWires()
        {
            SetHighlighted(null);
        }

        public void UnhighlightWires()
        {
            ResetHandleVisibility();
        }
    }
}
