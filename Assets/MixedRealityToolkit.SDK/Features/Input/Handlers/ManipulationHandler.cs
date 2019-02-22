// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Physics;
using System;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Utilities
{
    /// <summary>
    /// This script allows for an object to be movable, scalable, and rotatable with one or two hands. 
    /// You may also configure the script on only enable certain manipulations. The script works with 
    /// both HoloLens' gesture input and immersive headset's motion controller input.
    /// See Assets/HoloToolkit-Examples/Input/Readme/README_TwoHandManipulationTest.md
    /// for instructions on how to use the script.
    /// </summary>
    /// 
    public class ManipulationHandler : BaseFocusHandler,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<MixedRealityPose>, 
        IMixedRealitySourceStateHandler
    {
        #region Private Enums
        private enum HandMovementType
        {
            OneHandedOnly = 0,
            TwoHandedOnly,
            OneAndTwoHanded
        }
        private enum TwoHandedManipulation
        {
            Scale,
            Rotate,
            MoveScale,
            RotateScale,
            MoveRotateScale
        };
        #endregion Private Enums

        #region Serialized Fields
        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform = null;

        [SerializeField]
        [Tooltip("What manipulation will two hands perform?")]
        private TwoHandedManipulation ManipulationMode = TwoHandedManipulation.Scale;

        [SerializeField]
        [Tooltip("Constrain rotation along an axis")]
        private RotationConstraintType constraintOnRotation = RotationConstraintType.None;

        [SerializeField]
        [Tooltip("Constrain movement")]
        private MovementConstraintType constraintOnMovement = MovementConstraintType.None;

        [SerializeField]
        private HandMovementType handMoveType = HandMovementType.OneAndTwoHanded;

        [System.Flags]
        private enum State
        {
            Start = 0x000,
            Moving = 0x001,
            Scaling = 0x010,
            Rotating = 0x100,
            MovingScaling = 0x011,
            RotatingScaling = 0x110,
            MovingRotatingScaling = 0x111
        };

        [SerializeField]
        private TextMesh debugText;
        #endregion Serialized Fields

        #region Private Properties
        private State currentState;
        private TwoHandMoveLogic m_moveLogic;
        private TwoHandScaleLogic m_scaleLogic;
        private TwoHandRotateLogic m_rotateLogic;
        private GazeHandHelper gazeHandHelper;
        #endregion

        #region MonoBehaviour Functions
        private void Awake()
        {
            gazeHandHelper = new GazeHandHelper();
            m_moveLogic = new TwoHandMoveLogic(constraintOnMovement);
            m_rotateLogic = new TwoHandRotateLogic(constraintOnRotation);
            m_scaleLogic = new TwoHandScaleLogic();
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
            if (currentState != State.Start)
            {
                UpdateStateMachine();
            }
        }
        #endregion MonoBehaviour Functions

        #region Private Methods
        private void SetManipulationMode(TwoHandedManipulation mode)
        {
            ManipulationMode = mode;
        }
        private void UpdateStateMachine()
        {
            var handsPressedCount = gazeHandHelper.GetActiveHandCount();
            State newState = currentState;
            switch (currentState)
            {
                case State.Start:
                case State.Moving:
                    if (handsPressedCount == 0)
                    {
                        newState = State.Start;
                    }
                    else
                        if (handsPressedCount == 1 && handMoveType != HandMovementType.TwoHandedOnly)
                    {
                        newState = State.Moving;
                    }
                    else if (handsPressedCount > 1 && handMoveType != HandMovementType.OneHandedOnly)
                    {
                        switch (ManipulationMode)
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
                        OnOneHandMoveStarted();
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
                        OnTwoHandManipulationEnded();
                        break;
                }
            }
            else
            {
                switch (newState)
                {
                    case State.Moving:
                        OnOneHandMoveUpdated();
                        break;
                    case State.Scaling:
                    case State.Rotating:
                    case State.RotatingScaling:
                    case State.MovingRotatingScaling:
                    case State.MovingScaling:
                        OnTwoHandManipulationUpdated();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion Private Methods

        #region Event Handlers From Interfaces
        /// <summary>
        /// /// Event Handler receives input from inputSource
        /// </summary>
        public void OnInputDown(InputEventData eventData)
        {
            gazeHandHelper.AddSource(eventData);
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// Event Handler receives input from inputSource
        /// </summary>
        public void OnInputUp(InputEventData eventData)
        {
            gazeHandHelper.RemoveSource(eventData);
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// Event Handler receives input from IMixedRealityInputHandler<MixedRealityPose>
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            gazeHandHelper.UpdateSource(eventData);
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// Event Handler when a InputSource is lost- part of IMixedRealitySourceStateHander interface
        /// </summary>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            gazeHandHelper.RemoveSource(eventData);
            UpdateStateMachine();
            eventData.Use();
        }
        #endregion Event Handlers

        #region Private Event Handlers
        private void OnTwoHandManipulationUpdated()
        {
            var targetRotation = hostTransform.rotation;
            var targetPosition = hostTransform.position;
            var targetScale = hostTransform.localScale;

            if ((currentState & State.Moving) > 0)
            {
                targetPosition = m_moveLogic.Update(gazeHandHelper.GetHandsCentroid(), targetPosition);
            }
            if ((currentState & State.Rotating) > 0)
            {
                targetRotation = m_rotateLogic.Update(gazeHandHelper.GetHandPositionsDictionary(), targetRotation);
            }
            if ((currentState & State.Scaling) > 0)
            {
                targetScale = m_scaleLogic.UpdateMap(gazeHandHelper.GetHandPositionsDictionary());
            }

            hostTransform.position = targetPosition;
            hostTransform.rotation = targetRotation;
            hostTransform.localScale = targetScale;
        }
        private void OnOneHandMoveUpdated()
        {
            var targetPosition = m_moveLogic.Update(gazeHandHelper.GetFirstHand(), hostTransform.position);
            hostTransform.position = targetPosition;
        }
        private void OnTwoHandManipulationEnded() { }
        private void OnTwoHandManipulationStarted(State newState)
        {
            if ((newState & State.Rotating) > 0)
            {
                m_rotateLogic.Setup(gazeHandHelper.GetHandPositionsDictionary());
            }
            if ((newState & State.Moving) > 0)
            {
                m_moveLogic.Setup(gazeHandHelper.GetHandsCentroid(), hostTransform);
            }
            if ((newState & State.Scaling) > 0)
            {
                m_scaleLogic.Setup(gazeHandHelper.GetHandPositionsDictionary(), hostTransform);
            }
        }
        private void OnOneHandMoveStarted()
        {
            Assert.IsTrue(gazeHandHelper.GetHandPositionsDictionary().Count == 1);

            m_moveLogic.Setup(gazeHandHelper.GetFirstHand(), hostTransform);
        }
        private void OnManipulationStarted()
        {
            MixedRealityToolkit.InputSystem.PushModalInputHandler(gameObject);
        }
        private void OnManipulationEnded()
        {
            MixedRealityToolkit.InputSystem.PopModalInputHandler();
        }
        #endregion Private Event Handlers

        #region Unused Event Handlers
        public void OnSourceDetected(SourceStateEventData eventData) { }
        public void OnInputPressed(InputEventData<float> eventData) { }
        public void OnPositionInputChanged(InputEventData<Vector2> eventData) { }
        #endregion Unused Event Handlers
    }
}
