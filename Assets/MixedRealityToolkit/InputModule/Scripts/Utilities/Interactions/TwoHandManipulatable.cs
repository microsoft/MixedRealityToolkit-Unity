// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// #defines for features that are not yet implemented
#define TODO_ROTATE_FACE_USER

using MixedRealityToolkit.Common;
using MixedRealityToolkit.Common.Extensions;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using System;

namespace MixedRealityToolkit.InputModule.Utilities.Interations
{
    /// <summary>
    /// TO DO
    /// Notes:
    /// - If host transform is not provided, will default to the GameObject the script is on.
    /// - Grabbing any collidable on or below this gameobject will activate this script.
    /// </summary>
    public class TwoHandManipulatable : MonoBehaviour, IInputHandler, ISourceStateHandler
    {
        // Event that gets raised when the object begins moving
        public event Action StartedManipulating;
        // Event that gets raised when the object stops moving
        public event Action StoppedManipulating;

        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        public Transform HostTransform;

        public enum TwoHandedManipulation
        {
            Scale,
            Rotate,
            MoveScale,
            RotateScale,
            MoveRotateScale
        };

        [Tooltip("What manipulation will two hands perform?")]
        public TwoHandedManipulation ManipulationMode;

        [Tooltip("Constrain rotation along an axis")]
        public HandlebarRotateLogic.RotationConstraint ConstraintOnRotation = HandlebarRotateLogic.RotationConstraint.None;
        [Tooltip("If true, grabbing the object with one hand will initiate movement.")]
        public bool OneHandMovement = true;

        [Flags]
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

        private State currentState;
        private MoveSphericalCoordsLogic m_moveLogic;
        private ScaleLogic m_scaleLogic;
        private HandlebarRotateLogic m_rotateLogic;
        // Maps input id -> position of hand
        private readonly Dictionary<uint, Vector3> m_handsPressedLocationsMap = new Dictionary<uint, Vector3>();
        // Maps input id -> input source. Then obtain position of input source using currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);
        private readonly Dictionary<uint, IInputSource> m_handsPressedInputSourceMap = new Dictionary<uint, IInputSource>();

        private void Awake()
        {
            m_moveLogic = new MoveSphericalCoordsLogic();
            m_rotateLogic = new HandlebarRotateLogic(ConstraintOnRotation);
            m_scaleLogic = new ScaleLogic();
        }

        private void Update()
        {
            // Update positions of all hands
            foreach (var key in m_handsPressedInputSourceMap.Keys)
            {
                var inputSource = m_handsPressedInputSourceMap[key];
                Vector3 inputPosition = Vector3.zero;
                if (inputSource.TryGetGripPosition(key, out inputPosition))
                {
                    m_handsPressedLocationsMap[key] = inputPosition;
                }
            }

            if (currentState != State.Start)
            {
                UpdateStateMachine();
            }
        }

        private void Start()
        {
            if (HostTransform == null)
            {
                HostTransform = transform;
            }
        }

        private Vector3 GetInputPosition(InputEventData eventData)
        {
            Vector3 result = Vector3.zero;
            eventData.InputSource.TryGetGripPosition(eventData.SourceId, out result);
            return result;
        }
        public void OnInputDown(InputEventData eventData)
        {
            // Add to hand map
            m_handsPressedLocationsMap[eventData.SourceId] = GetInputPosition(eventData);
            m_handsPressedInputSourceMap[eventData.SourceId] = eventData.InputSource;
            UpdateStateMachine();
            eventData.Use();
        }

