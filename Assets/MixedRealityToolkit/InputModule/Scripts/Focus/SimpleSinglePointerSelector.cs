// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.Cursor;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Gaze;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Focus
{
    /// <summary>
    /// Script shows how to create your own 'point and commit' style pointer which can steal cursor focus
    /// using a pointing ray supported motion controller.
    /// This class uses the InputSourcePointer to define the rules of stealing focus when a pointing ray is detected
    /// with a motion controller that supports pointing.
    /// </summary>
    public class SimpleSinglePointerSelector : MonoBehaviour, ISourceStateHandler, IInputHandler
    {
        #region Settings

        [Tooltip("The stabilizer, if any, used to smooth out controller ray data.")]
        public BaseRayStabilizer ControllerPointerStabilizer;

        [Tooltip("The cursor, if any, which should follow the selected pointer.")]
        public BaseCursor Cursor;

        [Tooltip("True to search for a cursor if one isn't explicitly set.")]
        public bool SearchForCursorIfUnset = true;

        #endregion

        #region Data

        private bool started;
        private bool pointerWasChanged;

        private bool addedInputManagerListener;
        private IPointingSource currentPointer;

        private readonly InputSourcePointer inputSourcePointer = new InputSourcePointer();

        #endregion

        #region MonoBehaviour Implementation

        private void Start()
        {
            started = true;

            InputManager.AssertIsInitialized();
            FocusManager.AssertIsInitialized();
            GazeManager.AssertIsInitialized();

            AddInputManagerListenerIfNeeded();
            FindCursorIfNeeded();
            ConnectBestAvailablePointer();
        }

        private void OnEnable()
        {
            if (started)
            {
                AddInputManagerListenerIfNeeded();
            }
        }

        private void OnDisable()
        {
            RemoveInputManagerListenerIfNeeded();
        }

        #endregion

        #region Input Event Handlers

        void ISourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            // Nothing to do on source detected.
        }

        void ISourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (IsInputSourcePointerActive && inputSourcePointer.InputIsFromSource(eventData))
            {
                ConnectBestAvailablePointer();
            }
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            // Let the input fall to the next interactable object.
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            HandleInputAction(eventData);
        }

        #endregion

        #region Utilities

        private void AddInputManagerListenerIfNeeded()
        {
            if (!addedInputManagerListener)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
                addedInputManagerListener = true;
            }
        }

        private void RemoveInputManagerListenerIfNeeded()
        {
            if (addedInputManagerListener)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
                addedInputManagerListener = false;
            }
        }

        private void FindCursorIfNeeded()
        {
            if ((Cursor == null) && SearchForCursorIfUnset)
            {
                Debug.LogWarningFormat(
                    this,
                    "Cursor hasn't been explicitly set on \"{0}.{1}\". We'll search for a cursor in the hierarchy, but"
                        + " that comes with a performance cost, so it would be best if you explicitly set the cursor.",
                    name,
                    GetType().Name
                    );

                BaseCursor[] foundCursors = FindObjectsOfType<BaseCursor>();

                if ((foundCursors == null) || (foundCursors.Length == 0))
                {
                    Debug.LogErrorFormat(this, "Couldn't find cursor for \"{0}.{1}\".", name, GetType().Name);
                }
                else if (foundCursors.Length > 1)
                {
                    Debug.LogErrorFormat(
                        this,
                        "Found more than one ({0}) cursors for \"{1}.{2}\", so couldn't automatically set one.",
                        foundCursors.Length,
                        name,
                        GetType().Name
                        );
                }
                else
                {
                    Cursor = foundCursors[0];
                }
            }
        }

        private void SetPointer(IPointingSource newPointer)
        {
            if (currentPointer != newPointer)
            {
                if (currentPointer != null)
                {
                    FocusManager.Instance.UnregisterPointer(currentPointer);
                }

                currentPointer = newPointer;

                if (newPointer != null)
                {
                    FocusManager.Instance.RegisterPointer(newPointer);
                }

                if (Cursor != null)
                {
                    Cursor.Pointer = newPointer;
                }
            }

            Debug.Assert(currentPointer != null, "No Pointer Set!");
        }

        private void ConnectBestAvailablePointer()
        {
            IPointingSource bestPointer = null;
            var inputSources = InputManager.Instance.DetectedInputSources;

            for (var i = 0; i < inputSources.Count; i++)
            {
                if (SupportsPointingRay(inputSources[i]))
                {
                    AttachInputSourcePointer(inputSources[i]);
                    bestPointer = inputSourcePointer;
                    break;
                }
            }

            if (bestPointer == null)
            {
                bestPointer = GazeManager.Instance;
            }

            SetPointer(bestPointer);
        }

        private void HandleInputAction(InputEventData eventData)
        {
            // TODO: robertes: Investigate how this feels. Since "Down" will often be followed by "Click", is
            //       marking the event as used actually effective in preventing unintended app input during a
            //       pointer change?

            if (SupportsPointingRay(eventData))
            {
                if (IsInputSourcePointerActive && inputSourcePointer.InputIsFromSource(eventData))
                {
                    pointerWasChanged = false;
                }
                else
                {
                    AttachInputSourcePointer(eventData);
                    SetPointer(inputSourcePointer);
                    pointerWasChanged = true;
                }
            }
            else
            {
                if (IsGazePointerActive)
                {
                    pointerWasChanged = false;
                }
                else
                {
                    // TODO: robertes: see if we can treat voice separately from the other simple committers,
                    //       so voice doesn't steal from a pointing controller. I think input Kind would need
                    //       to come through with the event data.
                    SetPointer(GazeManager.Instance);
                    pointerWasChanged = true;
                }
            }

            if (pointerWasChanged)
            {
                // Since this input resulted in a pointer change, we mark the event as used to
                // prevent it from falling through to other handlers to prevent potentially
                // unintended input from reaching handlers that aren't being pointed at by
                // the new pointer.
                eventData.Use();
            }
        }

        private bool SupportsPointingRay(BaseInputEventData eventData)
        {
            return SupportsPointingRay(eventData.InputSource, eventData.SourceId);
        }

        private bool SupportsPointingRay(InputSourceInfo source)
        {
            return SupportsPointingRay(source.InputSource, source.SourceId);
        }

        private bool SupportsPointingRay(IInputSource inputSource, uint sourceId)
        {
            return inputSource.SupportsInputInfo(sourceId, SupportedInputInfo.Pointing);
        }

        private void AttachInputSourcePointer(BaseInputEventData eventData)
        {
            AttachInputSourcePointer(eventData.InputSource, eventData.SourceId);
        }

        private void AttachInputSourcePointer(InputSourceInfo source)
        {
            AttachInputSourcePointer(source.InputSource, source.SourceId);
        }

        private void AttachInputSourcePointer(IInputSource inputSource, uint sourceId)
        {
            inputSourcePointer.InputSource = inputSource;
            inputSourcePointer.InputSourceId = sourceId;
            inputSourcePointer.RayStabilizer = ControllerPointerStabilizer;
            inputSourcePointer.OwnAllInput = false;
            inputSourcePointer.ExtentOverride = null;
            inputSourcePointer.PrioritizedLayerMasksOverride = null;
        }

        private bool IsInputSourcePointerActive
        {
            get { return (currentPointer == inputSourcePointer); }
        }

        private bool IsGazePointerActive
        {
            get { return ReferenceEquals(currentPointer, GazeManager.Instance); }
        }

        #endregion
    }
}
