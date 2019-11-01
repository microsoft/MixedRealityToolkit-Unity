// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This class allows for HoloLens 1 style input, using a far gaze ray
    /// for focus with hand and gesture-based input and interaction across it.
    /// </summary>
    /// <remarks>
    /// This pointer's position is given by hand position (grip pose),
    /// and the input focus is given by head gaze.
    /// </remarks>
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

        private GazeProvider gazeProvider;
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

        public IMixedRealityInputSource InputSourceParent { get; private set; }

        public IMixedRealityCursor BaseCursor { get; set; }

        public ICursorModifier CursorModifier { get; set; }

        public bool IsInteractionEnabled => IsActive;

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

        public float SphereCastRadius
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
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

        public void OnPostSceneQuery()
        {
            if (isSelectPressed && IsInteractionEnabled)
            {
                CoreServices.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, Controller.ControllerHandedness);
            }
        }

        public void OnPreSceneQuery()
        {
            Vector3 newGazeOrigin = gazeProvider.GazePointer.Rays[0].Origin;
            Vector3 endPoint = newGazeOrigin + (gazeProvider.GazePointer.Rays[0].Direction * CoreServices.InputSystem.FocusProvider.GlobalPointingExtent);
            Rays[0].UpdateRayStep(ref newGazeOrigin, ref endPoint);
        }

        public void OnPreCurrentPointerTargetChange() { }

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
                return Quaternion.LookRotation(look);
            }
        }

        #endregion

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public void OnInputUp(InputEventData eventData)
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

        /// <inheritdoc />
        public void OnInputDown(InputEventData eventData)
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

        #endregion  IMixedRealityInputHandler Implementation

        protected override void OnEnable()
        {
            base.OnEnable();

            gazeProvider = CoreServices.InputSystem.GazeProvider as GazeProvider;
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

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
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

        #endregion IMixedRealitySourceStateHandler

        #region IMixedRealityInputHandler<MixedRealityPose>

        /// <inheritdoc />
        public void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId &&
                eventData.Handedness == Controller?.ControllerHandedness &&
                eventData.MixedRealityInputAction == poseAction)
            {
                sourcePosition = eventData.InputData.Position;
            }
        }

        #endregion IMixedRealityInputHandler<MixedRealityPose>
    }
}
