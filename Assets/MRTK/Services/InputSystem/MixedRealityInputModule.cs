// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Scripts/MRTK/Services/MixedRealityInputModule")]
    public class MixedRealityInputModule : StandaloneInputModule, IMixedRealityPointerHandler, IMixedRealitySourceStateHandler
    {
        protected class PointerData
        {
            public readonly IMixedRealityPointer pointer;

            public Vector3? lastMousePoint3d = null; // Last position of the pointer for the input in 3D.
            public PointerEventData.FramePressState nextPressState = PointerEventData.FramePressState.NotChanged;

            public MouseState mouseState = new MouseState();
            public PointerEventData eventDataLeft;
            public PointerEventData eventDataMiddle; // Middle and right are placeholders to simulate mouse input.
            public PointerEventData eventDataRight;

            public PointerData(IMixedRealityPointer pointer, EventSystem eventSystem)
            {
                this.pointer = pointer;
                eventDataLeft = new PointerEventData(eventSystem);
                eventDataMiddle = new PointerEventData(eventSystem);
                eventDataRight = new PointerEventData(eventSystem);
            }
        }

        /// <summary>
        /// Mapping from pointer id to event data and click state
        /// </summary>
        protected readonly Dictionary<int, PointerData> pointerDataToUpdate = new Dictionary<int, PointerData>();

        /// <summary>
        /// List of pointers that need one last frame of updates to remove
        /// </summary>
        protected readonly List<PointerData> pointerDataToRemove = new List<PointerData>();

        public Camera RaycastCamera { get; private set; }

        /// <summary>
        /// Whether the input module is auto initialized by event system or requires a manual call to Initialize()
        /// </summary>
        public bool ManualInitializationRequired { get; private set; } = false;

        /// <summary>
        /// Whether the input module should pause processing temporarily
        /// </summary>
        public bool ProcessPaused { get; set; } = false;

        public IEnumerable<IMixedRealityPointer> ActiveMixedRealityPointers
        {
            get
            {
                foreach (var pointerDataEntry in pointerDataToUpdate)
                {
                    yield return pointerDataEntry.Value.pointer;
                }
            }
        }

        /// <inheritdoc />
        public override void ActivateModule()
        {
            base.ActivateModule();

            if (CoreServices.InputSystem != null)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Initialize the input module.
        /// </summary>
        public void Initialize()
        {
            RaycastCamera = CoreServices.InputSystem.FocusProvider.UIRaycastCamera;
            foreach (IMixedRealityInputSource inputSource in CoreServices.InputSystem.DetectedInputSources)
            {
                OnSourceDetected(inputSource);
            }
            CoreServices.InputSystem.RegisterHandler<IMixedRealityPointerHandler>(this);
            CoreServices.InputSystem.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            ManualInitializationRequired = false;
        }

        /// <summary>
        /// Suspend the input module when a runtime profile change is about to happen.
        /// </summary>
        public void Suspend()
        {
            // Process once more to handle pointer removals.
            Process();
            // Set the flag so that we manually initialize the input module after the profile switch.
            ManualInitializationRequired = true;
        }

        /// <inheritdoc />
        public override void DeactivateModule()
        {
            if (CoreServices.InputSystem != null)
            {
                CoreServices.InputSystem.UnregisterHandler<IMixedRealityPointerHandler>(this);
                CoreServices.InputSystem.UnregisterHandler<IMixedRealitySourceStateHandler>(this);

                foreach (var p in pointerDataToUpdate)
                {
                    pointerDataToRemove.Add(p.Value);
                }
                pointerDataToUpdate.Clear();

                // Process once more to handle pointer removals.
                Process();
            }

            RaycastCamera = null;

            base.DeactivateModule();
        }

        private static readonly ProfilerMarker ProcessPerfMarker = new ProfilerMarker("[MRTK] MixedRealityInputModule.Process");

        /// <summary>
        /// Process the active pointers from MixedRealityInputManager and all other Unity input.
        /// </summary>
        public override void Process()
        {
            using (ProcessPerfMarker.Auto())
            {
                // Do not process when we are waiting for initialization
                if (ManualInitializationRequired || ProcessPaused)
                {
                    return;
                }
                CursorLockMode cursorLockStateBackup = Cursor.lockState;

                try
                {
                    // Disable cursor lock for MRTK pointers.
                    Cursor.lockState = CursorLockMode.None;

                    // Process pointer events as mouse events.
                    foreach (var p in pointerDataToUpdate)
                    {
                        PointerData pointerData = p.Value;
                        IMixedRealityPointer pointer = pointerData.pointer;

                        if (pointer.IsInteractionEnabled
                            && pointer.Rays != null
                            && pointer.Rays.Length > 0
                            && pointer.SceneQueryType == Physics.SceneQueryType.SimpleRaycast)
                        {
                            ProcessMouseEvent((int)pointer.PointerId);
                        }
                        else
                        {
                            ProcessMrtkPointerLost(pointerData);
                        }
                    }

                    for (int i = 0; i < pointerDataToRemove.Count; i++)
                    {
                        ProcessMrtkPointerLost(pointerDataToRemove[i]);
                    }
                    pointerDataToRemove.Clear();
                }
                finally
                {
                    Cursor.lockState = cursorLockStateBackup;
                }

                base.Process();
            }
        }

        /// <inheritdoc />
        public override bool IsModuleSupported()
        {
            return true;
        }

        private static readonly ProfilerMarker ProcessMrtkPointerLostPerfMarker = new ProfilerMarker("[MRTK] MixedRealityInputModule.ProcessMrtkPointerLost");

        private void ProcessMrtkPointerLost(PointerData pointerData)
        {
            using (ProcessMrtkPointerLostPerfMarker.Auto())
            {
                // Process a final mouse event in case the pointer is currently down.
                if (pointerData.lastMousePoint3d != null)
                {
                    IMixedRealityPointer pointer = pointerData.pointer;

                    ProcessMouseEvent((int)pointer.PointerId);

                    ResetMousePointerEventData(pointerData);
                }
            }
        }

        private static readonly ProfilerMarker GetMousePointerEventDataPerfMarker = new ProfilerMarker("[MRTK] MixedRealityInputModule.GetMousePointerEventData");

        /// <summary>
        /// Adds MRTK pointer support as mouse input for Unity UI.
        /// </summary>
        protected override MouseState GetMousePointerEventData(int pointerId)
        {
            using (GetMousePointerEventDataPerfMarker.Auto())
            {
                // Search for MRTK pointer with given id.
                // If found, generate mouse event data for pointer, otherwise call base implementation.
                PointerData pointerData;
                if (pointerDataToUpdate.TryGetValue(pointerId, out pointerData))
                {
                    UpdateMousePointerEventData(pointerData);
                    return pointerData.mouseState;
                }

                return base.GetMousePointerEventData(pointerId);
            }
        }

        private static readonly ProfilerMarker UpdateMousePointerEventDataPerfMarker = new ProfilerMarker("[MRTK] MixedRealityInputModule.UpdateMousePointerEventData");

        protected void UpdateMousePointerEventData(PointerData pointerData)
        {
            using (UpdateMousePointerEventDataPerfMarker.Auto())
            {
                IMixedRealityPointer pointer = pointerData.pointer;

                // Reset the RaycastCamera for projecting (used in calculating deltas)
                Debug.Assert(pointer.Rays != null && pointer.Rays.Length > 0);

                if (pointer.Controller != null && pointer.Controller.IsRotationAvailable)
                {
                    RaycastCamera.transform.position = pointer.Rays[0].Origin;
                    RaycastCamera.transform.rotation = Quaternion.LookRotation(pointer.Rays[0].Direction);
                }
                else
                {
                    // The pointer.Controller does not provide rotation, for example on HoloLens 1 hands.
                    // In this case pointer.Rays[0].Origin will be the head position, but we want the 
                    // hand to do drag operations, not the head.
                    // pointer.Position gives the position of the hand, use that to compute drag deltas.
                    RaycastCamera.transform.position = pointer.Position;
                    RaycastCamera.transform.rotation = Quaternion.LookRotation(pointer.Rays[0].Direction);
                }

                // Populate eventDataLeft
                pointerData.eventDataLeft.Reset();

                // The RayCastCamera is placed so that the current cursor position is in the center of the camera's view space.
                Vector3 viewportPos = new Vector3(0.5f, 0.5f, 1.0f);
                Vector2 newPos = RaycastCamera.ViewportToScreenPoint(viewportPos);

                // Populate initial data or drag data
                Vector2 lastPosition;
                if (pointerData.lastMousePoint3d == null)
                {
                    // For the first event, use the same position for 'last' and 'new'.
                    lastPosition = newPos;
                }
                else
                {
                    // Otherwise, re-project the last pointer position.
                    lastPosition = RaycastCamera.WorldToScreenPoint(pointerData.lastMousePoint3d.Value);
                }

                // Save off the 3D position of the cursor.
                pointerData.lastMousePoint3d = RaycastCamera.ViewportToWorldPoint(viewportPos);

                // Calculate delta
                pointerData.eventDataLeft.delta = newPos - lastPosition;
                pointerData.eventDataLeft.position = newPos;

                // Move the press position to allow dragging
                pointerData.eventDataLeft.pressPosition += pointerData.eventDataLeft.delta;

                // Populate raycast data
                pointerData.eventDataLeft.pointerCurrentRaycast = pointer.Result != null ? pointer.Result.Details.LastGraphicsRaycastResult : new RaycastResult();
                // TODO: Simulate raycast for 3D objects?

                // Populate the data for the buttons
                pointerData.eventDataLeft.button = PointerEventData.InputButton.Left;
                pointerData.mouseState.SetButtonState(PointerEventData.InputButton.Left, StateForPointer(pointerData), pointerData.eventDataLeft);

                // Need to provide data for middle and right button for MouseState, although not used by MRTK pointers.
                CopyFromTo(pointerData.eventDataLeft, pointerData.eventDataRight);
                pointerData.eventDataRight.button = PointerEventData.InputButton.Right;
                pointerData.mouseState.SetButtonState(PointerEventData.InputButton.Right, PointerEventData.FramePressState.NotChanged, pointerData.eventDataRight);

                CopyFromTo(pointerData.eventDataLeft, pointerData.eventDataMiddle);
                pointerData.eventDataMiddle.button = PointerEventData.InputButton.Middle;
                pointerData.mouseState.SetButtonState(PointerEventData.InputButton.Middle, PointerEventData.FramePressState.NotChanged, pointerData.eventDataMiddle);
            }
        }

        private static readonly ProfilerMarker ResetMousePointerEventDataPerfMarker = new ProfilerMarker("[MRTK] MixedRealityInputModule.ResetMousePointerEventData");

        protected void ResetMousePointerEventData(PointerData pointerData)
        {
            using (ResetMousePointerEventDataPerfMarker.Auto())
            {
                // Invalidate last mouse point.
                pointerData.lastMousePoint3d = null;
                pointerData.pointer.Result = null;

                pointerData.eventDataLeft.pointerCurrentRaycast = new RaycastResult();

                // Populate the data for the buttons
                pointerData.eventDataLeft.button = PointerEventData.InputButton.Left;
                pointerData.mouseState.SetButtonState(PointerEventData.InputButton.Left, PointerEventData.FramePressState.NotChanged, pointerData.eventDataLeft);

                // Need to provide data for middle and right button for MouseState, although not used by MRTK pointers.
                CopyFromTo(pointerData.eventDataLeft, pointerData.eventDataRight);
                pointerData.eventDataRight.button = PointerEventData.InputButton.Right;
                pointerData.mouseState.SetButtonState(PointerEventData.InputButton.Right, PointerEventData.FramePressState.NotChanged, pointerData.eventDataRight);

                CopyFromTo(pointerData.eventDataLeft, pointerData.eventDataMiddle);
                pointerData.eventDataMiddle.button = PointerEventData.InputButton.Middle;
                pointerData.mouseState.SetButtonState(PointerEventData.InputButton.Middle, PointerEventData.FramePressState.NotChanged, pointerData.eventDataMiddle);
            }
        }

        protected PointerEventData.FramePressState StateForPointer(PointerData pointerData)
        {
            PointerEventData.FramePressState ret = pointerData.nextPressState;

            // Reset state
            pointerData.nextPressState = PointerEventData.FramePressState.NotChanged;

            return ret;
        }

        #region IMixedRealityPointerHandler

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            int pointerId = (int)eventData.Pointer.PointerId;

            // OnPointerUp can be raised during an OnSourceLost. If the pointer has already been removed
            // from the pointerDataToUpdate Dictionary and added to the pointerDataToRemove list, then
            // that pointer's state update can be ignored.
            Debug.Assert(pointerDataToUpdate.ContainsKey(pointerId) || IsPointerIdInRemovedList(pointerId));
            if (pointerDataToUpdate.TryGetValue(pointerId, out PointerData pointerData))
            {
                pointerData.nextPressState = PointerEventData.FramePressState.Released;
            }
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            int pointerId = (int)eventData.Pointer.PointerId;
            Debug.Assert(pointerDataToUpdate.ContainsKey(pointerId));
            pointerDataToUpdate[pointerId].nextPressState = PointerEventData.FramePressState.Pressed;
        }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }

        private bool IsPointerIdInRemovedList(int pointerId)
        {
            for (int i = 0; i < pointerDataToRemove.Count; i++)
            {
                if (pointerDataToRemove[i].pointer.PointerId == pointerId)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region IMixedRealitySourceStateHandler

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            OnSourceDetected(eventData.InputSource);
        }

        private static readonly ProfilerMarker OnSourceDetectedPerfMarker = new ProfilerMarker("[MRTK] MixedRealityInputModule.OnSourceDetected");

        void OnSourceDetected(IMixedRealityInputSource inputSource)
        {
            using (OnSourceDetectedPerfMarker.Auto())
            {
                for (int i = 0; i < inputSource.Pointers.Length; i++)
                {
                    var pointer = inputSource.Pointers[i];
                    if (pointer.InputSourceParent == inputSource)
                    {
                        // This !ContainsKey is only necessary due to inconsistent initialization of
                        // various input providers and this class's ActivateModule() call.
                        int pointerId = (int)pointer.PointerId;
                        if (!pointerDataToUpdate.ContainsKey(pointerId))
                        {
                            pointerDataToUpdate.Add(pointerId, new PointerData(pointer, eventSystem));
                        }
                    }
                }
            }
        }

        private static readonly ProfilerMarker OnSourceLostPerfMarker = new ProfilerMarker("[MRTK] MixedRealityInputModule.OnSourceLost");

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            using (OnSourceLostPerfMarker.Auto())
            {
                var inputSource = eventData.InputSource;

                for (int i = 0; i < inputSource.Pointers.Length; i++)
                {
                    var pointer = inputSource.Pointers[i];
                    if (pointer.InputSourceParent == inputSource)
                    {
                        int pointerId = (int)pointer.PointerId;
                        if (!pointerDataToUpdate.ContainsKey(pointerId))
                        {
                            // During runtime profile switch this may happen but we can ignore
                            if (!MixedRealityToolkit.Instance.IsProfileSwitching)
                            {
                                Debug.LogError("The pointer you are trying to remove does not exist in the mapping dict!");
                            }
                            return;
                        }

                        if (pointerDataToUpdate.TryGetValue(pointerId, out PointerData pointerData))
                        {
                            Debug.Assert(!pointerDataToRemove.Contains(pointerData));
                            pointerDataToRemove.Add(pointerData);

                            pointerDataToUpdate.Remove(pointerId);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
