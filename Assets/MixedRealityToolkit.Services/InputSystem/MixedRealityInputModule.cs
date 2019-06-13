// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [RequireComponent(typeof(Camera))]
    public class MixedRealityInputModule : StandaloneInputModule, IMixedRealityPointerHandler, IMixedRealitySourceStateHandler
    {
        protected class PointerData
        {
            public IMixedRealityPointer pointer;

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

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
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

        public override void ActivateModule()
        {
            base.ActivateModule();

            if (InputSystem != null)
            {
                RaycastCamera = InputSystem.FocusProvider.UIRaycastCamera;

                foreach (IMixedRealityInputSource inputSource in InputSystem.DetectedInputSources)
                {
                    OnSourceDetected(inputSource);
                }

                InputSystem.Register(gameObject);
            }
        }

        public override void DeactivateModule()
        {
            if (InputSystem != null)
            {
                InputSystem.Unregister(gameObject);

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

        /// <summary>
        /// Process the active pointers from MixedRealityInputManager and all other Unity input.
        /// </summary>
        public override void Process()
        {
            CursorLockMode cursorLockStateBackup = Cursor.lockState;

            try
            {
                // Disable cursor lock for MRTK pointers.
                Cursor.lockState = CursorLockMode.None;

                // Process pointer events as mouse events.
                foreach(var p in pointerDataToUpdate)
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

        private void ProcessMrtkPointerLost(PointerData pointerData)
        {
            // Process a final mouse event in case the pointer is currently down.
            if (pointerData.lastMousePoint3d != null)
            {
                IMixedRealityPointer pointer = pointerData.pointer;

                pointer.Result = null;
                ProcessMouseEvent((int)pointer.PointerId);

                // Invalidate last mouse point.
                pointerData.lastMousePoint3d = null; 
            }
        }

        /// <summary>
        /// Adds MRTK pointer support as mouse input for Unity UI.
        /// </summary>
        protected override MouseState GetMousePointerEventData(int pointerId)
        {
            // Search for MRTK pointer with given id.
            // If found, generate mouse event data for pointer, otherwise call base implementation.
            PointerData pointerData;
            if (pointerDataToUpdate.TryGetValue(pointerId, out pointerData))
            {
                return GetMousePointerEventDataForMrtkPointer(pointerData);
            }

            return base.GetMousePointerEventData(pointerId);
        }

        protected MouseState GetMousePointerEventDataForMrtkPointer(PointerData pointerData)
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
            pointerData.eventDataLeft.pointerCurrentRaycast = (pointer.Result?.Details.Object != null) ? pointer.Result.Details.LastGraphicsRaycastResult : new RaycastResult();
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

            return pointerData.mouseState;
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
            Debug.Assert(pointerDataToUpdate.ContainsKey(pointerId));
            pointerDataToUpdate[pointerId].nextPressState = PointerEventData.FramePressState.Released;
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

        #endregion

        #region IMixedRealitySourceStateHandler

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            OnSourceDetected(eventData.InputSource);
        }

        void OnSourceDetected(IMixedRealityInputSource inputSource)
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

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            var inputSource = eventData.InputSource;

            for (int i = 0; i < inputSource.Pointers.Length; i++)
            {
                var pointer = inputSource.Pointers[i];
                if (pointer.InputSourceParent == inputSource)
                {
                    int pointerId = (int)pointer.PointerId;
                    Debug.Assert(pointerDataToUpdate.ContainsKey(pointerId));

                    PointerData pointerData = null;
                    if (pointerDataToUpdate.TryGetValue(pointerId, out pointerData))
                    {
                        Debug.Assert(!pointerDataToRemove.Contains(pointerData));
                        pointerDataToRemove.Add(pointerData);

                        pointerDataToUpdate.Remove(pointerId);
                    }
                }
            }
        }

        #endregion
    }
}