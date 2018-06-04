// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
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
        public Cursor Cursor;

        [Tooltip("If true, search for a cursor if one isn't explicitly set.")]
        [SerializeField]
        private bool searchForCursorIfUnset = true;
        public bool SearchForCursorIfUnset { get { return searchForCursorIfUnset; } set { searchForCursorIfUnset = value; } }

        [Tooltip("If true, always select the best pointer available (OS behavior does not auto-select).")]
        [SerializeField]
        private bool autoselectBestAvailable = false;
        public bool AutoselectBestAvailable { get { return autoselectBestAvailable; } set { autoselectBestAvailable = value; } }

        [Tooltip("The line pointer prefab to use, if any.")]
        [SerializeField]
        private GameObject linePointerPrefab = null;

        private PointerLine instantiatedPointerLine;

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
            // If a pointing controller just became available, set it as primary.
            if (autoselectBestAvailable && SupportsPointingRay(eventData))
            {
                ConnectBestAvailablePointer();
            }
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
            if ((Cursor == null) && searchForCursorIfUnset)
            {
                Debug.LogWarningFormat(
                    this,
                    "Cursor hasn't been explicitly set on \"{0}.{1}\". We'll search for a cursor in the hierarchy, but"
                        + " that comes with a performance cost, so it would be best if you explicitly set the cursor.",
                    name,
                    GetType().Name
                    );

                Cursor[] foundCursors = FindObjectsOfType<Cursor>();

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

            if (IsGazePointerActive)
            {
                DetachInputSourcePointer();
            }
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

            InteractionInputSource interactionInputSource = inputSource as InteractionInputSource;

            // If the InputSource is not an InteractionInputSource, we don't display any ray visualizations.
            if (interactionInputSource == null)
            {
                return;
            }

            // If no pointing ray prefab has been provided, we return early as there's nothing to display.
            if (linePointerPrefab == null)
            {
                return;
            }

            // If the pointer line hasn't already been instantiated, create it and store it here.
            if (instantiatedPointerLine == null)
            {
                instantiatedPointerLine = Instantiate(linePointerPrefab).GetComponent<PointerLine>();
            }

            inputSourcePointer.PointerRay = instantiatedPointerLine;

            Handedness handedness;
            if (interactionInputSource.TryGetHandedness(sourceId, out handedness))
            {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                // This updates the handedness of the pointer line, allowing for re-use if it was already in the scene.
                instantiatedPointerLine.ChangeHandedness((InteractionSourceHandedness)handedness);
#endif
            }
        }

        private void DetachInputSourcePointer()
        {
            if (instantiatedPointerLine != null)
            {
                Destroy(instantiatedPointerLine.gameObject);
            }
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
