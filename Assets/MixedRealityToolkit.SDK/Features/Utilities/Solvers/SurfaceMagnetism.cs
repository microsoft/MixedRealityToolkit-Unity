// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// SurfaceMagnetism casts rays to Surfaces in the world and aligns the object to the hit surface.
    /// </summary>
    public class SurfaceMagnetism : Solver
    {
        #region Enums

        /// <summary>
        /// Raycast direction mode for solver.
        /// CameraFacing = cast from head in facing direction
        /// ToObject = from head to object position
        /// ToLinkedPosition = from head to linked solver position
        /// </summary>
        public enum RaycastDirectionMode
        {
            CameraFacing = 0,
            ToObject,
            ToLinkedPosition
        }

        /// <summary>
        /// Orientation mode for solver
        /// None = no orienting
        /// Vertical = Face head, but always oriented up/down
        /// Full = Aligned to surface normal completely
        /// Blend = Blend between head & surface orientation
        /// </summary>
        public enum OrientationMode
        {
            None = 0,
            Vertical,
            Full,
            Blended
        }
        #endregion

        #region SurfaceMagnetism Parameters
        [SerializeField]
        [Tooltip("Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component")]
        private LayerMask[] magneticSurfaces = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component
        /// </summary>
        public LayerMask[] MagneticSurfaces
        {
            get { return magneticSurfaces; }
            set { magneticSurfaces = value; }
        }

        [SerializeField]
        [Tooltip("Max distance for raycast to check for surfaces")]
        private float maxDistance = 3.0f;

        /// <summary>
        /// Max distance for raycast to check for surfaces
        /// </summary>
        public float MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        [SerializeField]
        [Tooltip("Closest distance to bring object")]
        private float closeDistance = 0.5f;

        /// <summary>
        /// Closest distance to bring object
        /// </summary>
        public float CloseDistance
        {
            get { return closeDistance; }
            set { closeDistance = value; }
        }

        [SerializeField]
        [Tooltip("Offset from surface along surface normal")]
        private float surfaceNormalOffset = 0.5f;

        /// <summary>
        /// Offset from surface along surface normal
        /// </summary>
        public float SurfaceNormalOffset
        {
            get { return surfaceNormalOffset; }
            set { surfaceNormalOffset = value; }
        }

        [SerializeField]
        [Tooltip("Offset from surface along ray cast direction")]
        private float surfaceRayOffset = 0;

        /// <summary>
        /// Offset from surface along ray cast direction
        /// </summary>
        public float SurfaceRayOffset
        {
            get { return surfaceRayOffset; }
            set { surfaceRayOffset = value; }
        }

        [SerializeField]
        [Tooltip("Surface raycast mode for solver")]
        private SceneQueryType raycastMode = SceneQueryType.SimpleRaycast;

        /// <summary>
        /// Surface raycast mode for solver
        /// </summary>
        public SceneQueryType RaycastMode
        {
            get { return raycastMode; }
            set { raycastMode = value; }
        }

        #region Box Raycast Parameters

        [SerializeField]
        [Tooltip("Number of rays per edge, should be odd. Total casts is n^2")]
        private int boxRaysPerEdge = 3;

        /// <summary>
        /// Number of rays per edge, should be odd. Total casts is n^2
        /// </summary>
        public int BoxRaysPerEdge
        {
            get { return boxRaysPerEdge; }
            set { boxRaysPerEdge = value; }
        }

        [SerializeField]
        [Tooltip("If true, use orthographic casting for box lines instead of perspective")]
        private bool orthographicBoxCast = false;

        /// <summary>
        /// If true, use orthographic casting for box lines instead of perspective
        /// </summary>
        public bool OrthographicBoxCast
        {
            get { return orthographicBoxCast; }
            set { orthographicBoxCast = value; }
        }

        [SerializeField]
        [Tooltip("Align to ray cast direction if box cast hits many normals facing in varying directions")]
        private float maximumNormalVariance = 0.5f;

        /// <summary>
        /// Align to ray cast direction if box cast hits many normals facing in varying directions
        /// </summary>
        public float MaximumNormalVariance
        {
            get { return maximumNormalVariance; }
            set { maximumNormalVariance = value; }
        }

        #endregion

        #region Sphere Raycast Parameters

        [SerializeField]
        [Tooltip("Radius to use for sphere cast")]
        private float sphereSize = 1.0f;

        /// <summary>
        /// Radius to use for sphere cast
        /// </summary>
        public float SphereSize
        {
            get { return sphereSize; }
            set { sphereSize = value; }
        }

        #endregion

        [SerializeField]
        [Tooltip("When doing volume casts, use size override if non-zero instead of object's current scale")]
        private float volumeCastSizeOverride = 0;

        /// <summary>
        /// When doing volume casts, use size override if non-zero instead of object's current scale
        /// </summary>
        public float VolumeCastSizeOverride
        {
            get { return volumeCastSizeOverride; }
            set { volumeCastSizeOverride = value; }
        }

        [SerializeField]
        [Tooltip("When doing volume casts, use linked AltScale instead of object's current scale")]
        private bool useLinkedAltScaleOverride = false;

        /// <summary>
        /// When doing volume casts, use linked AltScale instead of object's current scale
        /// </summary>
        public bool UseLinkedAltScaleOverride
        {
            get { return useLinkedAltScaleOverride; }
            set { useLinkedAltScaleOverride = value; }
        }

        [SerializeField]
        [Tooltip("Raycast direction. Can cast from head in facing direction, or cast from head to object position")]
        private RaycastDirectionMode currentRaycastDirectionMode = RaycastDirectionMode.ToLinkedPosition;

        /// <summary>
        /// Raycast direction. Can cast from head in facing direction, or cast from head to object position
        /// </summary>
        public RaycastDirectionMode CurrentRaycastDirectionMode
        {
            get { return currentRaycastDirectionMode; }
            set { currentRaycastDirectionMode = value; }
        }

        [SerializeField]
        [Tooltip("Orientation mode. None = no orienting, Vertical = Face head, but always oriented up/down, Full = Aligned to surface normal completely")]
        private OrientationMode orientationMode = OrientationMode.Vertical;

        /// <summary>
        /// Orientation mode. None = no orienting, Vertical = Face head, but always oriented up/down, Full = Aligned to surface normal completely
        /// </summary>
        public OrientationMode CurrentOrientationMode
        {
            get { return orientationMode; }
            set { orientationMode = value; }
        }

        [SerializeField]
        [Tooltip("Orientation Blend Value where 0.0 = All head and 1.0 = All surface")]
        private float orientationBlend = 0.65f;

        /// <summary>
        /// Orientation Blend Value where 0.0 = All head and 1.0 = All surface
        /// </summary>
        public float OrientationBlend
        {
            get { return orientationBlend; }
            set { orientationBlend = value; }
        }

        [SerializeField]
        [Tooltip("If enabled, the debug lines will be drawn in the editor")]
        private bool debugEnabled = false;

        /// <summary>
        /// If enabled, the debug lines will be drawn in the editor
        /// </summary>
        public bool DebugEnabled
        {
            get { return debugEnabled; }
            set { debugEnabled = value; }
        }

        #endregion

        /// <summary>
        /// Whether or not the object is currently magnetized to a surface.
        /// </summary>
        public bool OnSurface { get; private set; }

        private const float MaxDot = 0.97f;
        private RayStep currentRayStep = new RayStep();
        private BoxCollider boxCollider;

        private Vector3 RaycastOrigin => SolverHandler.TransformTarget == null ? Vector3.zero : SolverHandler.TransformTarget.position;

        /// <summary>
        /// Which point should the ray cast toward? Not really the 'end' of the ray. The ray may be cast along
        /// the head facing direction, from the eye to the object, or to the solver's linked position (working from
        /// the previous solvers)
        /// </summary>
        private Vector3 RaycastEndPoint
        {
            get
            {
                Vector3 origin = RaycastOrigin;
                Vector3 endPoint = Vector3.forward;

                switch (CurrentRaycastDirectionMode)
                {
                    case RaycastDirectionMode.CameraFacing:
                        endPoint = SolverHandler.TransformTarget.position + SolverHandler.TransformTarget.forward;
                        break;

                    case RaycastDirectionMode.ToObject:
                        endPoint = transform.position;
                        break;

                    case RaycastDirectionMode.ToLinkedPosition:
                        endPoint = SolverHandler.GoalPosition;
                        break;
                }

                return endPoint;
            }
        }

        /// <summary>
        /// Calculate the raycast direction based on the two ray points
        /// </summary>
        private Vector3 RaycastDirection
        {
            get
            {
                Vector3 direction = Vector3.forward;

                if (CurrentRaycastDirectionMode == RaycastDirectionMode.CameraFacing)
                {
                    if (SolverHandler.TransformTarget != null)
                    {
                        direction = SolverHandler.TransformTarget.forward;
                    }
                }
                else
                {
                    direction = (RaycastEndPoint - RaycastOrigin).normalized;
                }

                return direction;
            }
        }

        /// <summary>
        /// A constant scale override may be specified for volumetric raycasts, otherwise uses the current value of the solver link's alt scale
        /// </summary>
        private float ScaleOverride => useLinkedAltScaleOverride ? SolverHandler.AltScale.Current.magnitude : volumeCastSizeOverride;

        /// <summary>
        /// Calculates how the object should orient to the surface.  May be none to pass shared orientation through,
        /// oriented to the surface but fully vertical, fully oriented to the surface normal, or a slerped blend
        /// of the vertical orientation and the pass-through rotation.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="surfaceNormal"></param>
        /// <returns>Quaternion, the orientation to use for the object</returns>
        private Quaternion CalculateMagnetismOrientation(Vector3 direction, Vector3 surfaceNormal)
        {
            // Calculate the surface rotation
            Vector3 newDirection = -surfaceNormal;

            if (IsNormalVertical(newDirection))
            {
                newDirection = direction;
            }

            newDirection.y = 0;

            var surfaceRot = Quaternion.LookRotation(newDirection, Vector3.up);

            switch (CurrentOrientationMode)
            {
                case OrientationMode.None:
                    return SolverHandler.GoalRotation;

                case OrientationMode.Vertical:
                    return surfaceRot;

                case OrientationMode.Full:
                    return Quaternion.LookRotation(-surfaceNormal, Vector3.up);

                case OrientationMode.Blended:
                    return Quaternion.Slerp(SolverHandler.GoalRotation, surfaceRot, orientationBlend);
                default:
                    return Quaternion.identity;
            }
        }

        public override void SolverUpdate()
        {
            // Pass-through by default
            GoalPosition = WorkingPosition;
            GoalRotation = WorkingRotation;

            // Determine raycast params. Update struct to skip instantiation
            Vector3 origin = RaycastOrigin;
            Vector3 endpoint = RaycastEndPoint;
            currentRayStep.UpdateRayStep(ref origin, ref endpoint);

            // Skip if there isn't a valid direction
            if (currentRayStep.Direction == Vector3.zero)
            {
                return;
            }

            switch (RaycastMode)
            {
                case SceneQueryType.SimpleRaycast:
                    SimpleRaycastStepUpdate(this.currentRayStep);
                    break;
                case SceneQueryType.BoxRaycast:
                    BoxRaycastStepUpdate(this.currentRayStep);
                    break;
                case SceneQueryType.SphereCast:
                    SphereRaycastStepUpdate(this.currentRayStep);
                    break;
            }

            // Do frame to frame updates of transform, smoothly toward the goal, if desired
            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        /// <summary>
        /// Calculate solver for simple raycast with provided ray
        /// </summary>
        /// <param name="rayStep">start/end ray passed by read-only reference to avoid struct-copy performance</param>
        private void SimpleRaycastStepUpdate(in RayStep rayStep)
        {
            bool isHit;
            RaycastHit result;

            // Do the cast!
            isHit = MixedRealityRaycaster.RaycastSimplePhysicsStep(rayStep, maxDistance, magneticSurfaces, out result);

            OnSurface = isHit;

            // Enforce CloseDistance
            Vector3 hitDelta = result.point - rayStep.Origin;
            float length = hitDelta.magnitude;

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
        }

        /// <summary>
        /// Calculate solver for sphere raycast with provided ray
        /// </summary>
        /// <param name="rayStep">start/end ray passed by read-only reference to avoid struct-copy performance</param>
        private void SphereRaycastStepUpdate(in RayStep rayStep)
        {
            bool isHit;
            RaycastHit result;

            // Do the cast!
            float size = ScaleOverride > 0 ? ScaleOverride : transform.lossyScale.x * sphereSize;
            isHit = MixedRealityRaycaster.RaycastSpherePhysicsStep(rayStep, size, maxDistance, magneticSurfaces, out result);

            OnSurface = isHit;

            // Enforce CloseDistance
            Vector3 hitDelta = result.point - rayStep.Origin;
            float length = hitDelta.magnitude;

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
        }

        /// <summary>
        /// Calculate solver for box raycast with provided ray
        /// </summary>
        /// <param name="rayStep">start/end ray passed by read-only reference to avoid struct-copy performance</param>
        private void BoxRaycastStepUpdate(in RayStep rayStep)
        {
            Vector3 scale = ScaleOverride > 0 ? transform.lossyScale.normalized * ScaleOverride : transform.lossyScale;

            Quaternion orientation = orientationMode == OrientationMode.None ?
                Quaternion.LookRotation(rayStep.Direction, Vector3.up) :
                CalculateMagnetismOrientation(rayStep.Direction, Vector3.up);

            Matrix4x4 targetMatrix = Matrix4x4.TRS(Vector3.zero, orientation, scale);

            if (this.boxCollider == null)
            {
                this.boxCollider = GetComponent<BoxCollider>();
            }

            Debug.Assert(boxCollider != null, $"Missing a box collider for Surface Magnetism on {gameObject}");

            Vector3 extents = boxCollider.size;

            Vector3[] positions;
            Vector3[] normals;
            bool[] hits;

            if (MixedRealityRaycaster.RaycastBoxPhysicsStep(rayStep, extents, transform.position, targetMatrix, maxDistance, magneticSurfaces, boxRaysPerEdge, orthographicBoxCast, out positions, out normals, out hits))
            {
                Plane plane;
                float distance;

                // Place an unconstrained plane down the ray. Don't use vertical constrain.
                FindPlacementPlane(rayStep.Origin, rayStep.Direction, positions, normals, hits, boxCollider.size.x, maximumNormalVariance, false, orientationMode == OrientationMode.None, out plane, out distance);

                // If placing on a horizontal surface, need to adjust the calculated distance by half the app height
                float verticalCorrectionOffset = 0;
                if (IsNormalVertical(plane.normal) && !Mathf.Approximately(rayStep.Direction.y, 0))
                {
                    float boxSurfaceVerticalOffset = targetMatrix.MultiplyVector(new Vector3(0, extents.y * 0.5f, 0)).magnitude;
                    Vector3 correctionVector = boxSurfaceVerticalOffset * (rayStep.Direction / rayStep.Direction.y);
                    verticalCorrectionOffset = -correctionVector.magnitude;
                }

                float boxSurfaceOffset = targetMatrix.MultiplyVector(new Vector3(0, 0, extents.z * 0.5f)).magnitude;

                // Apply boxSurfaceOffset to ray direction and not surface normal direction to reduce sliding
                GoalPosition = rayStep.Origin + rayStep.Direction * Mathf.Max(closeDistance, distance + surfaceRayOffset + boxSurfaceOffset + verticalCorrectionOffset) + plane.normal * (0 * boxSurfaceOffset + surfaceNormalOffset);
                GoalRotation = CalculateMagnetismOrientation(rayStep.Direction, plane.normal);
                OnSurface = true;
            }
            else
            {
                OnSurface = false;
            }
        }

        /// <summary>
        /// Calculates a plane from all raycast hit locations upon which the object may align. Used in Box Raycast Mode.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="positions"></param>
        /// <param name="normals"></param>
        /// <param name="hits"></param>
        /// <param name="assetWidth"></param>
        /// <param name="maxNormalVariance"></param>
        /// <param name="constrainVertical"></param>
        /// <param name="useClosestDistance"></param>
        /// <param name="plane"></param>
        /// <param name="closestDistance"></param>
        private void FindPlacementPlane(Vector3 origin, Vector3 direction, Vector3[] positions, Vector3[] normals, bool[] hits, float assetWidth, float maxNormalVariance, bool constrainVertical, bool useClosestDistance, out Plane plane, out float closestDistance)
        {
            int rayCount = positions.Length;

            Vector3 originalDirection = direction;

            if (constrainVertical)
            {
                direction.y = 0.0f;
                direction = direction.normalized;
            }

            // Go through all the points and find the closest distance
            closestDistance = float.PositiveInfinity;

            int numHits = 0;
            int closestPointIdx = -1;
            float farthestDistance = 0f;
            var averageNormal = Vector3.zero;

            for (int hitIndex = 0; hitIndex < rayCount; hitIndex++)
            {
                if (hits[hitIndex])
                {
                    float distance = Vector3.Dot(direction, positions[hitIndex] - origin);

                    if (distance < closestDistance)
                    {
                        closestPointIdx = hitIndex;
                        closestDistance = distance;
                    }

                    if (distance > farthestDistance)
                    {
                        farthestDistance = distance;
                    }

                    averageNormal += normals[hitIndex];
                    ++numHits;
                }
            }

            Vector3 closestPoint = positions[closestPointIdx];
            averageNormal /= numHits;

            // Calculate variance of all normals
            float variance = 0;

            for (int hitIndex = 0; hitIndex < rayCount; ++hitIndex)
            {
                if (hits[hitIndex])
                {
                    variance += (normals[hitIndex] - averageNormal).magnitude;
                }
            }

            variance /= numHits;

            // If variance is too high, I really don't want to deal with this surface
            // And if we don't even have enough rays, I'm not confident about this at all
            if (variance > maxNormalVariance || numHits < rayCount * 0.25f)
            {
                plane = new Plane(-direction, closestPoint);
                return;
            }

            // go through all the points and find the most orthogonal plane
            var lowAngle = float.PositiveInfinity;
            var highAngle = float.NegativeInfinity;
            int lowIndex = -1;
            int highIndex = -1;

            for (int hitIndex = 0; hitIndex < rayCount; hitIndex++)
            {
                if (hits[hitIndex] == false || hitIndex == closestPointIdx)
                {
                    continue;
                }

                Vector3 difference = positions[hitIndex] - closestPoint;

                if (constrainVertical)
                {
                    difference.y = 0.0f;
                    difference.Normalize();

                    if (difference == Vector3.zero)
                    {
                        continue;
                    }
                }

                difference.Normalize();

                float angle = Vector3.Dot(direction, difference);

                if (angle < lowAngle)
                {
                    lowAngle = angle;
                    lowIndex = hitIndex;
                }
            }

            if (!constrainVertical && lowIndex != -1)
            {
                for (int hitIndex = 0; hitIndex < rayCount; hitIndex++)
                {
                    if (hits[hitIndex] == false || hitIndex == closestPointIdx || hitIndex == lowIndex)
                    {
                        continue;
                    }

                    float dot = Mathf.Abs(Vector3.Dot((positions[hitIndex] - closestPoint).normalized, (positions[lowIndex] - closestPoint).normalized));

                    if (dot > MaxDot)
                    {
                        continue;
                    }

                    float nextAngle = Mathf.Abs(Vector3.Dot(direction, Vector3.Cross(positions[lowIndex] - closestPoint, positions[hitIndex] - closestPoint).normalized));

                    if (nextAngle > highAngle)
                    {
                        highAngle = nextAngle;
                        highIndex = hitIndex;
                    }
                }
            }

            Vector3 placementNormal;

            if (lowIndex != -1)
            {
                if (debugEnabled)
                {
                    Debug.DrawLine(closestPoint, positions[lowIndex], Color.red);
                }

                if (highIndex != -1)
                {
                    if (debugEnabled)
                    {
                        Debug.DrawLine(closestPoint, positions[highIndex], Color.green);
                    }

                    placementNormal = Vector3.Cross(positions[lowIndex] - closestPoint, positions[highIndex] - closestPoint).normalized;
                }
                else
                {
                    Vector3 planeUp = Vector3.Cross(positions[lowIndex] - closestPoint, direction);
                    placementNormal = Vector3.Cross(positions[lowIndex] - closestPoint, constrainVertical ? Vector3.up : planeUp).normalized;
                }

                if (debugEnabled)
                {
                    Debug.DrawLine(closestPoint, closestPoint + placementNormal, Color.blue);
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

            plane = new Plane(placementNormal, closestPoint);

            if (debugEnabled)
            {
                Debug.DrawRay(closestPoint, placementNormal, Color.cyan);
            }

            // Figure out how far the plane should be.
            if (!useClosestDistance && closestPointIdx >= 0)
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

        /// <summary>
        /// Checks if a normal is nearly vertical
        /// </summary>
        /// <param name="normal"></param>
        /// <returns>Returns true, if normal is vertical.</returns>
        private static bool IsNormalVertical(Vector3 normal) => 1f - Mathf.Abs(normal.y) < 0.01f;
    }
}
