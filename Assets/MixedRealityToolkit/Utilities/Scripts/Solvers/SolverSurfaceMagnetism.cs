// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Utilities.Solvers
{
    /// <summary>
    ///   SurfaceMagnetism casts rays to Surfaces in the world align the object to the surface.
    /// </summary>
    public class SolverSurfaceMagnetism : Solver
    {
        #region public enums
        public enum RaycastDirectionEnum
        {
            CameraFacing,
            ToObject,
            ToLinkedPosition
        }
        public enum RaycastModeEnum
        {
            Simple,
            Box,
            Sphere
        }

        public enum OrientModeEnum
        {
            None,
            Vertical,
            Full,
            Blended
        }
        #endregion

        #region public members
        [Tooltip("LayerMask to apply Surface Magnetism to")]
        public LayerMask MagneticSurface = 0;

        [Tooltip("Max distance to check for surfaces")]
        public float MaxDistance = 3.0f;
        [Tooltip("Closest distance to bring object")]
        public float CloseDistance = 0.5f;

        [Tooltip("Offset from surface along surface normal")]
        public float SurfaceNormalOffset = 0.5f;
        [Tooltip("Offset from surface along ray cast direction")]
        public float SurfaceRayOffset = 0;

        [Tooltip("Surface raycast mode.  Simple = single raycast, Complex = bbox corners")]
        public RaycastModeEnum raycastMode = RaycastModeEnum.Simple;

        [Tooltip("Number of rays per edge, should be odd. Total casts is n^2")]
        public int BoxRaysPerEdge = 3;

        [Tooltip("If true, use orthographic casting for box lines instead of perspective")]
        public bool OrthoBoxCast = false;

        [Tooltip("Align to ray cast direction if box cast hits many normals facing in varying directions")]
        public float MaximumNormalVariance = 0.5f;

        [Tooltip("Radius to use for sphere cast")]
        public float SphereSize = 1.0f;

        [Tooltip("When doing volume casts, use size override if non-zero instead of object's current scale")]
        public float VolumeCastSizeOverride = 0;

        [Tooltip("When doing volume casts, use linked AltScale instead of object's current scale")]
        public bool UseLinkedAltScaleOverride = false;

        // This is broken
        [Tooltip("Instead of using mesh normal, extract normal from tex coord (SR is reported to put smoothed normals in there)")]
        bool UseTexCoordNormals = false;

        [Tooltip("Raycast direction.  Can cast from head in facing dir, or cast from head to object position")]
        public RaycastDirectionEnum raycastDirection = RaycastDirectionEnum.ToLinkedPosition;

        [Tooltip("Orientation mode.  None = no orienting, Vertical = Face head, but always oriented up/down, Full = Aligned to surface normal completely")]
        public OrientModeEnum orientationMode = OrientModeEnum.Vertical;

        [Tooltip("Orientation Blend Value 0.0 = All head 1.0 = All surface")]
        public float OrientBlend = 0.65f;

        [HideInInspector]
        public bool OnSurface;
        #endregion

        #region private members
        private BoxCollider m_BoxCollider;
        private const float maxDot = 0.97f;
        #endregion

        protected override void Start()
        {
            base.Start();

            if (raycastMode == RaycastModeEnum.Box)
            {
                m_BoxCollider = GetComponent<BoxCollider>();
                if (m_BoxCollider == null)
                {
                    Debug.LogError("Box raycast mode requires a BoxCollider, but none was found!  Defaulting to Simple raycast mode");
                    raycastMode = RaycastModeEnum.Simple;
                }

                if (Application.isEditor)
                {
                    RaycastHelper.DebugEnabled = true;
                }
            }

            if (Application.isEditor && UseTexCoordNormals)
            {
                Debug.LogWarning("Disabling tex coord normals while in editor mode");
                UseTexCoordNormals = false;
            }
        }

        /// <summary>
        ///   Wraps the raycast call in one spot.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="result"></param>
        /// <returns>bool, true if a surface was hit</returns>
        private static bool DefaultRaycast(Vector3 origin, Vector3 direction, float distance, LayerMask surface, out RaycastResultHelper result)
        {
            return RaycastHelper.First(origin, direction, distance, surface, out result);
        }

        private static bool DefaultSpherecast(Vector3 origin, Vector3 direction, float radius, float distance, LayerMask surface, out RaycastResultHelper result)
        {
            return RaycastHelper.SphereFirst(origin, direction, radius, distance, surface, out result);
        }

        /// <summary>
        ///   Where should rays originate from?
        /// </summary>
        /// <returns>Vector3</returns>
        Vector3 GetRaycastOrigin()
        {
            if (solverHandler.TransformTarget == null)
            {
                return Vector3.zero;
            }
            return solverHandler.TransformTarget.position;
        }

        /// <summary>
        ///   Which point should the ray cast toward?  Not really the 'end' of the ray.  The ray may be cast along
        ///   the head facing direction, from the eye to the object, or to the solver's linked position (working from
        ///   the previous solvers)
        /// </summary>
        /// <returns>Vector3, a point on the ray besides the origin</returns>
        Vector3 GetRaycastEndPoint()
        {
            Vector3 ret = Vector3.forward;
            switch (raycastDirection)
            {
                case RaycastDirectionEnum.CameraFacing:
                    ret = solverHandler.TransformTarget.position + solverHandler.TransformTarget.forward;
                    break;

                case RaycastDirectionEnum.ToObject:
                    ret = transform.position;
                    break;

                case RaycastDirectionEnum.ToLinkedPosition:
                    ret = solverHandler.GoalPosition;
                    break;
            }
            return ret;
        }

        /// <summary>
        ///   Calculate the raycast direction based on the two ray points
        /// </summary>
        /// <returns>Vector3, the direction of the raycast</returns>
        Vector3 GetRaycastDirection()
        {
            Vector3 ret = Vector3.forward;
            if (raycastDirection == RaycastDirectionEnum.CameraFacing)
            {

                if (solverHandler.TransformTarget)
                {
                    ret = solverHandler.TransformTarget.forward;
                }
            }
            else
            {
                ret = (GetRaycastEndPoint() - GetRaycastOrigin()).normalized;
            }
            return ret;
        }

        /// <summary>
        ///   Calculates how the object should orient to the surface.  May be none to pass shared orientation through,
        ///   oriented to the surface but fully vertical, fully oriented to the surface normal, or a slerped blend
        ///   of the vertial orientation and the pass-through rotation.
        /// </summary>
        /// <param name="rayDir"></param>
        /// <param name="surfaceNormal"></param>
        /// <returns>Quaternion, the orientation to use for the object</returns>
        Quaternion CalculateMagnetismOrientation(Vector3 rayDir, Vector3 surfaceNormal)
        {
            // Calculate the surface rotation
            Vector3 newDir = -surfaceNormal;
            if (IsNormalVertical(newDir))
            {
                newDir = rayDir;
            }

            newDir.y = 0;

            Quaternion surfaceRot = Quaternion.LookRotation(newDir, Vector3.up);

            switch (orientationMode)
            {
                case OrientModeEnum.None:
                    return solverHandler.GoalRotation;

                case OrientModeEnum.Vertical:
                    return surfaceRot;

                case OrientModeEnum.Full:
                    return Quaternion.LookRotation(-surfaceNormal, Vector3.up);

                case OrientModeEnum.Blended:
                    return Quaternion.Slerp(solverHandler.GoalRotation, surfaceRot, OrientBlend);
                default:
                    return Quaternion.identity;
            }
        }

        /// <summary>
        ///   Checks if a normal is nearly vertical
        /// </summary>
        /// <param name="normal"></param>
        /// <returns>bool</returns>
        bool IsNormalVertical(Vector3 normal)
        {
            return 1f - Mathf.Abs(normal.y) < 0.01f;
        }

        /// <summary>
        ///   A constant scale override may be specified for volumetric raycasts, oherwise uses the current value of the solver link's alt scale
        /// </summary>
        /// <returns>float</returns>
        float GetScaleOverride()
        {
            if (UseLinkedAltScaleOverride)
            {
                return solverHandler.AltScale.Current.magnitude;
            }
            return VolumeCastSizeOverride;
        }

        public override void SolverUpdate()
        {
            // Pass-through by default
            this.GoalPosition = WorkingPos;
            this.GoalRotation = WorkingRot;

            // Determine raycast params
            Ray ray = new Ray(GetRaycastOrigin(), GetRaycastDirection());

            // Skip if there's no valid direction
            if (ray.direction == Vector3.zero)
            {
                return;
            }

            float ScaleOverride = GetScaleOverride();
            float len;
            bool bHit;
            RaycastResultHelper result;
            Vector3 hitDelta;

            switch (raycastMode)
            {
                case RaycastModeEnum.Simple:
                default:

                    // Do the cast!
                    bHit = DefaultRaycast(ray.origin, ray.direction, MaxDistance, MagneticSurface, out result);

                    OnSurface = bHit;

                    if (UseTexCoordNormals)
                    {
                        result.OverrideNormalFromTextureCoord();
                    }

                    // Enforce CloseDistance
                    hitDelta = result.Point - ray.origin;
                    len = hitDelta.magnitude;
                    if (len < CloseDistance)
                    {
                        result.OverridePoint(ray.origin + ray.direction * CloseDistance);
                    }

                    // Apply results
                    if (bHit)
                    {
                        GoalPosition = result.Point + SurfaceNormalOffset * result.Normal + SurfaceRayOffset * ray.direction;
                        GoalRotation = CalculateMagnetismOrientation(ray.direction, result.Normal);
                    }
                    break;

                case RaycastModeEnum.Box:
                    
                    Vector3 scale = transform.lossyScale;
                    if (ScaleOverride > 0)
                    {
                        scale = scale.normalized * ScaleOverride;
                    }

                    Quaternion orientation = orientationMode == OrientModeEnum.None ? Quaternion.LookRotation(ray.direction, Vector3.up) : CalculateMagnetismOrientation(ray.direction, Vector3.up);
                    Matrix4x4 targetMatrix = Matrix4x4.TRS(Vector3.zero, orientation, scale);

                    if (m_BoxCollider == null)
                    {
                        m_BoxCollider = this.GetComponent<BoxCollider>();
                    }

                    Vector3 extents = m_BoxCollider.size;

                    Vector3[] positions;
                    Vector3[] normals;
                    bool[] hits;

                    if (RaycastHelper.CastBoxExtents(extents, transform.position, targetMatrix, ray, MaxDistance, MagneticSurface, DefaultRaycast, BoxRaysPerEdge, OrthoBoxCast, out positions, out normals, out hits))
                    {
                        Plane plane;
                        float distance;

                        // place an unconstrained plane down the ray.  Never use vertical constrain.
                        FindPlacementPlane(ray.origin, ray.direction, positions, normals, hits, m_BoxCollider.size.x, MaximumNormalVariance, false, orientationMode == OrientModeEnum.None, out plane, out distance);

                        // If placing on a horzizontal surface, need to adjust the calculated distance by half the app height
                        float verticalCorrectionOffset = 0;
                        if (IsNormalVertical(plane.normal) && !Mathf.Approximately(ray.direction.y, 0))
                        {
                            float boxSurfaceOffsetVert = targetMatrix.MultiplyVector(new Vector3(0, extents.y / 2f, 0)).magnitude;
                            Vector3 correctionVec = boxSurfaceOffsetVert * (ray.direction / ray.direction.y);
                            verticalCorrectionOffset = -correctionVec.magnitude;
                        }

                        float boxSurfaceOffset = targetMatrix.MultiplyVector(new Vector3(0, 0, extents.z / 2f)).magnitude;

                        // Apply boxSurfaceOffset to rayDir and not surfaceNormalDir to reduce sliding
                        GoalPosition = ray.origin + ray.direction * Mathf.Max(CloseDistance, distance + SurfaceRayOffset + boxSurfaceOffset + verticalCorrectionOffset) + plane.normal * (0 * boxSurfaceOffset + SurfaceNormalOffset);
                        GoalRotation = CalculateMagnetismOrientation(ray.direction, plane.normal);
                        OnSurface = true;
                    }
                    else
                    {
                        OnSurface = false;
                    }
                    break;

                case RaycastModeEnum.Sphere:

                    // Do the cast!
                    float size = ScaleOverride > 0 ? ScaleOverride : transform.lossyScale.x * SphereSize;
                    bHit = DefaultSpherecast(ray.origin, ray.direction, size, MaxDistance, MagneticSurface, out result);
                    OnSurface = bHit;

                    // Enforce CloseDistance
                    hitDelta = result.Point - ray.origin;
                    len = hitDelta.magnitude;
                    if (len < CloseDistance)
                    {
                        result.OverridePoint(ray.origin + ray.direction * CloseDistance);
                    }

                    // Apply results
                    if (bHit)
                    {
                        GoalPosition = result.Point + SurfaceNormalOffset * result.Normal + SurfaceRayOffset * ray.direction;
                        GoalRotation = CalculateMagnetismOrientation(ray.direction, result.Normal);
                    }
                    break;
            }

            // Do frame to frame updates of transform, smoothly toward the goal, if desired
            UpdateWorkingPosToGoal();
            UpdateWorkingRotToGoal();
        }

        /// <summary>
        ///   Calculates a plane from all raycast hit locations upon which the object may align
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="positions"></param>
        /// <param name="normals"></param>
        /// <param name="hits"></param>
        /// <param name="assetWidth"></param>
        /// <param name="maxNormalVariance"></param>
        /// <param name="constrainVertical"></param>
        /// <param name="bUseClosestDistance"></param>
        /// <param name="plane"></param>
        /// <param name="closestDistance"></param>
        private static void FindPlacementPlane(Vector3 origin, Vector3 direction, Vector3[] positions, Vector3[] normals, bool[] hits, float assetWidth, float maxNormalVariance, bool constrainVertical, bool bUseClosestDistance, out Plane plane, out float closestDistance)
        {
            bool debugEnabled = RaycastHelper.DebugEnabled;

            int numRays = positions.Length;

            Vector3 originalDirection = direction;
            if (constrainVertical)
            {
                direction.y = 0.0f;
                direction = direction.normalized;
            }

            // go through all the points and find the closest distance
            int closestPoint = -1;
            closestDistance = float.PositiveInfinity;
            float farthestDistance = 0f;
            int numHits = 0;
            Vector3 averageNormal = Vector3.zero;

            for (int i = 0; i < numRays; i++)
            {
                if (hits[i] != false)
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
            if (variance > maxNormalVariance || numHits < numRays / 4)
            {
                plane = new Plane(-direction, positions[closestPoint]);
                return;
            }

            // go through all the points and find the most orthagonal plane
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
                if (constrainVertical)
                {
                    diff.y = 0.0f;
                    diff.Normalize();

                    if (diff == Vector3.zero)
                    {
                        continue;
                    }
                }
                else
                {
                    diff.Normalize();
                }

                float angle = Vector3.Dot(direction, diff);

                if (angle < lowAngle)
                {
                    lowAngle = angle;
                    lowIndex = i;
                }
            }

            if (!constrainVertical && lowIndex != -1)
            {
                for (int i = 0; i < numRays; i++)
                {
                    if (hits[i] == false || i == closestPoint || i == lowIndex)
                    {
                        continue;
                    }

                    float dot = Mathf.Abs(Vector3.Dot((positions[i] - positions[closestPoint]).normalized, (positions[lowIndex] - positions[closestPoint]).normalized));
                    if (dot > maxDot)
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
                    placementNormal = Vector3.Cross(positions[lowIndex] - positions[closestPoint], constrainVertical ? Vector3.up : planeUp).normalized;
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
            if (!bUseClosestDistance && closestPoint >= 0)
            {
                float centerPlaneDistance;
                Ray centerPlaneRay = new Ray(origin, originalDirection);
                if (plane.Raycast(centerPlaneRay, out centerPlaneDistance) || centerPlaneDistance != 0)
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
