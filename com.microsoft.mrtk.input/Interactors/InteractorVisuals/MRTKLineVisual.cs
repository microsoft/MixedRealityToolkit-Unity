// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This visual component helps align a <see cref="LineRenderer"/> with the Interactor, while giving it "bendy" qualities
    /// via the Bezier Data Provider
    /// </summary>
    [AddComponentMenu("MRTK/Input/MRTK Line Visual")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_LineVisual)]
    public class MRTKLineVisual : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField]
        [Tooltip("Color gradient when there is no applicable target.")]
        Gradient noTargetColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        ///Color gradient when there is no applicable target.
        /// </summary>
        public Gradient NoTargetColorGradient
        {
            get => noTargetColorGradient;
            set => noTargetColorGradient = value;
        }

        [SerializeField]
        [Tooltip("Color gradient when hovering over a valid target.")]
        Gradient validColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        /// Color gradient when hovering over a valid target.
        /// </summary>
        public Gradient ValidColorGradient
        {
            get => validColorGradient;
            set => validColorGradient = value;
        }

        [SerializeField]
        [Tooltip("Color gradient during a selection.")]
        Gradient selectActiveColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        /// Color gradient during a selection.
        /// </summary>
        public Gradient SelectActiveColorGradient
        {
            get => selectActiveColorGradient;
            set => selectActiveColorGradient = value;
        }

        [SerializeField]
        [Tooltip("On hit, the gradient will be applied evenly along the line renderer until it's total length is longer than this value multiplied by the ray interactor's max raycast distance")]
        [Range(0.01f, 1)]
        float maxGradientLength = 0.3f;

        /// <summary>
        /// On hit, the gradient will be applied evenly along the line renderer until it's total length is longer than this value multiplied by the ray interactor's max raycast distance.
        /// </summary>
        public float MaxGradientLength
        {
            get => maxGradientLength;
            set => maxGradientLength = value;
        }

        [SerializeField]
        [Tooltip("The width of the line.")]
        private AnimationCurve lineWidth = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        /// <summary>
        /// The width of the line.
        /// </summary>
        public AnimationCurve LineWidth
        {
            get => lineWidth;
            set => lineWidth = value;
        }

        [Range(0.0001f, 1f)]
        [SerializeField]
        [Tooltip("The overall multiplier that is applied to the LineRenderer to get the final width of the line.")]
        private float widthMultiplier = 0.0015f;

        /// <summary>
        /// The overall multiplier that is applied to the LineRenderer to get the final width of the line.
        /// </summary>
        public float WidthMultiplier
        {
            get => widthMultiplier;
            set => widthMultiplier = Mathf.Clamp(value, 0f, 10f);
        }

        [Tooltip("Where to place the first control point of the bezier curve.")]
        [SerializeField]
        [Range(0f, 0.5f)]
        private float startPointLerp = 0.267f;

        [SerializeField]
        [Tooltip("Where to place the second control point of the bezier curve.")]
        [Range(0.5f, 1f)]
        private float endPointLerp = 0.637f;

        [Header("Mixed Reality Line Renderer Settings")]
        [SerializeField]
        [Tooltip("The ray interactor which this visual represents.")]
        private XRRayInteractor rayInteractor;

        [SerializeField]
        [Tooltip("The line renderer this visual has control over.")]
        private LineRenderer lineRenderer = null;

        [SerializeField]
        [Tooltip("The line data that represented by this visual.")]
        private BaseMixedRealityLineDataProvider lineDataProvider = null;

        [SerializeField]
        [Tooltip("Whether to round the edges of the line renderer.")]
        private bool roundedEdges = true;

        /// <summary>
        /// Whether to round the edges of the line renderer.
        /// </summary>
        public bool RoundedEdges
        {
            get => roundedEdges;
            set => roundedEdges = value;
        }

        [SerializeField]
        [Tooltip("Whether to round the endpoints of the line renderer.")]
        private bool roundedCaps = true;

        /// <summary>
        /// Whether to round the endpoints of the line renderer.
        /// </summary>
        public bool RoundedCaps
        {
            get => roundedCaps;
            set => roundedCaps = value;
        }

        [SerializeField]
        [Tooltip("Whether the line renderer stops after hitting an object.")]
        private bool stopLineAtFirstRaycastHit = true;

        /// <summary>
        /// Whether the line renderer stops after hitting an object.
        /// </summary>
        public bool StopLineAtFirstRaycastHit
        {
            get => stopLineAtFirstRaycastHit;
            set => stopLineAtFirstRaycastHit = value;
        }

        // Reusable array for retrieving points from the XRRayInteractor
        private Vector3[] rayPositions = null;
        private int rayPositionsCount = -1;

        // reusable lists of the points used for the line renderer
        private Vector3[] rendererPositions;

        // reusable values derived from raycast hit data
        private Vector3 reticlePosition;
        private Transform hitTargetTransform;
        private Vector3 targetLocalHitPoint;
        private float hitDistance;

        /// <summary>
        /// Used internally to determine if the ray we are visualizing hit an object or not.
        /// </summary>
        private bool rayHasHit;

        // private array used to clear the line renderer when needed
        private readonly Vector3[] clearPositions = { Vector3.zero, Vector3.zero };

        // Property block for writing per-object material properties
        private MaterialPropertyBlock propertyBlock;

        #region MonoBehaviour

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Reset()
        {
            if (TryFindLineRenderer())
            {
                ClearLineRenderer();
                InitializeLineRendererProperties();
            }

            // Try to find a corresponding line data source and raise a warning if it was not initialized and does not exist
            if (lineDataProvider == null && !TryGetComponent(out lineDataProvider))
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
                InitializeLineRendererProperties();
            }
        }
