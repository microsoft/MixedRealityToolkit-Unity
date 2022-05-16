// Cospyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class GazeConePointer : GenericPointer,
            IMixedRealityQueryablePointer
    {
        private readonly Transform gazeTransform;
        private readonly BaseRayStabilizer stabilizer;
        private readonly GazeProvider gazeProvider;

        public override IMixedRealityController Controller { get; set; }

        private float pointerExtent;

        /// <inheritdoc />
        public override float PointerExtent
        {
            get => pointerExtent;
            set => pointerExtent = value;
        }

        // Is the pointer currently down
        private bool isDown = false;

        // Input source that raised pointer down
        private IMixedRealityInputSource currentInputSource;

        // Handedness of the input source that raised pointer down
        private Handedness currentHandedness = Handedness.None;

        /// <summary>
        /// Only for use when initializing Gaze Pointer on startup.
        /// </summary>
        internal void SetGazeInputSourceParent(IMixedRealityInputSource gazeInputSource)
        {
            InputSourceParent = gazeInputSource;
        }

        public float coneCastSphereRadius = 0.05f;
        public float coneCastRange = 10.0f;
        public float coneCastAngle = 1.5f;
        public float coneCastDistanceWeight = 0.25f;
        public float coneCastAngleWeight = 1.0f;
        public float coneCastDistanceToCenterWeight = 0.5f;
        public float coneCastAngleToCenterWeight = 0.0f;
        public LayerMask castLayerMask;

        // Values ultimately returned by SceneQuery
        private RaycastHit coneCastHit;

        public GazeConePointer(GazeProvider gazeProvider, string pointerName, IMixedRealityInputSource inputSourceParent, LayerMask[] raycastLayerMasks, float pointerExtent, Transform gazeTransform, BaseRayStabilizer stabilizer)
                    : base(pointerName, inputSourceParent)
        {
            this.gazeProvider = gazeProvider;
            PrioritizedLayerMasksOverride = raycastLayerMasks;
            this.pointerExtent = pointerExtent;
            this.gazeTransform = gazeTransform;
            this.stabilizer = stabilizer;
            IsInteractionEnabled = true;
        }

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] InternalConeCastPointer.OnPreSceneQuery");

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                Vector3 newGazeOrigin;
                Vector3 newGazeNormal;

                if (gazeProvider.IsEyeTrackingEnabledAndValid)
                {
                    newGazeOrigin = gazeProvider.LatestEyeGaze.origin;
                    newGazeNormal = gazeProvider.LatestEyeGaze.direction;
                }
                else
                {
                    if (gazeProvider.UseHeadGazeOverride && gazeProvider.overrideHeadPosition.HasValue && gazeProvider.overrideHeadForward.HasValue)
                    {
                        newGazeOrigin = gazeProvider.overrideHeadPosition.Value;
                        newGazeNormal = gazeProvider.overrideHeadForward.Value;
                    }
                    else
                    {
                        newGazeOrigin = gazeTransform.position;
                        newGazeNormal = gazeTransform.forward;
                    }

                    // Update gaze info from stabilizer
                    if (stabilizer != null)
                    {
                        stabilizer.UpdateStability(MixedRealityPlayspace.InverseTransformPoint(newGazeOrigin), MixedRealityPlayspace.InverseTransformDirection(newGazeNormal));
                        newGazeOrigin = MixedRealityPlayspace.TransformPoint(stabilizer.StablePosition);
                        newGazeNormal = MixedRealityPlayspace.TransformDirection(stabilizer.StableRay.direction);
                    }
                }

                Vector3 endPoint = newGazeOrigin + (newGazeNormal * pointerExtent);
                Rays[0].UpdateRayStep(ref newGazeOrigin, ref endPoint);

                coneCastHit = ConeCastUtility.ConeCastBest(newGazeOrigin, Vector3.Normalize(newGazeNormal), coneCastSphereRadius, coneCastRange, coneCastAngle, PrioritizedLayerMasksOverride[0], coneCastDistanceWeight, coneCastAngleWeight, coneCastDistanceToCenterWeight, coneCastAngleToCenterWeight);
            }
        }

        // Returns the hit values cached by the queryBuffer during the prescene query step
        public bool OnSceneQuery(LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out MixedRealityRaycastHit hitInfo, out RayStep Ray, out int rayStepIndex)
        {
            PrioritizedLayerMasksOverride = prioritizedLayerMasks;
            if (coneCastHit.collider != null)
            {
                hitInfo = new MixedRealityRaycastHit(true, coneCastHit);
                Ray = Rays[0];
                rayStepIndex = 0;
                return true;
            }
            else
            {
                hitInfo = new MixedRealityRaycastHit();
                Ray = Rays[0];
                rayStepIndex = 0;
                return false;
            }
        }

        /// <inheritdoc />
        public bool OnSceneQuery(LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out GameObject hitObject, out Vector3 hitPoint, out float hitDistance)
        {
            PrioritizedLayerMasksOverride = prioritizedLayerMasks;
            if (coneCastHit.collider != null)
            {
                hitObject = coneCastHit.collider.gameObject;
                hitPoint = coneCastHit.point;
                hitDistance = coneCastHit.distance;
                return true;
            }
            else
            {
                hitObject = null;
                hitPoint = Vector3.zero;
                hitDistance = Mathf.Infinity;
                return false;
            }
        }

        private static readonly ProfilerMarker OnPostSceneQueryPerfMarker = new ProfilerMarker("[MRTK] InternalConeCastPointer.OnPostSceneQuery");

        /// <inheritdoc />
        public override void OnPostSceneQuery()
        {
            using (OnPostSceneQueryPerfMarker.Auto())
            {
                if (isDown)
                {
                    CoreServices.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, Controller.ControllerHandedness);
                }
            }
        }

        /// <inheritdoc />
        public override void OnPreCurrentPointerTargetChange() { }

        /// <inheritdoc />
        public override Vector3 Position => gazeTransform.position;

        /// <inheritdoc />
        public override Quaternion Rotation => gazeTransform.rotation;


        /// <inheritdoc />
        public override void Reset()
        {
            Controller = null;
            BaseCursor = null;
            IsActive = false;
            IsFocusLocked = false;
        }

        /// <summary>
        /// Press this pointer. This sends a pointer down event across the input system.
        /// </summary>
        /// <param name="mixedRealityInputAction">The input action that corresponds to the pressed button or axis.</param>
        /// <param name="handedness">Optional handedness of the source that pressed the pointer.</param>
        public void RaisePointerDown(MixedRealityInputAction mixedRealityInputAction, Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null)
        {
            isDown = true;
            currentHandedness = handedness;
            currentInputSource = inputSource;
            CoreServices.InputSystem?.RaisePointerDown(this, mixedRealityInputAction, handedness, inputSource);
        }

        /// <summary>
        /// Release this pointer. This sends pointer clicked and pointer up events across the input system.
        /// </summary>
        /// <param name="mixedRealityInputAction">The input action that corresponds to the released button or axis.</param>
        /// <param name="handedness">Optional handedness of the source that released the pointer.</param>
        public void RaisePointerUp(MixedRealityInputAction mixedRealityInputAction, Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null)
        {
            isDown = false;
            currentHandedness = Handedness.None;
            currentInputSource = null;
            CoreServices.InputSystem?.RaisePointerClicked(this, mixedRealityInputAction, 0, handedness, inputSource);
            CoreServices.InputSystem?.RaisePointerUp(this, mixedRealityInputAction, handedness, inputSource);
        }
    }
}
