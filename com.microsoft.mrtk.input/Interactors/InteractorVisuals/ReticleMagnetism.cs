// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Magnetizes to the surface of nearby objects detected by an
    /// <see cref="Microsoft.MixedReality.Toolkit.Input.ProximityDetector"/>. Used
    /// to magnetize poke reticles onto nearby touchable surfaces. Optionally,
    /// an <see cref="Microsoft.MixedReality.Toolkit.Input.IVariableReticle"/> can be
    /// attached to show variable reticle visuals based on proximity.
    /// If an <see cref="Microsoft.MixedReality.Toolkit.IPokeInteractor"/> is found in
    /// the reticle's parents, the <see cref="IPokeInteractor.PokeRadius"/> will be taken
    /// into account when rendering the reticle.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Reticle Magnetism")]
    internal class ReticleMagnetism : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ProximityDetector used to detect nearby objects.")]
        private ProximityDetector detector;

        /// <summary>
        /// The <see cref="ProximityDetector"/> used to detect nearby objects.
        /// </summary>
        public ProximityDetector Detector
        {
            get => detector;
            set => detector = value;
        }

        [SerializeField]
        [Tooltip("Maximum magnetization distance. Must be smaller than the radius of the associated ProximityDetector.")]
        private float magnetRange = 0.07f;

        /// <summary>
        /// Maximum magnetization distance. Must be smaller than the radius
        /// of <see cref="ReticleMagnetism.Detector"/>.
        /// </summary>
        public float MagnetRange { get => magnetRange; set => magnetRange = value; }

        [SerializeField]
        [Tooltip("The reticle will stick to the surface normal of any BoxColliders with a depth larger than this thickness." +
                 "\nShallower BoxColliders will result in only the +Z of the collider being used for reticle orientation.")]
        private float colliderThicknessCutoff = 0.04f;

        [SerializeField]
        [Tooltip("Curve controlling the effectiveness of the position smoothing. Should be zero when the input is near zero.")]
        private AnimationCurve positionSmoothingCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Curve controlling the effectiveness of the position smoothing.
        /// Should be zero when the input is near zero.
        /// </summary>
        public AnimationCurve PositionSmoothingCurve { get => positionSmoothingCurve; set => positionSmoothingCurve = value; }

        [SerializeField]
        [Tooltip("Curve that drives the magnetism of the position of the reticle.")]
        private AnimationCurve positionMagnetismCurve;

        /// <summary>Curve that drives the magnetism of the position of the reticle.</summary>
        public AnimationCurve PositionMagnetismCurve { get => positionMagnetismCurve; set => positionMagnetismCurve = value; }

        [SerializeField]
        [Tooltip("Curve that drives the magnetism of the rotation of the reticle.")]
        private AnimationCurve rotationMagnetismCurve;

        /// <summary>Curve that drives the magnetism of the rotation of the reticle.</summary>
        public AnimationCurve RotationMagnetismCurve { get => rotationMagnetismCurve; set => rotationMagnetismCurve = value; }

        [SerializeField]
        [Tooltip("Curve that drives the variable progress/animation of the reticle, if one is present.")]
        private AnimationCurve variableReticleCurve;

        /// <summary>Curve that drives the variable progress/animation of the reticle, if one present.</summary>
        public AnimationCurve VariableReticleCurve { get => rotationMagnetismCurve; set => rotationMagnetismCurve = value; }

        [SerializeField]
        [Tooltip("The amount of smoothing to apply to the reticle's position.")]
        private float positionSmoothing = 0.1f;

        /// <summary>The amount of smoothing to apply to the reticle's position.</summary>
        public float PositionSmoothing { get => positionSmoothing; set => positionSmoothing = value; }

        [SerializeField]
        [Tooltip("The amount of smoothing to apply to the reticle's rotation.")]
        private float rotationSmoothing = 0.1f;

        /// <summary>The amount of smoothing to apply to the reticle's rotation.</summary>
        public float RotationSmoothing { get => rotationSmoothing; set => rotationSmoothing = value; }

        // Reference to the variable visuals.
        private IVariableReticle variableReticleVisuals;

        // The smoothed magnetization point, usually the nearest point on the nearest collider.
        // Not necessarily the same as the current reticle rotation!
        // The reticle position is lerped to this based on the distance from the surface.
        private Vector3 smoothedMagnetPosition;

        // The smoothed magnetization rotation, usually the surface normal of the nearest point
        // on the nearest collider. Not necessarily the same as the current reticle rotation!
        // The reticle rotation is lerped to this based on the distance from the surface.
        private Quaternion smoothedMagnetRotation;

        // The lerp factor for the reticle's position (lerping between the finger position
        // and the smoothedMagnetPosition)
        private float positionFraction;

        // The lerp factor for the reticle's rotation (lerping between the finger position
        // and the smoothedMagnetPosition)
        private float rotationFraction;

        // The lerp factor for the progression of the variable reticle.
        private float progressFraction;

        // Reference to an IPokeInteractor in our parent hierarchy, if one exists.
        private IPokeInteractor pokeInteractor;

        private void Awake()
        {
            // Optional.
            variableReticleVisuals = GetComponentInChildren<IVariableReticle>(includeInactive: true);
            pokeInteractor = gameObject.GetComponentInParent<IPokeInteractor>(includeInactive: true);
        }

        private void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;

            // On enabling, snap the reticle immediately to the anchor point, to 
            // avoid any chance of suddenly lerping the moment the reticle is visible.
            smoothedMagnetRotation = transform.parent.rotation;
            smoothedMagnetPosition = transform.parent.position;
        }

        private void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

        private void Update()
        {
            if (detector == null) { return; }

            float closestDistance = Mathf.Infinity;
            Vector3 closestPoint = Vector3.zero;
            Collider closestCollider = null;

            // The root point used for all collider checks, raycasts, or other computations.
            // Takes poke radius into account if we have a valid IPokeInteractor parent.
            Vector3 root = transform.parent.position + transform.forward * (pokeInteractor != null ? pokeInteractor.PokeRadius : 0.0f);

            foreach (Collider nearbyCollider in detector.DetectedColliders)
            {
                // Sometimes things can be destroyed in between when we detect them
                // and when we want to magnetize to them!
                if (nearbyCollider == null || (nearbyCollider is MeshCollider meshCollider && !meshCollider.convex)) { continue; }

                Vector3 nearestPoint = nearbyCollider.ClosestPoint(root);
                float distance = Vector3.Distance(root, nearestPoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = nearbyCollider;
                    closestPoint = nearestPoint;
                }
            }

            if (closestCollider == null) { return; }

            float fraction = Mathf.Clamp01(closestDistance / magnetRange);
            positionFraction = positionMagnetismCurve.Evaluate(fraction);
            rotationFraction = rotationMagnetismCurve.Evaluate(fraction);
            progressFraction = variableReticleCurve.Evaluate(fraction);

            // The closer we are to the surface, the less we smooth.
            // This is so that while we are actively pressing a surface, the reticle sticks with the finger accurately,
            // but when we are far away, we smoothly switch between "nearest surfaces".
            float smoothingFraction = positionSmoothingCurve.Evaluate(fraction);
            float positionSmoothingFactor = Mathf.Lerp(1.0f, Time.deltaTime / positionSmoothing, smoothingFraction);
            smoothedMagnetPosition += (closestPoint - smoothedMagnetPosition) * positionSmoothingFactor;

            Ray ray = new Ray(root, closestPoint - root);

            // Check ray normalization as shorthand for determining whether ray is valid or not.
            // Ray will be invalid if the closest point is inside the collider.
            bool insideCollider = Mathf.Approximately(ray.direction.sqrMagnitude, 0.0f);

            // Is the surface facing the camera? Don't magnetize to surfaces facing away from the user.
            bool surfaceFacingCamera = Vector3.Dot(ray.direction, Camera.main.transform.forward) > 0.0f;

            if (!insideCollider && surfaceFacingCamera)
            {
                // Compute framerate-independent rotation smoothing factor.
                float rotationSmoothingFactor = Time.deltaTime / rotationSmoothing;

                // Heuristic: If the collider is sufficiently deep (or spherical), we should snap to the surface normal.
                // If not, we should just snap to the +Z axis. (for buttons, etc)
                // More accurately, this should be checking if the interactable is a PressableButton or not, but
                // we don't have access to those namespaces/types in this package (intentionally!)
                bool shouldUseSurfaceNormal = false;

                if (closestCollider is BoxCollider boxCollider)
                {
                    // Compute global BoxCollider depth with lossyScale, and check whether
                    // it exceeds the colliderThicknessCutoff.
                    shouldUseSurfaceNormal |= boxCollider.transform.lossyScale.z * boxCollider.size.z > colliderThicknessCutoff;
                }
                else
                {
                    // Use surface normal for all sphere- and mesh colliders.
                    shouldUseSurfaceNormal = true;
                }

                // Compute overall rotation, either based on surface normal (for sufficiently non-planar colliders)
                // or by collider +Z.
                if (shouldUseSurfaceNormal && closestCollider.Raycast(ray, out RaycastHit hitInfo, magnetRange))
                {
                    // Slerp the smoothed rotation to the hit normal.
                    smoothedMagnetRotation = Quaternion.Slerp(smoothedMagnetRotation, Quaternion.LookRotation(-hitInfo.normal), rotationSmoothingFactor);
                }
                else
                {
                    // Slerp the smoothed rotation to the collider's +Z.
                    smoothedMagnetRotation = Quaternion.Slerp(smoothedMagnetRotation, Quaternion.LookRotation(closestCollider.transform.forward), rotationSmoothingFactor);
                }
            }

            // If we're using variable reticle visuals, update the visuals with the progress/proximity.
            if (variableReticleVisuals != null && variableReticleVisuals is RingReticle ringReticleVisuals)
            {
                ringReticleVisuals.UpdateVisuals(1.0f - progressFraction);
            }
        }

        private void OnBeforeRender()
        {
            Quaternion mixedRotation = Quaternion.Slerp(smoothedMagnetRotation, transform.parent.rotation, rotationFraction);

            // Note: fingertip radius offset is computed by the forward-vector of the *reticle*. This results in the
            // fingertip offset nicely wrapping around the finger when the magnetization does its job.
            Vector3 fingerThicknessOffset = transform.forward * (pokeInteractor != null ? pokeInteractor.PokeRadius : 0.0f);
            Vector3 mixedPosition = Vector3.Lerp(smoothedMagnetPosition, transform.parent.position + fingerThicknessOffset, positionFraction);
            transform.SetPositionAndRotation(mixedPosition, mixedRotation);
        }
    }
}
