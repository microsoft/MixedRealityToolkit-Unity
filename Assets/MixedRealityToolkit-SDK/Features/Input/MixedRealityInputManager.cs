// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Sources;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SDK.Input
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityInputSystem"/>
    /// </summary>
    public class MixedRealityInputManager : MixedRealityEventManager, IMixedRealityInputSystem
    {
        /// <inheritdoc />
        public event Action InputEnabled;

        /// <inheritdoc />
        public event Action InputDisabled;

        /// <inheritdoc />
        public HashSet<IMixedRealityInputSource> DetectedInputSources { get; } = new HashSet<IMixedRealityInputSource>();

        /// <inheritdoc />
        public IMixedRealityFocusProvider FocusProvider => focusProvider;
        private FocusProvider focusProvider;

        /// <inheritdoc />
        public IMixedRealityGazeProvider GazeProvider => gazeProvider;
        private GazeProvider gazeProvider;

        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();
        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        /// <inheritdoc />
        public bool IsInputEnabled => disabledRefCount <= 0;

        private int disabledRefCount;

        private SourceStateEventData sourceStateEventData;
        private SourcePoseEventData sourcePoseEventData;

        private FocusEventData focusEventData;

        private InputEventData inputEventData;
        private MixedRealityPointerEventData pointerEventData;

        private InputEventData<float> floatInputEventData;
        private InputEventData<Vector2> vector2InputEventData;
        private InputEventData<Vector3> positionInputEventData;
        private InputEventData<Quaternion> rotationInputEventData;
        private InputEventData<MixedRealityPose> poseInputEventData;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        private SpeechEventData speechEventData;
        private DictationEventData dictationEventData;

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        /// <summary>
        /// Current Speech Input Source.
        /// </summary>
        public SpeechInputSource SpeechInputSource { get; private set; }

        /// <summary>
        /// Current Dictation Input Source.
        /// </summary>
        public DictationInputSource DictationInputSource { get; private set; }

        /// <summary>
        /// Current Touch Screen Input Source.
        /// </summary>
        public TouchscreenInputSource TouchscreenInputSource { get; private set; }

        #region IMixedRealityManager Implementation

        /// <summary>
        /// Constructor
        /// </summary>
        public MixedRealityInputManager()
        {
            // Input system is critical, so should be processed before all other managers
            Priority = 1;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
            InputEnabled?.Invoke();
        }

        private void InitializeInternal()
        {
            if (CameraCache.Main.transform.parent == null)
            {
                var cameraParent = new GameObject("Body");
                CameraCache.Main.transform.SetParent(cameraParent.transform);
            }

            focusProvider = CameraCache.Main.gameObject.EnsureComponent<FocusProvider>();
            gazeProvider = CameraCache.Main.gameObject.EnsureComponent<GazeProvider>();

            bool addedComponents = false;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                var eventSystems = UnityEngine.Object.FindObjectsOfType<EventSystem>();
                var standaloneInputModules = UnityEngine.Object.FindObjectsOfType<StandaloneInputModule>();

                CameraCache.Main.transform.position = Vector3.zero;
                CameraCache.Main.transform.rotation = Quaternion.identity;

                if (eventSystems.Length == 0)
                {
                    focusProvider.UIRaycastCamera.gameObject.EnsureComponent<EventSystem>();
                    addedComponents = true;
                }
                else
                {
                    bool raiseWarning;

                    if (eventSystems.Length == 1)
                    {
                        raiseWarning = eventSystems[0].gameObject != FocusProvider.UIRaycastCamera.gameObject;
                    }
                    else
                    {
                        raiseWarning = true;
                    }

                    if (raiseWarning)
                    {
                        Debug.LogWarning("Found an existing event system in your scene. The Mixed Reality Input System requires only one, and must be found on the UIRaycastCamera.");
                    }
                }

                if (standaloneInputModules.Length == 0)
                {
                    focusProvider.UIRaycastCamera.gameObject.EnsureComponent<StandaloneInputModule>();
                    addedComponents = true;
                }
                else
                {
                    bool raiseWarning;

                    if (standaloneInputModules.Length == 1)
                    {
                        raiseWarning = standaloneInputModules[0].gameObject != FocusProvider.UIRaycastCamera.gameObject;
                    }
                    else
                    {
                        raiseWarning = true;
                    }

                    if (raiseWarning)
                    {
                        Debug.LogWarning("Found an existing Standalone Input Module in your scene. The Mixed Reality Input System requires only one, and must be found on the UIRaycastCamera.");
                    }
                }
            }

#endif // Unity Editor

            if (!addedComponents)
            {
                focusProvider.UIRaycastCamera.gameObject.EnsureComponent<EventSystem>();
                focusProvider.UIRaycastCamera.gameObject.EnsureComponent<StandaloneInputModule>();
            }

            sourceStateEventData = new SourceStateEventData(EventSystem.current);
            sourcePoseEventData = new SourcePoseEventData(EventSystem.current);

            focusEventData = new FocusEventData(EventSystem.current);

            inputEventData = new InputEventData(EventSystem.current);
            pointerEventData = new MixedRealityPointerEventData(EventSystem.current);

            floatInputEventData = new InputEventData<float>(EventSystem.current);
            vector2InputEventData = new InputEventData<Vector2>(EventSystem.current);
            positionInputEventData = new InputEventData<Vector3>(EventSystem.current);
            rotationInputEventData = new InputEventData<Quaternion>(EventSystem.current);
            poseInputEventData = new InputEventData<MixedRealityPose>(EventSystem.current);

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

            speechEventData = new SpeechEventData(EventSystem.current);
            dictationEventData = new DictationEventData(EventSystem.current);

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

            if (MixedRealityManager.Instance.ActiveProfile.IsSpeechCommandsEnabled)
            {
                SpeechInputSource = new SpeechInputSource(
                    MixedRealityManager.Instance.ActiveProfile.SpeechCommandsProfile.SpeechCommands
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
                    , (UnityEngine.Windows.Speech.ConfidenceLevel)MixedRealityManager.Instance.ActiveProfile.SpeechRecognitionConfidenceLevel
#endif
                );
            }

            if (MixedRealityManager.Instance.ActiveProfile.IsDictationEnabled)
            {
                DictationInputSource = new DictationInputSource();
            }

            if (MixedRealityManager.Instance.ActiveProfile.IsTouchScreenInputEnabled)
            {
                TouchscreenInputSource = new TouchscreenInputSource();
            }
        }

        /// <inheritdoc />
        public override void Reset()
        {
            InputDisabled?.Invoke();
            base.Reset();
            InitializeInternal();
            InputEnabled?.Invoke();
        }

        public override void Destroy()
        {
            InputDisabled?.Invoke();

            if (Application.isPlaying)
            {
                base.Destroy();
                return;
            }

            focusProvider = CameraCache.Main.gameObject.EnsureComponent<FocusProvider>();
            gazeProvider = CameraCache.Main.gameObject.EnsureComponent<GazeProvider>();

            focusProvider.enabled = false;

            if (FocusProvider.UIRaycastCamera != null)
            {
                if (Application.isEditor)
                {
                    UnityEngine.Object.DestroyImmediate(FocusProvider.UIRaycastCamera.gameObject);
                }
                else
                {
                    UnityEngine.Object.Destroy(FocusProvider.UIRaycastCamera.gameObject);
                }
            }

            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(focusProvider);
            }
            else
            {
                UnityEngine.Object.Destroy(focusProvider);
            }

            gazeProvider.enabled = false;

            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(gazeProvider);
            }
            else
            {
                UnityEngine.Object.Destroy(gazeProvider);
            }

            SpeechInputSource?.Dispose();
            DictationInputSource?.Dispose();
            TouchscreenInputSource?.Dispose();
            base.Destroy();
        }

        #endregion IMixedRealityManager Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            Debug.Assert(eventData != null);
            var baseInputEventData = ExecuteEvents.ValidateEventData<BaseInputEventData>(eventData);
            Debug.Assert(baseInputEventData != null);
            Debug.Assert(baseInputEventData.InputSource != null, $"Failed to find an input source for {baseInputEventData}");
            Debug.Assert(!baseInputEventData.used);

            // Send the event to global listeners
            base.HandleEvent(eventData, eventHandler);

            if (baseInputEventData.used)
            {
                // All global listeners get a chance to see the event, but if any of them marked it used, we stop
                // the event from going any further.
                return;
            }

            GameObject focusedObject = FocusProvider?.GetFocusedObject(baseInputEventData);

            // Handle modal input if one exists
            if (modalInputStack.Count > 0)
            {
                GameObject modalInput = modalInputStack.Peek();

                // If there is a focused object in the hierarchy of the modal handler, start the event bubble there
                if (focusedObject != null && modalInput != null && focusedObject.transform.IsChildOf(modalInput.transform))
                {
                    if (ExecuteEvents.ExecuteHierarchy(focusedObject, baseInputEventData, eventHandler) && baseInputEventData.used)
                    {
                        return;
                    }
                }
                // Otherwise, just invoke the event on the modal handler itself
                else
                {
                    if (ExecuteEvents.ExecuteHierarchy(modalInput, baseInputEventData, eventHandler) && baseInputEventData.used)
                    {
                        return;
                    }
                }
            }

            // If event was not handled by modal, pass it on to the current focused object
            if (focusedObject != null)
            {
                if (ExecuteEvents.ExecuteHierarchy(focusedObject, baseInputEventData, eventHandler) && baseInputEventData.used)
                {
                    return;
                }
            }

            // If event was not handled by the focused object, pass it on to any fallback handlers
            if (fallbackInputStack.Count > 0)
            {
                GameObject fallbackInput = fallbackInputStack.Peek();
                if (ExecuteEvents.ExecuteHierarchy(fallbackInput, baseInputEventData, eventHandler) && baseInputEventData.used)
                {
                    // return;
                }
            }
        }

        /// <summary>
        /// Register a <see cref="GameObject"/> to listen to events that will receive all input events, regardless
        /// of which other <see cref="GameObject"/>s might have handled the event beforehand.
        /// <remarks>Useful for listening to events when the <see cref="GameObject"/> is currently not being raycasted against by the <see cref="FocusProvider"/>.</remarks>
        /// </summary>
        /// <param name="listener">Listener to add.</param>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to input events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion IEventSystemManager Implementation

        #region Input Disabled Options

        /// <summary>
        /// Push a disabled input state onto the input manager.
        /// While input is disabled no events will be sent out and the cursor displays
        /// a waiting animation.
        /// </summary>
        public void PushInputDisable()
        {
            ++disabledRefCount;

            if (disabledRefCount == 1)
            {
                InputDisabled?.Invoke();
                focusProvider.enabled = false;
                gazeProvider.enabled = false;
            }
        }

        /// <summary>
        /// Pop disabled input state. When the last disabled state is 
        /// popped off the stack input will be re-enabled.
        /// </summary>
        public void PopInputDisable()
        {
            --disabledRefCount;
            Debug.Assert(disabledRefCount >= 0, "Tried to pop more input disable than the amount pushed.");

            if (disabledRefCount == 0)
            {
                InputEnabled?.Invoke();
                focusProvider.enabled = true;
                gazeProvider.enabled = true;
            }
        }

        /// <summary>
        /// Clear the input disable stack, which will immediately re-enable input.
        /// </summary>
        public void ClearInputDisableStack()
        {
            bool wasInputDisabled = disabledRefCount > 0;
            disabledRefCount = 0;

            if (wasInputDisabled)
            {
                InputEnabled?.Invoke();
                focusProvider.enabled = true;
                gazeProvider.enabled = true;
            }
        }

        #endregion Input Disabled Options

        #region Modal Input Options

        /// <summary>
        /// Push a game object into the modal input stack. Any input handlers
        /// on the game object are given priority to input events before any focused objects.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public void PushModalInputHandler(GameObject inputHandler)
        {
            modalInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the modal input stack.
        /// </summary>
        public void PopModalInputHandler()
        {
            if (modalInputStack.Count > 0)
            {
                modalInputStack.Pop();

            }
        }

        /// <summary>
        /// Clear all modal input handlers off the stack.
        /// </summary>
        public void ClearModalInputStack()
        {
            modalInputStack.Clear();
        }

        #endregion Modal Input Options

        #region Fallback Input Handler Options

        /// <summary>
        /// Push a game object into the fallback input stack. Any input handlers on
        /// the game object are given input events when no modal or focused objects consume the event.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public void PushFallbackInputHandler(GameObject inputHandler)
        {
            fallbackInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the fallback input stack.
        /// </summary>
        public void PopFallbackInputHandler()
        {
            fallbackInputStack.Pop();
        }

        /// <summary>
        /// Clear all fallback input handlers off the stack.
        /// </summary>
        public void ClearFallbackInputStack()
        {
            fallbackInputStack.Clear();
        }

        #endregion Fallback Input Handler Options

        #region Input Events

        #region Input Source Events

        /// <inheritdoc />
        public uint GenerateNewSourceId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var inputSource in DetectedInputSources)
            {
                if (inputSource.SourceId == newId)
                {
                    return GenerateNewSourceId();
                }
            }

            return newId;
        }

        /// <inheritdoc />
        public IMixedRealityInputSource RequestNewGenericInputSource(string name, IMixedRealityPointer[] pointers = null)
        {
            return new BaseGenericInputSource(name, pointers);
        }

        #region Input Source State Events

        /// <inheritdoc />
        public void RaiseSourceDetected(IMixedRealityInputSource source, IMixedRealityController controller = null)
        {
            // Create input event
            sourceStateEventData.Initialize(source, controller);

            Debug.Assert(!DetectedInputSources.Contains(source), $"{source.SourceName} has already been registered with the Input Manager!");

            DetectedInputSources.Add(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourceStateHandler> OnSourceDetectedEventHandler =
            delegate (IMixedRealitySourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        /// <inheritdoc />
        public void RaiseSourceLost(IMixedRealityInputSource source, IMixedRealityController controller = null)
        {
            // Create input event
            sourceStateEventData.Initialize(source, controller);

            Debug.Assert(DetectedInputSources.Contains(source), $"{source.SourceName} was never registered with the Input Manager!");

            DetectedInputSources.Remove(source);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceLostEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourceStateHandler> OnSourceLostEventHandler =
                delegate (IMixedRealitySourceStateHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                    handler.OnSourceLost(casted);
                };

        #endregion Input Source State Events

        #region Input Source Pose Events

        /// <inheritdoc />
        public void RaiseSourceTrackingStateChanged(IMixedRealityInputSource source, IMixedRealityController controller, TrackingState state)
        {
            // Create input event
            sourcePoseEventData.Initialize(source, controller, state);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePoseEventData, OnSourcePoseChangedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector2 position)
        {
            // Create input event
            sourcePoseEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePoseEventData, OnSourcePoseChangedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector3 position)
        {
            // Create input event
            sourcePoseEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePoseEventData, OnSourcePoseChangedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseSourceRotationChanged(IMixedRealityInputSource source, IMixedRealityController controller, Quaternion rotation)
        {
            // Create input event
            sourcePoseEventData.Initialize(source, controller, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePoseEventData, OnSourcePoseChangedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseSourcePoseChanged(IMixedRealityInputSource source, IMixedRealityController controller, MixedRealityPose position)
        {
            // Create input event
            sourcePoseEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePoseEventData, OnSourcePoseChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourcePoseHandler> OnSourcePoseChangedEventHandler =
                delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData>(eventData);
                    handler.OnSourcePoseChanged(casted);
                };

        #endregion Input Source Pose Events

        #endregion Input Source Events

        #region Focus Events

        /// <inheritdoc />
        public void RaisePreFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnPreFocusChangedHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnPreFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                try
                {
                    // When shutting down a game, we can sometime get old references to game objects that have been cleaned up.
                    // We'll ignore when this happens.
                    ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.GameObjectReference, focusEventData, OnPreFocusChangedHandler);
                }
                catch (Exception)
                {
                    // ignored.
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusChangedHandler> OnPreFocusChangedHandler =
                delegate (IMixedRealityFocusChangedHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnBeforeFocusChange(casted);
                };

        /// <inheritdoc />
        public void RaiseFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnFocusChangedHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                try
                {
                    // When shutting down a game, we can sometime get old references to game objects that have been cleaned up.
                    // We'll ignore when this happens.
                    ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.GameObjectReference, focusEventData, OnFocusChangedHandler);
                }
                catch (Exception)
                {
                    // ignored.
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusChangedHandler> OnFocusChangedHandler =
            delegate (IMixedRealityFocusChangedHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseFocusEnter(IMixedRealityPointer pointer, GameObject focusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(focusedObject, focusEventData, OnFocusEnterEventHandler);

            GraphicInputEventData graphicEventData;
            if (FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out graphicEventData))
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicEventData, ExecuteEvents.pointerEnterHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusHandler> OnFocusEnterEventHandler =
                delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnFocusEnter(casted);
                };

        /// <inheritdoc />
        public void RaiseFocusExit(IMixedRealityPointer pointer, GameObject unfocusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(unfocusedObject, focusEventData, OnFocusExitEventHandler);

            GraphicInputEventData graphicEventData;
            if (FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out graphicEventData))
            {
                ExecuteEvents.ExecuteHierarchy(unfocusedObject, graphicEventData, ExecuteEvents.pointerExitHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusHandler> OnFocusExitEventHandler =
                delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                    handler.OnFocusExit(casted);
                };

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerDownEventHandler =
            delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                handler.OnPointerDown(casted);
            };

        /// <inheritdoc />
        public void RaisePointerDown(IMixedRealityPointer pointer, MixedRealityInputAction inputAction)
        {
            // Create input event
            pointerEventData.Initialize(pointer.InputSourceParent, inputAction);

            ExecutePointerDown(HandlePointerDown(pointer));
        }

        /// <inheritdoc />
        public void RaisePointerDown(IMixedRealityPointer pointer, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            pointerEventData.Initialize(pointer.InputSourceParent, handedness, inputAction);

            ExecutePointerDown(HandlePointerDown(pointer));
        }

        private GraphicInputEventData HandlePointerDown(IMixedRealityPointer pointer)
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnPointerDownEventHandler);
            GraphicInputEventData graphicEventData;
            FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out graphicEventData);
            return graphicEventData;
        }

        private static void ExecutePointerDown(GraphicInputEventData graphicInputEventData)
        {
            if (graphicInputEventData != null && graphicInputEventData.selectedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(graphicInputEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerDownHandler);
            }
        }

        #endregion Pointer Down

        #region Pointer Click

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnInputClickedEventHandler =
                delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                    handler.OnPointerClicked(casted);
                };

        /// <inheritdoc />
        public void RaisePointerClicked(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, int count)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction, count);

            HandleClick();
        }

        /// <inheritdoc />
        public void RaisePointerClicked(IMixedRealityPointer pointer, Handedness handedness, MixedRealityInputAction inputAction, int count)
        {
            // Create input event
            pointerEventData.Initialize(pointer, handedness, inputAction, count);

            HandleClick();
        }

        private void HandleClick()
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnInputClickedEventHandler);

            // NOTE: In Unity UI, a "click" happens on every pointer up, so we have RaisePointerUp call the pointerClickHandler.
        }

        #endregion Pointer Click

        #region Pointer Up

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerUpEventHandler =
            delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                handler.OnPointerUp(casted);
            };

        /// <inheritdoc />
        public void RaisePointerUp(IMixedRealityPointer pointer, MixedRealityInputAction inputAction)
        {
            // Create input event
            pointerEventData.Initialize(pointer.InputSourceParent, inputAction);

            ExecutePointerUp(HandlePointerUp(pointer));
        }

        /// <inheritdoc />
        public void RaisePointerUp(IMixedRealityPointer pointer, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            pointerEventData.Initialize(pointer.InputSourceParent, handedness, inputAction);

            ExecutePointerUp(HandlePointerUp(pointer));
        }

        private static void ExecutePointerUp(GraphicInputEventData graphicInputEventData)
        {
            if (graphicInputEventData != null)
            {
                if (graphicInputEventData.selectedObject != null)
                {
                    ExecuteEvents.ExecuteHierarchy(graphicInputEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.ExecuteHierarchy(graphicInputEventData.selectedObject, graphicInputEventData, ExecuteEvents.pointerClickHandler);
                }

                graphicInputEventData.Clear();
            }
        }

        private GraphicInputEventData HandlePointerUp(IMixedRealityPointer pointer)
        {
            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnPointerUpEventHandler);

            GraphicInputEventData graphicEventData;
            FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out graphicEventData);
            return graphicEventData;
        }

        #endregion Pointer Up

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnInputDownEventHandler =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        #endregion Input Down

        #region Input Pressed

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnInputPressedEventHandler =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<float>>(eventData);
                handler.OnInputPressed(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            floatInputEventData.Initialize(source, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            floatInputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, MixedRealityInputAction inputAction, float pressAmount)
        {
            // Create input event
            floatInputEventData.Initialize(source, inputAction, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, OnInputPressedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, float pressAmount)
        {
            // Create input event
            floatInputEventData.Initialize(source, handedness, inputAction, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, OnInputPressedEventHandler);
        }

        #endregion Input Pressed

        #region Input Up

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnInputUpEventHandler =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        #endregion Input Up

        #region Input Position Changed

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnTwoDoFInputChanged =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnPositionInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector2 inputPosition)
        {
            // Create input event
            vector2InputEventData.Initialize(source, inputAction, inputPosition);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(vector2InputEventData, OnTwoDoFInputChanged);
        }

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector2 inputPosition)
        {
            // Create input event
            vector2InputEventData.Initialize(source, handedness, inputAction, inputPosition);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(vector2InputEventData, OnTwoDoFInputChanged);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialInputHandler> OnPositionInputChanged =
            delegate (IMixedRealitySpatialInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnPositionChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector3 position)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnPositionInputChanged);
        }

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector3 position)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnPositionInputChanged);
        }

        #endregion Input Position Changed

        #region Input Rotation Changed

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialInputHandler> OnRotationInputChanged =
                delegate (IMixedRealitySpatialInputHandler handler, BaseEventData eventData)
                {
                    var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                    handler.OnRotationChanged(casted);
                };

        /// <inheritdoc />
        public void RaiseRotationInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Quaternion rotation)
        {
            // Create input event
            rotationInputEventData.Initialize(source, inputAction, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnRotationInputChanged);
        }

        /// <inheritdoc />
        public void RaiseRotationInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Quaternion rotation)
        {
            // Create input event
            rotationInputEventData.Initialize(source, handedness, inputAction, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnRotationInputChanged);
        }

        #endregion Input Rotation Changed

        #region Input Pose Changed

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialInputHandler> OnPoseInputChanged =
            delegate (IMixedRealitySpatialInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<MixedRealityPose>>(eventData);
                handler.OnPoseInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePoseInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, MixedRealityPose inputData)
        {
            // Create input event
            poseInputEventData.Initialize(source, inputAction, inputData);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(poseInputEventData, OnPoseInputChanged);
        }

        /// <inheritdoc />
        public void RaisePoseInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, MixedRealityPose inputData)
        {
            // Create input event
            poseInputEventData.Initialize(source, handedness, inputAction, inputData);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(poseInputEventData, OnPoseInputChanged);
        }

        #endregion Input Pose Changed

        #endregion Generic Input Events

        #region Gestures

        #region Hold Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityHoldHandler> OnHoldStartedEventHandler =
            delegate (IMixedRealityHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseHoldStarted(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldStartedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseHoldStarted(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityHoldHandler> OnHoldCompletedEventHandler =
            delegate (IMixedRealityHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseHoldCompleted(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCompletedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseHoldCompleted(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityHoldHandler> OnHoldCanceledEventHandler =
            delegate (IMixedRealityHoldHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnHoldCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseHoldCanceled(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCanceledEventHandler);
        }

        /// <inheritdoc />
        public void RaiseHoldCanceled(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnHoldCanceledEventHandler);
        }

        #endregion Hold Events

        #region Navigation Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityNavigationHandler> OnNavigationStartedEventHandler =
            delegate (IMixedRealityNavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnNavigationStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseNavigationStarted(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnNavigationStartedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseNavigationStarted(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnNavigationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityNavigationHandler> OnNavigationUpdatedEventHandler =
            delegate (IMixedRealityNavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnNavigationUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseNavigationUpdated(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector3 normalizedOffset)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnNavigationUpdatedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseNavigationUpdated(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector3 normalizedOffset)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnNavigationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityNavigationHandler> OnNavigationCompletedEventHandler =
            delegate (IMixedRealityNavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnNavigationCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseNavigationCompleted(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector3 normalizedOffset)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnNavigationCompletedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseNavigationCompleted(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector3 normalizedOffset)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, normalizedOffset);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnNavigationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityNavigationHandler> OnNavigationCanceledEventHandler =
            delegate (IMixedRealityNavigationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnNavigationCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseNavigationCanceled(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnNavigationCanceledEventHandler);
        }

        /// <inheritdoc />
        public void RaiseNavigationCanceled(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnNavigationCanceledEventHandler);
        }

        #endregion Navigation Events

        #region Manipulation Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityManipulationHandler> OnManipulationStartedEventHandler =
            delegate (IMixedRealityManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnManipulationStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseManipulationStarted(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnManipulationStartedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseManipulationStarted(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnManipulationStartedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityManipulationHandler> OnManipulationUpdatedEventHandler =
            delegate (IMixedRealityManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnManipulationUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseManipulationUpdated(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector3 cumulativeDelta)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnManipulationUpdatedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseManipulationUpdated(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector3 cumulativeDelta)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnManipulationUpdatedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityManipulationHandler> OnManipulationCompletedEventHandler =
            delegate (IMixedRealityManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnManipulationCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseManipulationCompleted(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector3 cumulativeDelta)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnManipulationCompletedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseManipulationCompleted(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector3 cumulativeDelta)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, cumulativeDelta);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnManipulationCompletedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityManipulationHandler> OnManipulationCanceledEventHandler =
            delegate (IMixedRealityManipulationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnManipulationCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseManipulationCanceled(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            // Create input event
            positionInputEventData.Initialize(source, inputAction, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnManipulationCanceledEventHandler);
        }

        /// <inheritdoc />
        public void RaiseManipulationCanceled(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, Vector3.zero);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnManipulationCanceledEventHandler);
        }

        #endregion Manipulation Events

        #endregion Gestures

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        #region Speech Keyword Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (IMixedRealitySpeechHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        /// <inheritdoc />
        public void RaiseSpeechCommandRecognized(IMixedRealityInputSource source, MixedRealityInputAction inputAction, UnityEngine.Windows.Speech.ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, UnityEngine.Windows.Speech.SemanticMeaning[] semanticMeanings, string text)
        {
            // Create input event
            speechEventData.Initialize(source, inputAction, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(speechEventData, OnSpeechKeywordRecognizedEventHandler);
        }

        #endregion Speech Keyword Events

        #region Dictation Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationHypothesisEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationHypothesis(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationHypothesis(IMixedRealityInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationHypothesis, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationHypothesisEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationResultEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationResult(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationResult(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationResultEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationCompleteEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationComplete(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationComplete(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationCompleteEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationErrorEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationError(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationError(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationErrorEventHandler);
        }

        #endregion Dictation Events

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        #endregion Input Events
    }
}
