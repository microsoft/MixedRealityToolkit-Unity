// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA
using System.Collections.Generic;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for gestures and interaction source information from the WSA APIs, which gives access to various system-supported gestures
    /// and positional information for the various inputs that Windows gestures supports.
    /// This is mostly a wrapper on top of GestureRecognizer and InteractionManager.
    /// </summary>
    public class InteractionInputSource : BaseInputSource
    {
        // This enumeration gives the manager two different ways to handle the recognizer. Both will
        // set up the recognizer. The first causes the recognizer to start
        // immediately. The second allows the recognizer to be manually started at a later time.
        public enum RecognizerStartBehavior { AutoStart, ManualStart }

        [Tooltip("Whether the recognizer should be activated on start.")]
        public RecognizerStartBehavior RecognizerStart;

        [Tooltip("Set to true to use the use rails (guides) for the navigation gesture, as opposed to full 3D navigation.")]
        public bool UseRailsNavigation = false;

        /// <summary>
        /// Always true initially so we only initialize our interaction sources 
        /// after all <see cref="Singleton{T}"/> Instances have been properly initialized.
        /// </summary>
        private bool delayInitialization = true;

#if UNITY_WSA
        protected GestureRecognizer GestureRecognizer;
        protected GestureRecognizer NavigationGestureRecognizer;
#endif

        #region IInputSource Capabilities and SourceData

#if UNITY_WSA
        private struct SourceCapability<TReading>
        {
            public bool IsSupported;
            public bool IsAvailable;
            public TReading CurrentReading;
        }

        private struct AxisButton2D
        {
            public bool Pressed;
            public Vector2 Position;

            public static AxisButton2D GetThumbstick(InteractionSourceState interactionSource)
            {
                return new AxisButton2D
                {
#if UNITY_2017_2_OR_NEWER
                    Pressed = interactionSource.thumbstickPressed,
                    Position = interactionSource.thumbstickPosition,
#else
                    Pressed = false,
                    Position = default(Vector2)
#endif
                };
            }

            public static AxisButton2D GetTouchpad(InteractionSourceState interactionSource)
            {
                return new AxisButton2D
                {
#if UNITY_2017_2_OR_NEWER
                    Pressed = interactionSource.touchpadPressed,
                    Position = interactionSource.touchpadPosition,
#else
                    Pressed = false,
                    Position = default(Vector2)
#endif
                };
            }
        }

        private struct TouchpadData
        {
            public AxisButton2D AxisButton;
            public bool Touched;

            public static TouchpadData GetTouchpad(InteractionSourceState interactionSource)
            {
                return new TouchpadData
                {
                    AxisButton = AxisButton2D.GetTouchpad(interactionSource),
#if UNITY_2017_2_OR_NEWER
                    Touched = interactionSource.touchpadTouched,
#else
                    Touched = false,
#endif
                };
            }
        }

        private struct AxisButton1D
        {
            public bool Pressed;
            public double PressedAmount;

            public static AxisButton1D GetSelect(InteractionSourceState interactionSource)
            {
                return new AxisButton1D
                {
#if UNITY_2017_2_OR_NEWER
                    Pressed = interactionSource.selectPressed,
                    PressedAmount = interactionSource.selectPressedAmount,
#else
                    Pressed = false,
                    PressedAmount = 0f
#endif
                };
            }
        }

        /// <summary>
        /// Data for an interaction source.
        /// </summary>
        private class SourceData
        {
            public SourceData(InteractionSource interactionSource)
            {
                Source = interactionSource;
            }

            public void ResetUpdatedBooleans()
            {
                ThumbstickPositionUpdated = false;
                TouchpadPositionUpdated = false;
                TouchpadTouchedUpdated = false;
                PositionUpdated = false;
                RotationUpdated = false;
                SelectPressedAmountUpdated = false;
            }

            public uint SourceId { get { return Source.id; } }
            public InteractionSourceKind SourceKind { get { return Source.kind; } }
#if UNITY_2017_2_OR_NEWER
            public InteractionSourceHandedness Handedness { get { return Source.handedness; } }
#endif
            public readonly InteractionSource Source;
            public SourceCapability<Vector3> PointerPosition;
            public SourceCapability<Quaternion> PointerRotation;
            public SourceCapability<Ray> PointingRay;
            public SourceCapability<Vector3> GripPosition;
            public SourceCapability<Quaternion> GripRotation;
            public SourceCapability<AxisButton2D> Thumbstick;
            public SourceCapability<TouchpadData> Touchpad;
            public SourceCapability<AxisButton1D> Select;
            public SourceCapability<bool> Grasp;
            public SourceCapability<bool> Menu;

            public bool ThumbstickPositionUpdated;
            public bool TouchpadPositionUpdated;
            public bool TouchpadTouchedUpdated;
            public bool PositionUpdated;
            public bool RotationUpdated;
            public bool SelectPressedAmountUpdated;
        }

        /// <summary>
        /// Dictionary linking each source ID to its data.
        /// </summary>
        private readonly Dictionary<uint, SourceData> sourceIdToData = new Dictionary<uint, SourceData>(4);
#endif
        #endregion IInputSource Capabilities and SourceData

        #region MonoBehaviour APIs

        protected virtual void Awake()
        {
#if UNITY_WSA
            GestureRecognizer = new GestureRecognizer();
#if UNITY_2017_2_OR_NEWER
            GestureRecognizer.Tapped += GestureRecognizer_Tapped;

            GestureRecognizer.HoldStarted += GestureRecognizer_HoldStarted;
            GestureRecognizer.HoldCompleted += GestureRecognizer_HoldCompleted;
            GestureRecognizer.HoldCanceled += GestureRecognizer_HoldCanceled;

            GestureRecognizer.ManipulationStarted += GestureRecognizer_ManipulationStarted;
            GestureRecognizer.ManipulationUpdated += GestureRecognizer_ManipulationUpdated;
            GestureRecognizer.ManipulationCompleted += GestureRecognizer_ManipulationCompleted;
            GestureRecognizer.ManipulationCanceled += GestureRecognizer_ManipulationCanceled;
#else
            GestureRecognizer.TappedEvent += GestureRecognizer_Tapped;

            GestureRecognizer.HoldStartedEvent += GestureRecognizer_HoldStarted;
            GestureRecognizer.HoldCompletedEvent += GestureRecognizer_HoldCompleted;
            GestureRecognizer.HoldCanceledEvent += GestureRecognizer_HoldCanceled;

            GestureRecognizer.ManipulationStartedEvent += GestureRecognizer_ManipulationStarted;
            GestureRecognizer.ManipulationUpdatedEvent += GestureRecognizer_ManipulationUpdated;
            GestureRecognizer.ManipulationCompletedEvent += GestureRecognizer_ManipulationCompleted;
            GestureRecognizer.ManipulationCanceledEvent += GestureRecognizer_ManipulationCanceled;
#endif
            GestureRecognizer.SetRecognizableGestures(GestureSettings.Tap |
                                                      GestureSettings.ManipulationTranslate |
                                                      GestureSettings.Hold);

            // We need a separate gesture recognizer for navigation, since it isn't compatible with manipulation
            NavigationGestureRecognizer = new GestureRecognizer();

#if UNITY_2017_2_OR_NEWER
            NavigationGestureRecognizer.NavigationStarted += NavigationGestureRecognizer_NavigationStarted;
            NavigationGestureRecognizer.NavigationUpdated += NavigationGestureRecognizer_NavigationUpdated;
            NavigationGestureRecognizer.NavigationCompleted += NavigationGestureRecognizer_NavigationCompleted;
            NavigationGestureRecognizer.NavigationCanceled += NavigationGestureRecognizer_NavigationCanceled;
#else
            NavigationGestureRecognizer.NavigationStartedEvent += NavigationGestureRecognizer_NavigationStarted;
            NavigationGestureRecognizer.NavigationUpdatedEvent += NavigationGestureRecognizer_NavigationUpdated;
            NavigationGestureRecognizer.NavigationCompletedEvent += NavigationGestureRecognizer_NavigationCompleted;
            NavigationGestureRecognizer.NavigationCanceledEvent += NavigationGestureRecognizer_NavigationCanceled;
#endif
            if (UseRailsNavigation)
            {
                NavigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationRailsX |
                                                                    GestureSettings.NavigationRailsY |
                                                                    GestureSettings.NavigationRailsZ);
            }
            else
            {
                NavigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationX |
                                                                    GestureSettings.NavigationY |
                                                                    GestureSettings.NavigationZ);
            }

            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                GestureRecognizer.StartCapturingGestures();
                NavigationGestureRecognizer.StartCapturingGestures();
            }
