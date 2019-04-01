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
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(MixedRealityInputModule))]
        public class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                MixedRealityInputModule t = (MixedRealityInputModule)target;

                base.OnInspectorGUI();

                if (Application.isPlaying && t.RaycastCamera != null)
                {
                    foreach (var pointerData in t.pointerDataToUpdate)
                    {
                        IMixedRealityPointer pointer = pointerData.Value.pointer;
                        if (pointer.Rays != null && pointer.Rays.Length > 0)
                        {
                            t.RaycastCamera.transform.position = pointer.Rays[0].Origin;
                            t.RaycastCamera.transform.rotation = Quaternion.LookRotation(pointer.Rays[0].Direction, Vector3.up);

                            t.RaycastCamera.Render();

                            GUILayout.Label(pointer.PointerName);
                            GUILayout.Label(pointer.ToString());
                            GUILayout.Label(pointer.PointerId.ToString());
                            GUILayout.Label(t.RaycastCamera.targetTexture);
                        }
                    }
                }
            }

            public override bool RequiresConstantRepaint()
            {
                return true;
            }
        }
#endif

        // TODO: Make sure Unity UI input capturing participates in MRTK focus locking correctly.
        // TODO: Consider simulating touch events rather than mouse events for PokePointer. What are the tradeoffs?

        protected class PointerData
        {
            public IMixedRealityPointer pointer;

            public Vector3? lastMousePoint3d = null; // Last position of the pointer for the input in 3D.
            public PointerEventData.FramePressState nextClickState = PointerEventData.FramePressState.NotChanged;

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

        protected Camera RaycastCamera { get; private set; }

        public override void ActivateModule()
        {
            base.ActivateModule();

            RaycastCamera = MixedRealityToolkit.InputSystem?.FocusProvider.UIRaycastCamera;

            MixedRealityToolkit.InputSystem?.Register(gameObject);
        }

        public override void DeactivateModule()
        {
            MixedRealityToolkit.InputSystem?.Unregister(gameObject);

            RaycastCamera = null;

            base.DeactivateModule();
        }

        public override bool ShouldActivateModule()
        {
            return pointerDataToUpdate.Count > 0 || pointerDataToRemove.Count > 0 || base.ShouldActivateModule();
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
                        // Remove Unity pointer if it exists, but leave the MRTK pointer in pointersData unless/until the pointer's source device is lost/disconnected.
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
            if (pointerData.lastMousePoint3d != null)
            {
                IMixedRealityPointer pointer = pointerData.pointer;

                pointer.Result = null;
                ProcessMouseEvent((int)pointer.PointerId);

                pointerData.lastMousePoint3d = null; // Mark last mouse position invalid.
            }
        }

        /// <summary>
        /// Adds MRTK pointer support as mouse input for Unity UI.
        /// </summary>
        protected override MouseState GetMousePointerEventData(int id)
        {
            // Search for MRTK pointer with given id.
            // If found, generate mouse event data for pointer, otherwise call base implementation.
            PointerData pointerData;
            if (pointerDataToUpdate.TryGetValue(id, out pointerData))
            {
                return GetMousePointerEventDataForMrtkPointer(pointerData);
            }

            return base.GetMousePointerEventData(id);
        }

        protected MouseState GetMousePointerEventDataForMrtkPointer(PointerData pointerData)
        {
            IMixedRealityPointer pointer = pointerData.pointer;

            // Reset the RaycastCamera for projecting (used in calculating deltas)
            Debug.Assert(pointer.Rays != null && pointer.Rays.Length > 0);
            RaycastCamera.transform.position = pointer.Rays[0].Origin;
            RaycastCamera.transform.rotation = Quaternion.LookRotation(pointer.Rays[0].Direction);

            // Populate data
            pointerData.eventDataLeft.Reset();

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
            PointerEventData.FramePressState ret = pointerData.nextClickState;

            // Reset state
            pointerData.nextClickState = PointerEventData.FramePressState.NotChanged;

            return ret;
        }

        #region IMixedRealityPointerHandler

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            int id = (int)eventData.Pointer.PointerId;
            Debug.Assert(pointerDataToUpdate.ContainsKey(id));
            pointerDataToUpdate[id].nextClickState = PointerEventData.FramePressState.Released;
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            int id = (int)eventData.Pointer.PointerId;
            Debug.Assert(pointerDataToUpdate.ContainsKey(id));
            pointerDataToUpdate[id].nextClickState = PointerEventData.FramePressState.Pressed;
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }

        #endregion

        #region IMixedRealitySourceStateHandler

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            var inputSource = eventData.InputSource;
            for (int i = 0; i < inputSource.Pointers.Length; i++)
            {
                var pointer = inputSource.Pointers[i];
                if (pointer.InputSourceParent == inputSource)
                {
                    int id = (int)pointer.PointerId;
                    Debug.Assert(!pointerDataToUpdate.ContainsKey(id));
                    pointerDataToUpdate.Add(id, new PointerData(pointer, eventSystem));
                }
            }
        }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            var inputSource = eventData.InputSource;
            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                var pointer = inputSource.Pointers[i];
                if (pointer.InputSourceParent == inputSource)
                {
                    int id = (int)pointer.PointerId;
                    Debug.Assert(pointerDataToUpdate.ContainsKey(id));

                    PointerData pointerData = null;
                    if (pointerDataToUpdate.TryGetValue(id, out pointerData))
                    {
                        Debug.Assert(!pointerDataToRemove.Contains(pointerData));
                        pointerDataToRemove.Add(pointerData);

                        pointerDataToUpdate.Remove(id);
                    }
                }
            }
        }

        #endregion
    }
}