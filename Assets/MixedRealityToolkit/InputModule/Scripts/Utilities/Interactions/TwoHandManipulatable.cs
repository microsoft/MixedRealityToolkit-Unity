// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.UX.BoundingBoxes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


namespace MixedRealityToolkit.InputModule.Utilities.Interactions
{
    /// <summary>
    /// This script allows for an object to be movable, scalable, and rotatable with one or two hands. 
    /// You may also configure the script on only enable certain manipulations. The script works with 
    /// both HoloLens' gesture input and immersive headset's motion controller input.
    /// See Assets/MixedRealityToolkit-Examples/Input/Readme/README_TwoHandManipulationTest.md
    /// for instructions on how to use the script.
    /// </summary>
    public class TwoHandManipulatable : MonoBehaviour, IPointerHandler, ISourceStateHandler
    {
        /// <summary>
        /// Event that gets raised when user begins manipulating the object
        /// </summary>
        public event Action StartedManipulating;

        /// <summary>
        /// Event that gets raised when the user ends manipulation
        /// </summary>
        public event Action StoppedManipulating;

        private enum TwoHandedManipulation
        {
            Scale,
            Rotate,
            MoveScale,
            RotateScale,
            MoveRotateScale
        }

        [Flags]
        private enum State
        {
            Start = 0x000,
            Moving = 0x001,
            Scaling = 0x010,
            Rotating = 0x100,
            MovingScaling = 0x011,
            RotatingScaling = 0x110,
            MovingRotatingScaling = 0x111,
        }

        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform;

        [SerializeField]
        [Tooltip("To visualize the object bounding box, drop the MixedRealityToolkit/UX/Prefabs/BoundingBoxes/BoundingBoxBasic.prefab here. This is optional.")]
        private BoundingBox boundingBoxPrefab;

        [SerializeField]
        [Tooltip("What manipulation will two hands perform?")]
        private TwoHandedManipulation manipulationMode;

        [SerializeField]
        [Tooltip("Constrain rotation along an axis")]
        private TwoHandRotateLogic.RotationConstraint constraintOnRotation = TwoHandRotateLogic.RotationConstraint.None;

        [SerializeField]
        [Tooltip("If true, grabbing the object with one hand will initiate movement.")]
        private bool oneHandMovement = true;

        private BoundingBox boundingBoxInstance;
        private State currentState;
        private TwoHandMoveLogic mMoveLogic;
        private TwoHandScaleLogic mScaleLogic;
        private TwoHandRotateLogic mRotateLogic;

        /// <summary>
        /// Maps input id -> position of hand
        /// </summary>
        private readonly Dictionary<uint, Vector3> handsPressedLocationsMap = new Dictionary<uint, Vector3>();

        /// <summary>
        /// The input sources that are currently being pressed.
        /// </summary>
        private readonly HashSet<IInputSource> pressedInputSources = new HashSet<IInputSource>();

        private bool ShowBoundingBox
        {
            set
            {
                if (boundingBoxInstance == null && boundingBoxPrefab != null)
                {
                    // Instantiate Bounding Box from the Prefab
                    boundingBoxInstance = Instantiate(boundingBoxPrefab);
                }

                Debug.Assert(boundingBoxInstance != null);

                if (value)
                {
                    boundingBoxInstance.Target = gameObject;
                    boundingBoxInstance.gameObject.SetActive(true);
                }
                else
                {
                    boundingBoxInstance.Target = null;
                    boundingBoxInstance.gameObject.SetActive(false);
                }
            }
        }

        #region Unity APIs