#endif
        }

        protected virtual void OnEnable()
        {
            if (!delayInitialization)
            {
                // The first time we call OnEnable we skip this.
                InitializeSources();
            }
        }

        protected virtual void Start()
        {
            if (delayInitialization)
            {
                delayInitialization = false;
                InitializeSources();
            }
        }

        protected virtual void OnDisable()
        {
#if UNITY_WSA
            StopGestureRecognizer();

#if UNITY_2017_2_OR_NEWER
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
#else
            InteractionManager.SourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.SourcePressed -= InteractionManager_InteractionSourcePressed;
            InteractionManager.SourceUpdated -= InteractionManager_InteractionSourceUpdated;
            InteractionManager.SourceReleased -= InteractionManager_InteractionSourceReleased;
            InteractionManager.SourceLost -= InteractionManager_InteractionSourceLost;
#endif

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();
            for (var i = 0; i < states.Length; i++)
            {
                // NOTE: We don't care whether the source ID previously existed or not, so we blindly call Remove:
                sourceIdToData.Remove(states[i].source.id);
                InputManager.Instance.RaiseSourceLost(this, states[i].source.id);
            }
#endif
        }

        protected virtual void OnDestroy()
        {
#if UNITY_WSA
            if (GestureRecognizer != null)
            {
#if UNITY_2017_2_OR_NEWER
                GestureRecognizer.Tapped -= GestureRecognizer_Tapped;

                GestureRecognizer.HoldStarted -= GestureRecognizer_HoldStarted;
                GestureRecognizer.HoldCompleted -= GestureRecognizer_HoldCompleted;
                GestureRecognizer.HoldCanceled -= GestureRecognizer_HoldCanceled;

                GestureRecognizer.ManipulationStarted -= GestureRecognizer_ManipulationStarted;
                GestureRecognizer.ManipulationUpdated -= GestureRecognizer_ManipulationUpdated;
                GestureRecognizer.ManipulationCompleted -= GestureRecognizer_ManipulationCompleted;
                GestureRecognizer.ManipulationCanceled -= GestureRecognizer_ManipulationCanceled;
#else
                GestureRecognizer.TappedEvent -= GestureRecognizer_Tapped;

                GestureRecognizer.HoldStartedEvent -= GestureRecognizer_HoldStarted;
                GestureRecognizer.HoldCompletedEvent -= GestureRecognizer_HoldCompleted;
                GestureRecognizer.HoldCanceledEvent -= GestureRecognizer_HoldCanceled;

                GestureRecognizer.ManipulationStartedEvent -= GestureRecognizer_ManipulationStarted;
                GestureRecognizer.ManipulationUpdatedEvent -= GestureRecognizer_ManipulationUpdated;
                GestureRecognizer.ManipulationCompletedEvent -= GestureRecognizer_ManipulationCompleted;
                GestureRecognizer.ManipulationCanceledEvent -= GestureRecognizer_ManipulationCanceled;

#endif
                GestureRecognizer.Dispose();
            }

            if (NavigationGestureRecognizer != null)
            {
#if UNITY_2017_2_OR_NEWER
                NavigationGestureRecognizer.NavigationStarted -= NavigationGestureRecognizer_NavigationStarted;
                NavigationGestureRecognizer.NavigationUpdated -= NavigationGestureRecognizer_NavigationUpdated;
                NavigationGestureRecognizer.NavigationCompleted -= NavigationGestureRecognizer_NavigationCompleted;
                NavigationGestureRecognizer.NavigationCanceled -= NavigationGestureRecognizer_NavigationCanceled;
#else
                NavigationGestureRecognizer.NavigationStartedEvent -= NavigationGestureRecognizer_NavigationStarted;
                NavigationGestureRecognizer.NavigationUpdatedEvent -= NavigationGestureRecognizer_NavigationUpdated;
                NavigationGestureRecognizer.NavigationCompletedEvent -= NavigationGestureRecognizer_NavigationCompleted;
                NavigationGestureRecognizer.NavigationCanceledEvent -= NavigationGestureRecognizer_NavigationCanceled;
#endif
                NavigationGestureRecognizer.Dispose();
            }
#endif
        }

        #endregion MonoBehaviour APIs

        /// <summary>
        /// Make sure the gesture recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StartGestureRecognizer()
        {
#if UNITY_WSA
            if (GestureRecognizer != null && !GestureRecognizer.IsCapturingGestures())
            {
                GestureRecognizer.StartCapturingGestures();
            }
            if (NavigationGestureRecognizer != null && !NavigationGestureRecognizer.IsCapturingGestures())
            {
                NavigationGestureRecognizer.StartCapturingGestures();
            }
#endif
        }

        /// <summary>
        /// Make sure the gesture recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StopGestureRecognizer()
        {
#if UNITY_WSA
            if (GestureRecognizer != null && GestureRecognizer.IsCapturingGestures())
            {
                GestureRecognizer.StopCapturingGestures();
            }

            if (NavigationGestureRecognizer != null && NavigationGestureRecognizer.IsCapturingGestures())
            {
                NavigationGestureRecognizer.StopCapturingGestures();
            }
#endif
        }

        private void InitializeSources()
        {
            InputManager.AssertIsInitialized();

#if UNITY_WSA
            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartGestureRecognizer();
            }

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();
            for (var i = 0; i < states.Length; i++)
            {
                GetOrAddSourceData(states[i].source);
                InputManager.Instance.RaiseSourceDetected(this, states[i].source.id);
            }

