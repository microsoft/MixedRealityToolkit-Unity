// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interactor helper object aligns a <see cref="LineRenderer"/> with the Interactor.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public class MRTKRayInteractorVisual : MonoBehaviour, IXRCustomReticleProvider
    {
        [Header("Visual Settings")]

        [SerializeField]
        [Tooltip("The reticle (cursor), usually an IVariableReticle, along with a proximity light.")]
        private GameObject reticle;

        /// <summary>
        /// The reticle (cursor), usually an IVariableReticle, along with a proximity light.
        /// </summary>
        public GameObject Reticle
        {
            get => reticle;
            set
            {
                if (reticle != value)
                {
                    reticle = value;
                    if (reticle != null)
                    {
                        variableSelectReticle = reticle.GetComponentInChildren<IVariableReticle>();
                    }
                }
            }
        }

        [SerializeField]
        [Tooltip("Color gradient when there is no applicable target")]
        Gradient noTargetColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate a valid state.
        /// </summary>
        public Gradient NoTargetColorGradient
        {
            get => noTargetColorGradient;
            set => noTargetColorGradient = value;
        }

        [SerializeField]
        [Tooltip("Color gradient when hovering over a valid target")]
        Gradient validColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate a valid state.
        /// </summary>
        public Gradient ValidColorGradient
        {
            get => validColorGradient;
            set => validColorGradient = value;
        }

        [SerializeField]
        [Tooltip("Color gradient during a selection")]
        Gradient selectActiveColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate a valid state.
        /// </summary>
        public Gradient SelectActiveColorGradient
        {
            get => selectActiveColorGradient;
            set => selectActiveColorGradient = value;
        }

        [SerializeField]
        private AnimationCurve lineWidth = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public AnimationCurve LineWidth
        {
            get => lineWidth;
            set => lineWidth = value;
        }

        [Range(0.0001f, 1f)]
        [SerializeField]
        private float widthMultiplier = 0.0015f;

        public float WidthMultiplier
        {
            get => widthMultiplier;
            set => widthMultiplier = Mathf.Clamp(value, 0f, 10f);
        }

        [Tooltip("Where to place the first control point of the bezier curve")]
        [SerializeField]
        [Range(0f, 0.5f)]
        private float startPointLerp = 0.267f;

        [SerializeField]
        [Tooltip("Where to place the second control point of the bezier curve")]
        [Range(0.5f, 1f)]
        private float endPointLerp = 0.637f;

        [Header("Mixed Reality Line Renderer Settings")]
        [SerializeField]
        [Tooltip("The ray interactor which this visual represents.")]
        private XRRayInteractor rayInteractor;

        [SerializeField]
        private bool roundedEdges = true;

        public bool RoundedEdges
        {
            get => roundedEdges;
            set => roundedEdges = value;
        }

        [SerializeField]
        private bool roundedCaps = true;

        public bool RoundedCaps
        {
            get => roundedCaps;
            set => roundedCaps = value;
        }

        [SerializeField]
        [HideInInspector]
        private LineRenderer lineRenderer = null;

        [SerializeField]
        [Tooltip("The line data that represented by this visual.")]
        internal BaseMixedRealityLineDataProvider lineDataProvider = null;

        // reusable lists of the points returned by the XRRayInteractor
        Vector3[] rayPositions;
        int rayPositionsCount = -1;

        // reusable lists of the points used for the line renderer
        private Vector3[] rendererPositions;

        // reusable vectors for determining the raycast hit data
        private Vector3 reticlePosition;
        private Vector3 reticleNormal;
        private int endPositionInLine;

        // private array used to clear the line renderer when needed
        readonly Vector3[] clearPositions = { Vector3.zero, Vector3.zero };

        // Property block for writing per-object material properties
        private MaterialPropertyBlock propertyBlock;

        // If an interactable requests a custom reticle, it'll be referenced
        // here.
        private GameObject customReticle;

        // The IVariableReticle associated with our standard, non-custom reticle.
        private IVariableReticle variableSelectReticle;

        // The IVariableReticle associated with a custom reticle.
        private IVariableReticle customVariableReticle;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Reset()
        {
            if (TryFindLineRenderer())
            {
                ClearLineRenderer();
                UpdateLineRendererProperties();
            }

            // Try to find a corresponding line data source and raise a warning if it does not exist
            lineDataProvider = GetComponent<BaseMixedRealityLineDataProvider>();
            if (lineDataProvider == null)
            {
                Debug.LogWarning("No Line Data Provider found for Interactor Line Visual.", this);
                enabled = false;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnValidate()
        {
            // We check if this instance has actually been changed, since OnValidate() is called
            // on save and Unity detects any setter calls (even if we're just setting the same value)
            // as dirtying the line renderer. If MRTK is consumed via UPM, this causes
            // Unity to try to save "changes" to a prefab in an immutable folder.
            if (UnityEditor.EditorUtility.IsDirty(this))
            {
                UpdateLineRendererProperties();
            }
        }
#endif // UNITY_EDITOR

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            rayInteractor.selectEntered.AddListener(LocateTargetHitPoint);

            propertyBlock = new MaterialPropertyBlock();

            if (reticle != null)
            {
                variableSelectReticle = reticle.GetComponentInChildren<IVariableReticle>();
            }

            // mafinc - Start the line renderer off disabled (invisible), we'll enable it
            // when we have enough data for it to render properly.
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }

            Reset();
            Application.onBeforeRender += OnBeforeRenderLineVisual;
            UpdateLineRendererProperties();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }

            // Update reticle one last time to shut it off.
            UpdateReticle(false);

            Application.onBeforeRender -= OnBeforeRenderLineVisual;
        }

        private bool stopLineAtFirstRaycastHit = true;
        private Vector3 targetLocalHitPoint;
        private Vector3 targetLocalHitNormal;
        private float hitDistance;
        private Transform hitTargetTransform;
        /// <summary>
        /// Used internally to determine if the ray we are visualizing hit an object or not.
        /// </summary>
        private bool rayHasHit;

        public void LocateTargetHitPoint(SelectEnterEventArgs args)
        {
            // If no hit, abort.
            if (!rayInteractor.TryGetCurrentRaycast(
                out RaycastHit? raycastHit,
                out _,
                out UnityEngine.EventSystems.RaycastResult? raycastResult,
                out _,
                out bool isUIHitClosest))
            {
                return;
            }

            // Record relevant data about the hit point.
            if (raycastResult.HasValue && isUIHitClosest)
            {
                hitTargetTransform = raycastResult.Value.gameObject.transform;
                targetLocalHitPoint = hitTargetTransform.InverseTransformPoint(raycastResult.Value.worldPosition);
                targetLocalHitNormal = hitTargetTransform.InverseTransformDirection(raycastResult.Value.worldNormal);
                hitDistance = (raycastResult.Value.worldPosition - rayPositions[0]).magnitude;
            }
            else if (raycastHit.HasValue)
            {
                // In the case of affordances/handles, we can stick the ray right on to the handle.
                if (args.interactableObject is ISnapInteractable snappable)
                {
                    hitTargetTransform = snappable.HandleTransform;
                    targetLocalHitPoint = Vector3.zero;
                    targetLocalHitNormal = Vector3.up;
                }
                else
                {
                    hitTargetTransform = raycastHit.Value.collider.transform;
                    targetLocalHitPoint = hitTargetTransform.InverseTransformPoint(raycastHit.Value.point);
                    targetLocalHitNormal = hitTargetTransform.InverseTransformPoint(raycastHit.Value.normal);
                }

                hitDistance = (raycastHit.Value.point - rayPositions[0]).magnitude;
            }
        }

        #region IXRCustomReticleProvider Implementation

        /// <inheritdoc />
        public bool AttachCustomReticle(GameObject reticleInstance)
        {
            // If we don't already have a custom reticle,
            // disable our standard reticle.
            if (customReticle == null)
            {
                if (reticle != null)
                {
                    reticle.SetActive(false);
                }
            }
            else if (customReticle != null)
            {
                // Otherwise, disable our current custom reticle.
                customReticle.SetActive(false);
            }

            // Install the new custom reticle.
            customReticle = reticleInstance;
            if (customReticle != null)
            {
                customReticle.SetActive(true);
                customVariableReticle = customReticle.GetComponentInChildren<IVariableReticle>();
            }
            return false;
        }

        /// <inheritdoc />
        public bool RemoveCustomReticle()
        {
            if (customReticle != null)
            {
                customReticle.SetActive(false);
            }

            // If we have a standard reticle, re-enable that one.
            if (reticle != null)
            {
                reticle.SetActive(true);
            }

            customReticle = null;
            return false;
        }

        #endregion IXRCustomReticleProvider Implementation

        private void ClearLineRenderer()
        {
            if (TryFindLineRenderer())
            {
                lineRenderer.SetPositions(clearPositions);
                lineRenderer.positionCount = 0;
            }

            UpdateReticle(false);
        }

        private void OnBeforeRenderLineVisual()
        {
            UpdateLineVisual();
        }

        private void UpdateLineVisual()
        {
            UpdateLineRendererProperties();

            if (lineRenderer == null)
            {
                return;
            }
            if (rayInteractor == null)
            {
                lineRenderer.enabled = false;
                return;
            }

            // Get all the line sample points from the ILineRenderable interface
            if (!rayInteractor.GetLinePoints(ref rayPositions, out rayPositionsCount))
            {
                lineRenderer.enabled = false;
                ClearLineRenderer();
                return;
            }

            // Sanity check.
            if (rayPositions == null ||
                rayPositions.Length == 0 ||
                rayPositionsCount == 0 ||
                rayPositionsCount > rayPositions.Length)
            {
                lineRenderer.enabled = false;
                ClearLineRenderer();
                return;
            }

            // Finally enable the line renderer if we pass the other checks
            lineRenderer.enabled = rayInteractor.isHoverActive;

            // Assign the first point to the ray origin
            lineDataProvider.FirstPoint = rayPositions[0];

            IVariableSelectInteractor variableSelectInteractor = rayInteractor as IVariableSelectInteractor;

            if (variableSelectInteractor != null)
            {
                lineRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_Shift_", variableSelectInteractor.SelectProgress);
                lineRenderer.colorGradient = ColorUtilities.GradientLerp(ValidColorGradient, SelectActiveColorGradient, variableSelectInteractor.SelectProgress);
                lineRenderer.SetPropertyBlock(propertyBlock);
            }

            // If the interactor is currently selecting, lock the end of the ray to the selected object
            if (rayInteractor.interactablesSelected.Count > 0)
            {
                // Assign the last point to the one saved by the callback
                lineDataProvider.LastPoint = hitTargetTransform.TransformPoint(targetLocalHitPoint);
                // Cursor and proximity light should follow the same last point
                reticlePosition = lineDataProvider.LastPoint;
                reticleNormal = hitTargetTransform.TransformDirection(targetLocalHitNormal);
                rayHasHit = true;
            }
            // Otherwise draw out the line exactly as the Ray Interactor perscribes
            else
            {
                // If the ray hits an object, truncate the visual appropriately
                // Remove the last point in the list to keep the number of points consistent.
                if (rayInteractor.TryGetHitInfo(out reticlePosition, out reticleNormal, out endPositionInLine, out bool isValidTarget))
                {
                    // End the line at the current hit point.
                    if ((isValidTarget || stopLineAtFirstRaycastHit) && endPositionInLine > 0 && endPositionInLine < rayPositionsCount)
                    {
                        rayPositions[endPositionInLine] = reticlePosition;
                        rayPositionsCount = endPositionInLine + 1;

                        hitDistance = (reticlePosition - rayPositions[0]).magnitude;
                        rayHasHit = true;
                    }
                    else
                    {
                        rayHasHit = false;
                    }
                }
                else
                {
                    rayHasHit = false;
                }

                // Assign the last point to last point in the data structure
                lineDataProvider.LastPoint = rayPositions[rayPositionsCount - 1];

                // If we are hovering over a valid object, lerp the color based on pinchedness if applicable
                if (rayHasHit)
                {
                    if (variableSelectInteractor != null)
                    {
                        lineRenderer.colorGradient = ColorUtilities.GradientLerp(ValidColorGradient, SelectActiveColorGradient, variableSelectInteractor.SelectProgress);
                    }
                    else
                    {
                        lineRenderer.colorGradient = ColorUtilities.GradientLerp(ValidColorGradient, SelectActiveColorGradient, rayInteractor.hasSelection ? 1 : 0);
                    }
                }
                else
                {
                    lineRenderer.colorGradient = NoTargetColorGradient;
                }
            }

            UpdateReticle(rayInteractor.hasHover || rayInteractor.hasSelection);

            // Project forward based on pointer direction to get an 'expected' position of the first control point if we've hit an object
            if (rayHasHit)
            {
                Vector3 startPoint = lineDataProvider.FirstPoint;
                Vector3 expectedPoint = startPoint + rayInteractor.rayOriginTransform.forward * hitDistance;

                // Lerp between the expected position and the expected point if we've hit an object
                lineDataProvider.SetPoint(1, Vector3.Lerp(startPoint, expectedPoint, startPointLerp));

                // Get our next 'expected' position by lerping between the expected point and the end point
                // The result will be a line that starts moving in the pointer's direction then bends towards the target
                expectedPoint = Vector3.Lerp(expectedPoint, lineDataProvider.LastPoint, endPointLerp);

                lineDataProvider.SetPoint(2, Vector3.Lerp(startPoint, expectedPoint, endPointLerp));
            }

            // Set positions for the rendered ray visual after passing it through the lineDataProvider
            lineRenderer.positionCount = lineStepCount;

            if (rendererPositions == null || rendererPositions.Length != lineRenderer.positionCount)
            {
                rendererPositions = new Vector3[lineStepCount];
            }

            for (int i = 0; i < lineStepCount; i++)
            {
                float normalizedDistance = GetNormalizedPointAlongLine(i);
                rendererPositions[i] = lineDataProvider.GetPoint(normalizedDistance);
            }
            lineRenderer.SetPositions(rendererPositions);
        }

        // Based on whether the ray hit anything, the current hit position, normal, etc,
        // update the current reticle (either standard or custom).
        private void UpdateReticle(bool showCursor)
        {
            // Grab the reticle we're currently using
            GameObject reticleToUse = customReticle != null ? customReticle : reticle;
            IVariableReticle variableReticleToUse = customReticle != null ? customVariableReticle : variableSelectReticle;

            if (reticleToUse == null) { return; }

            if (showCursor)
            {
                // Set the relevant reticle position/normal and ensure it's active.
                reticleToUse.transform.position = reticlePosition;
                reticleToUse.transform.forward = reticleNormal;

                if (reticleToUse.activeSelf == false)
                {
                    reticleToUse.SetActive(true);
                }

                if (variableReticleToUse != null)
                {
                    if (rayInteractor is IVariableSelectInteractor variableSelectInteractor)
                    {
                        variableReticleToUse.UpdateVisuals(variableSelectInteractor.SelectProgress);
                    }
                    else
                    {
                        variableReticleToUse.UpdateVisuals(rayInteractor.isSelectActive ? 1 : 0);
                    }
                }
            }
            else
            {
                reticleToUse.SetActive(false);
            }
        }

        private void UpdateLineRendererProperties()
        {
            if (TryFindLineRenderer())
            {
                // Set line renderer properties
                lineRenderer.numCapVertices = RoundedCaps ? 8 : 0;
                lineRenderer.numCornerVertices = RoundedEdges ? 8 : 0;
                lineRenderer.useWorldSpace = true;
                lineRenderer.startWidth = 1;
                lineRenderer.endWidth = 1;
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
                lineRenderer.widthCurve = LineWidth;
                lineRenderer.widthMultiplier = WidthMultiplier;
                lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
                lineRenderer.lightProbeUsage = LightProbeUsage.Off;
            }
        }

        private bool TryFindLineRenderer()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
                if (lineRenderer == null)
                {
                    Debug.LogWarning("No Line Renderer found for MRTK Ray Interactor Visual.", this);
                    enabled = false;
                    return false;
                }
            }
            return true;
        }

        [Range(2, 2048)]
        [SerializeField]
        [Tooltip("Number of steps to interpolate along line in Interpolated step mode")]
        private int lineStepCount = 16;

        /// <summary>
        /// Gets the normalized distance along the line path (range 0 to 1) going the given number of steps provided
        /// </summary>
        /// <param name="stepNum">Number of steps to take "walking" along the curve </param>
        protected virtual float GetNormalizedPointAlongLine(int stepNum)
        {
            // Normalized length along line
            float normalizedDistance = (1f / (lineStepCount - 1)) * stepNum;

            return normalizedDistance;
        }
    }
}
