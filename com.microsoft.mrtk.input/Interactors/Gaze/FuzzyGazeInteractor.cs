// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interactor used for interacting with interactables at a distance. This is handled via raycasts
    /// that update the current set of valid targets for this interactor.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("MRTK/Input/Fuzzy Gaze Interactor")]
    public class FuzzyGazeInteractor : GazeInteractor
    {
        #region FuzzyGazeInteractor

        // TODO: these fields need to override their respective equivalents in the underlying XRRayInteractor class
        // i.e. coneAngle should change the sphereCastRadius, maxGazeDistance should change the raycastDistance
        [SerializeField]
        [Tooltip("The angle where objects will be considered for fuzzy eye targeting")]
        private float coneAngle = 10.0f;

        [SerializeField]
        [Tooltip("The minimum distance an object can be at to be considered for fuzzy eye targeting")]
        private float minGazeDistance = 0.3f;

        [SerializeField]
        [Tooltip("The maximum distance an object can be at to be considered for fuzzy eye targeting")]
        private float maxGazeDistance = 10.0f;

        // Weights for the various criteria we use to determine the "gaze score" of an object. Lower scores means that the object is more
        // likely to be the target of the gaze.
        [SerializeField]
        private float distanceWeight = 0.25f;

        [SerializeField]
        private float angleWeight = 1;

        [SerializeField]
        private float distanceToCenterWeight = 0.5f;

        [SerializeField]
        private float angleToCenterWeight = 0.0f;

        [SerializeField]
        [Tooltip("Used to adjust the precision of the fuzzy gaze physics raycasts. Each level adds an additional physics cast to improve precision of scoring target hit results")]
        [Range(0, MaxPrecision)]
        internal int precision = 0;

        internal const int MaxPrecision = 4;
        private const int RaycastPrecision = MaxPrecision + 1;

        [SerializeField]
        private AnimationCurve precisionCurve;

        [SerializeField]
        [Tooltip("Determines whether a raycast following the eye gaze is performed to improve scoring results")]
        internal bool performAdditionalRaycast = true;

        private static readonly ProfilerMarker IsHitValidPerfMarker =
            new ProfilerMarker("[MRTK] FuzzyGazeInteractor.IsHitValid");

        private bool IsHitValid(IXRInteractable target, RaycastHit hit)
        {
            using (IsHitValidPerfMarker.Auto())
            {
                // Immediately reject our hit if we can't hover it.
                // This lets the ray "pass through" any objects that reject gaze hovers.
                if (target is IXRHoverInteractable hoverInteractable && !hoverInteractable.IsHoverableBy(this))
                {
                    return false;
                }

                Vector3 directionToHit = hit.point - transform.position;
                float angleToHit = Vector3.Angle(transform.forward.normalized, directionToHit);

                float distanceToCollider = directionToHit.magnitude;

                return IsHitValid(angleToHit, distanceToCollider);
            }
        }

        internal bool IsHitValid(float angle, float distance) => angle < coneAngle && (minGazeDistance < distance && distance < maxGazeDistance);

        private static readonly ProfilerMarker ScoreHitPerfMarker =
            new ProfilerMarker("[MRTK] FuzzyGazeInteractor.ScoreHit");

        /// <summary>
        /// Scores the interactable and its associated raycast hit according to our several criteria, such as distance, raycast hit angle, and target center angle.
        /// </summary>
        /// <param name="hit">The raycast hit which indicates where the target was hit.</param>
        private float ScoreHit(RaycastHit hit)
        {
            using (ScoreHitPerfMarker.Auto())
            {
                Vector3 origin = transform.position;
                Vector3 direction = transform.forward;

                Vector3 hitPoint = hit.point;
                Vector3 directionToHit = hitPoint - origin;
                float angleToHit = Vector3.Angle(direction, directionToHit);
                Vector3 hitDistance = hit.collider.transform.position - hitPoint;
                Vector3 directionToCenter = hit.collider.transform.position - origin;
                float angleToCenter = Vector3.Angle(direction, directionToCenter);

                // Additional work to see if there is a better point slightly further ahead on the direction line. This is only allowed if the collider isn't a mesh collider.
                if (hit.collider.GetType() != typeof(MeshCollider))
                {
                    Vector3 pointFurtherAlongGazePath = (sphereCastRadius * 0.5f * direction.normalized) + FindNearestPointOnLine(origin, direction, hitPoint);
                    Vector3 closestPointToPointFurtherAlongGazePath = hit.collider.ClosestPoint(pointFurtherAlongGazePath);
                    Vector3 directionToSecondaryPoint = closestPointToPointFurtherAlongGazePath - origin;
                    float angleToSecondaryPoint = Vector3.Angle(direction, directionToSecondaryPoint);

                    if (angleToSecondaryPoint < angleToHit)
                    {
                        hitPoint = closestPointToPointFurtherAlongGazePath;
                        directionToHit = directionToSecondaryPoint;
                        angleToHit = angleToSecondaryPoint;
                        hitDistance = hit.collider.transform.position - hitPoint;
                    }
                }

                float distanceScore = distanceWeight == 0 ? 0.0f : (distanceWeight * directionToHit.magnitude);
                float angleScore = angleWeight == 0 ? 0.0f : (angleWeight * angleToHit);
                float centerScore = distanceToCenterWeight == 0 ? 0.0f : (distanceToCenterWeight * hitDistance.magnitude);
                float centerAngleScore = angleToCenterWeight == 0 ? 0.0f : (angleToCenterWeight * angleToCenter);
                float finalScore = distanceScore + angleScore + centerScore + centerAngleScore;
                return finalScore;
            }
        }

        private static readonly ProfilerMarker ConecastScoreComparePerfMarker =
            new ProfilerMarker("[MRTK] FuzzyGazeInteractor.ConecastScoreCompare");

        /// <summary>
        /// Compares raycast hits by distance in ascending order.
        /// </summary>
        /// <param name="a">The first interactable to compare.</param>
        /// <param name="b">The second interactable to compare.</param>
        /// <returns>
        /// Returns -1 if a is a higher priority target than b, otherwise return 1.
        /// </returns>
        private static int ConecastScoreCompare(GazeRaycastHitResult a, GazeRaycastHitResult b)
        {
            using (ConecastScoreComparePerfMarker.Auto())
            {
                IXRInteractable interactableA = a.targetInteractable;
                IXRInteractable interactableB = b.targetInteractable;

                float aScore = InteractableScoreMap[interactableA];
                float bScore = InteractableScoreMap[interactableB];

                if (aScore < bScore)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        private static readonly ProfilerMarker GetValidTargetsPerfMarker =
            new ProfilerMarker("[MRTK] FuzzyGazeInteractor.GetValidTargets");

        #region Gaze raycast hit tracking

        // TODO: remove or refactor this section out when the XRRayInteractor exposes the raycast hits
        // Otherwise, it's mimicking the logic found in XRRayInteractor

        /// <summary>
        /// A structure representing a raycast hit result that originated
        /// from a <see cref="Microsoft.MixedReality.Toolkit.Input.FuzzyGazeInteractor">FuzzyGazeInteractor</see> object.
        /// </summary>
        public struct GazeRaycastHitResult
        {
            /// <summary>
            /// The raycast hit for fuzzy gaze interactor.
            /// </summary>
            public RaycastHit raycastHit;

            /// <summary>
            /// The interactable object that was hit by the gaze's raycast.
            /// </summary>
            public IXRInteractable targetInteractable;

            /// <summary>
            /// The precision level of the fuzzy gaze's raycast.
            /// </summary>
            public int precisionLevel;

            /// <summary>
            /// Helper function for determining whether this hit result was from a raycast.
            /// </summary>
            public bool IsRaycast => precisionLevel == RaycastPrecision;
        }

        private const int MaxRaycastHits = 10;
        // Allocate space for precision levels 0 -> MaxPrecision, and then reserve 1 more row for a potential dedicated raycast that follows the eye gaze.
        int[] raycastHitCounts = new int[MaxPrecision + 2];
        protected RaycastHit[][] allRaycastHits = new RaycastHit[MaxPrecision + 2][];

        protected List<GazeRaycastHitResult> baseTargetsRaycastHitResults = new List<GazeRaycastHitResult>();

        private GazeRaycastHitResult preciseHitResult;

        /// <summary>
        /// Returns the best case estimate for the hit location for fuzzy gaze.
        /// </summary>
        public GazeRaycastHitResult PreciseHitResult => preciseHitResult;

        /// <summary>
        /// A workaround function for getting where the spherecast hit on the target.
        /// This mimics the code in the XRRayInteractor's UpdateRaycastHits() function.
        /// </summary>
        private void UpdateRaycastHits(int targetPrecision, float castRadius)
        {
            // Perform the raycast
            Transform effectiveRayOrigin = rayOriginTransform != null ? rayOriginTransform : transform;

            // initialize the jagged array entry if it is null
            if (allRaycastHits[targetPrecision] == null)
            {
                allRaycastHits[targetPrecision] = new RaycastHit[MaxRaycastHits];
            }

            if (castRadius > 0.0f)
            {
                raycastHitCounts[targetPrecision] = UnityEngine.Physics.SphereCastNonAlloc(effectiveRayOrigin.position, castRadius, effectiveRayOrigin.forward,
                           allRaycastHits[targetPrecision], maxRaycastDistance, raycastMask, raycastTriggerInteraction);
            }
            else
            {
                raycastHitCounts[targetPrecision] = UnityEngine.Physics.RaycastNonAlloc(effectiveRayOrigin.position, effectiveRayOrigin.forward,
                           allRaycastHits[targetPrecision], maxRaycastDistance, raycastMask, raycastTriggerInteraction);
            }
        }

        #endregion Gaze raycast hit tracking

        /// <summary>
        /// Updates the hit results at the chosen precision level gazeRaycastHitResults for all colliders with IXRInteractable.
        /// </summary>
        /// <param name="targetPrecision">The target precision level we are updating.</param>
        private void UpdateHitResults(int targetPrecision)
        {
            for (var i = 0; i < raycastHitCounts[targetPrecision]; i++)
            {
                var raycastHit = allRaycastHits[targetPrecision][i];

                // Gets the interactable associated with the collider
                // Skip this step if the collider in question is not an interactable
                if (!interactionManager.TryGetInteractableForCollider(raycastHit.collider, out var interactable))
                {
                    continue;
                }

                GazeRaycastHitResult gazeRaycastHitResult = new GazeRaycastHitResult()
                {
                    raycastHit = raycastHit,
                    targetInteractable = interactable,
                    precisionLevel = targetPrecision
                };
                baseTargetsRaycastHitResults.Add(gazeRaycastHitResult);
            }
        }

        /// <summary>
        /// Reusable mapping of Interactables to their score from <see cref="ScoreHit"/> (used for sort).
        /// </summary>
        private static readonly Dictionary<IXRInteractable, float> InteractableScoreMap = new Dictionary<IXRInteractable, float>();

        /// <summary>
        /// Reusable mapping of Interactables to their "best" raycast hit. The best hit is the hit from the highest precision level.
        /// </summary>
        private static readonly Dictionary<IXRInteractable, GazeRaycastHitResult> InteractableRaycastHitMap = new Dictionary<IXRInteractable, GazeRaycastHitResult>();

        /// <summary>
        /// Used to avoid GC alloc that would happen if passing <see cref="ConecastScoreCompare"/> directly
        /// as an argument to <see cref="List{T}.Sort(Comparison{T})"/>.
        /// </summary>
        private static readonly Comparison<GazeRaycastHitResult> InteractableScoreComparison = ConecastScoreCompare;

        private static readonly ProfilerMarker SortPerfMarker =
            new ProfilerMarker("[MRTK] FuzzyGazeInteractor.Sort");

        /// <summary>
        /// Refreshes <see cref="InteractableScoreMap"/> with the scores for all interactables currently in <paramref name="hitResults"/> according to <paramref name="fuzzyGazeInteractor"/>.
        /// Then, uses <see cref="ConecastScoreCompare"/> with these refreshed scores to sort <paramref name="hitResults"/>.
        /// </summary>
        private static void Sort(FuzzyGazeInteractor fuzzyGazeInteractor, List<GazeRaycastHitResult> hitResults)
        {
            using (SortPerfMarker.Auto())
            {
                InteractableScoreMap.Clear();
                InteractableRaycastHitMap.Clear();
                foreach (GazeRaycastHitResult result in hitResults)
                {
                    IXRInteractable interactable = result.targetInteractable;
                    RaycastHit raycastHit = result.raycastHit;

                    float score = fuzzyGazeInteractor.ScoreHit(raycastHit);
                    if (!InteractableScoreMap.ContainsKey(interactable) || InteractableScoreMap[interactable] > score)
                    {
                        InteractableScoreMap[interactable] = score;
                    }

                    // Update the RaycastHit mapping where applicable
                    if (!InteractableRaycastHitMap.ContainsKey(interactable) || InteractableRaycastHitMap[interactable].precisionLevel < result.precisionLevel)
                    {
                        InteractableRaycastHitMap[interactable] = result;
                    }
                }

                hitResults.Sort(InteractableScoreComparison);
            }
        }

        /// <summary>
        /// Used to project the point onto a line moving in the specified direction which passes through the specified origin.
        /// </summary>
        private static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
        {
            direction.Normalize();
            Vector3 lhs = point - origin;

            float dotP = Vector3.Dot(lhs, direction);
            return origin + direction * dotP;
        }

        #endregion FuzzyGazeInteractor

        #region XRBaseInteractor

        /// <inheritdoc />
        /// <remarks>
        /// This differs from the underlying XRRayInteractor implementation because the best valid target is
        /// not always the closest one with the fuzzy gaze scenario. This makes the XRRayInteractor's optimization of
        /// invalidating targets which are further from the user than another UI or invalid target (for example, an object
        /// partially covered by a wall or UI element) not applicable for this scenario. Thus this implementation
        /// doesn't call base.GetValidTargets at any point.
        /// </remarks>
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            using (GetValidTargetsPerfMarker.Auto())
            {
                // Populate targets with the first valid interactable for fuzzy gaze targeting
                targets.Clear();
                foreach (GazeRaycastHitResult gazeRaycastHitResult in baseTargetsRaycastHitResults)
                {
                    IXRInteractable target = gazeRaycastHitResult.targetInteractable;
                    RaycastHit raycastHit = gazeRaycastHitResult.raycastHit;
                    if (IsHitValid(target, raycastHit))
                    {
                        // The precise hit result does not always correspond to a raycast from the gaze origin
                        // along the gaze vector, since an object which is not hit by this raycast may
                        // end up being the better fuzzy gaze target.
                        preciseHitResult = InteractableRaycastHitMap[target];
                        targets.Add(target);
                        // only add the first valid target to the list of valid targets
                        return;
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            // Gets the raycast hits associated with the passed in sets of base targets
            //
            // This is a bit of an unfortunate workaround, since the XRRayInteractor's internal tracker of the raycast hit data is
            // private. As a result, we end up doing twice the number of raycast calls for the Gaze Interactor, making this a
            // potential avenue for performance improvement in the future.
            // In addition, the raycast hit results will be slightly different between the base class and the fuzzy gaze interactor.
            // This should not be noticeable when running the application, but it's another reason why we need the raycast hit
            // information from the underlying XRRayInteractor.
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                for (int targetPrecision = 0; targetPrecision <= precision; targetPrecision++)
                {
                    float castRadius = precisionCurve.Evaluate((float)targetPrecision / MaxPrecision) * sphereCastRadius;

                    UpdateRaycastHits(targetPrecision, castRadius);
                }

                if (performAdditionalRaycast)
                {
                    UpdateRaycastHits(RaycastPrecision, 0.0f);
                }

                // Now associate these targets with their corresponding raycast hit where applicable
                // The raycast hits are generated from the Fuzzy Gaze Interactor's own cache of raycast hits
                // In the future, this will derive from the XRRayInteractor
                baseTargetsRaycastHitResults.Clear();

                for (int targetPrecision = 0; targetPrecision <= precision; targetPrecision++)
                {
                    UpdateHitResults(targetPrecision);
                }

                if (performAdditionalRaycast)
                {
                    UpdateHitResults(RaycastPrecision);
                }

                Sort(this, baseTargetsRaycastHitResults);
            }

            base.PreprocessInteractor(updatePhase);
        }

        #endregion XRBaseInteractor

        #region MonoBehaviour

#if UNITY_EDITOR
        private const float RaycastGizmoScale = 0.07f;

        /// <summary>
        /// When in editor, draws an approximation of what is the "Near Object" area
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!isHoverActive)
                return;

            Vector3 origin = transform.position;
            Vector3 centralAxis = transform.forward.normalized;
            Vector3 upAxis = transform.up.normalized;

            float GizmoAngle = coneAngle * 0.5f;
            float GizmoAngleRad = GizmoAngle * Mathf.Deg2Rad;

            float intersectionDist = Mathf.Clamp(sphereCastRadius / Mathf.Tan(GizmoAngleRad), 0, maxGazeDistance);
            float sideDist = intersectionDist / Mathf.Cos(GizmoAngleRad);
            float peripheralDist = Mathf.Max(sideDist * (intersectionDist - minGazeDistance) / intersectionDist, 0);

            Vector3 coneStart = origin + minGazeDistance * centralAxis;
            Vector3 coneDropoffPoint = origin + Mathf.Min(Mathf.Max(intersectionDist, minGazeDistance), maxGazeDistance) * centralAxis;
            Vector3 coneEnd = origin + maxGazeDistance * centralAxis;

            // Draw something approximating the GazeInteractor's field of view

            // Draw the field of view constrained by the spherecast physics calls
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(coneStart, coneEnd);
            Gizmos.DrawLine(coneDropoffPoint + Vector3.left * sphereCastRadius, coneEnd + Vector3.left * sphereCastRadius);
            Gizmos.DrawLine(coneDropoffPoint + Vector3.right * sphereCastRadius, coneEnd + Vector3.right * sphereCastRadius);

            // Draw the field of view that is restricted due to the cone angle
            Quaternion leftRayRotation = Quaternion.AngleAxis(-GizmoAngle, upAxis);
            Quaternion rightRayRotation = Quaternion.AngleAxis(GizmoAngle, upAxis);

            Vector3 leftRayDirection = leftRayRotation * centralAxis;
            Vector3 rightRayDirection = rightRayRotation * centralAxis;

            Gizmos.DrawRay(transform.position + leftRayDirection * minGazeDistance / Mathf.Cos(GizmoAngleRad), leftRayDirection * peripheralDist);
            Gizmos.DrawRay(transform.position + rightRayDirection * minGazeDistance / Mathf.Cos(GizmoAngleRad), rightRayDirection * peripheralDist);

            // Draw the wire disc representing the start of the field of view, minGazeDistance away from the gaze origin
            UnityEditor.Handles.color = hasHover ? Color.red : Color.cyan;
            UnityEditor.Handles.DrawWireDisc(coneStart,
                                             centralAxis,
                                             Mathf.Min(minGazeDistance * Mathf.Tan(GizmoAngleRad), sphereCastRadius));

            // Draw the wire disc representing the very edge of the field of view, including discs showing based on the selected precision level
            for (int targetPrecision = 0; targetPrecision <= precision; targetPrecision++)
            {
                float castRadius = precisionCurve.Evaluate((float)targetPrecision / MaxPrecision) * sphereCastRadius;
                if (castRadius > 0)
                {
                    UnityEditor.Handles.DrawWireDisc(coneEnd,
                                                     centralAxis,
                                                     castRadius);
                }
                else
                {
                    UnityEditor.Handles.DrawSolidDisc(coneEnd, centralAxis, sphereCastRadius * RaycastGizmoScale);
                }
            }

            if (performAdditionalRaycast)
            {
                UnityEditor.Handles.DrawSolidDisc(coneEnd, centralAxis, sphereCastRadius * RaycastGizmoScale);
            }
        }
#endif

        #endregion MonoBehaviour
    }
}