#if UNITY_2017_2_OR_NEWER
            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
#else
            InteractionManager.SourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.SourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.SourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.SourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.SourceLost += InteractionManager_InteractionSourceLost;
#endif

#else
            RecognizerStart = RecognizerStartBehavior.ManualStart;
#endif
        }

        public void StartHaptics(uint sourceId, float intensity)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StartHaptics(intensity);
            }
#endif
        }

        public void StartHaptics(uint sourceId, float intensity, float durationInSeconds)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StartHaptics(intensity, durationInSeconds);
            }
#endif
        }

        public void StopHaptics(uint sourceId)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StopHaptics();
            }
#endif
        }

        public bool TryGetHandedness(uint sourceId, out Handedness handedness)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                handedness = (Handedness)sourceData.Handedness;
                return true;
            }
#endif

            handedness = default(Handedness);
            return false;
        }

        #region BaseInputSource implementations

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            var retVal = SupportedInputInfo.None;
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                retVal |= GetSupportFlag(sourceData.PointerPosition, SupportedInputInfo.PointerPosition);
                retVal |= GetSupportFlag(sourceData.PointerRotation, SupportedInputInfo.PointerRotation);
                retVal |= GetSupportFlag(sourceData.GripPosition, SupportedInputInfo.GripPosition);
                retVal |= GetSupportFlag(sourceData.GripRotation, SupportedInputInfo.GripRotation);
                retVal |= GetSupportFlag(sourceData.PointingRay, SupportedInputInfo.Pointing);
                retVal |= GetSupportFlag(sourceData.Thumbstick, SupportedInputInfo.Thumbstick);
                retVal |= GetSupportFlag(sourceData.Touchpad, SupportedInputInfo.Touchpad);
                retVal |= GetSupportFlag(sourceData.Select, SupportedInputInfo.Select);
                retVal |= GetSupportFlag(sourceData.Menu, SupportedInputInfo.Menu);
                retVal |= GetSupportFlag(sourceData.Grasp, SupportedInputInfo.Grasp);
            }