        private void Awake()
        {
            mMoveLogic = new TwoHandMoveLogic();
            mRotateLogic = new TwoHandRotateLogic(constraintOnRotation);
            mScaleLogic = new TwoHandScaleLogic();
        }

        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = transform;
            }
        }

        private void Update()
        {
            foreach (IInputSource source in pressedInputSources)
            {
                Vector3 inputPosition;
                if (InteractionInputSources.Instance.TryGetGripPosition(source.SourceId, out inputPosition))
                {
                    handsPressedLocationsMap[source.SourceId] = inputPosition;
                }
            }

            if (currentState != State.Start)
            {
                UpdateStateMachine();
            }
        }

        #endregion Unity APIs

        #region SourceStateHandling

        public void OnSourceDetected(SourceStateEventData eventData) { }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.InputSource);
            UpdateStateMachine();
            eventData.Use();
        }

        public void OnSourcePositionChanged(SourcePositionEventData eventData) { }

        public void OnSourceRotationChanged(SourceRotationEventData eventData) { }

        #endregion SourceStateHandling

        #region PointerHandling

        public void OnPointerUp(ClickEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.InputSource);
            UpdateStateMachine();
            eventData.Use();
        }

        public void OnPointerDown(ClickEventData eventData)
        {
            // Add to hand map
            Vector3 result;
            InteractionInputSources.Instance.TryGetGripPosition(eventData.SourceId, out result);
            handsPressedLocationsMap[eventData.SourceId] = result;
            UpdateStateMachine();
            eventData.Use();
        }

        public void OnPointerClicked(ClickEventData eventData) { }

        #endregion PointerHandling

        private void UpdateStateMachine()
        {
            var handsPressedCount = handsPressedLocationsMap.Count;
            State newState = currentState;
            switch (currentState)
            {
                case State.Start:
                case State.Moving:
                    if (handsPressedCount == 0)
                    {
                        newState = State.Start;
                    }
                    else if (handsPressedCount == 1 && oneHandMovement)
                    {
                        newState = State.Moving;
                    }
                    else if (handsPressedCount > 1)
                    {
                        switch (manipulationMode)
                        {
                            case TwoHandedManipulation.Scale:
                                newState = State.Scaling;
                                break;
                            case TwoHandedManipulation.Rotate:
                                newState = State.Rotating;
                                break;
                            case TwoHandedManipulation.MoveScale:
                                newState = State.MovingScaling;
                                break;
                            case TwoHandedManipulation.RotateScale:
                                newState = State.RotatingScaling;
                                break;
                            case TwoHandedManipulation.MoveRotateScale:
                                newState = State.MovingRotatingScaling;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    break;
                case State.Scaling:
                case State.Rotating:
                case State.MovingScaling:
                case State.RotatingScaling:
                case State.MovingRotatingScaling:
                    // TODO: if < 2, make this go to start state ('drop it')
                    if (handsPressedCount == 0)
                    {
                        newState = State.Start;
                    }
                    else if (handsPressedCount == 1)
                    {
                        newState = State.Moving;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            InvokeStateUpdateFunctions(currentState, newState);
            currentState = newState;
        }

        private void InvokeStateUpdateFunctions(State oldState, State newState)
        {
            if (newState != oldState)
            {
                switch (newState)
                {
                    case State.Moving:
                        OnOneHandManipulationStarted();
                        break;
                    case State.Start:
                        OnManipulationEnded();
                        break;
                    case State.RotatingScaling:
                    case State.MovingRotatingScaling:
                    case State.Scaling:
                    case State.Rotating:
                    case State.MovingScaling:
                        OnTwoHandManipulationStarted(newState);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("newState", newState, null);
                }

                switch (oldState)
                {
                    case State.Start:
                        OnManipulationStarted();
                        break;
                    case State.Scaling:
                    case State.Rotating:
                    case State.RotatingScaling:
                    case State.MovingRotatingScaling:
                    case State.MovingScaling:
                        break;
                    case State.Moving:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("oldState", oldState, null);
                }
            }
            else
            {
                switch (newState)
                {
                    case State.Moving:
                        OnOneHandManipulationUpdated();
                        break;
                    case State.Scaling:
                    case State.Rotating:
                    case State.RotatingScaling:
                    case State.MovingRotatingScaling:
                    case State.MovingScaling:
                        OnTwoHandManipulationUpdated();
                        break;
                    case State.Start:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("newState", newState, null);
                }
            }
        }

        private void OnTwoHandManipulationStarted(State newState)
        {
            if ((newState & State.Rotating) > 0)
            {
                mRotateLogic.Setup(handsPressedLocationsMap);
            }

            if ((newState & State.Moving) > 0)
            {
                mMoveLogic.Setup(GetHandsCentroid(), hostTransform);
            }

            if ((newState & State.Scaling) > 0)
            {
                mScaleLogic.Setup(handsPressedLocationsMap, hostTransform);
            }
        }

        private void OnTwoHandManipulationUpdated()
        {
            var targetPosition = hostTransform.position;
            var targetRotation = hostTransform.rotation;
            var targetScale = hostTransform.localScale;

            if ((currentState & State.Moving) > 0)
            {
                targetPosition = mMoveLogic.Update(GetHandsCentroid(), targetPosition);
            }
            if ((currentState & State.Rotating) > 0)
            {
                targetRotation = mRotateLogic.Update(handsPressedLocationsMap, targetRotation);
            }
            if ((currentState & State.Scaling) > 0)
            {
                targetScale = mScaleLogic.Update(handsPressedLocationsMap);
            }

            hostTransform.position = targetPosition;
            hostTransform.rotation = targetRotation;
            hostTransform.localScale = targetScale;
        }

        private void OnOneHandManipulationStarted()
        {
            Assert.IsTrue(handsPressedLocationsMap.Count == 1);
            mMoveLogic.Setup(handsPressedLocationsMap.Values.First(), hostTransform);
        }

        private void OnOneHandManipulationUpdated()
        {
            hostTransform.position = mMoveLogic.Update(handsPressedLocationsMap.Values.First(), hostTransform.position);
        }

        private void OnManipulationStarted()
        {
            if (StartedManipulating != null)
            {
                StartedManipulating();
            }

            InputManager.Instance.PushModalInputHandler(gameObject);

            //Show Bounding Box visual on manipulation interaction
            ShowBoundingBox = true;
        }

        private void OnManipulationEnded()
        {
            if (StoppedManipulating != null)
            {
                StoppedManipulating();
            }

            InputManager.Instance.PopModalInputHandler();

            //Hide Bounding Box visual on release
            ShowBoundingBox = false;
        }

        private Vector3 GetHandsCentroid()
        {
            Vector3 result = Vector3.zero;
            foreach (Vector3 value in handsPressedLocationsMap.Values)
            {
                result = result + value;
            }

            return result / handsPressedLocationsMap.Count;
        }

        private void RemoveSourceIdFromHandMap(IInputSource source)
        {
            if (handsPressedLocationsMap.ContainsKey(source.SourceId))
            {
                handsPressedLocationsMap.Remove(source.SourceId);
            }

            if (pressedInputSources.Contains(source))
            {
                pressedInputSources.Remove(source);
            }
        }
    }
}