        public void OnInputUp(InputEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.SourceId);
            UpdateStateMachine();
            eventData.Use();
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
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
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.SourceId);
            UpdateStateMachine();
            eventData.Use();
        }

        private void UpdateStateMachine()
        {
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
                        if (handsPressedCount == 1 && OneHandMovement)
                    {
                        newState = State.Moving;
                    }
                    else if (handsPressedCount > 1)
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

        private void OnTwoHandManipulationUpdated()
        {
            var targetRotation = HostTransform.rotation;
            var targetPosition = HostTransform.position;
            var targetScale = HostTransform.localScale;

            if ((currentState & State.Moving) > 0)
            {
                targetPosition = m_moveLogic.Update(GetHandsCentroid(), targetPosition);
            }
            if ((currentState & State.Rotating) > 0)
            {
                targetRotation = m_rotateLogic.Update(m_handsPressedLocationsMap, HostTransform, targetRotation);
            }
            if ((currentState & State.Scaling) > 0)
            {
                targetScale = m_scaleLogic.Update(m_handsPressedLocationsMap);
            }

            HostTransform.position = targetPosition;
            HostTransform.rotation = targetRotation;
            HostTransform.localScale = targetScale;
        }

        private void OnOneHandMoveUpdated()
        {
            var targetPosition = m_moveLogic.Update(m_handsPressedLocationsMap.Values.First(), HostTransform.position);

            HostTransform.position = targetPosition;

#if !TODO_ROTATE_FACE_USER
            // TODO: Rotate object to face user as needed.
            TryRotateObjectToFaceUser();
#endif

        }

        private void OnTwoHandManipulationEnded()
        {

        }

        private Vector3 GetHandsCentroid()
        {
            Vector3 result = m_handsPressedLocationsMap.Values.Aggregate(Vector3.zero, (current, state) => current + state);
            return result / m_handsPressedLocationsMap.Count;
        }

        private void OnTwoHandManipulationStarted(State newState)
        {
            if ((newState & State.Rotating) > 0)
            {
                m_rotateLogic.Setup(m_handsPressedLocationsMap, HostTransform);
            }
            if ((newState & State.Moving) > 0)
            {
                m_moveLogic.Setup(GetHandsCentroid(), HostTransform);
            }
            if ((newState & State.Scaling) > 0)
            {
                m_scaleLogic.Setup(m_handsPressedLocationsMap, HostTransform);
            }
        }

        private void OnOneHandMoveStarted()
        {
            Assert.IsTrue(m_handsPressedLocationsMap.Count == 1);

            m_moveLogic.Setup(m_handsPressedLocationsMap.Values.First(), HostTransform);
        }
        private void OnManipulationStarted()
        {
            if (StartedManipulating != null)
            {
                StartedManipulating();
            }
            InputManager.Instance.PushModalInputHandler(gameObject);
        }
        private void OnManipulationEnded()
        {
            if (StoppedManipulating != null)
            {
                StoppedManipulating();
            }
            InputManager.Instance.PopModalInputHandler();
        }
    }

    /// <summary>
    /// RotateLogic implements common logic for rotating holograms using a handlebar metaphor. 
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    public class HandlebarRotateLogic
    {
        private const float MinHandDistanceForPitchM = 0.1f;
        private const float RotationMultiplier = 2f;
        public enum RotationConstraint
        {
            None,
            XAxisOnly,
            YAxisOnly,
            ZAxisOnly
        };

        private readonly RotationConstraint m_rotationConstraint;
        /// <summary>
        /// The current rotation constraint might be modified based on disambiguation logic, for example
        /// XOrYBasedOnInitialHandPosition might change the current rotation constraint based on the 
        /// initial hand positions at the start
        /// </summary>
        private RotationConstraint m_currentRotationConstraint;
        public RotationConstraint GetCurrentRotationConstraint()
        {
            return m_currentRotationConstraint;
        }
        private Vector3 m_previousHandlebarRotation;

        public HandlebarRotateLogic(RotationConstraint rotationConstraint)
        {
            m_rotationConstraint = rotationConstraint;
        }

        public void Setup(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot)
        {

            m_currentRotationConstraint = m_rotationConstraint;
            m_previousHandlebarRotation = GetHandlebarDirection(handsPressedMap, manipulationRoot);
        }

        private Vector3 ProjectHandlebarGivenConstraint(RotationConstraint constraint, Vector3 handlebarRotation, Transform manipulationRoot)
        {
            Vector3 result = handlebarRotation;
            switch (constraint)
            {
                case RotationConstraint.XAxisOnly:
                    result.x = 0;
                    break;
                case RotationConstraint.YAxisOnly:
                    result.y = 0;
                    break;
                case RotationConstraint.ZAxisOnly:
                    result.z = 0;
                    break;
            }
            return Camera.main.transform.TransformDirection(result);
        }

        private Vector3 GetHandlebarDirection(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot)
        {
            Assert.IsTrue(handsPressedMap.Count > 1);
            var handsEnumerator = handsPressedMap.Values.GetEnumerator();
            handsEnumerator.MoveNext();
            var hand1 = handsEnumerator.Current;
            handsEnumerator.MoveNext();
            var hand2 = handsEnumerator.Current;

            // We project the handlebar direction into camera space because otherwise when we move our body the handlebard will move even 
            // though, relative to our heads, the handlebar is not moving.
            hand1 = Camera.main.transform.InverseTransformPoint(hand1);
            hand2 = Camera.main.transform.InverseTransformPoint(hand2);

            return hand2 - hand1;
        }

        public Quaternion Update(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot, Quaternion currentRotation)
        {
            var handlebarDirection = GetHandlebarDirection(handsPressedMap, manipulationRoot);
            var handlebarDirectionProjected = ProjectHandlebarGivenConstraint(m_currentRotationConstraint, handlebarDirection,
                manipulationRoot);
            var prevHandlebarDirectionProjected = ProjectHandlebarGivenConstraint(m_currentRotationConstraint,
                m_previousHandlebarRotation, manipulationRoot);
            m_previousHandlebarRotation = handlebarDirection;

            var rotationDelta = Quaternion.FromToRotation(prevHandlebarDirectionProjected, handlebarDirectionProjected);

            var angle = 0f;
            var axis = Vector3.zero;
            rotationDelta.ToAngleAxis(out angle, out axis);
            angle *= RotationMultiplier;

            if (m_currentRotationConstraint == RotationConstraint.YAxisOnly)
            {
                // If we are rotating about Y axis, then make sure we rotate about global Y axis.
                // Since the angle is obtained from a quaternion, we need to properly orient it (up or down) based
                // on the original axis-angle representation. 
                axis = Vector3.up * Vector3.Dot(axis, Vector3.up);
            }
            return Quaternion.AngleAxis(angle, axis) * currentRotation;
        }
    }

    /// <summary>
    /// Implements a movement logic that uses the model of angular rotations along a sphere whose 
    /// radius varies. The angle to move by is computed by looking at how much the hand changes
    /// relative to a pivot point (slightly below and in front of the head).
    /// 
    /// This implementation was copied from the HandDraggable logic from the experiences central prototyping team.
    /// $\CPT\Templates\UnityHoloLens\Main\Assets\ProtoShared\Scripts\Hands\HandDraggable.cs
    /// </summary>
    public class MoveSphericalCoordsLogic
    {
        private float m_handRefDistance;
        private float m_objRefDistance;
        /// <summary>
        /// The initial angle between the hand and the object
        /// </summary>
        private Quaternion m_gazeAngularOffset;
        /// <summary>
        /// The initial object position
        /// </summary>
        private Vector3 m_draggingPosition;

        private const float DistanceScale = 2f;

        public void Setup(Vector3 startHandPositionMeters, Transform manipulationRoot)
        {
            var newHandPosition = startHandPositionMeters;

            // The pivot is just below and in front of the head.
            var pivotPosition = GetHandPivotPosition();

            m_handRefDistance = Vector3.Distance(newHandPosition, pivotPosition);
            m_objRefDistance = Vector3.Distance(manipulationRoot.position, pivotPosition);

            var objDirectoin = Vector3.Normalize(manipulationRoot.position - pivotPosition);
            var handDirection = Vector3.Normalize(newHandPosition - pivotPosition);

            // We transform the forward vector of the object, the direction of the object, and the direction of the hand
            // to camera space so everything is relative to the user's perspective.
            objDirectoin = Camera.main.transform.InverseTransformDirection(objDirectoin);
            handDirection = Camera.main.transform.InverseTransformDirection(handDirection);

            // Store the original rotation between the hand an object
            m_gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirectoin);
            m_draggingPosition = manipulationRoot.position;
        }

        public Vector3 Update(Vector3 centroid, Vector3 manipulationObjectPosition)
        {
            var newHandPosition = centroid;
            var pivotPosition = GetHandPivotPosition();

            // Compute the pivot -> hand vector in camera space
            var newHandDirection = Vector3.Normalize(newHandPosition - pivotPosition);
            newHandDirection = Camera.main.transform.InverseTransformDirection(newHandDirection);

            // The direction the object should face is the current hand direction rotated by the original hand -> object rotation.
            var targetDirection = Vector3.Normalize(m_gazeAngularOffset * newHandDirection);
            targetDirection = Camera.main.transform.TransformDirection(targetDirection);

            // Compute how far away the object should be based on the ratio of the current to original hand distance
            var currentHandDistance = Vector3.Magnitude(newHandPosition - pivotPosition);
            var distanceRatio = currentHandDistance / m_handRefDistance;
            var distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * DistanceScale : 0;
            var targetDistance = m_objRefDistance + distanceOffset;

            var newPosition = pivotPosition + (targetDirection * targetDistance);

            var newDistance = Vector3.Distance(newPosition, pivotPosition);
            if (newDistance > 4f)
            {
                newPosition = pivotPosition + Vector3.Normalize(newPosition - pivotPosition) * 4f;
            }

            m_draggingPosition = newPosition;


            return m_draggingPosition;
        }

        /// <returns>A point that is below and just in front of the head.</returns>
        public static Vector3 GetHandPivotPosition()
        {
            Vector3 pivot = Camera.main.transform.position + new Vector3(0, -0.2f, 0) - Camera.main.transform.forward * 0.2f; // a bit lower and behind
            return pivot;
        }
    }

    public class ScaleLogic
    {
        private Vector3 m_startObjectScale;
        private float m_startHandDistanceMeters;

        public virtual void Setup(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot)
        {
            m_startHandDistanceMeters = GetMinDistanceBetweenHands(handsPressedMap);
            m_startObjectScale = manipulationRoot.transform.localScale;
        }

        /// <summary>
        /// Finds the minimum distance between all pairs of hands
        /// </summary>
        /// <returns></returns>
        private float GetMinDistanceBetweenHands(Dictionary<uint, Vector3> handsPressedMap)
        {
            var result = float.MaxValue;
            Vector3[] handLocations = new Vector3[handsPressedMap.Values.Count];
            handsPressedMap.Values.CopyTo(handLocations, 0);
            for (int i = 0; i < handLocations.Length; i++)
            {
                for (int j = i + 1; j < handLocations.Length; j++)
                {
                    var distance = Vector3.Distance(handLocations[i], handLocations[j]);
                    if (distance < result)
                    {
                        result = distance;
                    }
                }
            }
            return result;
        }

        public virtual Vector3 Update(Dictionary<uint, Vector3> handsPressedMap)
        {
            var ratioMultiplier = GetMinDistanceBetweenHands(handsPressedMap) / m_startHandDistanceMeters;
            return m_startObjectScale * ratioMultiplier;
        }

    }
}