#endif
            return retVal;
        }

        public override bool TryGetSourceKind(uint sourceId, out InteractionSourceInfo sourceKind)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceKind = (InteractionSourceInfo)sourceData.SourceKind;
                return true;
            }
#endif

            sourceKind = default(InteractionSourceInfo);
            return false;
        }

        public override bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerPosition, out position))
            {
                return true;
            }
#endif

            position = default(Vector3);
            return false;
        }

        public override bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerRotation, out rotation))
            {
                return true;
            }
#endif

            rotation = default(Quaternion);
            return false;
        }

        public override bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointingRay, out pointingRay))
            {
                return true;
            }
#endif

            pointingRay = default(Ray);
            return false;
        }

        public override bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripPosition, out position))
            {
                return true;
            }
#endif

            position = default(Vector3);
            return false;
        }

        public override bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripRotation, out rotation))
            {
                return true;
            }
#endif

            rotation = default(Quaternion);
            return false;
        }

        public override bool TryGetThumbstick(uint sourceId, out bool thumbstickPressed, out Vector2 thumbstickPosition)
        {
#if UNITY_WSA
            SourceData sourceData;
            AxisButton2D thumbstick;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Thumbstick, out thumbstick))
            {
                thumbstickPressed = thumbstick.Pressed;
                thumbstickPosition = thumbstick.Position;
                return true;
            }
#endif

            thumbstickPressed = false;
            thumbstickPosition = Vector2.zero;
            return false;
        }

        public override bool TryGetTouchpad(uint sourceId, out bool touchpadPressed, out bool touchpadTouched, out Vector2 touchpadPosition)
        {
#if UNITY_WSA
            SourceData sourceData;
            TouchpadData touchpad;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Touchpad, out touchpad))
            {
                touchpadPressed = touchpad.AxisButton.Pressed;
                touchpadTouched = touchpad.Touched;
                touchpadPosition = touchpad.AxisButton.Position;
                return true;
            }
#endif

            touchpadPressed = false;
            touchpadTouched = false;
            touchpadPosition = Vector2.zero;
            return false;
        }

        public override bool TryGetSelect(uint sourceId, out bool selectPressed, out double selectPressedAmount)
        {
#if UNITY_WSA
            SourceData sourceData;
            AxisButton1D select;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Select, out select))
            {
                selectPressed = select.Pressed;
                selectPressedAmount = select.PressedAmount;
                return true;
            }
#endif

            selectPressed = false;
            selectPressedAmount = 0;
            return false;
        }

        public override bool TryGetGrasp(uint sourceId, out bool graspPressed)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Grasp, out graspPressed))
            {
                return true;
            }
