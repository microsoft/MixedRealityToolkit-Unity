// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [RequireComponent(typeof(Camera))]
    public class MixedRealityInputModule : PointerInputModule, IMixedRealityPointerHandler, IMixedRealitySourceStateHandler
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
                    foreach (var pointer in t.pointersToUpdate)
                    {
                        if (t.m_PointerData.ContainsKey((int)pointer.PointerId))
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

        protected readonly MouseState mouseState = new MouseState();

        /// <summary>
        /// last position of the pointer for the input in 3D
        /// </summary>
        protected readonly Dictionary<int, Vector3> lastMousePoint3D = new Dictionary<int, Vector3>();

        /// <summary>
        /// mapping from pointer id into click state
        /// </summary>
        protected readonly Dictionary<int, PointerEventData.FramePressState> nextClickState = new Dictionary<int, PointerEventData.FramePressState>();

        /// <summary>
        /// List of pointers that are active and should be updated every frame
        /// </summary>
        protected readonly List<IMixedRealityPointer> pointersToUpdate = new List<IMixedRealityPointer>();

        /// <summary>
        /// List of pointers that need one last frame of updates to remove
        /// </summary>
        protected readonly List<IMixedRealityPointer> pointersToRemove = new List<IMixedRealityPointer>();

        protected Camera RaycastCamera { get; private set; }

        public override void ActivateModule()
        {
            base.ActivateModule();

            RaycastCamera = MixedRealityToolkit.InputSystem.FocusProvider.UIRaycastCamera;

            MixedRealityToolkit.InputSystem.Register(gameObject);
        }

        public override void DeactivateModule()
        {
            base.DeactivateModule();

            MixedRealityToolkit.InputSystem?.Unregister(gameObject);
        }

        /// <summary>
        /// Process the active pointers from MixedRealityInputManager
        /// </summary>
        public override void Process()
        {
            SendUpdateEventToSelectedObject();

            // Update normal events
            for (int i = 0; i < pointersToUpdate.Count; i++)
            {
                if (pointersToUpdate[i].IsInteractionEnabled
                    && pointersToUpdate[i].Rays != null
                    && pointersToUpdate[i].Rays.Length > 0
                    && pointersToUpdate[i].SceneQueryType == Physics.SceneQueryType.SimpleRaycast)
                {
                    ProcessPointerEvent(pointersToUpdate[i]);
                }
                else
                {
                    // Remove Unity pointer if it exists, but leave the MRTK pointer in pointersToUpdate unless/until the pointer's source device is lost/disconnected.
                    RemoveUnityPointer(pointersToUpdate[i]);
                }
            }

            // Update removed pointers (from lost/disconnected controllers) one last time
            for (int i = 0; i < pointersToRemove.Count; i++)
            {
                RemoveUnityPointer(pointersToRemove[i]);
            }
            pointersToRemove.Clear();
        }

        private void RemoveUnityPointer(IMixedRealityPointer pointer)
        {
            PointerEventData data;
            GetPointerData((int)pointer.PointerId, out data, create: false);

            if (data != null)
            {
                pointer.Result = null;
                ProcessPointerEvent(pointer);

                RemovePointerData(data);
            }
        }

        /// <summary>
        /// Similar to ProcessMouseEvent in StandaloneInputModule 
        /// https://bitbucket.org/Unity-Technologies/ui/src/a3f89d5f7d145e4b6fa11cf9f2de768fea2c500f/UnityEngine.UI/EventSystem/InputModules/StandaloneInputModule.cs
        /// </summary>
        protected void ProcessPointerEvent(IMixedRealityPointer pointer)
        {
            int id = (int)pointer.PointerId;

            var mouseData = GetPointerEventData(pointer);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

        /// <summary>
        /// Override to always be active
        /// </summary>
        public override bool ShouldActivateModule()
        {
            return true;
        }

        /// <summary>
        /// Similar to GetMousePointerEventData from PointerInputModule.cs 
        /// https://bitbucket.org/Unity-Technologies/ui/src/a3f89d5f7d145e4b6fa11cf9f2de768fea2c500f/UnityEngine.UI/EventSystem/InputModules/PointerInputModule.cs
        /// This no longer performs a raycast, but simply compiles the pointer data, also takes in pointerData
        /// </summary>
        protected MouseState GetPointerEventData(IMixedRealityPointer pointerData)
        {
            int id = (int)pointerData.PointerId;

            // Reset the RaycastCamera for projecting (used in calculating deltas)
            RaycastCamera.transform.position = pointerData.Rays[0].Origin;
            RaycastCamera.transform.rotation = Quaternion.LookRotation(pointerData.Rays[0].Direction);

            // Populate  data
            PointerEventData leftData;
            var created = GetPointerData(id, out leftData, true);
            leftData.Reset();

            // Populate initial data or drag data
            Vector2 lastPosition;
            if (created)
            {
                // On create, center the position (used for last position)
                lastPosition = RaycastCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 1.0f));
            }
            else
            {
                // Otherwise, reproject the last pointer position
                lastPosition = RaycastCamera.WorldToScreenPoint(lastMousePoint3D[id]);
            }

            // Save off the 3D position of the cursor
            lastMousePoint3D[id] = RaycastCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f));

            // Calculate delta
            Vector2 newPos = RaycastCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 1.0f));
            leftData.delta = newPos - lastPosition;
            leftData.position = newPos;

            // Move the press position to allow dragging
            leftData.pressPosition += leftData.delta;

            // Populate raycast data
            leftData.pointerCurrentRaycast = (pointerData.Result?.Details.Object != null) ? pointerData.Result.Details.LastGraphicsRaycastResult : new RaycastResult();
            // TODO: Simulate raycast for 3D objects?

            // Populate the data for the buttons
            leftData.button = PointerEventData.InputButton.Left;
            mouseState.SetButtonState(PointerEventData.InputButton.Left, StateForPointer(id), leftData);

            return mouseState;
        }

        /// <summary>
        /// Similar to StateForMouseButton in PointerInputModule  
        /// https://bitbucket.org/Unity-Technologies/ui/src/a3f89d5f7d145e4b6fa11cf9f2de768fea2c500f/UnityEngine.UI/EventSystem/InputModules/PointerInputModule.cs
        /// </summary>
        protected PointerEventData.FramePressState StateForPointer(int id)
        {
            PointerEventData.FramePressState ret = PointerEventData.FramePressState.NotChanged;

            nextClickState.TryGetValue(id, out ret);
            nextClickState[id] = PointerEventData.FramePressState.NotChanged;

            return ret;
        }

        /// <summary>
        /// Override from PointerInputModule.cs 
        /// https://bitbucket.org/Unity-Technologies/ui/src/a3f89d5f7d145e4b6fa11cf9f2de768fea2c500f/UnityEngine.UI/EventSystem/InputModules/PointerInputModule.cs
        /// Modified to work with locked cursor
        /// </summary>
        protected override void ProcessMove(PointerEventData pointerEvent)
        {
            var targetGO = pointerEvent.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerEvent, targetGO);
        }

        /// <summary>
        /// Override from PointerInputModule.cs 
        /// https://bitbucket.org/Unity-Technologies/ui/src/a3f89d5f7d145e4b6fa11cf9f2de768fea2c500f/UnityEngine.UI/EventSystem/InputModules/PointerInputModule.cs
        /// Modified to work with locked cursor
        /// </summary>
        protected override void ProcessDrag(PointerEventData pointerEvent)
        {
            if (!pointerEvent.IsPointerMoving() || pointerEvent.pointerDrag == null)
            {
                return;
            }

            if (!pointerEvent.dragging
                && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                pointerEvent.dragging = true;
            }

            // Drag notification
            if (pointerEvent.dragging)
            {
                // Before doing drag we should cancel any pointer down state
                // And clear selection!
                if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;
                }
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
            }
        }

        /// <summary>
        /// Duplicate from PointerInputModule.cs since its private there
        /// https://bitbucket.org/Unity-Technologies/ui/src/a3f89d5f7d145e4b6fa11cf9f2de768fea2c500f/UnityEngine.UI/EventSystem/InputModules/PointerInputModule.cs
        /// Unmodified
        /// </summary>
        private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
        {
            if (!useDragThreshold)
                return true;

            return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
        }

        /// <summary>
        /// Duplicate from StandaloneInputModule.cs since we arent deriving from that
        /// https://bitbucket.org/Unity-Technologies/ui/src/a3f89d5f7d145e4b6fa11cf9f2de768fea2c500f/UnityEngine.UI/EventSystem/InputModules/StandaloneInputModule.cs
        /// Unmodified
        /// </summary>
        protected bool SendUpdateEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        /// <summary>
        /// Duplicate from StandaloneInputModule.cs since we arent deriving from that
        /// https://bitbucket.org/Unity-Technologies/ui/src/a3f89d5f7d145e4b6fa11cf9f2de768fea2c500f/UnityEngine.UI/EventSystem/InputModules/StandaloneInputModule.cs
        /// Unmodified
        /// </summary>
        protected void ProcessMousePress(MouseButtonEventData data)
        {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (data.PressedThisFrame())
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if (data.ReleasedThisFrame())
            {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if (currentOverGo != pointerEvent.pointerEnter)
                {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }

        #region IMixedRealityPointerHandler

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            nextClickState[(int)eventData.Pointer.PointerId] = PointerEventData.FramePressState.Released;
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            nextClickState[(int)eventData.Pointer.PointerId] = PointerEventData.FramePressState.Pressed;
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
                    Debug.Assert(!pointersToUpdate.Contains(pointer));
                    pointersToUpdate.Add(pointer);
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
                    Debug.Assert(pointersToUpdate.Contains(pointer));
                    pointersToUpdate.Remove(pointer);

                    Debug.Assert(!pointersToRemove.Contains(pointer));
                    pointersToRemove.Add(pointer);
                }
            }
        }

        #endregion

    }
}