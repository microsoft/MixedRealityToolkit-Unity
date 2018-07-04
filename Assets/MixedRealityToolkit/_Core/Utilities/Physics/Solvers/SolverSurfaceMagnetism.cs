// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Solvers
{
    /// <summary>
    /// SurfaceMagnetism casts rays to Surfaces in the world align the object to the surface.
    /// </summary>
    public class SolverSurfaceMagnetism : Solver
    {
        private const float MaxDot = 0.97f;

        private enum RaycastDirectionEnum
        {
            CameraFacing,
            ToObject,
            ToLinkedPosition
        }

        private enum OrientModeEnum
        {
            None,
            Vertical,
            Full,
            Blended
        }

        [SerializeField]
        [Tooltip("LayerMask to apply Surface Magnetism to")]
        private LayerMask[] magneticSurfaces = { UnityEngine.Physics.DefaultRaycastLayers };

        [SerializeField]
        [Tooltip("Max distance to check for surfaces")]
        private float maxDistance = 3.0f;

        [SerializeField]
        [Tooltip("Closest distance to bring object")]
        private float closeDistance = 0.5f;

        [SerializeField]
        [Tooltip("Offset from surface along surface normal")]
        private float surfaceNormalOffset = 0.5f;

        [SerializeField]
        [Tooltip("Offset from surface along ray cast direction")]
        private float surfaceRayOffset = 0;

        [SerializeField]
        [Tooltip("Surface raycast mode")]
        private RaycastModeType raycastMode = RaycastModeType.Simple;

        [SerializeField]
        [Tooltip("Number of rays per edge, should be odd. Total casts is n^2")]
        private int boxRaysPerEdge = 3;

        [SerializeField]
        [Tooltip("If true, use orthographic casting for box lines instead of perspective")]
        private bool orthographicBoxCast = false;

        [SerializeField]
        [Tooltip("Align to ray cast direction if box cast hits many normals facing in varying directions")]
        private float maximumNormalVariance = 0.5f;

        [SerializeField]
        [Tooltip("Radius to use for sphere cast")]
        private float sphereSize = 1.0f;

        [SerializeField]
        [Tooltip("When doing volume casts, use size override if non-zero instead of object's current scale")]
        private float volumeCastSizeOverride = 0;

        [SerializeField]
        [Tooltip("When doing volume casts, use linked AltScale instead of object's current scale")]
        private bool useLinkedAltScaleOverride = false;

        [SerializeField]
        [Tooltip("Raycast direction.  Can cast from head in facing dir, or cast from head to object position")]
        private RaycastDirectionEnum raycastDirection = RaycastDirectionEnum.ToLinkedPosition;

        [SerializeField]
        [Tooltip("Orientation mode.  None = no orienting, Vertical = Face head, but always oriented up/down, Full = Aligned to surface normal completely")]
        private OrientModeEnum orientationMode = OrientModeEnum.Vertical;

        [SerializeField]
        [Tooltip("Orientation Blend Value 0.0 = All head 1.0 = All surface")]
        private float orientBlend = 0.65f;

        [SerializeField]
        private static bool debugEnabled;

        private BoxCollider boxCollider;

        private void OnValidate()
        {
            if (raycastMode == RaycastModeType.Box)
            {
                boxCollider = gameObject.EnsureComponent<BoxCollider>();
            }
        }

        /// <summary>
        ///   Where should rays originate from?
        /// </summary>
        /// <returns>Vector3</returns>
        private Vector3 GetRaycastOrigin()
        {
            return SolverHandler.TransformTarget == null ? Vector3.zero : SolverHandler.TransformTarget.position;
        }

        /// <summary>
        /// Which point should the ray cast toward?  Not really the 'end' of the ray.  The ray may be cast along
        /// the head facing direction, from the eye to the object, or to the solver's linked position (working from
        /// the previous solvers)
        /// </summary>
        /// <returns>Vector3, a point on the ray besides the origin</returns>
        private Vector3 GetRaycastEndPoint()
        {
            Vector3 endPoint = Vector3.forward;
            switch (raycastDirection)
            {
                case RaycastDirectionEnum.CameraFacing:
                    endPoint = SolverHandler.TransformTarget.position + SolverHandler.TransformTarget.forward;
                    break;

                case RaycastDirectionEnum.ToObject:
                    endPoint = transform.position;
                    break;

                case RaycastDirectionEnum.ToLinkedPosition:
                    endPoint = SolverHandler.GoalPosition;
                    break;
            }

            return endPoint;
        }

        /// <summary>
        /// Calculate the raycast direction based on the two ray points
        /// </summary>
        /// <returns>Vector3, the direction of the raycast</returns>
        private Vector3 GetRaycastDirection()
        {
            Vector3 direction = Vector3.forward;
            if (raycastDirection == RaycastDirectionEnum.CameraFacing)
            {
                if (SolverHandler.TransformTarget != null)
                {
                    direction = SolverHandler.TransformTarget.forward;
                }
            }
            else
            {
                direction = (GetRaycastEndPoint() - GetRaycastOrigin()).normalized;
            }

            return direction;
        }

        /// <summary>
        /// Calculates how the object should orient to the surface.  May be none to pass shared orientation through,
        /// oriented to the surface but fully vertical, fully oriented to the surface normal, or a slerped blend
        /// of the vertical orientation and the pass-through rotation.
        /// </summary>
        /// <param name="rayDir"></param>
        /// <param name="surfaceNormal"></param>
        /// <returns>Quaternion, the orientation to use for the object</returns>
        private Quaternion CalculateMagnetismOrientation(Vector3 rayDir, Vector3 surfaceNormal)
        {
            // Calculate the surface rotation
            Vector3 newDir = -surfaceNormal;

            if (IsNormalVertical(newDir))
            {
                newDir = rayDir;
            }

            newDir.y = 0;

            var surfaceRot = Quaternion.LookRotation(newDir, Vector3.up);

            switch (orientationMode)
            {
                case OrientModeEnum.None:
                    return SolverHandler.GoalRotation;

                case OrientModeEnum.Vertical:
                    return surfaceRot;

                case OrientModeEnum.Full:
                    return Quaternion.LookRotation(-surfaceNormal, Vector3.up);

                case OrientModeEnum.Blended:
                    return Quaternion.Slerp(SolverHandler.GoalRotation, surfaceRot, orientBlend);

                default:
                    return Quaternion.identity;
            }
        }

        /// <summary>
        /// Checks if a normal is nearly vertical
        /// </summary>
        /// <param name="normal"></param>
        /// <returns>bool</returns>
        private static bool IsNormalVertical(Vector3 normal)
        {
            return 1f - Mathf.Abs(normal.y) < 0.01f;
        }

        /// <summary>
        /// A constant scale override may be specified for volumetric raycasts, otherwise uses the current value of the solver link's alt scale
        /// </summary>
        /// <returns>float</returns>
        private float GetScaleOverride()
        {
            return useLinkedAltScaleOverride ? SolverHandler.AltScale.magnitude : volumeCastSizeOverride;
        }

        public override void SolverUpdate()
        {
            // Pass-through by default
            GoalPosition = WorkingPosition;
            GoalRotation = WorkingRot;

            // Determine raycast params
            RayStep rayStep = new RayStep(GetRaycastOrigin(), GetRaycastEndPoint());

            // Skip if there's no valid direction
            if (rayStep.Direction == Vector3.zero)
            {
                return;
            }

            float scaleOverride = GetScaleOverride();
            float length;
            bool isHit;
            RaycastHit result;
            Vector3 hitDelta;

            switch (raycastMode)
            {
                case RaycastModeType.Simple:

                    // Do the cast!
                    isHit = MixedRealityRaycaster.RaycastSimplePhysicsStep(rayStep, magneticSurfaces, out result);

                    // Enforce CloseDistance
                    hitDelta = result.point - rayStep.Origin;
                    length = hitDelta.magnitude;

                    if (length < closeDistance)
                    {
                        result.point = rayStep.Origin + rayStep.Direction * closeDistance;
                    }

                    // Apply results
                    if (isHit)
                    {
                        GoalPosition = result.point + surfaceNormalOffset * result.normal + surfaceRayOffset * rayStep.Direction;
                        GoalRotation = CalculateMagnetismOrientation(rayStep.Direction, result.normal);
                    }
                    break;

                case RaycastModeType.Box:

                    Vector3 scale = transform.lossyScale;
                    if (scaleOverride > 0)
                    {
                        scale = scale.normalized * scaleOverride;
                    }

                    Quaternion orientation = orientationMode == OrientModeEnum.None ?
                        Quaternion.LookRotation(rayStep.Direction, Vector3.up) :
                        CalculateMagnetismOrientation(rayStep.Direction, Vector3.up);

                    Matrix4x4 targetMatrix = Matrix4x4.TRS(Vector3.zero, orientation, scale);

                    if (boxCollider == null)
                    {
                        boxCollider = GetComponent<BoxCollider>();
                    }

                    Vector3 extents = boxCollider.size;

                    Vector3[] positions;
                    Vector3[] normals;
                    bool[] hits;

                    if (MixedRealityRaycaster.RaycastBoxPhysicsStep(rayStep, extents, transform.position, targetMatrix, maxDistance, magneticSurfaces, boxRaysPerEdge, orthographicBoxCast, out positions, out normals, out hits))
                    {
                        Plane plane;
                        float distance;

                        // place an unconstrained plane down the ray.  Never use vertical constrain.
                        FindPlacementPlane(rayStep.Origin, rayStep.Direction, positions, normals, hits, boxCollider.size.x, maximumNormalVariance, orientationMode == OrientModeEnum.None, out plane, out distance);

                        // If placing on a horizontal surface, need to adjust the calculated distance by half the app height
                        float verticalCorrectionOffset = 0;
                        if (IsNormalVertical(plane.normal) && !Mathf.Approximately(rayStep.Direction.y, 0))
                        {
                            float boxSurfaceVerticalOffset = targetMatrix.MultiplyVector(new Vector3(0, extents.y * 0.5f, 0)).magnitude;
                            Vector3 correctionVec = boxSurfaceVerticalOffset * (rayStep.Direction / rayStep.Direction.y);
                            verticalCorrectionOffset = -correctionVec.magnitude;
                        }

                        float boxSurfaceOffset = targetMatrix.MultiplyVector(new Vector3(0, 0, extents.z * 0.5f)).magnitude;

                        // Apply boxSurfaceOffset to ray direction and not surface normal direction to reduce sliding
                        GoalPosition = rayStep.Origin + rayStep.Direction * Mathf.Max(closeDistance, distance + surfaceRayOffset + boxSurfaceOffset + verticalCorrectionOffset) + plane.normal * (0 * boxSurfaceOffset + surfaceNormalOffset);
                        GoalRotation = CalculateMagnetismOrientation(rayStep.Direction, plane.normal);
                    }
                    break;

                case RaycastModeType.Sphere:

                    // Do the cast!
                    float size = scaleOverride > 0 ? scaleOverride : transform.lossyScale.x * sphereSize;
                    isHit = MixedRealityRaycaster.RaycastSpherePhysicsStep(rayStep, size, magneticSurfaces, out result);

                    // Enforce CloseDistance
                    hitDelta = result.point - rayStep.Origin;
                    length = hitDelta.magnitude;

                    if (length < closeDistance)
                    {
                        result.point = rayStep.Origin + rayStep.Direction * closeDistance;
                    }

                    // Apply results
                    if (isHit)
                    {
                        GoalPosition = result.point + surfaceNormalOffset * result.normal + surfaceRayOffset * rayStep.Direction;
                        GoalRotation = CalculateMagnetismOrientation(rayStep.Direction, result.normal);
                    }
                    break;
            }

            // Do frame to frame updates of transform, smoothly toward the goal, if desired
            UpdateWorkingPosToGoal();
            UpdateWorkingRotToGoal();
        }

        /// <summary>
        /// Calculates a plane from all raycast hit locations upon which the object may align
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="positions"></param>
        /// <param name="normals"></param>
        /// <param name="hits"></param>
        /// <param name="assetWidth"></param>
        /// <param name="maxNormalVariance"></param>
        /// <param name="useClosestDistance"></param>
        /// <param name="plane"></param>
        /// <param name="closestDistance"></param>
        private static void FindPlacementPlane(Vector3 origin, Vector3 direction, Vector3[] positions, Vector3[] normals, bool[] hits, float assetWidth, float maxNormalVariance, bool useClosestDistance, out Plane plane, out float closestDistance)
        {
            int numRays = positions.Length;

            Vector3 originalDirection = direction;

            // go through all the points and find the closest distance
            int closestPoint = -1;
            closestDistance = float.PositiveInfinity;
            float farthestDistance = 0f;
            int numHits = 0;
            Vector3 averageNormal = Vector3.zero;

            for (int i = 0; i < numRays; i++)
            {
                if (hits[i])
                {
                    float dist = Vector3.Dot(direction, positions[i] - origin);

                    if (dist < closestDistance)
                    {
                        closestPoint = i;
                        closestDistance = dist;
                    }
                    if (dist > farthestDistance)
                    {
                        farthestDistance = dist;
                    }

                    averageNormal += normals[i];
                    ++numHits;
                }
            }
            averageNormal /= numHits;

            // Calculate variance of all normals
            float variance = 0;
            for (int i = 0; i < numRays; ++i)
            {
                if (hits[i] != false)
                {
                    variance += (normals[i] - averageNormal).magnitude;
                }
            }
            variance /= numHits;

            // If variance is too high, I really don't want to deal with this surface
            // And if we don't even have enough rays, I'm not confident about this at all
            if (variance > maxNormalVariance || numHits < numRays * 0.25f)
            {
                plane = new Plane(-direction, positions[closestPoint]);
                return;
            }

            // go through all the points and find the most orthogonal plane
            float lowAngle = float.PositiveInfinity;
            int lowIndex = -1;
            float highAngle = float.NegativeInfinity;
            int highIndex = -1;

            for (int i = 0; i < numRays; i++)
            {
                if (hits[i] == false || i == closestPoint)
                {
                    continue;
                }

                Vector3 diff = (positions[i] - positions[closestPoint]);
                diff.Normalize();

                float angle = Vector3.Dot(direction, diff);

                if (angle < lowAngle)
                {
                    lowAngle = angle;
                    lowIndex = i;
                }
            }

            if (lowIndex != -1)
            {
                for (int i = 0; i < numRays; i++)
                {
                    if (hits[i] == false || i == closestPoint || i == lowIndex)
                    {
                        continue;
                    }

                    float dot = Mathf.Abs(Vector3.Dot((positions[i] - positions[closestPoint]).normalized, (positions[lowIndex] - positions[closestPoint]).normalized));
                    if (dot > MaxDot)
                    {
                        continue;
                    }

                    Vector3 normal = Vector3.Cross(positions[lowIndex] - positions[closestPoint], positions[i] - positions[closestPoint]).normalized;

                    float nextAngle = Mathf.Abs(Vector3.Dot(direction, normal));

                    if (nextAngle > highAngle)
                    {
                        highAngle = nextAngle;
                        highIndex = i;
                    }
                }
            }

            Vector3 placementNormal;
            if (lowIndex != -1)
            {
                if (debugEnabled)
                {
                    Debug.DrawLine(positions[closestPoint], positions[lowIndex], Color.red);
                }

                if (highIndex != -1)
                {
                    if (debugEnabled)
                    {
                        Debug.DrawLine(positions[closestPoint], positions[highIndex], Color.green);
                    }
                    placementNormal = Vector3.Cross(positions[lowIndex] - positions[closestPoint], positions[highIndex] - positions[closestPoint]).normalized;
                }
                else
                {
                    Vector3 planeUp = Vector3.Cross(positions[lowIndex] - positions[closestPoint], direction);
                    placementNormal = Vector3.Cross(positions[lowIndex] - positions[closestPoint], planeUp).normalized;
                }

                if (debugEnabled)
                {
                    Debug.DrawLine(positions[closestPoint], positions[closestPoint] + placementNormal, Color.blue);
                }
            }
            else
            {
                placementNormal = direction * -1.0f;
            }

            if (Vector3.Dot(placementNormal, direction) > 0.0f)
            {
                placementNormal *= -1.0f;
            }

            plane = new Plane(placementNormal, positions[closestPoint]);

            if (debugEnabled)
            {
                Debug.DrawRay(positions[closestPoint], placementNormal, Color.cyan);
            }

            // Figure out how far the plane should be.
            if (!useClosestDistance && closestPoint >= 0.0f)
            {
                float centerPlaneDistance;
                if (plane.Raycast(new Ray(origin, originalDirection), out centerPlaneDistance) || !centerPlaneDistance.Equals(0.0f))
                {
                    // When the plane is nearly parallel to the user, we need to clamp the distance to where the raycasts hit.
                    closestDistance = Mathf.Clamp(centerPlaneDistance, closestDistance, farthestDistance + assetWidth * 0.5f);
                }
                else
                {
                    Debug.LogError("FindPlacementPlane: Not expected to have the center point not intersect the plane.");
                }
            }
        }
    }
}