#endif

            graspPressed = false;
            return false;
        }

        public override bool TryGetMenu(uint sourceId, out bool menuPressed)
        {

#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Menu, out menuPressed))
            {
                return true;
            }
#endif

            menuPressed = false;
            return false;
        }

#if UNITY_WSA
        private bool TryGetReading<TReading>(SourceCapability<TReading> capability, out TReading reading)
        {
            if (capability.IsAvailable)
            {
                Debug.Assert(capability.IsSupported);

                reading = capability.CurrentReading;
                return true;
            }
            reading = default(TReading);
            return false;
        }

        private SupportedInputInfo GetSupportFlag<TReading>(SourceCapability<TReading> capability, SupportedInputInfo flagIfSupported)
        {
            return (capability.IsSupported ? flagIfSupported : SupportedInputInfo.None);
        }
#endif
        #endregion

#if UNITY_WSA
        /// <summary>
        /// Gets the source data for the specified interaction source if it already exists, otherwise creates it.
        /// </summary>
        /// <param name="interactionSource">Interaction source for which data should be retrieved.</param>
        /// <returns>The source data requested.</returns>
        private SourceData GetOrAddSourceData(InteractionSource interactionSource)
        {
            SourceData sourceData;
            if (!sourceIdToData.TryGetValue(interactionSource.id, out sourceData))
            {
                sourceData = new SourceData(interactionSource);
                sourceIdToData.Add(sourceData.SourceId, sourceData);

                // TODO: robertes: whenever we end up adding, should we first synthesize a SourceDetected? Or
                //       perhaps if we keep strict track of all sources, we should never need to just-in-time add anymore.
            }

            return sourceData;
        }

        /// <summary>
        /// Updates the source information.
        /// </summary>
        /// <param name="interactionSourceState">Interaction source to use to update the source information.</param>
        /// <param name="sourceData">SourceData structure to update.</param>
        private void UpdateSourceData(InteractionSourceState interactionSourceState, SourceData sourceData)
        {
            Debug.Assert(interactionSourceState.source.id == sourceData.SourceId, "An UpdateSourceState call happened with mismatched source ID.");
            Debug.Assert(interactionSourceState.source.kind == sourceData.SourceKind, "An UpdateSourceState call happened with mismatched source kind.");

            Vector3 newPointerPosition = Vector3.zero;
            sourceData.PointerPosition.IsAvailable =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.sourcePose.TryGetPosition(out newPointerPosition, InteractionSourceNode.Pointer);
#else
                interactionSourceState.properties.location.TryGetPosition(out newPointerPosition);
#endif
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.PointerPosition.IsSupported |= sourceData.PointerPosition.IsAvailable;

            Vector3 newGripPosition = Vector3.zero;
            sourceData.GripPosition.IsAvailable =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.sourcePose.TryGetPosition(out newGripPosition, InteractionSourceNode.Grip);
#else
                false;
#endif
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.GripPosition.IsSupported |= sourceData.GripPosition.IsAvailable;

            if (CameraCache.Main.transform.parent != null)
            {
                newPointerPosition = CameraCache.Main.transform.parent.TransformPoint(newPointerPosition);
                newGripPosition = CameraCache.Main.transform.parent.TransformPoint(newGripPosition);
            }

            if (sourceData.PointerPosition.IsAvailable || sourceData.GripPosition.IsAvailable)
            {
                sourceData.PositionUpdated = !(sourceData.PointerPosition.CurrentReading.Equals(newPointerPosition) && sourceData.GripPosition.CurrentReading.Equals(newGripPosition));
            }

            sourceData.PointerPosition.CurrentReading = newPointerPosition;
            sourceData.GripPosition.CurrentReading = newGripPosition;

            Quaternion newPointerRotation = Quaternion.identity;
            sourceData.PointerRotation.IsAvailable =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.sourcePose.TryGetRotation(out newPointerRotation, InteractionSourceNode.Pointer);
#else
                false;
#endif
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.PointerRotation.IsSupported |= sourceData.PointerRotation.IsAvailable;

            Quaternion newGripRotation = Quaternion.identity;
            sourceData.GripRotation.IsAvailable =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.sourcePose.TryGetRotation(out newGripRotation, InteractionSourceNode.Grip);
#else
                false;
#endif
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.GripRotation.IsSupported |= sourceData.GripRotation.IsAvailable;

            if (CameraCache.Main.transform.parent != null)
            {
                newPointerRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(newPointerRotation.eulerAngles);
                newGripRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(newGripRotation.eulerAngles);
            }

            if (sourceData.PointerRotation.IsAvailable || sourceData.GripRotation.IsAvailable)
            {
                sourceData.RotationUpdated = !(sourceData.PointerRotation.CurrentReading.Equals(newPointerRotation) && sourceData.GripRotation.CurrentReading.Equals(newGripRotation));
            }
            sourceData.PointerRotation.CurrentReading = newPointerRotation;
            sourceData.GripRotation.CurrentReading = newGripRotation;

            Vector3 pointerForward = Vector3.zero;
            sourceData.PointingRay.IsSupported =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.source.supportsPointing;
#else
                false;
#endif
            sourceData.PointingRay.IsAvailable =
#if UNITY_2017_2_OR_NEWER
                sourceData.PointerPosition.IsAvailable && interactionSourceState.sourcePose.TryGetForward(out pointerForward, InteractionSourceNode.Pointer);
#else
                false;
#endif

            if (CameraCache.Main.transform.parent != null)
            {
                pointerForward = CameraCache.Main.transform.parent.TransformDirection(pointerForward);
            }

            sourceData.PointingRay.CurrentReading = new Ray(sourceData.PointerPosition.CurrentReading, pointerForward);

            sourceData.Thumbstick.IsSupported =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.source.supportsThumbstick;
#else
                false;
#endif
            sourceData.Thumbstick.IsAvailable = sourceData.Thumbstick.IsSupported;
            if (sourceData.Thumbstick.IsAvailable)
            {
                AxisButton2D newThumbstick = AxisButton2D.GetThumbstick(interactionSourceState);
                sourceData.ThumbstickPositionUpdated = sourceData.Thumbstick.CurrentReading.Position != newThumbstick.Position;
                sourceData.Thumbstick.CurrentReading = newThumbstick;
            }
            else
            {
                sourceData.Thumbstick.CurrentReading = default(AxisButton2D);
            }

            sourceData.Touchpad.IsSupported =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.source.supportsTouchpad;
#else
                false;
#endif
            sourceData.Touchpad.IsAvailable = sourceData.Touchpad.IsSupported;
            if (sourceData.Touchpad.IsAvailable)
            {
                TouchpadData newTouchpad = TouchpadData.GetTouchpad(interactionSourceState);
                sourceData.TouchpadPositionUpdated = !sourceData.Touchpad.CurrentReading.AxisButton.Position.Equals(newTouchpad.AxisButton.Position);
                sourceData.TouchpadTouchedUpdated = !sourceData.Touchpad.CurrentReading.Touched.Equals(newTouchpad.Touched);
                sourceData.Touchpad.CurrentReading = newTouchpad;
            }
            else
            {
                sourceData.Touchpad.CurrentReading = default(TouchpadData);
            }

            sourceData.Select.IsSupported = true; // All input mechanisms support "select".
            sourceData.Select.IsAvailable = sourceData.Select.IsSupported;
            AxisButton1D newSelect = AxisButton1D.GetSelect(interactionSourceState);
            sourceData.SelectPressedAmountUpdated = !sourceData.Select.CurrentReading.PressedAmount.Equals(newSelect.PressedAmount);
            sourceData.Select.CurrentReading = newSelect;

            sourceData.Grasp.IsSupported =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.source.supportsGrasp;
#else
                false;
#endif
            sourceData.Grasp.IsAvailable = sourceData.Grasp.IsSupported;
            sourceData.Grasp.CurrentReading =
#if UNITY_2017_2_OR_NEWER
                (sourceData.Grasp.IsAvailable && interactionSourceState.grasped);
#else
                false;
#endif

            sourceData.Menu.IsSupported =
#if UNITY_2017_2_OR_NEWER
                interactionSourceState.source.supportsMenu;
#else
                false;
#endif
            sourceData.Menu.IsAvailable = sourceData.Menu.IsSupported;
            sourceData.Menu.CurrentReading =
#if UNITY_2017_2_OR_NEWER
                (sourceData.Menu.IsAvailable && interactionSourceState.menuPressed);
#else
                false;
#endif
        }
