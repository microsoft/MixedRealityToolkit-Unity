// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.SDK.UX;
using Microsoft.MixedReality.Toolkit.SDK.UX.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Physics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
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


    public class TwoHandManipulatable : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityInputHandler, IMixedRealitySpatialInputHandler
    {
       private enum HandMovementType
        {
            oneHandedOnly = 0,
            twoHandedOnly,
            oneAndTwoHanded
        }

        #region fields
        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform = null;

        /// <summary>
        /// enum describing range of affine xforms that are allowed.
        /// </summary>
        private enum TwoHandedManipulation
        {
            Scale,
            Rotate,
            MoveScale,
            RotateScale,
            MoveRotateScale
        };

        [SerializeField]
        [Tooltip("What manipulation will two hands perform?")]
        private TwoHandedManipulation ManipulationMode = TwoHandedManipulation.Scale;

        [SerializeField]
        [Tooltip("Constrain rotation along an axis")]
        private RotationConstraintType constraintOnRotation = RotationConstraintType.None;

        [SerializeField]
        private HandMovementType handMoveType = HandMovementType.oneAndTwoHanded;

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
        #endregion Fields

        #region Private Properties
        /// <summary>
        /// private properties that store transform information.
        /// </summary>
        private State currentState;
        private TwoHandMoveLogic m_moveLogic;
        private TwoHandScaleLogic m_scaleLogic;
        private TwoHandRotateLogic m_rotateLogic;
        private GameObject cubeLeft;
        private GameObject cubeRight;
        private bool usingPose = false;

        [SerializeField]
        private TextMesh debugText;

        /// <summary>
        /// Maps input id -> position of hand
        /// </summary>
        private readonly Dictionary<uint, Vector3> m_handsPressedLocationsMap = new Dictionary<uint, Vector3>();

        /// <summary>
        /// Maps input id -> input source. Then obtain position of input source using currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);
        /// </summary>
        private readonly Dictionary<uint, IMixedRealityInputSource> m_handsPressedInputSourceMap = new Dictionary<uint, IMixedRealityInputSource>();

        private readonly Dictionary<uint, Vector3> m_handsPoseMap = new Dictionary<uint, Vector3>();
        #endregion

        #region Monobehaviour Functions
        /// <summary>
        /// Private Methods
        /// </summary>
        private void Awake()
        {
            m_moveLogic = new TwoHandMoveLogic();
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
            //Update positions of all hands
            foreach (var key in m_handsPressedInputSourceMap.Keys)
            {
                var inputSource = m_handsPressedInputSourceMap[key];
                Vector3 inputPosition = Vector3.zero;

                if (inputSource.SourceName.Contains("Hand") == false)
                {
                    inputSource.Pointers[0].TryGetPointerPosition(out inputPosition);
                }
                else
                {
                   inputPosition = m_handsPoseMap[key];
                }

                m_handsPressedLocationsMap[key] = inputPosition;
            }

            if (currentState != State.Start)
            {
                UpdateStateMachine();
            }
        }
        #endregion Monobehaviour Functions

        #region Private Methods
        /// <summary>
        /// SetManipulationMode
        /// </summary>
        private void SetManipulationMode(TwoHandedManipulation mode)
        {
            ManipulationMode = mode;
        }

        private void UpdateStateMachine()
        {
            foreach (KeyValuePair<uint, Vector3> entry in m_handsPressedLocationsMap)
            {
                if (entry.Value == Vector3.zero)
                {
                    return;
                }
            }
            var handsPressedCount = m_handsPressedLocationsMap.Count;
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
                        if (handsPressedCount == 1 && handMoveType != HandMovementType.twoHandedOnly)
                    {
                        newState = State.Moving;
                    }
                    else if (handsPressedCount > 1 && handMoveType != HandMovementType.oneHandedOnly)
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

        private Vector3 GetHandsCentroid()
        {
            Vector3 result = m_handsPressedLocationsMap.Values.Aggregate(Vector3.zero, (current, state) => current + state);
            return result / m_handsPressedLocationsMap.Count;
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

        private void RemoveSourceIdFromHandMap(uint sourceId)
        {
            if (m_handsPressedLocationsMap.ContainsKey(sourceId))
            {
                m_handsPressedLocationsMap.Remove(sourceId);
            }

            if (m_handsPressedInputSourceMap.ContainsKey(sourceId))
            {
                m_handsPressedInputSourceMap.Remove(sourceId);
            }

            if (m_handsPoseMap.ContainsKey(sourceId))
            {
                m_handsPoseMap.Remove(sourceId);
            }
        }
        #endregion Private Methods

        #region Event Handlers From Interfaces
        /// <summary>
        /// /// Event Handler receives input from inputSource
        /// </summary>
        public void OnInputDown(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction.Description.ToUpper() != "NONE" && m_handsPressedInputSourceMap.ContainsKey(eventData.SourceId) == false)
            {
                if (m_handsPressedInputSourceMap.Count == 0)
                {
                    GameObject.Find("DebugLog").GetComponent<TextMesh>().text = eventData.MixedRealityInputAction.Description + " " +
                                                                            Time.unscaledDeltaTime.ToString();
                }
                else if (m_handsPressedInputSourceMap.Count == 1)
                {
                    GameObject.Find("DebugOut").GetComponent<TextMesh>().text = eventData.MixedRealityInputAction.Description + " " +
                                                                            Time.unscaledDeltaTime.ToString();
                }

                GameObject.Find("DebugText").GetComponent<TextMesh>().text = eventData.SourceId.ToString();
            }

            Vector3 inputPosition = Vector3.zero;
            m_handsPressedInputSourceMap[eventData.SourceId] = eventData.InputSource;

            // Add to hand map
            if (eventData.InputSource.SourceName.Contains("Hand") == true)
            {
                inputPosition = m_handsPoseMap[eventData.SourceId];
            }
            else
            {
                eventData.InputSource.Pointers[0].TryGetPointerPosition(out inputPosition);   
            }
            m_handsPressedLocationsMap[eventData.SourceId] = inputPosition;

            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// Event Handler receives input from inputSource
        /// </summary>
        public void OnInputUp(InputEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.SourceId);
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// from IMixedRealitySpacialInputHandler
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPoseInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (debugText)
            {
           //    debugText.text = "On pose changed " + eventData.MixedRealityInputAction.Description + " " + Time.unscaledDeltaTime.ToString();
            }
            if (eventData.InputSource.SourceName.Contains("Hand"))
            {
                m_handsPoseMap[eventData.SourceId] = eventData.InputData.Position;
            }
        }

        /// <summary>
        /// OnSourceLost
        /// </summary>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.SourceId);
            UpdateStateMachine();
            eventData.Use();
        }
        #endregion Event Handlers

        #region Unused Event Handlers
        /// <summary>
        /// OnSourceDetected Event Handler
        /// </summary>
        public void OnSourceDetected(SourceStateEventData eventData) { }
        public void OnInputPressed(InputEventData<float> eventData)
        {
        }

        public void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
        }

        public void OnPositionChanged(InputEventData<Vector3> eventData)
        {
        }

        public void OnRotationChanged(InputEventData<Quaternion> eventData)
        {
        }
        #endregion Unused Event Handlers

        #region Private Event Handlers
        private void OnTwoHandManipulationUpdated()
        {
            
#if UNITY_2017_2_OR_NEWER
            var targetRotation = hostTransform.rotation;
            var targetPosition = hostTransform.position;
            var targetScale = hostTransform.localScale;

            if ((currentState & State.Moving) > 0)
            {
                targetPosition = m_moveLogic.Update(GetHandsCentroid(), targetPosition);
            }
            if ((currentState & State.Rotating) > 0)
            {
                targetRotation = m_rotateLogic.Update(m_handsPressedLocationsMap, hostTransform, targetRotation);
            }
            if ((currentState & State.Scaling) > 0)
            {
                targetScale = m_scaleLogic.UpdateMap(m_handsPressedLocationsMap);
            }

            hostTransform.position = targetPosition;
            hostTransform.rotation = targetRotation;
            hostTransform.localScale = targetScale;
#endif // UNITY_2017_2_OR_NEWER
        }

        private void OnOneHandMoveUpdated()
        {
            var targetPosition = m_moveLogic.Update(m_handsPressedLocationsMap.Values.First(), hostTransform.position);

            hostTransform.position = targetPosition;
        }

        private void OnTwoHandManipulationEnded()
        {
#if UNITY_2017_2_OR_NEWER
            // This implementation currently does nothing
#endif // UNITY_2017_2_OR_NEWER
        }

        private void OnTwoHandManipulationStarted(State newState)
        {
#if UNITY_2017_2_OR_NEWER
            if ((newState & State.Rotating) > 0)
            {
                m_rotateLogic.Setup(m_handsPressedLocationsMap, hostTransform);
            }
            if ((newState & State.Moving) > 0)
            {
                m_moveLogic.Setup(GetHandsCentroid(), hostTransform);
            }
            if ((newState & State.Scaling) > 0)
            {
                m_scaleLogic.Setup(m_handsPressedLocationsMap, hostTransform);
            }
#endif // UNITY_2017_2_OR_NEWER
        }

        private void OnOneHandMoveStarted()
        {
            Assert.IsTrue(m_handsPressedLocationsMap.Count == 1);

            m_moveLogic.Setup(m_handsPressedLocationsMap.Values.First(), hostTransform);
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
    }
}