#endif // UNITY_EDITOR

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            propertyBlock = new MaterialPropertyBlock();

            rayInteractor.selectEntered.AddListener(LocateTargetHitPoint);
            Application.onBeforeRender += UpdateLineVisual;

            Reset();
            UpdateLineVisual();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            rayInteractor.selectEntered.RemoveListener(LocateTargetHitPoint);
            Application.onBeforeRender -= UpdateLineVisual;

            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }

        #endregion

        #region LineVisual Updates

        private static readonly ProfilerMarker UpdateLinePerfMarker = new ProfilerMarker("[MRTK] MRTKLineVisual.UpdateLineVisual");

        // Cached value of the current gradient. Used to avoid making calls to lineRenderer.colorGradient, which allocs
        private Gradient cachedGradient = new Gradient();

        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
        private void UpdateLineVisual()
        {
            using(UpdateLinePerfMarker.Auto())
            {
                InitializeLineRendererProperties();

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

                // Exit early if the line renderer is ultimately disabled
                if (!lineRenderer.enabled)
                {
                    ClearLineRenderer();
                    return;
                }

                // Assign the first point to the ray origin
                lineDataProvider.FirstPoint = rayPositions[0];

                // If the interactor is currently selecting, lock the end of the ray to the selected object
                if (rayInteractor.hasSelection)
                {
                    // Assign the last point to the one saved by the callback
                    lineDataProvider.LastPoint = hitTargetTransform.TransformPoint(targetLocalHitPoint);
                    rayHasHit = true;
                }
                // Otherwise draw out the line exactly as the Ray Interactor prescribes
                else
                {
                    // If the ray hits an object, truncate the visual appropriately
                    // Remove the last point in the list to keep the number of points consistent.
                    if (rayInteractor.TryGetHitInfo(out reticlePosition, out _, out int endPositionInLine, out bool isValidTarget))
                    {
                        // End the line at the current hit point.
                        if ((isValidTarget || StopLineAtFirstRaycastHit) && endPositionInLine > 0 && endPositionInLine < rayPositionsCount)
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
                }

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

                // Now handle coloring the line visual
                // If our interactor is a variable select interactor, change the material property based on select progress
                IVariableSelectInteractor variableSelectInteractor = rayInteractor as IVariableSelectInteractor;
                if (variableSelectInteractor != null)
                {
                    lineRenderer.GetPropertyBlock(propertyBlock);
                    propertyBlock.SetFloat("_Shift_", variableSelectInteractor.SelectProgress);
                    lineRenderer.SetPropertyBlock(propertyBlock);
                }

                // If we are hovering over a valid object or are currently selecting one, lerp the color based on selectedness
                if (rayHasHit || rayInteractor.hasSelection)
                {
                    if (variableSelectInteractor != null)
                    {
                        cachedGradient = ColorUtilities.GradientLerp(ValidColorGradient, SelectActiveColorGradient, variableSelectInteractor.SelectProgress);
                    }
                    else
                    {
                        cachedGradient = ColorUtilities.GradientLerp(ValidColorGradient, SelectActiveColorGradient, rayInteractor.hasSelection ? 1 : 0);
                    }

                    // apply the compression effect
                    var compressionAmount = Mathf.Clamp(rayInteractor.maxRaycastDistance * MaxGradientLength / hitDistance, 0.0f, 1.0f);
                    cachedGradient = ColorUtilities.GradientCompress(cachedGradient, 0.0f, compressionAmount);
                }
                else
                {
                    cachedGradient = NoTargetColorGradient;
                }

                lineRenderer.colorGradient = cachedGradient;
            }
        }

        private void InitializeLineRendererProperties()
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

        /// <summary>
        /// Used to locate and lock the raycast hit data on a select
        /// </summary>
        private void LocateTargetHitPoint(SelectEnterEventArgs args)
        {
            // If no hit interactable, abort
            if (args == null)
            {
                return;
            }

            bool hitPointAndTransformUpdated = false;

            // In the case of affordances/handles, we can stick the ray right on to the handle.
            if (args.interactableObject is ISnapInteractable snappable)
            {
                hitTargetTransform = snappable.HandleTransform;
                targetLocalHitPoint = Vector3.zero;
                hitPointAndTransformUpdated = true;
            }

            // In the case of an IScrollable being selected, ensure that the line visual locks to
            // the scroller and not to the a list item within the scroller, such as a button.
            if (args.interactableObject is IScrollable scrollable &&
                scrollable.IsScrolling &&
                scrollable.ScrollingInteractor == (IXRInteractor)rayInteractor)
            {
                hitTargetTransform = scrollable.ScrollableTransform;
                targetLocalHitPoint = scrollable.ScrollingLocalAnchorPosition;
                hitPointAndTransformUpdated = true;
            }

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

            // If we haven't even gotten any ray positions yet, abort.
            if (rayPositions == null || rayPositionsCount <= 0)
            {
                return;
            }

            // Record relevant data about the hit point.
            if (raycastResult.HasValue && isUIHitClosest)
            {
                hitTargetTransform = raycastResult.Value.gameObject.transform;
                targetLocalHitPoint = hitTargetTransform.InverseTransformPoint(raycastResult.Value.worldPosition);
                hitDistance = (raycastResult.Value.worldPosition - rayPositions[0]).magnitude;
            }
            else if (raycastHit.HasValue)
            {
                if (!hitPointAndTransformUpdated)
                {
                    hitTargetTransform = raycastHit.Value.collider.transform;
                    targetLocalHitPoint = hitTargetTransform.InverseTransformPoint(raycastHit.Value.point);
                }

                hitDistance = (hitTargetTransform.TransformPoint(targetLocalHitPoint) - rayPositions[0]).magnitude;
            }
        }

        #endregion

        private bool TryFindLineRenderer()
        {
            if (lineRenderer == null)
            {
                if (!TryGetComponent(out lineRenderer))
                {
                    Debug.LogWarning("No Line Renderer found for MRTK Ray Interactor Visual.", this);
                    enabled = false;
                    return false;
                }
            }
            return true;
        }

        private void ClearLineRenderer()
        {
            if (TryFindLineRenderer())
            {
                lineRenderer.SetPositions(clearPositions);
                lineRenderer.positionCount = 0;
            }
        }

        [Range(2, 128)]
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