#if UNITY_2017_2_OR_NEWER
        #region InteractionManager Events

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            SourceData sourceData = GetOrAddSourceData(args.state.source);

            sourceData.ResetUpdatedBooleans();

            UpdateSourceData(args.state, sourceData);

            if (sourceData.PositionUpdated)
            {
                InputManager.Instance.RaiseSourcePositionChanged(this, sourceData.SourceId, sourceData.PointerPosition.CurrentReading, sourceData.GripPosition.CurrentReading);
            }

            if (sourceData.RotationUpdated)
            {
                InputManager.Instance.RaiseSourceRotationChanged(this, sourceData.SourceId, sourceData.PointerRotation.CurrentReading, sourceData.GripRotation.CurrentReading);
            }

            if (sourceData.ThumbstickPositionUpdated)
            {
                InputManager.Instance.RaiseInputPositionChanged(this, sourceData.SourceId, InteractionSourcePressInfo.Thumbstick, sourceData.Thumbstick.CurrentReading.Position);
            }

            if (sourceData.TouchpadPositionUpdated)
            {
                InputManager.Instance.RaiseInputPositionChanged(this, sourceData.SourceId, InteractionSourcePressInfo.Touchpad, sourceData.Touchpad.CurrentReading.AxisButton.Position);
            }

            if (sourceData.TouchpadTouchedUpdated)
            {
                if (sourceData.Touchpad.CurrentReading.Touched)
                {
                    InputManager.Instance.RaiseTouchpadTouched(this, sourceData.SourceId);
                }
                else
                {
                    InputManager.Instance.RaiseTouchpadReleased(this, sourceData.SourceId);
                }
            }

            if (sourceData.SelectPressedAmountUpdated)
            {
                InputManager.Instance.RaiseSelectPressedAmountChanged(this, sourceData.SourceId, sourceData.Select.CurrentReading.PressedAmount);
            }
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            var pressType = (InteractionSourcePressInfo)args.pressType;
            // HACK: If we're not dealing with a spatial controller we may not get Select called properly
            if (args.state.source.kind != InteractionSourceKind.Controller || !args.state.source.supportsPointing)
            {
                pressType = InteractionSourcePressInfo.Select;
            }

            InputManager.Instance.RaiseSourceUp(this, args.state.source.id, pressType);
        }

        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            var pressType = (InteractionSourcePressInfo)args.pressType;
            // HACK: If we're not dealing with a spatial controller we may not get Select called properly
            if (args.state.source.kind != InteractionSourceKind.Controller || !args.state.source.supportsPointing)
            {
                pressType = InteractionSourcePressInfo.Select;
            }

            InputManager.Instance.RaiseSourceDown(this, args.state.source.id, pressType);
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            InputManager.Instance.RaiseSourceLost(this, args.state.source.id);

            // NOTE: We don't care whether the source ID previously existed or not, so we blindly call Remove:
            sourceIdToData.Remove(args.state.source.id);
        }

        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            UpdateSourceData(args.state, GetOrAddSourceData(args.state.source));

            InputManager.Instance.RaiseSourceDetected(this, args.state.source.id);
        }

        #endregion InteractionManager Events

        #region Raise GestureRecognizer Events

        // TODO: robertes: Should these also cause source state data to be stored/updated? What about SourceDetected synthesized events?

        protected void GestureRecognizer_Tapped(TappedEventArgs args)
        {
            InputManager.Instance.RaiseInputClicked(this, args.source.id, InteractionSourcePressInfo.Select, args.tapCount);
        }

        protected void GestureRecognizer_HoldStarted(HoldStartedEventArgs args)
        {
            InputManager.Instance.RaiseHoldStarted(this, args.source.id);
        }

        protected void GestureRecognizer_HoldCanceled(HoldCanceledEventArgs args)
        {
            InputManager.Instance.RaiseHoldCanceled(this, args.source.id);
        }

        protected void GestureRecognizer_HoldCompleted(HoldCompletedEventArgs args)
        {
            InputManager.Instance.RaiseHoldCompleted(this, args.source.id);
        }

        protected void GestureRecognizer_ManipulationStarted(ManipulationStartedEventArgs args)
        {
            InputManager.Instance.RaiseManipulationStarted(this, args.source.id);
        }

        protected void GestureRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs args)
        {
            InputManager.Instance.RaiseManipulationUpdated(this, args.source.id, args.cumulativeDelta);
        }

        protected void GestureRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            InputManager.Instance.RaiseManipulationCompleted(this, args.source.id, args.cumulativeDelta);
        }

        protected void GestureRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs args)
        {
            InputManager.Instance.RaiseManipulationCanceled(this, args.source.id);
        }

        protected void NavigationGestureRecognizer_NavigationStarted(NavigationStartedEventArgs args)
        {
            InputManager.Instance.RaiseNavigationStarted(this, args.source.id);
        }

        protected void NavigationGestureRecognizer_NavigationUpdated(NavigationUpdatedEventArgs args)
        {
            InputManager.Instance.RaiseNavigationUpdated(this, args.source.id, args.normalizedOffset);
        }

        protected void NavigationGestureRecognizer_NavigationCompleted(NavigationCompletedEventArgs args)
        {
            InputManager.Instance.RaiseNavigationCompleted(this, args.source.id, args.normalizedOffset);
        }

        protected void NavigationGestureRecognizer_NavigationCanceled(NavigationCanceledEventArgs args)
        {
            InputManager.Instance.RaiseNavigationCanceled(this, args.source.id);
        }

        #endregion //Raise GestureRecognizer Events
