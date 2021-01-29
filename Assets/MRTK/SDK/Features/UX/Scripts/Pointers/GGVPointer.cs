// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This class allows for HoloLens 1 style input, using a far gaze ray
    /// for focus with hand and gesture-based input and interaction across it.
    /// </summary>
    /// <remarks>
    /// <para>GGV stands for gaze, gesture, and voice.
    /// This pointer's position is given by hand position (grip pose),
    /// and the input focus is given by head gaze.</para>
    /// </remarks>
    [AddComponentMenu("Scripts/MRTK/SDK/GGVPointer")]
    public class GGVPointer : InputSystemGlobalHandlerListener,
        IMixedRealityPointer,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<MixedRealityPose>,
        IMixedRealitySourceStateHandler
    {
        [Header("Pointer")]
        [SerializeField]
        private MixedRealityInputAction selectAction = MixedRealityInputAction.None;

        [SerializeField]
        private MixedRealityInputAction poseAction = MixedRealityInputAction.None;

        private IMixedRealityGazeProvider gazeProvider;
        private Vector3 sourcePosition;
        private bool isSelectPressed;
        private Handedness lastControllerHandedness;

        #region IMixedRealityPointer

        private IMixedRealityController controller;

        /// <inheritdoc />
        public IMixedRealityController Controller
        {
            get { return controller; }
            set
            {
                controller = value;

                if (controller != null && this != null)
                {
                    gameObject.name = $"{Controller.ControllerHandedness}_GGVPointer";
                    pointerName = gameObject.name;
                    InputSourceParent = controller.InputSource;
                }
            }
        }

        private uint pointerId;

        /// <inheritdoc />
        public uint PointerId
        {
            get
            {
                if (pointerId == 0)
                {
                    pointerId = CoreServices.InputSystem.FocusProvider.GenerateNewPointerId();
                }

                return pointerId;
            }
        }

        private string pointerName = string.Empty;

        /// <inheritdoc />
        public string PointerName
        {
            get { return pointerName; }
            set
            {
                pointerName = value;
                if (this != null)
                {
                    gameObject.name = value;
                }
            }
        }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSourceParent { get; private set; }

        /// <inheritdoc />
        public IMixedRealityCursor BaseCursor { get; set; }

        /// <inheritdoc />
        public ICursorModifier CursorModifier { get; set; }

        /// <inheritdoc />
        public bool IsInteractionEnabled => IsActive;

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public bool IsFocusLocked { get; set; }

        /// <inheritdoc />
        public bool IsTargetPositionLockedOnFocusLock { get; set; }

        public RayStep[] Rays { get; protected set; } = { new RayStep(Vector3.zero, Vector3.forward) };

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        public IMixedRealityFocusHandler FocusTarget { get; set; }

        /// <inheritdoc />
        public IPointerResult Result { get; set; }

        /// <inheritdoc />
        public virtual SceneQueryType SceneQueryType { get; set; } = SceneQueryType.SimpleRaycast;

        /// <inheritdoc />
        public float SphereCastRadius
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        private static bool Equals(IMixedRealityPointer left, IMixedRealityPointer right)
        {
            return left != null && left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityPointer)obj);
        }

        private bool Equals(IMixedRealityPointer other)
        {
            return other != null && PointerId == other.PointerId && string.Equals(PointerName, other.PointerName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)PointerId;
                hashCode = (hashCode * 397) ^ (PointerName != null ? PointerName.GetHashCode() : 0);
                return hashCode;
            }
        }

        private static readonly ProfilerMarker OnPostSceneQueryPerfMarker = new ProfilerMarker("[MRTK] GGVPointer.OnPostSceneQuery");

        /// <inheritdoc />
        public void OnPostSceneQuery()
        {
            using (OnPostSceneQueryPerfMarker.Auto())
            {
                if (isSelectPressed && IsInteractionEnabled)
                {
                    CoreServices.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, Controller.ControllerHandedness);
                }
            }
        }

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] GGVPointer.OnPreSceneQuery");

        /// <inheritdoc />
        public void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                Vector3 newGazeOrigin = gazeProvider.GazePointer.Rays[0].Origin;
                Vector3 endPoint = newGazeOrigin + (gazeProvider.GazePointer.Rays[0].Direction * CoreServices.InputSystem.FocusProvider.GlobalPointingExtent);
                Rays[0].UpdateRayStep(ref newGazeOrigin, ref endPoint);
            }
        }

        /// <inheritdoc />
        public void OnPreCurrentPointerTargetChange() { }

        /// <inheritdoc />
        public void Reset()
        {
            Controller = null;
            BaseCursor = null;
            IsActive = false;
            IsFocusLocked = false;
        }

        /// <inheritdoc />
        public virtual Vector3 Position => sourcePosition;

        /// <inheritdoc />
        public virtual Quaternion Rotation
        {
            get
            {
                // Previously we were simply returning the InternalGazeProvider rotation here.
                // This caused issues when the head rotated, but the hand stayed where it was.
                // Now we're returning a rotation based on the vector from the camera position
                // to the hand. This rotation is not affected by rotating your head.
                Vector3 look = Position - CameraCache.Main.transform.position;

                // If the input source is at the same position as the camera, assume it's the camera and return the InternalGazeProvider rotation.
                // This prevents passing Vector3.zero into Quaternion.LookRotation, which isn't possible and causes a console log.
                if (look == Vector3.zero)
                {
                    return Quaternion.LookRotation(gazeProvider.GazePointer.Rays[0].Direction);
                }

                return Quaternion.LookRotation(look);
            }
        }

        #endregion

        #region IMixedRealityInputHandler Implementation

        private static readonly ProfilerMarker OnInputUpPerfMarker = new ProfilerMarker("[MRTK] GGVPointer.OnInputUp");

        /// <inheritdoc />
        public void OnInputUp(InputEventData eventData)
        {
            using (OnInputUpPerfMarker.Auto())
            {
                if (eventData.SourceId == InputSourceParent.SourceId)
                {
                    if (eventData.MixedRealityInputAction == selectAction)
                    {
                        isSelectPressed = false;
                        if (IsInteractionEnabled)
                        {
                            BaseCursor c = gazeProvider.GazePointer.BaseCursor as BaseCursor;
                            if (c != null)
                            {
                                c.SourceDownIds.Remove(eventData.SourceId);
                            }

                            CoreServices.InputSystem.RaisePointerClicked(this, selectAction, 0, Controller.ControllerHandedness);
                            CoreServices.InputSystem.RaisePointerUp(this, selectAction, Controller.ControllerHandedness);

                            // For GGV, the gaze pointer does not set this value itself. 
                            // See comment in OnInputDown for more details.
                            gazeProvider.GazePointer.IsFocusLocked = false;
                        }
                    }
                }
            }
        }

        private static readonly ProfilerMarker OnInputDownPerfMarker = new ProfilerMarker("[MRTK] GGVPointer.OnInputDown");

        /// <inheritdoc />
        public void OnInputDown(InputEventData eventData)
        {
            using (OnInputDownPerfMarker.Auto())
            {
                if (eventData.SourceId == InputSourceParent.SourceId)
                {
                    if (eventData.MixedRealityInputAction == selectAction)
                    {
                        isSelectPressed = true;
                        lastControllerHandedness = Controller.ControllerHandedness;
                        if (IsInteractionEnabled)
                        {
                            BaseCursor c = gazeProvider.GazePointer.BaseCursor as BaseCursor;
                            if (c != null)
                            {
                                c.SourceDownIds.Add(eventData.SourceId);
                            }

                            CoreServices.InputSystem.RaisePointerDown(this, selectAction, Controller.ControllerHandedness);

                            // For GGV, the gaze pointer does not set this value itself as it does not receive input 
                            // events from the hands. Because this value is important for certain gaze behavior, 
                            // such as positioning the gaze cursor, it is necessary to set it here.
                            gazeProvider.GazePointer.IsFocusLocked = (gazeProvider.GazePointer.Result?.Details.Object != null);
                        }
                    }
                }
            }
        }

        #endregion  IMixedRealityInputHandler Implementation

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();

            gazeProvider = CoreServices.InputSystem.GazeProvider;
            BaseCursor c = gazeProvider.GazePointer.BaseCursor as BaseCursor;
            if (c != null)
            {
                c.VisibleSourcesCount++;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (gazeProvider != null)
            {
                BaseCursor c = gazeProvider.GazePointer.BaseCursor as BaseCursor;
                if (c != null)
                {
                    c.VisibleSourcesCount--;
                }
            }
        }

        #endregion MonoBehaviour Implementation

        #region InputSystemGlobalHandlerListener Implementation

        /// <inheritdoc />
        protected override void RegisterHandlers()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        }

        /// <inheritdoc />
        protected override void UnregisterHandlers()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        }

        #endregion InputSystemGlobalHandlerListener Implementation

        #region IMixedRealitySourceStateHandler

        /// <inheritdoc />
        public void OnSourceDetected(SourceStateEventData eventData) { }

        private static readonly ProfilerMarker OnSourceLostPerfMarker = new ProfilerMarker("[MRTK] GGVPointer.OnSourceLost");

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            using (OnSourceLostPerfMarker.Auto())
            {
                if (eventData.SourceId == InputSourceParent.SourceId)
                {
                    BaseCursor c = gazeProvider.GazePointer.BaseCursor as BaseCursor;
                    if (c != null)
                    {
                        c.SourceDownIds.Remove(eventData.SourceId);
                    }

                    if (isSelectPressed)
                    {
                        // Raise OnInputUp if pointer is lost while select is pressed
                        CoreServices.InputSystem.RaisePointerUp(this, selectAction, lastControllerHandedness);

                        // For GGV, the gaze pointer does not set this value itself. 
                        // See comment in OnInputDown for more details.
                        gazeProvider.GazePointer.IsFocusLocked = false;
                    }

                    // Destroy the pointer since nobody else is destroying us
                    GameObjectExtensions.DestroyGameObject(gameObject);
                }
            }
        }

        #endregion IMixedRealitySourceStateHandler

        #region IMixedRealityInputHandler<MixedRealityPose>

        private static readonly ProfilerMarker OnInputChangedPerfMarker = new ProfilerMarker("[MRTK] GGVPointer.OnInputChanged");

        /// <inheritdoc />
        public void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            using (OnInputChangedPerfMarker.Auto())
            {
                if (eventData.SourceId == Controller?.InputSource.SourceId &&
                    eventData.Handedness == Controller?.ControllerHandedness &&
                    eventData.MixedRealityInputAction == poseAction)
                {
                    sourcePosition = eventData.InputData.Position;
                }
            }
        }

        #endregion IMixedRealityInputHandler<MixedRealityPose>
    }
}