#else

        #region InteractionManager Events

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceState state)
        {
            SourceData sourceData = GetOrAddSourceData(state.source);

            sourceData.ResetUpdatedBooleans();

            UpdateSourceData(state, sourceData);

            if (sourceData.PositionUpdated)
            {
                InputManager.Instance.RaiseSourcePositionChanged(this, sourceData.SourceId, sourceData.PointerPosition.CurrentReading, sourceData.GripPosition.CurrentReading);
            }

            if (sourceData.RotationUpdated)
            {
                InputManager.Instance.RaiseSourceRotationChanged(this, sourceData.SourceId, sourceData.PointerRotation.CurrentReading, sourceData.GripRotation.CurrentReading);
            }

            if (sourceData.ThumbstickPositionUpdated)
            {
                InputManager.Instance.RaiseInputPositionChanged(this, sourceData.SourceId, InteractionSourcePressInfo.Thumbstick, sourceData.Thumbstick.CurrentReading.Position);
            }

            if (sourceData.TouchpadPositionUpdated)
            {
                InputManager.Instance.RaiseInputPositionChanged(this, sourceData.SourceId, InteractionSourcePressInfo.Touchpad, sourceData.Touchpad.CurrentReading.AxisButton.Position);
            }

            if (sourceData.TouchpadTouchedUpdated)
            {
                if (sourceData.Touchpad.CurrentReading.Touched)
                {
                    InputManager.Instance.RaiseTouchpadTouched(this, sourceData.SourceId);
                }
                else
                {
                    InputManager.Instance.RaiseTouchpadReleased(this, sourceData.SourceId);
                }
            }

            if (sourceData.SelectPressedAmountUpdated)
            {
                InputManager.Instance.RaiseSelectPressedAmountChanged(this, sourceData.SourceId, sourceData.Select.CurrentReading.PressedAmount);
            }
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceState state)
        {
            InputManager.Instance.RaiseSourceUp(this, state.source.id, InteractionSourcePressInfo.Select);
        }

        private void InteractionManager_InteractionSourcePressed(InteractionSourceState state)
        {
            InputManager.Instance.RaiseSourceDown(this, state.source.id, InteractionSourcePressInfo.Select);
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceState state)
        {
            // NOTE: We don't care whether the source ID previously existed or not, so we blindly call Remove:
            sourceIdToData.Remove(state.source.id);

            InputManager.Instance.RaiseSourceLost(this, state.source.id);
        }

        private void InteractionManager_InteractionSourceDetected(InteractionSourceState state)
        {
            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            UpdateSourceData(state, GetOrAddSourceData(state.source));

            InputManager.Instance.RaiseSourceDetected(this, state.source.id);
        }

        #endregion InteractionManager Events

        #region Raise GestureRecognizer Events

        protected void GestureRecognizer_Tapped(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            InputManager.Instance.RaiseInputClicked(this, 0, InteractionSourcePressInfo.Select, tapCount);
        }

        protected void GestureRecognizer_HoldStarted(InteractionSourceKind source, Ray headray)
        {
            InputManager.Instance.RaiseHoldStarted(this, 0);
        }

        protected void GestureRecognizer_HoldCanceled(InteractionSourceKind source, Ray headray)
        {
            InputManager.Instance.RaiseHoldCanceled(this, 0);
        }

        protected void GestureRecognizer_HoldCompleted(InteractionSourceKind source, Ray headray)
        {
            InputManager.Instance.RaiseHoldCompleted(this, 0);
        }

        protected void GestureRecognizer_ManipulationStarted(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            InputManager.Instance.RaiseManipulationStarted(this, 0);
        }

        protected void GestureRecognizer_ManipulationUpdated(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            InputManager.Instance.RaiseManipulationUpdated(this, 0, cumulativeDelta);
        }

        protected void GestureRecognizer_ManipulationCompleted(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            InputManager.Instance.RaiseManipulationCompleted(this, 0, cumulativeDelta);
        }

        protected void GestureRecognizer_ManipulationCanceled(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            InputManager.Instance.RaiseManipulationCanceled(this, 0);
        }

        protected void NavigationGestureRecognizer_NavigationStarted(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            InputManager.Instance.RaiseNavigationStarted(this, 0);
        }

        protected void NavigationGestureRecognizer_NavigationUpdated(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            InputManager.Instance.RaiseNavigationUpdated(this, 0, normalizedOffset);
        }

        protected void NavigationGestureRecognizer_NavigationCompleted(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            InputManager.Instance.RaiseNavigationCompleted(this, 0, normalizedOffset);
        }

        protected void NavigationGestureRecognizer_NavigationCanceled(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            InputManager.Instance.RaiseNavigationCanceled(this, 0);
        }

        #endregion //Raise GestureRecognizer Events
#endif
#endif

    }
}
